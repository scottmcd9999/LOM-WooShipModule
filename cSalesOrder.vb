Imports System.Data.SqlClient

Public Class cSalesOrder
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
	Event Loaded()
	Event SalesOrderNotFound()

	'Private sods As List(Of cSalesOrder_Detail)
	Private aNewLines() As cSalesOrder_Detail

	Private fNew As Boolean
	'Private fDirty As Boolean
	Private fLoading As Boolean
	Property cmd As SqlCommand
	Property IsNew As Boolean
	Property IsDirty As Boolean
	Property IsAuto As Boolean
	Property SODetails As List(Of cSalesOrder_Detail)
	ReadOnly Property ClassErrors As String
		Get
			Return errs.ToString
		End Get
	End Property
	Private errs As New System.Text.StringBuilder

	Public Sub New()
		SODetails = New List(Of cSalesOrder_Detail)
	End Sub

	Private Sub AddError(msg As String)
		errs.AppendLine(msg)
	End Sub
	ReadOnly Property NewSOLineCount As Integer
		Get
			If Not aNewLines Is Nothing Then
				Return aNewLines.GetUpperBound(0) + 1
			Else
				Return 0
			End If
		End Get
	End Property
	ReadOnly Property SOLineCount As Integer
		Get
			If Not SODetails Is Nothing Then
				Return SODetails.Count
			Else
				Return 0
			End If
		End Get
	End Property

	Function GetNewDetailLine(LineIndex As Integer) As cSalesOrder_Detail
		If Not aNewLines Is Nothing Then
			Return aNewLines(LineIndex)
		Else
			Return Nothing
		End If
	End Function
	Function GetSalesOrderDetailLine(LineIndex As Integer) As cSalesOrder_Detail
		If SODetails Is Nothing Then
			Return Nothing
		Else
			Return SODetails(LineIndex)
		End If

	End Function
	Property SalesOrder As String
	Property OrderTaker As String
	'Property SalesOrderID As String
	Property Customer As String
	Property ShipTo As Integer
	Property ShipVia As String
	Property OrderDate As DateTime
	Property PromisedDate As DateTime
	Property CustomerPO As String
	Property Status As String
	Property Terms As String
	Property LastUpdated As DateTime
	Property CurrencyConvRate As Double
	Property TradeCurrency As String
	Property Note As String
	Property Comment As String
	Property PriceUofM As String
	Property StockUofM As String
	Property TotalPrice As Double
	Property UserValues As Integer
	'Function ModSOLine(SODetail As Integer, QtyPicked As Double, Packlist As String, ParentPacklist As String, PaymentDocNo As String, PostingDate As Date, pl As cPacklist) As Boolean
	'	Try
	'		'/ the only modification we make is to Qtys, so that's all we need
	'		'/ a single SOD line can be associated with multiple Picks/Packlists
	'		'/ so we need to keep track of those so we can create invoices
	'		If aDetails Is Nothing Then
	'			Return False
	'		Else
	'			For i As Integer = 0 To aDetails.GetUpperBound(0)
	'				'/ Packlist is tied to a specific SODetail line, so we must work against that line ONLY for this pick
	'				If aDetails(i).SODetail = SODetail Then
	'					'/09-19-2014: Add below to DECREASE the quantity instead of overwrite
	'					aDetails(i).IssueQty = aDetails(i).IssueQty + pl.Quantity ' aDetails(i).IssueQty
	'					'aDetails(i).Packlist = Packlist
	'					'aDetails(i).ParentPacklist = ParentPacklist
	'					aDetails(i).PaymentDocNum = PaymentDocNo
	'					aDetails(i).PostingDate = PostingDate
	'					aDetails(i).AddPacklist(pl)
	'					aDetails(i).IsDirty = True
	'					Exit For
	'				End If
	'			Next
	'		End If
	'		Return True
	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "cSalesOrder.ModSOLine Error")
	'		MessageBox.Show(String.Format("Error in ModSOLine: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'		Return False
	'	Finally
	'	End Try
	'End Function

	'Function AddSOLine(Material As String, PromisedDate As Date, Quantity As Double, UnitCost As Double, Optional Notes As String = "", Optional PriceUofM As String = "ea", _
	'                   Optional GridID As String = "")
	'    Try

	'        If aNewLines Is Nothing Then
	'            ReDim aNewLines(0)
	'        Else
	'            ReDim Preserve aNewLines(aNewLines.GetUpperBound(0) + 1)
	'        End If

	'        Dim sod As New cSalesOrder_Detail

	'        sod.Material = Material
	'        sod.Quantity = Quantity
	'        sod.PromisedDate = PromisedDate
	'        sod.PriceUofM = PriceUofM
	'        sod.TotalPrice = UnitCost
	'        sod.UnitCost = UnitCost / Quantity
	'        sod.Notes = Notes
	'        sod.GridID = GridID
	'        aNewLines(aNewLines.GetUpperBound(0)) = sod
	'        Return True
	'    Catch ex As Exception
	'        logger.Error(ex.ToString, "cSalesOrder.AddSOLine Error")
	'        MessageBox.Show(String.Format("Error in AddSOLine: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'        Return False
	'    Finally
	'    End Try
	'End Function
	Private Function Fill(rdr As DataRow) As Boolean

		Try
			SalesOrder = rdr("Sales_Order")
			LastUpdated = rdr("Last_Updated")

			If Not IsDBNull(rdr("Customer")) Then
				Customer = rdr("Customer")
			End If
			If Not IsDBNull(rdr("Ship_To")) Then
				ShipTo = rdr("Ship_To")
			End If
			If Not IsDBNull(rdr("Order_Taken_By")) Then
				OrderTaker = rdr("Order_Taken_By")
			End If
			If Not IsDBNull(rdr("Terms")) Then
				Terms = rdr("Terms")
			End If
			If Not IsDBNull(rdr("Order_Date")) Then
				OrderDate = rdr("Order_Date")
			End If
			If Not IsDBNull(rdr("Promised_Date")) Then
				PromisedDate = rdr("Promised_Date")
			End If
			If Not IsDBNull(rdr("Customer_PO")) Then
				CustomerPO = rdr("Customer_PO")
			End If
			If Not IsDBNull(rdr("Status")) Then
				Status = rdr("Status")
			End If
			If Not IsDBNull(rdr("Currency_Conv_Rate")) Then
				CurrencyConvRate = rdr("Currency_Conv_Rate")
			End If
			If Not IsDBNull(rdr("Trade_Currency")) Then
				TradeCurrency = rdr("Trade_Currency")
			End If
			If Not IsDBNull(rdr("Note_Text")) Then
				Note = rdr("Note_Text")
			End If
			If Not IsDBNull(rdr("Comment")) Then
				Comment = rdr("Comment")
			End If
			If Not IsDBNull(rdr("User_Values")) Then
				UserValues = rdr("User_Values")
			End If
			TotalPrice = rdr("Total_Price")
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "cSalesOrder.Fill Error")
			MessageBox.Show(String.Format("Error in Fill: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function

	Private Function GetSODetails(SalesOrder As String) As Boolean
		Try
			cmd.CommandText = "SELECT SO_Detail FROM SO_Detail WHERE Sales_Order='" & SalesOrder & "'"
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				For Each dtr As DataRow In dt.Rows
					Dim cSOD As New cSalesOrder_Detail With {.cmd = cmd}
					SODetails.Add(cSOD)
				Next
			End Using
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "cSalesOrder.GetDetails Error")
			MessageBox.Show(String.Format("Error in GetDetails: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function

	'Function Load(Customer As String, PO As String) As Boolean

	'    Try
	'        Dim bFound As Boolean
	'        Dim so As String

	'        If fNew Then
	'            mSO = New SalesOrder
	'            mSO.Customer = Customer
	'            mSO.CustomerPO = PO
	'            Return True
	'        Else
	'            Using con As New SqlConnection(JBConnection)
	'                con.Open()
	'                Using cmd As New SqlCommand("SELECT * FROM SO_Header WHERE Customer='" & Customer.Replace("'", "''") & "' AND Customer_PO='" & PO & "' AND Status='Open'", con)
	'                    Using rdr = cmd.ExecuteReader
	'                        If rdr.HasRows Then
	'                            bFound = Fill(rdr)
	'                            '/ there will only be one Open for Customer + PO
	'                            'rdr.Read()
	'                            so = rdr("Sales_Order")

	'                            If bFound Then
	'                                RaiseEvent Loaded()
	'                            End If
	'                        Else
	'                            RaiseEvent SalesOrderNotFound()
	'                        End If
	'                    End Using
	'                End Using

	'            End Using
	'            If bFound Then
	'                GetDetails(so)
	'            End If
	'            Return bFound
	'        End If
	'    Catch ex As Exception
	'        logger.Error(ex.ToString, "cSalesOrder.Load Error")
	'        MessageBox.Show(String.Format("Error in Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'        Return False
	'    Finally
	'    End Try
	'End Function
	Function Load(Customer As String, PO As String, Optional OpenOnly As Boolean = True, Optional LoadDetails As Boolean = False) As Boolean
		Try
			Dim sql As String

			If OpenOnly Then
				sql = "SELECT * FROM SO_Header WHERE Customer=" & AddString(Customer) & " AND Customer_PO=" & AddString(PO) & " AND Status='Open'"
			Else
				sql = "SELECT * FROM SO_Header WHERE Customer=" & AddString(Customer) & " AND Customer_PO=" & AddString(PO)
			End If

			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand With {.Connection = con}
					cmd.CommandText = sql
					Using dt As New DataTable
						dt.Load(cmd.ExecuteReader)
						If dt.Rows.Count Then
							If Fill(dt.Rows(0)) Then
								If LoadDetails Then
									If GetSODetails(SalesOrder) Then
										RaiseEvent Loaded()
										Return True
									Else
										Return False
									End If
								Else
									RaiseEvent Loaded()
									Return True
								End If
							Else
								Return False
							End If
						Else
							Return False
						End If
					End Using
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.cSalesOrder.Load Error")
			MessageBox.Show(String.Format("Error in WooShipModule.cSalesOrder.Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try

	End Function
	Function Load(SalesOrder As String, Optional cmd As SqlCommand = Nothing, Optional OpenOnly As Boolean = True, Optional LoadDetails As Boolean = True) As Boolean

		Try
			If cmd Is Nothing Then
				Using con As New SqlConnection(JBConnection)
					con.Open()
					cmd = New SqlCommand With {.Connection = con}
				End Using

			End If
			Dim sql As String

			If OpenOnly Then
				sql = "SELECT * FROM SO_Header WHERE Sales_Order='" & SalesOrder & "' AND Status='Open'"
			Else
				sql = "SELECT * FROM SO_Header WHERE Sales_Order='" & SalesOrder & "'"
			End If

			cmd.CommandText = sql
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count Then
					If Fill(dt.Rows(0)) Then
						If LoadDetails Then
							If GetSODetails(SalesOrder) Then
								RaiseEvent Loaded()
								Return True
							Else
								Return False
							End If
						Else
							RaiseEvent Loaded()
							Return True
						End If
					Else
						Return False
					End If
				Else
					Return False
				End If
			End Using

		Catch ex As Exception
			logger.Error(ex.ToString, "cSalesOrder.Load Error")
			MessageBox.Show(String.Format("Error in Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function

	Function AddSalesOrderHeader() As Boolean
		Try
			'/ this is done in a Transactions, and the cmd object is created elsewhere with that transaction
			'/ the same cmd object is used to add SODetail, Deliveries, etc:
			'/ first make sure the SO does not already exist:
			'Using con As New SqlConnection(JBConnection)
			'	con.Open()
			'	Using cmd As New SqlCommand
			'		cmd.Connection = con
			cmd.CommandText = "SELECT COUNT(*) FROM SO_Header WHERE Sales_Order=" & AddString(SalesOrder)
			If cmd.ExecuteScalar > 0 Then
				AddError("Sales Order " & SalesOrder & " already exists in Jobboss")
				Return False
			End If

			If Customer Is Nothing Or (OrderDate = DateTime.MinValue) Then
				AddError("No Customer defined, or no Order Date defined")
				Return False
			End If

			Dim soh As String = "INSERT INTO SO_Header(Sales_Order,Customer,Ship_To,Order_Taken_By,Ship_Via,Tax_Code,Terms,Sales_Tax_Amt,Sales_Tax_Rate," _
								& "Order_Date,Promised_Date,Customer_PO,Status,Total_Price,Currency_Conv_Rate,Trade_Currency,Fixed_Rate,Trade_Date," _
								& "Note_Text,Comment,Last_Updated,User_Values,Source)"

			SalesOrder = NextSONumber(cmd)

			Dim vals As String = " VALUES("
			vals = vals & AddString(SalesOrder)
			vals = vals & "," & AddString(Customer)
			vals = vals & "," & AddString(ShipTo, DbType.Double)
			vals = vals & ",NULL" 'order taken by
			vals = vals & "," & AddString(ShipVia)
			vals = vals & ",NULL" 'tax code
			vals = vals & ",NULL" ' terms
			vals = vals & ",0" 'sale tax amt
			vals = vals & ",0" 'sales tax rate
			vals = vals & "," & AddString(OrderDate, DbType.Date)
			vals = vals & "," & AddString(PromisedDate, DbType.Date)

			If CustomerPO Is Nothing Then
				vals = vals & ",NULL"
			Else
				vals = vals & "," & AddString(CustomerPO)
			End If

			vals = vals & "," & AddString("Open") 'status
			vals = vals & ",0" 'total price
			vals = vals & ",1" 'currency conv rate
			vals = vals & ",1" 'trade currency
			vals = vals & ",1" 'fixed rate
			vals = vals & "," & AddString(Now, DbType.Date) 'trade date
			vals = vals & ",NULL" 'notes
			vals = vals & ",NULL" ' comments
			vals = vals & "," & AddString(Now, DbType.DateTime)
			vals = vals & ",NULL"
			vals = vals & "," & AddString("System")
			vals = vals & ")"

			cmd.CommandText = soh & vals
			cmd.ExecuteNonQuery()

			cmd.CommandText = "UPDATE Auto_Number SET Last_Nbr=" & AddString(SalesOrder) & " WHERE Type='SalesOrder'"
			cmd.ExecuteNonQuery()

			Return True
			'	End Using
			'End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "cSalesOrder.AddSalesOrderHeader Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in AddSalesOrderHeader: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			AddError(ex.ToString)
			Return False
		Finally
		End Try

	End Function

End Class
