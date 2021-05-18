
Imports System.Data.SqlClient
Public Class cPacklistDetail
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property Packlist As String
	Property PacklistDetail As Integer
	Property PacklistOID As String
	Property SalesOrder As String
	Property MaterialTrans As Integer
	Property Material As String
	Property SODetail As Integer
	Property Job As String
	Property Quantity As Double
	Property UnitPrice As Double
	Property IsCreditMemo As Boolean
	Property Notes As String
	Property DueDate As Date
	Property TrackingNbr As String
	Property ObjectID As String
	Property sod As cSalesOrder_Detail
	Property soh As cSalesOrder
	Property cmd As SqlCommand
	Private errs As New System.Text.StringBuilder
	ReadOnly Property ClassErrors As String
		Get
			Return errs.ToString
		End Get
	End Property
	Private Sub AddError(msg As String)
		errs.AppendLine(" -- " & msg)
	End Sub
	Function AddPacklistDetail() As Boolean

		Try
			'Dim sod As New cSalesOrder_Detail With {.cmd = cmd}
			'If Not sod.LoadSODetail(SODetail, cmd) Then
			'	AddError("Adding Packlist Detail - Unable to load Sales Order Detail")
			'	Return False
			'End If

			'Dim soh As New cSalesOrder With {.cmd = cmd}
			'If Not soh.Load(sod.SalesOrder, cmd) Then
			'	AddError("Adding Packlist Detail - Unable to load Sales Order header")
			'	Return False
			'End If
			Dim rootPLD As String = "INSERT INTO [Packlist_Detail]([Packlist],[SO_Detail],[PO_Number],[Due_Date],[Unit_Price]," _
								 & "[Price_UofM],[Quantity],[Promised_Qty],[Note_Text],[Last_Updated],[Order_Unit]," _
								 & "[Price_Unit_Conv],[ObjectID],[Packlist_OID],[Freight_Amt],[Cartons],[Pallets],[Weight],Tracking_Nbr) "

			Dim sql As String = AddString(Packlist, DbType.String)
			sql = sql & "," & AddString(SODetail, DbType.Double, "NULL")
			sql = sql & "," & AddString(soh.CustomerPO, DbType.String, "NULL") 'po number
			sql = sql & "," & AddString(DueDate, DbType.Date, "NULL") 'due date
			sql = sql & "," & AddString(sod.UnitCost, DbType.Double, "NULL") 'unit price
			sql = sql & "," & AddString(sod.PriceUofM, DbType.String, "NULL") 'price uofm
			sql = sql & "," & AddString(Quantity, DbType.Double) 'qty
			sql = sql & "," & AddString(Quantity, DbType.Double) 'promised qyt
			sql = sql & "," & AddString(Notes, DbType.String, "NULL") 'note_text
			sql = sql & "," & AddString(Now, DbType.DateTime) ' last updated
			sql = sql & "," & AddString(sod.PriceUofM, DbType.String, "NULL") 'order unitCost
			sql = sql & "," & AddString(sod.PriceUnitconv, DbType.Double, "NULL") 'price unit vonc
			ObjectID = System.Guid.NewGuid.ToString.ToUpper
			sql = sql & "," & AddString(ObjectID, DbType.String)
			sql = sql & "," & AddString(PacklistOID.ToUpper) 'oid
			sql = sql & ",0,0,0,0" 'last 4 default to 0.
			sql = sql & "," & AddString(TrackingNbr, DbType.String, "NULL")

			cmd.CommandText = rootPLD & " VALUES(" & sql & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "SELECT Packlist_Detail FROM Packlist_Detail WHERE Packlist_Detailkey=(SELECT Scope_Identity())"
			Me.PacklistDetail = cmd.ExecuteScalar
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "ShipStationIntegration.cPacklistDetail.AddPacklistDetail Error")
			If Not bIsAuto Then
				MessageBox.Show(String.Format("Error in ShipStationIntegration.cPacklistDetail.AddPacklistDetail: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try

	End Function
	Function LoadPacklistDetail(PacklistDetail As Integer) As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				Return Load(PacklistDetail, cmd)
			End Using
		End Using
	End Function
	Function LoadPacklistDetail(PacklistDetail As Integer, cmd As SqlCommand) As Boolean
		Return Load(PacklistDetail, cmd)
	End Function
	Function LoadPacklistDetailFromSOD(SODetail As Integer) As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				Return LoadFromSODetail(SODetail, cmd)
			End Using
		End Using
	End Function
	Function LoadPacklistDetailFromSOD(SODetail As Integer, cmd As SqlCommand) As Boolean
		Return LoadFromSODetail(SODetail, cmd)
	End Function

	Private Function Load(PacklistDetail As Integer, cmd As SqlCommand) As Boolean

		cmd.CommandText = "SELECT * FROM Packlist_Detail WHERE Packlist_Detail=" & PacklistDetail
		Using dt As New DataTable
			dt.Load(cmd.ExecuteReader)
			If dt.Rows.Count = 0 Then
				Return False
			Else
				Dim dtr As DataRow = dt.Rows(0)
				_PacklistDetail = PacklistDetail
				Packlist = dtr("Packlist")
				ObjectID = dtr("ObjectID").ToString
				Quantity = dtr("Quantity")
				If Not IsDBNull(dtr("SO_Detail")) Then
					SODetail = dtr("SO_Detail")
				End If
				If Not IsDBNull(dtr("Job")) Then
					Job = dtr("Job")
				End If
				If Not IsDBNull(dtr("Material_Trans")) Then
					MaterialTrans = dtr("Material_Trans")
				End If
				UnitPrice = dtr("Unit_Price")
			End If
		End Using

		Return True

	End Function
	Private Function LoadFromSODetail(SODetail As Integer, cmd As SqlCommand) As Boolean

		cmd.CommandText = "SELECT * FROM Packlist_Detail WHERE SO_Detail=" & SODetail
		Using dt As New DataTable
			dt.Load(cmd.ExecuteReader)
			If dt.Rows.Count = 0 Then
				Return False
			Else
				Dim dtr As DataRow = dt.Rows(0)
				_PacklistDetail = PacklistDetail
				Packlist = dtr("Packlist")
				ObjectID = dtr("ObjectID").ToString
				Quantity = dtr("Quantity")
				If Not IsDBNull(dtr("SO_Detail")) Then
					SODetail = dtr("SO_Detail")
				End If
				If Not IsDBNull(dtr("Job")) Then
					Job = dtr("Job")
				End If
				If Not IsDBNull(dtr("Material_Trans")) Then
					MaterialTrans = dtr("Material_Trans")
				End If
				UnitPrice = dtr("Unit_Price")
			End If
		End Using

		Return True

	End Function
End Class
