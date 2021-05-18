Imports System.Data.SqlClient
Public Class cMaterialTrans
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
	Property cmd As SqlCommand
	Property MaterialTransID As Integer
	Property SourceID As Integer
	Property Vendor As String
	Property Document As String
	Property TranType As String
	Property MatTransDate As Date
	Property StockUofM As String
	Property CostUofM As String
	Property Qty As Double
	Property UnitCost As Double
	Property OID As String
	Property ClassError
	Property IsAuto As Boolean

	Function AddSOLinePick(SODetail As Integer, Material As String, LocationID As String, Quantity As Integer, Optional Lot As String = "") As Boolean

		Try
			Dim mat As New cMaterial With {.cmd = cmd}
			If Not mat.Load(Material) Then
				ClassError = "Unable to load Material " & Material & ": " & mat.ClassError
				Return False
			End If

			Dim hdr As String = "INSERT INTO Material_Trans(SO_Detail,Location_ID,Lot,Tran_Type,Material_Trans_Date,Unit_Cost,Purch_Unit_Weight," _
				& " Stock_UofM,Cost_UofM,Reason,Quantity,Last_Updated,ObjectID)"

			Dim vals As String = AddString(SODetail)
			vals = vals & "," & AddString(LocationID)
			vals = vals & "," & AddString(Lot,, "NULL") 'Lot
			vals = vals & "," & AddString("Issue") 'tran type
			vals = vals & "," & AddString(Now, DbType.Date) 'mattrans date

			Dim prefs As New cPreferences With {.cmd = cmd}

			If Not prefs.Load Then
				Return False
			End If

			Dim unitcost As Double
			Select Case prefs.InventoryCostMethod
				Case "Last"
					unitcost = mat.LastCost
				Case "Standard"
					unitcost = mat.StandardCost
				Case "Average"
					unitcost = mat.AverageCost
			End Select

			vals = vals & "," & AddString(unitcost, DbType.Double) 'unit cost
			vals = vals & "," & AddString(1, DbType.Double) 'purch_unit_weight, always 1 for SO Issues
			'vals = vals & "," & AddString(mat.PurchaseUofM)
			vals = vals & "," & AddString(mat.StockUofM)
			vals = vals & "," & AddString(mat.CostUofM)
			vals = vals & ",NULL"  'reason & AddString(Reason)
			'/ issued are negative quantities:
			vals = vals & "," & AddString(Quantity * -1, DbType.Double) 'quantity
			vals = vals & "," & AddString(Now, DbType.DateTime) 'last updated
			vals = vals & "," & AddString(System.Guid.NewGuid.ToString) 'objectid

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "SELECT Material_Trans FROM Material_Trans WHERE Material_TransKey=(SELECT Scope_Identity())"
			MaterialTransID = cmd.ExecuteScalar

			'logger.Info("MatTransID: " & MaterialTransID)
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "cMaterialTrans.AddSOLinePick Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in AddSOLinePick: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try
	End Function
	Function AddMaterialAdjustment(Material As String, LocationID As String, Reason As String, Quantity As Integer) As Boolean

		Try
			Dim mat As New cMaterial With {.cmd = cmd, .IsAuto = IsAuto}
			If Not mat.Load(Material) Then
				Return False
			End If

			cmd.CommandText = "INSERT INTO Material_Trans(Material,Location_ID,Lot,Tran_Type,Material_Trans_Date,Unit_Cost,Stock_UofM," _
				& " Cost_UofM,Reason,Quantity,Last_Updated,ObjectID)"

			Dim vals As String = AddString(Material)
			vals = vals & "," & AddString(LocationID)
			vals = vals & ",NULL" 'Lot
			vals = vals & "," & AddString("Adjustment") 'tran type
			vals = vals & "," & AddString(Now, DbType.Date) 'mattrans date
			vals = vals & ",0" 'unit cost
			vals = vals & "," & AddString(mat.StockUofM)
			vals = vals & "," & AddString(mat.CostUofM)
			vals = vals & "," & AddString(Reason)
			vals = vals & "," & AddString(Quantity, DbType.Double) 'quantity
			vals = vals & "," & AddString(Now, DbType.DateTime) 'last updated
			vals = vals & "," & AddString(System.Guid.NewGuid.ToString) 'objectid

			cmd.CommandText = cmd.CommandText & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "SELECT Material_Trans FROM Material_Trans WHERE Material_TransKey=(SELECT Scope_Identity())"
			MaterialTransID = cmd.ExecuteScalar

			logger.Info("MatTransID: " & MaterialTransID)
			'If Reason = "Assembly" Then
			'	'/ this is a SubAssembly pick, and we need to store data in usr_SAM
			'	'/ add to the log:
			'	cmd.CommandText = "INSERT INTO usr_SAM_Trans(Material_Trans) VALUES(" & MaterialTransID & ")"
			'	cmd.ExecuteNonQuery()
			'End If

			Return True
		Catch ex As Exception
			If IsAuto Then
				logger.Info(ex.ToString)
			Else
				MessageBox.Show("Error in  MAM.cMaterialTrans.AddMaterialAdjustment: " & Environment.NewLine & Environment.NewLine & ex.ToString, "Error")
			End If
			Return False
		End Try
	End Function

	Function AddVendorTransJobReceipt() As Boolean
		Try
			'/ adds the Vendor Transaction Job receipt - this has no value in the Vendor Trans field. The second transaction will be almost identical, 
			'/ except it will include the ID from this transaction.
			Dim hdr As String = "INSERT INTO Material_Trans(Vendor,Document,Source,Location_ID,Lot,Tran_Type,Material_Trans_Date,Unit_Cost,Stock_UofM," _
					& " Cost_UofM,Reason,Quantity,Last_Updated,ObjectID)"
			Dim vals As String = AddString(Vendor)
			vals = vals & "," & AddString(Document, , "NULL")
			vals = vals & "," & AddString(SourceID, DbType.Double)
			vals = vals & ",NULL,NULL" 'loc id, lot
			vals = vals & "," & AddString(TranType)
			vals = vals & "," & AddString(Now, DbType.DateTime)
			vals = vals & ",0" 'unit code
			vals = vals & ",'ea'" 'stock uofm
			vals = vals & ",NULL,NULL" 'cost uofm, reason
			vals = vals & "," & AddString(Qty, DbType.Double)
			vals = vals & "," & AddString(Now, DbType.DateTime)
			OID = System.Guid.NewGuid.ToString.ToUpper
			vals = vals & "," & AddString(OID)

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "SELECT Material_Trans FROM Material_Trans WHERE Material_TransKey=(SELECT SCOPE_IDENTITY())"
			MaterialTransID = cmd.ExecuteScalar

			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "AddJobReceipt Error")
			MessageBox.Show(String.Format("Error in AddJobReceipt: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try

	End Function
	Function AddJobReceipt(VendorTrans As Integer) As Boolean
		Dim hdr As String = "INSERT INTO Material_Trans(Vendor,Document,Source,Location_ID,Lot,Tran_Type,Material_Trans_Date,Addl_Cost,Unit_Cost,Stock_UofM," _
					& " Cost_UofM,Reason,Quantity,Last_Updated,Vendor_Trans,ObjectID)"
		Dim vals As String = AddString(Vendor)
		vals = vals & "," & AddString(Document,, "NULL")
		vals = vals & "," & AddString(SourceID, DbType.Double)
		vals = vals & ",NULL,NULL" 'loc id, lot
		vals = vals & "," & AddString(TranType)
		vals = vals & "," & AddString(Now, DbType.DateTime) 'matransdate
		vals = vals & ",0" '& AddString(addl) addl cost
		vals = vals & ",0" 'unit cost
		vals = vals & ",'ea'" 'stock uofm
		vals = vals & ",NULL,NULL" 'cost uofm, reason
		vals = vals & "," & AddString(Qty, DbType.Double)
		vals = vals & "," & AddString(Now, DbType.DateTime)
		vals = vals & "," & AddString(VendorTrans, DbType.Double)
		OID = System.Guid.NewGuid.ToString.ToUpper
		vals = vals & "," & AddString(OID)

		cmd.CommandText = hdr & " VALUES(" & vals & ")"
		cmd.ExecuteNonQuery()

		cmd.CommandText = "SELECT Material_Trans FROM Material_Trans WHERE Material_TransKey=(SELECT SCOPE_IDENTITY())"
		MaterialTransID = cmd.ExecuteScalar

		Return True
	End Function
	Function AddInvoiceTrans() As Boolean
		Dim hdr As String = "INSERT INTO Material_Trans(Vendor,Document,Source,Location_ID,Lot,Tran_Type,Material_Trans_Date,Addl_Cost,Unit_Cost,Stock_UofM," _
					& " Purch_Unit_Weight,Cost_UofM,Reason,Quantity,Last_Updated,Vendor_Trans,Tax_Assigned,ObjectID)"
		Dim vals As String = AddString(Vendor)
		vals = vals & "," & AddString(Document,, "NULL")
		vals = vals & "," & AddString(SourceID, DbType.Double)
		vals = vals & ",NULL,NULL" 'loc id, lot
		vals = vals & "," & AddString(TranType)
		vals = vals & "," & AddString(Now, DbType.DateTime) 'matransdate
		vals = vals & ",0" '& AddString(addl) addl cost
		vals = vals & "," & UnitCost 'unit cost
		vals = vals & "," & AddString(StockUofM) 'stock uofm
		vals = vals & ",1" 'purch unit weight
		vals = vals & "," & AddString(CostUofM)
		vals = vals & ",NULL" ' reason
		vals = vals & "," & AddString(Qty, DbType.Double)
		vals = vals & "," & AddString(Now, DbType.DateTime)
		vals = vals & ",NULL" '& AddString(VendorTrans, DbType.Double)
		OID = System.Guid.NewGuid.ToString.ToUpper
		vals = vals & ",0" 'tax assigned
		vals = vals & "," & AddString(OID)

		cmd.CommandText = hdr & " VALUES(" & vals & ")"
		cmd.ExecuteNonQuery()

		cmd.CommandText = "SELECT Material_Trans FROM Material_Trans WHERE Material_TransKey=(SELECT SCOPE_IDENTITY())"
		MaterialTransID = cmd.ExecuteScalar

		Return True
	End Function
	Function AddInvoiceVendorTrans(VendorTrans As String) As Boolean
		Try
			Dim hdr As String = "INSERT INTO Material_Trans(Vendor,Document,Source,Location_ID,Lot,Tran_Type,Material_Trans_Date,Addl_Cost,Unit_Cost,Stock_UofM," _
								& " Purch_Unit_Weight,Cost_UofM,Reason,Quantity,Last_Updated,Vendor_Trans,Tax_Assigned,ObjectID)"
			Dim vals As String = AddString(Vendor)
			vals = vals & "," & AddString(Document,, "NULL")
			vals = vals & "," & AddString(SourceID, DbType.Double)
			vals = vals & ",NULL,NULL" 'loc id, lot
			vals = vals & "," & AddString(TranType)
			vals = vals & "," & AddString(Now, DbType.DateTime) 'matransdate
			vals = vals & ",0" '& AddString(addl) addl cost
			vals = vals & "," & UnitCost 'unit cost
			vals = vals & "," & AddString(StockUofM, , "NULL") 'stock uofm
			vals = vals & ",1" 'purch unit weight
			vals = vals & "," & AddString(CostUofM,, "NULL")
			vals = vals & ",NULL" ' reason
			vals = vals & "," & AddString(Qty, DbType.Double)
			vals = vals & "," & AddString(Now, DbType.DateTime)
			vals = vals & "," & AddString(VendorTrans, DbType.Double)
			OID = System.Guid.NewGuid.ToString.ToUpper
			vals = vals & ",0" 'tax assigned
			vals = vals & "," & AddString(OID)

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "SELECT Material_Trans FROM Material_Trans WHERE Material_TransKey=(SELECT SCOPE_IDENTITY())"
			MaterialTransID = cmd.ExecuteScalar

			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "VelocityConcurIntegration.cMaterialTrans.AddInvoiceVendorTrans Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in VelocityConcurIntegration.cMaterialTrans.AddInvoiceVendorTrans: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try
	End Function
End Class
