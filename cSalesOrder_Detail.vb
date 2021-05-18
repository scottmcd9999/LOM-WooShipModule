Imports System.Data.SqlClient
Public Class cSalesOrder_Detail
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Event Loaded()
	Event Dirty()
	Event Saved()

	'Private _SalesOrder As String
	Property SODetail As Integer
	'Private _SODetail As Integer
	Private _HasActivity As Boolean
	Private _OriginalQty As Double

	'Private Packlists() As cPacklist
	Private fLoading As Boolean
	Property cmd As SqlCommand
	Property IsDirty As Boolean
	Property IsNew As Boolean
	Property IsAuto As Boolean
	Property Notes As String
	Property ExtDesc As String
	Property Customer As String
	Property SalesOrder As String
	Property Material As String
	Property UnitCost As Double
	Property PromisedDate As Date
	Property Quantity As Double
	Property LineTotal As Double
	Property DeferredQty As Integer
	Property BackorderQty As Integer
	Property PriceUofM As String
	ReadOnly Property OriginalQty As Double
		Get
			Return _OriginalQty
		End Get
	End Property
	Property Status As String
	Property LastUpdated As DateTime
	Property PriceUnitconv As Double
	Property SalesCode As String
	Property GLAccount As String
	Property Commissionable As Boolean
	Property CommissionPct As Double
	Property TaxCode As String
	Property PrepaidAmount As Double
	Property PickedQty As Integer
	Property ShippedQty As Integer
	Property SOLine As String
	Property ShipTo As Integer
	Property Job As String
	Property UnitPrice As Double
	Property PacklistDate As Date
	Property Delivery As cDelivery
	Property Matl As cMaterial
	ReadOnly Property ClassErrors As String
		Get
			Return errs.ToString
		End Get
	End Property
	Private errs As New System.Text.StringBuilder

	Private Sub AddError(msg As String)
		errs.AppendLine(msg)
	End Sub
	'ReadOnly Property PacklistCount As Integer
	'	Get
	'		If Packlists Is Nothing Then
	'			Return 0
	'		Else
	'			Return Packlists.GetUpperBound(0) + 1
	'		End If
	'	End Get
	'End Property
	'Function AddPacklist(PL As cPacklist)
	'	Try
	'		If Packlists Is Nothing Then
	'			ReDim Packlists(0)
	'		Else
	'			'/ make sure it doesn't already exist
	'			For i As Integer = 0 To Packlists.GetUpperBound(0)
	'				If Packlists(i).Packlist = PL.Packlist Then
	'					Return True
	'				End If
	'			Next
	'			ReDim Preserve Packlists(Packlists.GetUpperBound(0) + 1)
	'		End If

	'		Packlists(Packlists.GetUpperBound(0)) = PL
	'		Return True
	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "cSalesOrder_Detail.AddPacklist Error")
	'		MessageBox.Show(String.Format("Error in AddPacklist: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'		Return False
	'	Finally
	'	End Try
	'End Function
	'Function GetPacklist(PacklistIndex As Integer) As cPacklist
	'	Try
	'		If Packlists Is Nothing Then
	'			Dim c As New cPacklist
	'			Return c
	'		Else
	'			Return Packlists(PacklistIndex)
	'		End If
	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "cSalesOrder_Detail.GetPacklist Error")
	'		MessageBox.Show(String.Format("Error in GetPacklist: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'		Return Nothing
	'	Finally
	'	End Try
	'End Function
	Function LoadSODetail(SODetail As Integer) As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				cmd.CommandText = "SELECT * FROM SO_Detail WHERE SO_Detail=" & SODetail
				Using dt As New DataTable
					dt.Load(cmd.ExecuteReader)
					If dt.Rows.Count = 0 Then
						Return False
					Else
						Return Load(dt.Rows(0))
					End If
				End Using
			End Using
		End Using
	End Function
	Function LoadSODetail(Customer As String, Material As String, Quantity As Integer, PromisedDate As Date) As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				cmd.CommandText = "SELECT * FROM SO_Detail sod INNER JOIN SO_Header soh ON sod.Sales_Order=soh.Sales_Order WHERE soh.Customer=" & AddString(Customer) _
					& " AND sod.Material=" & AddString(Material) & " AND Order_Qty=" & Quantity & " AND Promised_Date=" & AddString(PromisedDate, DbType.Date)
				Using dt As New DataTable
					dt.Load(cmd.ExecuteReader)
					If dt.Rows.Count = 0 Then
						Return False
					Else
						Return Load(dt.Rows(0))
					End If
				End Using
			End Using
		End Using
	End Function
	Function LoadSODetail(SODetail As Integer, cmd As SqlCommand) As Boolean
		cmd.CommandText = "SELECT * FROM SO_Detail WHERE SO_Detail=" & SODetail
		Using dt As New DataTable
			dt.Load(cmd.ExecuteReader)
			If dt.Rows.Count = 0 Then
				Return False
			Else
				Return Load(dt.Rows(0))
			End If
		End Using
	End Function
	Function LoadSODetail(Customer As String, Material As String, Quantity As Integer, PromisedDate As Date, Order As String, cmd As SqlCommand) As Boolean
		cmd.CommandText = "SELECT * FROM SO_Detail sod INNER JOIN SO_Header soh ON sod.Sales_Order=soh.Sales_Order WHERE soh.Customer=" & AddString(Customer) _
			& " AND sod.Material=" & AddString(Material) & " AND Order_Qty=" & Quantity & " AND Promised_Date=" & AddString(PromisedDate, DbType.Date)
		Using dt As New DataTable
			dt.Load(cmd.ExecuteReader)
			If dt.Rows.Count = 0 Then
				Return False
			Else
				Return Load(dt.Rows(0))
			End If
		End Using
	End Function
	Private Function Load(dtr As DataRow) As Boolean
		Try
			fLoading = True
			SODetail = dtr("SO_Detail")
			SalesOrder = dtr("Sales_Order")
			LastUpdated = dtr("Last_Updated")
			If Not IsDBNull(dtr("SO_Line")) Then
				SOLine = dtr("SO_Line")
			End If
			If Not IsDBNull(dtr("Material")) Then
				Material = dtr("Material").ToString.Trim
				Matl = New cMaterial With {.cmd = cmd}
				If Not Matl.Load(Material) Then
					Return False
				End If
			End If
				If Not IsDBNull(dtr("Ship_To")) Then
				ShipTo = dtr("Ship_To")
			End If
			If Not IsDBNull(dtr("Job")) Then
				Job = dtr("Job")
			End If
			If Not IsDBNull(dtr("Status")) Then
				Status = dtr("Status")
			End If
			If Not IsDBNull(dtr("Unit_Cost")) Then
				UnitCost = dtr("Unit_Cost")
			End If
			If Not IsDBNull(dtr("Unit_Price")) Then
				UnitPrice = dtr("Unit_Price")
			End If
			If Not IsDBNull(dtr("Promised_Date")) Then
				PromisedDate = dtr("Promised_Date")
			End If
			If Not IsDBNull(dtr("Order_Qty")) Then
				Quantity = dtr("Order_Qty")
				_OriginalQty = dtr("Order_Qty")
			End If
			If Not IsDBNull(dtr("Job")) Then
				Job = dtr("Job")
			Else
				Job = ""
			End If
			If Not IsDBNull(dtr("Shipped_Qty")) Then
				ShippedQty = dtr("Shipped_Qty")
			End If
			If Not IsDBNull(dtr("Picked_Qty")) Then
				PickedQty = dtr("Picked_Qty")
			End If

			PriceUnitconv = dtr("Price_Unit_Conv")

			If Not IsDBNull(dtr("Commissionable")) Then
				Commissionable = dtr("Commissionable")
			End If
			If Not IsDBNull(dtr("Commission_Pct")) Then
				CommissionPct = dtr("Commission_Pct")
			End If
			If Not IsDBNull(dtr("Sales_Code")) Then
				SalesCode = dtr("Sales_Code")
			End If
			If Not String.IsNullOrEmpty(SalesCode) Then
				GLAccount = GetGLAccountFromSalesCode(SalesCode, cmd)
			End If
			If Not IsDBNull(dtr("Tax_Code")) Then
				TaxCode = dtr("Tax_Code")
			End If
			If Not IsDBNull(dtr("Prepaid_Amt")) Then
				PrepaidAmount = dtr("Prepaid_Amt")
			End If

			If Not IsDBNull(dtr("Price_UofM")) Then
				PriceUofM = dtr("Price_UofM")
			End If
			DeferredQty = dtr("Deferred_Qty")
			BackorderQty = dtr("Backorder_Qty")

			LineTotal = Quantity * UnitCost

			cmd.CommandText = "SELECT Customer FROM SO_Header WHERE Sales_Order='" & _SalesOrder & "'"
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				_Customer = dt.Rows(0).Item("Customer").ToString
			End Using

			Delivery = New cDelivery
			If Not Delivery.Load(SODetail, cmd) Then
				Return False
			End If
			fLoading = False

			Return True

		Catch ex As Exception
			logger.Error(ex.ToString, "cSalesOrder_Detail.Load Error")
			MessageBox.Show(String.Format("Error in Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function

	'Function AddSOLine() As Boolean
	'	Try
	'		'Using con As New SqlConnection(JBConnection)
	'		'	con.Open()
	'		'	Using cmd As New SqlCommand
	'		'		cmd.Connection = con
	'		Dim sod As String = "INSERT INTO SO_Detail(Sales_Order,SO_Line,PO,Line,Material,Ship_To,Drop_Ship,Quote,Job,Status,Make_Buy,Unit_Price,Discount_Pct,Price_UofM," _
	'												   & "Total_Price,Deferred_Qty,Prepaid_Amt,Unit_Cost,Order_Qty,Stock_UofM,Backorder_Qty,Picked_Qty,Shipped_Qty,Returned_Qty,Certs_Required,Taxable," _
	'												   & "Commissionable,Commission_Pct,Sales_Code,Note_Text,Promised_Date,Last_Updated,Price_Unit_Conv,Rev,Tax_Code,Ext_Description,Cost_UofM," _
	'												   & "Cost_Unit_Conv,Res_Type,Res_ID,Res_Qty,Partial_Res,Prepaid_Trade_Amt,ObjectID,CommissionIncluded)"

	'		logger.Info("SO Line Material: " & Material)
	'		Dim mat As New cMaterial With {.cmd = cmd}
	'		If Not mat.Load(Material) Then
	'			AddError("Could not load Material " & Material)
	'			Return False
	'		End If

	'		If String.IsNullOrEmpty(mat.PriceUofM) Then
	'			mat.PriceUofM = "ea"
	'		End If

	'		If String.IsNullOrEmpty(mat.StockUofM) Then
	'			mat.StockUofM = "ea"
	'		End If

	'		If String.IsNullOrEmpty(mat.CostUofM) Then
	'			mat.CostUofM = "ea"
	'		End If

	'		'/ ecommerce platform dictates the price, so we don't check for anything else:
	'		Dim sellprice As Double = UnitPrice ' mat.GetQuantityPriceForMaterial(Quantity)
	'		'If sellprice = 0 Then
	'		'	sellprice = mat.SellingPrice
	'		'End If

	'		Dim cust As New cCustomer With {.cmd = cmd}
	'		If Not cust.Load(Customer) Then
	'			AddError("Could not load Jobboss Customer " & Customer)
	'			Return False
	'		End If

	'		'If cust.DiscountPct <> 0 Then
	'		'	sellprice = sellprice - (sellprice * (cust.DiscountPct / 100))
	'		'End If

	'		If String.IsNullOrEmpty(SOLine) Then
	'			SOLine = NextSOLine()
	'		End If

	'		Dim vals As String = " VALUES("
	'		vals = vals & AddString(SalesOrder)
	'		vals = vals & "," & AddString(SOLine)
	'		vals = vals & ",NULL" 'po
	'		vals = vals & ",NULL" 'line
	'		vals = vals & "," & AddString(Material)
	'		vals = vals & "," & AddString(ShipTo, DbType.Double)
	'		vals = vals & "," & AddString(0, DbType.Double) 'drop ship to
	'		vals = vals & ",NULL" 'quote
	'		vals = vals & "," & AddString(Job, DbType.String, "NULL") 'job
	'		vals = vals & ",'Open'" 'status
	'		vals = vals & "," & AddString("M") 'make buy
	'		'/ Unit Price is the price WITHOUT DISCOUNT:
	'		vals = vals & "," & AddString(JBRound(sellprice, 3), DbType.Double) 'unit price
	'		vals = vals & "," & AddString(cust.DiscountPct, DbType.Double) 'discount pct
	'		vals = vals & "," & AddString(mat.PriceUofM)
	'		'/ Total Price INCLUDES any discounts:
	'		vals = vals & "," & AddString(JBRound(Quantity * sellprice, 2), DbType.Double) 'total price
	'		vals = vals & "," & AddString(DeferredQty, DbType.Double) 'deferred qty
	'		vals = vals & "," & AddString("0", DbType.Double) 'prepaid amount
	'		vals = vals & "," & AddString("0", DbType.Double) 'unit cost
	'		vals = vals & "," & AddString(Quantity, DbType.Double) 'quantity
	'		vals = vals & "," & AddString(mat.StockUofM,, "ea")
	'		vals = vals & "," & AddString(BackorderQty, DbType.Double) 'backorder qty
	'		vals = vals & "," & AddString(0, DbType.Double) 'picked qty
	'		vals = vals & "," & AddString(0, DbType.Double) 'shipped qty
	'		vals = vals & "," & AddString(0, DbType.Double) 'returned qty
	'		vals = vals & "," & AddString(0, DbType.Double) 'certs
	'		vals = vals & "," & AddString(0, DbType.Double) 'taxable
	'		vals = vals & "," & AddString(0, DbType.Double) 'commissionable
	'		vals = vals & "," & AddString(0, DbType.Double) 'comm pct

	'		If Not String.IsNullOrEmpty(mat.Sales_Code) Then
	'			If mat.Sales_Code.Length <> 0 Then
	'				vals = vals & "," & AddString(mat.Sales_Code)
	'			Else
	'				'/ does the customer have a sales code?
	'				If cust.SalesCode.Length > 0 Then
	'					vals = vals & "," & AddString(cust.SalesCode)
	'				Else
	'					vals = vals & ",NULL"
	'				End If
	'			End If
	'		Else
	'			vals = vals & ",NULL"
	'		End If

	'		vals = vals & "," & AddString(Notes, , "NULL") 'notes
	'		vals = vals & "," & AddString(PromisedDate, DbType.Date)
	'		vals = vals & "," & AddString(Now, DbType.DateTime) 'last updated
	'		vals = vals & "," & AddString(mat.UofMConversionFactor, DbType.Double)
	'		vals = vals & "," & AddString(mat.Rev, , "NULL")
	'		vals = vals & "," & AddString(TaxCode,, "NULL") 'tax code
	'		If Not String.IsNullOrEmpty(ExtDesc) Then
	'			vals = vals & "," & AddString(ExtDesc, , "NULL")
	'		Else
	'			vals = vals & "," & AddString(mat.ExtendedDesc, , "NULL")
	'		End If

	'		vals = vals & "," & AddString(mat.CostUofM)
	'		vals = vals & "," & AddString(mat.UofMConversionFactor, DbType.Double) 'cost unit conv
	'		vals = vals & ",NULL" 'res type
	'		vals = vals & ",NULL" 'res id
	'		vals = vals & ",NULL" 'res qty
	'		vals = vals & "," & AddString(0, DbType.Double) 'partial res
	'		vals = vals & "," & AddString(0, DbType.Double) 'prepaid amt
	'		vals = vals & "," & AddString(System.Guid.NewGuid.ToString.ToUpper) 'objectid
	'		vals = vals & "," & AddString(0, DbType.Double) 'comm incl
	'		vals = vals & ")"

	'		cmd.CommandText = sod & vals
	'		cmd.ExecuteNonQuery()

	'		cmd.CommandText = "SELECT SO_Detail FROM SO_Detail WHERE SO_Detailkey=(SELECT Scope_Identity())"
	'		_SODetail = cmd.ExecuteScalar

	'		Return True
	'		'	End Using
	'		'End Using

	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "cSalesOrder_Detail.AddSOLine Error")
	'		If Not IsAuto Then
	'			MessageBox.Show(String.Format("Error in AddSOLine: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'		End If
	'		AddError(ex.ToString)
	'		Return False
	'	Finally
	'	End Try

	'End Function

	'Private Function NextSOLine() As String
	'	'/ Returns the next available SO Line value
	'	Try
	'		'Using con As New SqlConnection(JBConnection)
	'		'	con.Open()
	'		'	Using cmd As New SqlCommand
	'		'		cmd.Connection = con
	'		Dim lin As Integer = 1
	'		SOLine = lin.ToString.PadLeft(3, "0")
	'		Dim ok As Boolean
	'		Do Until ok = True
	'			cmd.CommandText = "SELECT COUNT(*) FROM SO_Detail WHERE Sales_Order='" & _SalesOrder & "' AND SO_Line='" & SOLine & "'"
	'			If cmd.ExecuteScalar = 0 Then
	'				ok = True
	'			Else
	'				lin += 1
	'				SOLine = lin.ToString.PadLeft(3, "0")
	'			End If
	'		Loop
	'		Return SOLine
	'		'	End Using
	'		'End Using

	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "cSalesOrder_Detail.NextSOLine Error")
	'		If Not IsAuto Then
	'			MessageBox.Show(String.Format("Error in NextSOLine: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'		End If
	'		AddError(ex.ToString)
	'		Return "0"
	'	Finally
	'	End Try
	'End Function

	'Function PickLine(cmd As SqlCommand) As Boolean
	'	Try
	'		'/ first make sure we can pick:
	'		logger.Info("PickLine Material: " & Material)
	'		cmd.CommandText = "SELECT ISNULL(SUM(On_Hand_Qty),0) AS OHQ FROM Material_Location WHERE Material=" & AddString(Material)
	'		If cmd.ExecuteScalar < Quantity Then
	'			logger.Info("No material found to pick " & Material)
	'			AddError("Insufficient Quantity to Pick " & Material)
	'			Return False
	'		End If

	'		cmd.CommandText = "SELECT * FROM Material_Location WHERE Material=" & AddString(Material) & " ORDER BY On_Hand_Qty"

	'		Dim currqty As Integer = Quantity

	'		Using dt As New DataTable
	'			dt.Load(cmd.ExecuteReader)
	'			Do Until currqty = 0
	'				For Each dtr As DataRow In dt.Rows
	'					If dtr("On_Hand_Qty") > currqty Then
	'						'/ decrement the location by that amount:
	'						If Not PickSOLine(currqty, dtr("Location_ID"), "", Now.ToShortDateString) Then
	'							logger.Info("Could not pick " & Material & " From " & dtr("Location_ID"))
	'							AddError("Could not pick Sales Order Line " & SOLine)
	'							Return False
	'						End If
	'						cmd.CommandText = "UPDATE Material_Location SET On_Hand_Qty=On_Hand_Qty-" & currqty _
	'							& ",Last_Updated=" & AddString(Now, DbType.DateTime) & " WHERE Material_Location=" & dtr("Material_Location")
	'						cmd.ExecuteNonQuery()
	'						currqty = 0
	'					ElseIf dtr("On_Hand_Qty") <= currqty Then
	'						'/ Pick the amount in the location, then delete the location
	'						If Not PickSOLine(dtr("On_Hand_Qty"), dtr("Location_ID"), "", Now.ToShortDateString) Then
	'							logger.Info("Could not pick " & Material & " From " & dtr("Location_ID"))
	'							AddError("Could not pick Sales Order Line " & SOLine)
	'							Return False
	'						End If
	'						cmd.CommandText = "DELETE FROM Material_Location WHERE Material_Location=" & dtr("Material_Location")
	'						cmd.ExecuteNonQuery()
	'						currqty = currqty - dtr("On_Hand_Qty")
	'					End If
	'				Next
	'			Loop
	'		End Using
	'		Return True
	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "ShipStationIntegration.cSalesOrder_Detail.PickLine Error")
	'		Return False
	'		'MessageBox.Show(String.Format("Error in ShipStationIntegration.cSalesOrder_Detail.PickLine: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'	Finally

	'	End Try



	'End Function


	'Private Function PickSOLine(Quantity As String, Location As String, Lot As String, PickDate As Date) As Boolean
	'	Try

	'		Dim pick As String = "INSERT INTO Material_Trans(SO_Detail,Location_ID,Lot,Tran_Type,Material_Trans_Date,Unit_Cost,Purch_Unit_Weight,Stock_UofM," _
	'													& "Cost_UofM,Quantity,Last_Updated,ObjectID)"

	'		Dim mat As New cMaterial With {.cmd = cmd}
	'		mat.Load(Material, "")
	'		Dim puw As Double = 1 'GetPurchaseUnitWeight(Material, cmd)

	'		Dim vals As String = " VALUES("
	'		vals = vals & AddString(SODetail, DbType.Double)
	'		vals = vals & "," & AddString(Location)
	'		If Not String.IsNullOrEmpty(Lot) > 0 Then
	'			vals = vals & "," & AddString(Lot)
	'		Else
	'			vals = vals & ",NULL"
	'		End If

	'		vals = vals & "," & AddString("Issue")
	'		vals = vals & "," & AddString(PickDate, DbType.DateTime)
	'		vals = vals & "," & AddString(0, DbType.Double)
	'		vals = vals & "," & AddString(puw, DbType.Double)
	'		vals = vals & "," & AddString(mat.StockUofM)
	'		vals = vals & "," & AddString(mat.CostUofM)
	'		vals = vals & "," & AddString(Quantity * -1, DbType.Double)
	'		vals = vals & "," & AddString(Now, DbType.DateTime)
	'		vals = vals & "," & AddString(System.Guid.NewGuid.ToString.ToUpper)
	'		vals = vals & ")"

	'		cmd.CommandText = pick & vals
	'		logger.Info(cmd.CommandText)
	'		cmd.ExecuteNonQuery()

	'		Return True
	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "cSalesOrder_Detail.PickSOLine Error")
	'		'MessageBox.Show(String.Format("Error in PickSOLine: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'		Return False
	'	Finally
	'	End Try

	'End Function

	Function UpdateSODetailLine(SODetail As Integer) As Boolean
		Try
			'/ update the SOD line to show shipped qtys:
			cmd.CommandText = "SELECT ISNULL(SUM(Quantity),0) FROM Material_Trans WHERE Tran_Type='Issue' AND SO_Detail=" & SODetail
			'/ Issue is a negative qty, so multiple by -1:
			Dim currpicked As Integer = cmd.ExecuteScalar * -1
			'/ get the qty currently shipped for the SOD:
			cmd.CommandText = "SELECT ISNULL(SUM(Shipped_Quantity),0) FROM Delivery WHERE SO_Detail=" & SODetail & " AND Shipped_Date IS NOT NULL AND Packlist IS NOT NULL"
			Dim currshipped As Integer = cmd.ExecuteScalar
			'/ now build the SOD update statement:
			cmd.CommandText = "UPDATE SO_Detail SET Picked_Qty=" & currpicked & ",Shipped_Qty=" & currshipped
			'& ", Backorder_Qty=" _
			'	& sod.OriginalQty - currpicked & ",Deferred_Qty=" & sod.OriginalQty - currpicked
			'/ it is possible to pick more than the SO quantity, so we need to check that:
			If OriginalQty <= currshipped Then
				'/ line is fully shipped:
				cmd.CommandText = cmd.CommandText & ",Status='Shipped',Backorder_Qty=0,Deferred_Qty=0"
			Else
				'/ not fully shipped so leave status and such as-is
				cmd.CommandText = cmd.CommandText & ",Status='Backorder',Backorder_Qty=" & OriginalQty - currpicked & ",Deferred_Qty=" & OriginalQty - currpicked
			End If

			cmd.CommandText = cmd.CommandText & " ,Last_Updated=getdate() WHERE SO_Detail=" & SODetail
			', Promised_Date='" & packlist.PostedDate.ToShortDateString & "'" _
			'cmd.CommandText = cmd.CommandText & " WHERE SO_Detail=" & SODetail
			cmd.ExecuteNonQuery()

			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "cSalesOrder_Detail.UpdateSODetailLine Error")
			'MessageBox.Show(String.Format("Error in UpdateSODetailLine: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try

	End Function
End Class
