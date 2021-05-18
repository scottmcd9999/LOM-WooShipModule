Imports System.Data.SqlClient
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Grid
Imports DevExpress.XtraEditors

Public Class ucSalesOrders
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Function ProcessSalesOrder() As Boolean

		Try
			Dim SalesOrder As String = gvSalesOrders.GetFocusedRowCellValue("Sales_Order")
			Dim msgs As New List(Of String)
			Dim fail As Boolean
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand With {.Connection = con}
					If String.IsNullOrEmpty(gvSalesOrders.GetFocusedRowCellValue("Tracking").ToString & "") Then
						MessageBox.Show("You haven't entered the Tracking number for this shipment.", "No Tracking Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
						Return True
					End If

					If String.IsNullOrEmpty(gvSalesOrders.GetFocusedRowCellValue("Ship_Via").ToString & "") Then
						MessageBox.Show("You haven't entered the Shipper for this shipment", "No Shipper Information", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
						Return True
					End If

					If MessageBox.Show("Are you sure you want to process this Sales Order and create Packlist, Invoice, and Payment information?", "Confirm Process", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) <> DialogResult.Yes Then
						Return True
					End If

					cmd.CommandText = "SELECT DISTINCT SO_Detail, SO_Line FROM SO_Detail sd WHERE sd.Sales_Order=" & AddString(SalesOrder) _
									& " AND sd.Status IN ('Open','Backorder')"
					Using dtsod As New DataTable
						dtsod.Load(cmd.ExecuteReader)
						For Each dtsodr As DataRow In dtsod.Rows
							cmd.CommandText = "SELECT wsd.SO_Detail, wsd.Serial_Number, COUNT(*) AS DupCount FROM usr_Woo_SO_Detail wsd" _
								& " INNER JOIN SO_Detail sd ON wsd.SO_Detail=sd.SO_Detail" _
								& " WHERE sd.Sales_Order=" & AddString(SalesOrder) & " GROUP BY wsd.SO_Detail, wsd.Serial_Number" _
								& " HAVING COUNT(*) > 1"

							Using dt As New DataTable
								dt.Load(cmd.ExecuteReader)
								If dt.Rows.Count > 0 Then
									MessageBox.Show("You have duplicate Serial Numbers for one or more picks in the Sales Order lines. Correct the Serial Numbers and try this again.", "Duplicate Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
									Return False
								End If
							End Using

							cmd.CommandText = "SELECT wsd.Quantity, wsd.SO_Detail AS WSD_SODET, sd.SO_Detail AS SODET, sd.Order_Qty-sd.Picked_Qty AS RemQty," _
								& " sd.Order_Qty, sd.SO_Line, sd.Material, wsd.Serial_Number, wsd.Location" _
								& " FROM usr_Woo_SO_Detail wsd LEFT OUTER JOIN SO_Detail sd ON wsd.SO_Detail=sd.SO_Detail" _
								& " WHERE sd.Sales_Order=" & AddString(SalesOrder) _
								& " AND wsd.Date_Processed IS NULL AND wsd.Processed=0 AND wsd.SO_Detail=" & dtsodr("SO_Detail")
							Using dt As New DataTable
								dt.Load(cmd.ExecuteReader)
								Dim remqty As Integer = 0
								Dim pickedqty As Integer = 0
								If dt.Rows.Count > 0 Then
									remqty = dt.Rows(0).Item("RemQty")
									For Each dtr As DataRow In dt.Rows
										If IsDBNull(dtr("SODET")) Then
											msgs.Add("One or more Sales Order lines have not been picked")
										End If
										pickedqty += dtr("Quantity")
										'/ does that Serial Num exist in the location?
										cmd.CommandText = "SELECT COUNT(*) FROM Material_Location WHERE Material=" & AddString(dtr("Material")) _
											 & " AND Lot=" & AddString(dtr("Serial_Number")) & " AND Location_ID=" & AddString(dtr("Location"))
										If cmd.ExecuteScalar = 0 Then
											msgs.Add("Material " & dtr("Material") & " with Serial/Lot number " & dtr("Serial_Number") & " was not found in Location " & dtr("Location"))
											fail = True
										End If
									Next
								End If

								If pickedqty < remqty Then
									msgs.Add("Line " & dtsodr("SO_Line") & " in Sales Order " & SalesOrder & " has not been fully picked")
								End If

								If pickedqty > remqty Then
									msgs.Add("Line " & dtsodr("SO_Line") & " in Sales Order " & SalesOrder & " has been over picked")
								End If
							End Using
						Next
					End Using
				End Using
			End Using

			If msgs.Count > 0 Then
				Dim msg As String = ""
				For i As Integer = 0 To msgs.Count - 1
					msg = msg & " -- " & msgs(i) & Environment.NewLine
				Next

				msg = "One or more lines on Sales Order " & SalesOrder & " may have errors: " & Environment.NewLine & Environment.NewLine & msg
				If Not fail Then
					msg = msg & Environment.NewLine & "Do you still want to process this Sales Order and create a Packlist, Invoice, and Payment file?"
					If XtraMessageBox.Show(msg, "Confirm Picks", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
						Return False
					End If
				Else
					XtraMessageBox.Show(msg & Environment.NewLine & "Correct these isses and try this operation again.", "Unable to Process", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
					Return False
				End If
			End If

			Dim errs As New cErrors
			Dim shipvia As String = gvSalesOrders.GetFocusedRowCellValue("Ship_Via")
			Dim tracking As String = gvSalesOrders.GetFocusedRowCellValue("Tracking")
			If FinishSalesOrderProcess(SalesOrder, shipvia, tracking, errs, False) Then
				MessageBox.Show("Sales Order " & SalesOrder & " has been processed.", "Process Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
				LoadSalesOrders()
			Else
				MessageBox.Show("Unable to complete the Sales Order processing. Review the Errors section for more information.", "Unable to Complete", MessageBoxButtons.OK, MessageBoxIcon.Error)
			End If
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.ProcessSalesOrder Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.ProcessSalesOrder: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Function
	Function ClearSalesOrderPicks() As Boolean

		If MessageBox.Show("Clear all Inventory Picks for Sales Order " & gvSalesOrders.GetFocusedRowCellValue("Sales_Order") & "?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) = DialogResult.No Then
			Return False
		End If

		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand With {.Connection = con}
				cmd.CommandText = "DELETE FROM usr_Woo_SO_Detail WHERE SO_Detail IN (SELECT SO_Detail FROM SO_Detail" _
					& " WHERE Sales_Order=" & AddString(gvSalesOrders.GetFocusedRowCellValue("Sales_Order")) & ")"
				cmd.ExecuteNonQuery()
				LoadSOPicks(gvSODetails.GetFocusedRowCellValue("SO_Detail"))
			End Using
		End Using
	End Function
	Function LoadSalesOrders() As Boolean
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand With {.Connection = con}
					cmd.CommandText = "SELECT DISTINCT Order_ID, uwd.Sales_Order,soh.Order_Date,soh.Ship_Via,uwd.Tracking FROM usr_Woo_Data uwd" _
						& " INNER JOIN SO_Header soh ON uwd.Sales_Order=soh.Sales_Order" _
						& " WHERE Order_Created<>0 AND" _
						& " Date_Order_Created IS NOT NULL AND Processed=0 AND Date_Processed IS NULL" _
						& " ORDER BY Sales_Order"
					Using dt As New DataTable
						dt.Load(cmd.ExecuteReader)
						Dim col As New DataColumn
						col.ColumnName = "Process"
						col.DataType = GetType(String)
						dt.Columns.Add(col)

						col = New DataColumn
						col.ColumnName = "Clear Picks"
						col.DataType = GetType(String)
						dt.Columns.Add(col)

						Dim del As New DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit
						With del
							.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor
							.Buttons(0).Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph
							'.Buttons(0).Appearance
							.Buttons(0).ImageOptions.Image = My.Resources.database_import
							.Buttons(0).Appearance.Options.UseImage = True
							.Buttons(0).Caption = "Process"
							.Buttons(0).Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
							AddHandler .ButtonClick, AddressOf ProcessSalesOrder
						End With

						Dim clr As New DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit
						With clr
							.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor
							.Buttons(0).Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph
							'.Buttons(0).Appearance
							.Buttons(0).ImageOptions.Image = My.Resources.cleanup
							.Buttons(0).Appearance.Options.UseImage = True
							.Buttons(0).Caption = "Clear"
							.Buttons(0).Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
							AddHandler .ButtonClick, AddressOf ClearSalesOrderPicks
						End With

						grSalesOrders.DataSource = dt
						With gvSalesOrders
							.Columns("Order_ID").Visible = False
							.Columns("Sales_Order").Caption = "Sales Order"
							.Columns("Sales_Order").OptionsColumn.ReadOnly = True
							.Columns("Sales_Order").OptionsColumn.AllowEdit = False
							'.Columns("Order_ID").Visible = False
							.Columns("Order_Date").Caption = "Order Date"
							.Columns("Order_Date").OptionsColumn.ReadOnly = True
							.Columns("Order_Date").OptionsColumn.AllowEdit = False
							.Columns("Process").AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
							.Columns("Clear Picks").AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
							.Columns("Ship_Via").Caption = "Ship Via"
							Dim cb As New DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit
							With cb
								cmd.CommandText = "SELECT Code FROM User_Code WHERE Type='ShipVia' ORDER BY Code"
								Using dts As New DataTable
									dts.Load(cmd.ExecuteReader)
									.DataSource = dts
									.DisplayMember = "Code"
									.ValueMember = "Code"
									.NullText = ""
								End Using
							End With
							.Columns("Ship_Via").ColumnEdit = cb
							.Columns("Process").ColumnEdit = del
							.Columns("Clear Picks").ColumnEdit = clr
						End With
					End Using
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.LoadSalesOrders Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.LoadSalesOrders: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Function

	Function DeleteRow() As Boolean
		Try
			If MessageBox.Show("Are you sure you want to remove this row?", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) <> DialogResult.Yes Then
				Return False
			End If
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand With {.Connection = con}
					cmd.CommandText = "DELETE FROM usr_Woo_SO_Detail WHERE ID=" & gvSerialNumbers.GetFocusedRowCellValue("ID")
					cmd.ExecuteNonQuery()
					LoadSOPicks(gvSODetails.GetFocusedRowCellValue("SO_Detail"))
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.DeleteRow Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.DeleteRow: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Function
	Private Function LoadSOPicks(SODetail As Integer) As Boolean
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand
					cmd.Connection = con
					grSerialNumbers.DataSource = Nothing

					cmd.CommandText = "SELECT ID,Woo_Data_ID,SO_Detail,Serial_Number,Location,Quantity FROM usr_Woo_SO_Detail WHERE SO_Detail=" & SODetail
					Using dts As New DataTable
						dts.Load(cmd.ExecuteReader)
						Dim col As New DataColumn
						col.ColumnName = "IsNew"
						col.DataType = GetType(Boolean)
						col.DefaultValue = False
						dts.Columns.Add(col)

						col = New DataColumn
						col.ColumnName = "Delete"
						col.DataType = GetType(String)
						dts.Columns.Add(col)

						Dim del As New DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit
						With del
							.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor
							.Buttons(0).Kind = DevExpress.XtraEditors.Controls.ButtonPredefines.Delete
							'.Buttons(0).Appearance
							.Buttons(0).ImageOptions.Image = My.Resources.button_cancel
							.Buttons(0).Appearance.Options.UseImage = True
							.Buttons(0).Caption = "Delete"
							AddHandler .ButtonClick, AddressOf DeleteRow
						End With
						grSerialNumbers.DataSource = dts
						Dim cb As New DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit
						With cb
							cmd.CommandText = "SELECT Location_ID FROM Location ORDER BY Location_ID"
							Using dtm As New DataTable
								dtm.Load(cmd.ExecuteReader)
								cb.DataSource = dtm
								.DisplayMember = "Location_ID"
								.ValueMember = "Location_ID"
								.NullText = ""
							End Using
							gvSerialNumbers.Columns("Location").ColumnEdit = cb
						End With

						gvSerialNumbers.Columns("Delete").ColumnEdit = del
						gvSerialNumbers.Columns("Delete").ShowButtonMode = ShowButtonModeEnum.ShowAlways

						Dim matl As String = gvSODetails.GetRowCellValue(gvSODetails.FocusedRowHandle, "Material")
						Dim sn As New Repository.RepositoryItemGridLookUpEdit
						With sn
							cmd.CommandText = "SELECT Location_ID AS Location, Lot, Material,On_Hand_Qty AS [Qty Available] FROM Material_Location WHERE Material=" & AddString(matl) _
								& " AND Lot IS NOT NULL"
							'cmd.CommandText = "SELECT Location_ID AS Location, Lot, Material,On_Hand_Qty AS [Qty Available]" _
							'	& " FROM Material_Location ml LEFT OUTER JOIN usr_Woo_SO_Detail wsd ON ml.Lot=wsd.Serial_Number" _
							'	& " WHERE Material=" & AddString(matl) & " AND Lot IS NOT NULL AND wsd.ID IS NULL"
							Using dtsn As New DataTable
								dtsn.Load(cmd.ExecuteReader)
								.DataSource = dtsn
								.DisplayMember = "Lot"
								.ValueMember = "Lot"
								.NullText = ""
							End Using
							gvSerialNumbers.Columns("Serial_Number").ColumnEdit = sn
							AddHandler .EditValueChanged, AddressOf SerialPick
						End With

						Dim nm As New DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit
						nm.MinValue = 1
						nm.MaxValue = 10000000

						nm.EditMask = "D"
						gvSerialNumbers.Columns("Quantity").ColumnEdit = nm
					End Using
				End Using
			End Using

			With gvSerialNumbers
				.Columns("ID").Visible = False
				.Columns("Woo_Data_ID").Visible = False
				.Columns("SO_Detail").Visible = False
				.Columns("Quantity").AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
				.Columns("IsNew").Visible = False
				.Columns("Serial_Number").Caption = "Serial Number"
				.Columns("Delete").Caption = "Delete Row?"
				.Columns("Delete").AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center
			End With
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.LoadSOPicks Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.LoadSOPicks: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Function
	Private Sub SerialPick(sender As Object, e As EventArgs)
		Dim edit As GridLookUpEdit = TryCast(sender, GridLookUpEdit)
		Dim row As DataRowView = TryCast(edit.Properties.GetRowByKeyValue(edit.EditValue), DataRowView)
		'MsgBox(row.Item("Location_ID"))
		gvSerialNumbers.SetRowCellValue(gvSerialNumbers.FocusedRowHandle, "Location", row.Item("Location"))
	End Sub
	Private Sub ucSalesOrders_Load(sender As Object, e As EventArgs) Handles Me.Load
		LoadSalesOrders()
	End Sub

	Private Sub gvSalesOrders_FocusedRowChanged(sender As Object, e As FocusedRowChangedEventArgs) Handles gvSalesOrders.FocusedRowChanged
		Try
			grSerialNumbers.DataSource = Nothing
			grSODetails.DataSource = Nothing
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand With {.Connection = con}
					cmd.CommandText = "SELECT sd.SO_Detail,sd.SO_Line,sd.Material,sd.Order_Qty, sd.Picked_Qty, sd.Order_Qty-sd.Picked_Qty AS Remaining_Qty," _
						& "sd.Promised_Date,wd.ID FROM SO_Detail sd INNER JOIN" _
						& " usr_Woo_Data wd ON sd.SO_Detail=wd.SO_Detail WHERE sd.Sales_Order=" & AddString(gvSalesOrders.GetFocusedRowCellValue("Sales_Order"))
					Using dt As New DataTable
						dt.Load(cmd.ExecuteReader)
						grSODetails.DataSource = dt
						With gvSODetails
							.Columns("ID").Visible = False
							.Columns("SO_Detail").Visible = False
							.Columns("Order_Qty").Caption = "Order Quantity"
							.Columns("Order_Qty").AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near
							.Columns("Promised_Date").Caption = "Promised Date"
							.Columns("SO_Line").Caption = "Line #"
						End With
						'ActiveControl = Me.Controls("grSODetails")
						gvSODetails.FocusedRowHandle = 0
						LoadSOPicks(gvSODetails.GetRowCellValue(gvSODetails.FocusedRowHandle, "SO_Detail"))
						gpSOLines.Text = "Lines for Sales Order " & gvSalesOrders.GetRowCellValue(e.FocusedRowHandle, "Sales_Order")
					End Using
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.gvSalesOrders_FocusedRowChanged Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.gvSalesOrders_FocusedRowChanged: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Sub

	Private Sub gvSODetails_FocusedRowChanged(sender As Object, e As FocusedRowChangedEventArgs) Handles gvSODetails.FocusedRowChanged

		Try
			LoadSOPicks(gvSODetails.GetRowCellValue(e.FocusedRowHandle, "SO_Detail"))
			gpSerialNums.Text = "Sales Order Pick details for Line " & gvSODetails.GetFocusedRowCellValue("SO_Line") & ", Material " & gvSODetails.GetFocusedRowCellValue("Material")
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.gvSODetails_FocusedRowChanged Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.gvSODetails_FocusedRowChanged: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Sub

	Private Sub gvSerialNumbers_InitNewRow(sender As Object, e As InitNewRowEventArgs) Handles gvSerialNumbers.InitNewRow
		Try
			gvSerialNumbers.SetRowCellValue(e.RowHandle, "SO_Detail", gvSODetails.GetFocusedRowCellValue("SO_Detail"))
			gvSerialNumbers.SetRowCellValue(e.RowHandle, "Woo_Data_ID", gvSODetails.GetFocusedRowCellValue("ID"))
			gvSerialNumbers.SetRowCellValue(e.RowHandle, "Quantity", 1)
			gvSerialNumbers.SetRowCellValue(e.RowHandle, "Date_Processed", Now)
			gvSerialNumbers.SetRowCellValue(e.RowHandle, "Last_Updated", Now)
			gvSerialNumbers.SetRowCellValue(e.RowHandle, "Processed", 0)
			gvSerialNumbers.SetRowCellValue(e.RowHandle, "IsNew", True)
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.gvSerialNumbers_InitNewRow Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.gvSerialNumbers_InitNewRow: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Sub
	Private Sub gvSerialNumbers_RowUpdated(sender As Object, e As RowObjectEventArgs) Handles gvSerialNumbers.RowUpdated
		Try
			gvSerialNumbers.CloseEditor()
			gvSerialNumbers.UpdateCurrentRow()
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using trn As SqlTransaction = con.BeginTransaction
					Using cmd As New SqlCommand With {.Connection = con, .Transaction = trn}
						Dim dtr As DataRowView = e.Row
						If Not String.IsNullOrEmpty(dtr("Serial_Number")) And Not String.IsNullOrEmpty(dtr("Location")) And dtr("Woo_Data_ID") > 0 _
							And dtr("SO_Detail") > 0 And dtr("Quantity") > 0 Then
							If dtr("IsNew") = True Then
								cmd.CommandText = "INSERT INTO usr_Woo_SO_Detail(Woo_Data_ID,SO_Detail,Location,Serial_Number," _
																						& "Quantity)"
								Dim vals As String = dtr("Woo_Data_ID")
								vals = vals & "," & dtr("SO_Detail")
								vals = vals & "," & AddString(dtr("Location"))
								vals = vals & "," & AddString(dtr("Serial_Number"))
								vals = vals & "," & dtr("Quantity")
								cmd.CommandText = cmd.CommandText & " VALUES(" & vals & ")"
								cmd.ExecuteNonQuery()
							Else
								cmd.CommandText = "UPDATE usr_Woo_SO_Detail SET Quantity=" & dtr("Quantity") _
									& ",Location=" & AddString(dtr("Location")) _
									& ",Serial_Number=" & AddString(dtr("Serial_Number")) _
									& " WHERE ID=" & dtr("ID")
								cmd.ExecuteNonQuery()
							End If
							dtr("IsNew") = False
						Else
							MessageBox.Show("You must have a Serial Number, Location, and Quantity for each line", "Invalid or Missing values", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
						End If
					End Using
					trn.Commit()
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.gvSerialNumbers_RowUpdated Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.gvSerialNumbers_RowUpdated: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Sub

	Private Sub gvSerialNumbers_ValidateRow(sender As Object, e As ValidateRowEventArgs) Handles gvSerialNumbers.ValidateRow
		Try
			'/ assume it's valid until we determine otherwise:
			If e.Row Is Nothing Then
				Return
			End If
			e.Valid = True
			Dim msg As String = Environment.NewLine & Environment.NewLine & "Modify your data and try this again, or press Esc to remove the row."
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand With {.Connection = con}
					Dim dtr As DataRowView = e.Row
					If IsDBNull(dtr("Serial_Number")) Or IsDBNull(dtr("Location")) Then
						Return
					End If

					If String.IsNullOrEmpty(dtr("Serial_Number")) Or String.IsNullOrEmpty(dtr("Location")) _
							Or dtr("SO_Detail") = 0 Or dtr("Quantity") = 0 Then
						MessageBox.Show("You must enter values for Serial Number, Location, and Quantity" & msg, "Missing Values", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
						e.Valid = False
					Else
						Dim matl As String = gvSODetails.GetFocusedRowCellValue("Material")
						cmd.CommandText = "SELECT COUNT(*) FROM Material_Location WHERE Material=" & AddString(matl) _
							& " AND Location_ID=" & AddString(dtr("Location")) & " AND Lot=" & AddString(dtr("Serial_Number"))
						If cmd.ExecuteScalar = 0 Then
							MessageBox.Show("Material " & matl & " with Lot/Serial Number " & dtr("Serial_Number") & " does not exist in the " & dtr("Location") & " material location.", "Unable to Source Material", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
							e.Valid = False
						End If
					End If
					Dim soqty As Integer = gvSODetails.GetFocusedRowCellValue("Order_Qty")
					Dim remqty As Integer = gvSODetails.GetFocusedRowCellValue("Remaining_Qty")
					If dtr("ID") = 0 Then
						Dim matl As String = gvSODetails.GetFocusedRowCellValue("Material")
						cmd.CommandText = "SELECT COUNT(*) FROM usr_Woo_SO_Detail wsd INNER JOIN SO_Detail sd ON wsd.SO_Detail=sd.SO_Detail" _
							& " WHERE wsd.Serial_Number=" & AddString(dtr("Serial_Number")) & " And sd.Material=" & AddString(matl) _
							& " AND Date_Processed IS NULL"
						If cmd.ExecuteScalar > 0 Then
							MessageBox.Show("You've already used Serial Number " & dtr("Serial_Number") & " for Material " & matl & msg, "Duplicate Serial Numbers", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
							e.Valid = False
						End If
						cmd.CommandText = "SELECT ISNULL(SUM(Quantity),0) AS Qty FROM usr_Woo_SO_Detail WHERE SO_Detail=" & dtr("SO_Detail")
						Dim qty As Integer = cmd.ExecuteScalar

						If qty + dtr("Quantity") > soqty Then
							MessageBox.Show("You have already picked the entire quantity of " & soqty & " for this Sales Order line.", "Line Fully Picked", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
							dtr.Delete()
							e.Valid = False
						End If
					Else
						cmd.CommandText = "SELECT ISNULL(SUM(Quantity),0) AS Qty FROM usr_Woo_SO_Detail WHERE SO_Detail=" & dtr("SO_Detail") _
							& " AND ID<>" & dtr("ID")
						Dim qty As Integer = cmd.ExecuteScalar
						If qty + dtr("Quantity") > soqty Then
							MessageBox.Show("You can't pick " & dtr("Quantity") & " for this Sales Order line. The maximum quantity you can pick is " & remqty & msg, "Line Fully Picked", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
							e.Valid = False
						End If
					End If
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.ucSalesOrders.gvSerialNumbers_ValidateRow Error")
			MessageBox.Show(String.Format("Error in WooShipModule.ucSalesOrders.gvSerialNumbers_ValidateRow: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try

	End Sub

	Private Sub gvSerialNumbers_InvalidRowException(sender As Object, e As InvalidRowExceptionEventArgs) Handles gvSerialNumbers.InvalidRowException
		logger.Info(e.ErrorText)
		e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction
	End Sub
End Class
