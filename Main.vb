Imports System.Data.SqlClient
Module Main
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Function FinishSalesOrderProcess(SalesOrder As String, ShipVia As String, Tracking As String, ByRef errs As cErrors, IsAuto As Boolean) As Boolean
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using trn As SqlTransaction = con.BeginTransaction
					Using cmd As New SqlCommand With {.Connection = con, .Transaction = trn}
						Dim soh As New cSalesOrder With {.cmd = cmd}
						If Not soh.Load(SalesOrder, cmd,, False) Then
							logger.Info(SalesOrder & ": " & soh.ClassErrors)
							errs.LogError(SalesOrder, "Unable to load Sales Order: " & soh.ClassErrors)
							Return False
						End If
						Dim cust As New cCustomer With {.cmd = cmd}
						If Not cust.Load(soh.Customer, cmd) Then
							logger.Info(SalesOrder & ": " & cust.Errors)
							errs.LogError(SalesOrder, "Unable to load Customer " & soh.Customer & ": " & cust.Errors)
							Return False
						End If
						Dim finpref As New cFinPrefs
						If Not finpref.Load(cmd) Then
							logger.Info("Could not load Financial Preferences - SO " & SalesOrder)
							errs.LogError(SalesOrder, "Could not load Financial Preferences")
							Return False
						End If
						Dim plh As New cPacklist_Header With {.cmd = cmd}
						'/ now create packlist:
						With plh
							.soh = soh
							.Customer = cust
							.ShipTo = soh.ShipTo
							.ShipVia = ShipVia
							.PacklistDate = Now
							.PromisedDate = Now
							.Notes = "Woo Integration"
							If Not .AddPacklistHeader(IsAuto) Then
								errs.LogError(SalesOrder, "Could not add Packlist Header: " & plh.ClassErrors)
								Return False
							End If
						End With
						'/ create picks for the so detail lines:
						cmd.CommandText = "SELECT DISTINCT SO_Detail, SO_Line FROM SO_Detail sd WHERE sd.Sales_Order=" & AddString(SalesOrder) _
							& " AND sd.Status IN ('Open','Backorder')"
						Using dtsod As New DataTable
							dtsod.Load(cmd.ExecuteReader)
							For Each dtsodr As DataRow In dtsod.Rows
								Dim sod As New cSalesOrder_Detail With {.cmd = cmd}
								If Not sod.LoadSODetail(dtsodr("SO_Detail"), cmd) Then
									errs.LogError(SalesOrder, "Unable to Load Sales Order Line " & dtsodr("SO_Line") & " for Sales Order " & SalesOrder)
									Return False
								Else
									soh.SODetails.Add(sod)
								End If
								'cmd.CommandText = "SELECT wsd.Quantity, wsd.SO_Detail AS WSD_SODET, sd.SO_Detail AS SODET, sd.Order_Qty-sd.Picked_Qty AS RemQty," _
								'	& " sd.Order_Qty, sd.SO_Line, sd.Material FROM usr_Woo_SO_Detail wsd LEFT OUTER JOIN SO_Detail sd ON wsd.SO_Detail=sd.SO_Detail" _
								'	& " WHERE sd.Sales_Order=" & AddString(SalesOrder) _
								'	& " AND wsd.Date_Processed IS NULL AND wsd.Processed=0 AND wsd.SO_Detail=" & dtsodr("SO_Detail")
								cmd.CommandText = "SELECT wsd.SO_Detail,sd.Material,wsd.Location,wsd.Serial_Number,wsd.Quantity" _
									& " FROM usr_Woo_SO_Detail wsd LEFT OUTER JOIN SO_Detail sd ON wsd.SO_Detail=sd.SO_Detail" _
									& " WHERE sd.Sales_Order=" & AddString(SalesOrder) _
									& " AND wsd.Date_Processed IS NULL AND wsd.Processed=0 AND wsd.SO_Detail=" & dtsodr("SO_Detail")
								Dim pickqty As Double = 0
								Using dtpick As New DataTable
									dtpick.Load(cmd.ExecuteReader)

									For Each dtrp As DataRow In dtpick.Rows
										'/ Sales Order lines are picked for each distinct Serial Number:
										Dim mattrans As New cMaterialTrans With {.cmd = cmd}
										With mattrans
											'/ Pick the sales order line:
											If Not .AddSOLinePick(dtrp("SO_Detail"), dtrp("Material"), dtrp("Location"), dtrp("Quantity"), dtrp("Serial_Number")) Then
												errs.LogError(SalesOrder, "Unable to Pick " & dtrp("Material") & " from " & dtrp("Location") & ", Lot/Serial " & dtrp("Serial_Number"))
												Return False
											Else
												sod.PickedQty = sod.PickedQty + dtrp("Quantity")
												pickqty += dtrp("Quantity")
												Dim invt As New cInventory
												With invt
													.Material = dtrp("Material")
													.LocationID = dtrp("Location")
													.QtyIssued = dtrp("Quantity")
													.Lot = dtrp("Serial_Number")
													'/ relieve inventory for the picked materials:
													If Not .RelieveInventory(cmd) Then
														errs.LogError(SalesOrder, "Unable to Issue " & .Material & ", Serial Number " & dtrp("Serial_Number") & " from location " & .LocationID)
														Return False
													End If
												End With
											End If
										End With
									Next
								End Using
								'/ single Packlist line for the same material, even with different Lot numbers:
								Dim pld As New cPacklistDetail With {.cmd = cmd}
								'Dim pl_sod As New cSalesOrder_Detail
								'pl_sod = soh.SODetails(i)
								With pld
									.Packlist = plh.Packlist
									.Material = sod.Material
									.Quantity = pickqty ' sod.PickedQty
									.UnitPrice = sod.UnitPrice
									.PacklistOID = plh.PLHOID
									.SalesOrder = soh.SalesOrder
									.SODetail = sod.SODetail
									.DueDate = sod.PromisedDate
									.Notes = "Woo Integration"
									.TrackingNbr = Tracking
									pld.sod = sod
									pld.soh = plh.soh
									If Not .AddPacklistDetail() Then
										errs.LogError(SalesOrder, "Unable to add Packlist Detail: " & pld.ClassErrors)
										Return False
									End If
									plh.plds.Add(pld)
								End With
								'/ update the Delivery:
								cmd.CommandText = "UPDATE Delivery SET Shipped_Date=" & AddString(Now, DbType.Date) & ",Packlist=" & AddString(plh.Packlist) _
											& ",Shipped_Quantity=" & sod.PickedQty & ",Remaining_Quantity=0,Last_Updated=" & AddString(Now, DbType.DateTime) & " WHERE SO_Detail=" & sod.SODetail & " And Packlist Is NULL" _
											& " And Shipped_Date Is NULL"
								cmd.ExecuteNonQuery()
								'/ update the sales order line to show Shipped and Picked qtys:
								If Not sod.UpdateSODetailLine(sod.SODetail) Then
									errs.LogError(SalesOrder, "Unable To update Line On Sales Order " & SalesOrder)
									Return False
								End If
								'/ clear the picks for that SODetail:
								cmd.CommandText = "UPDATE usr_Woo_SO_Detail SET Processed=1,Date_Processed=getdate() WHERE SO_Detail =" & sod.SODetail
								cmd.ExecuteNonQuery()
							Next
							'/ now create Invoice:
							Dim hdr As New cInvoiceHeader
							With hdr
								.cmd = cmd
								.soh = soh
								.plh = plh
								.cust = cust
								.Customer = soh.Customer
								.DocumentDate = soh.OrderDate
								If Not .AddInvoiceHeader() Then
									errs.LogError(SalesOrder, "Unable To add Invoice Header: " & hdr.ClassErrors)
									Return False
								End If
							End With
							plh.inv = hdr
							cmd.CommandText = "UPDATE Packlist_Header SET Invoice=" & AddString(hdr.Document) & ",Invoiced=1" _
								& " WHERE Packlist=" & AddString(plh.Packlist)
							cmd.ExecuteNonQuery()

							For i As Integer = 0 To plh.plds.Count - 1
								Dim invdet As New cInvoiceDetail
								With invdet
									.cmd = cmd
									.inv = hdr
									.pld = plh.plds(i)
									.sod = plh.plds(i).sod
									.DocLine = (i + 1).ToString
									.SourceType = 1
									'/ if the SalesCode is set for the Sales Order Detail line, we use that
									'/ otherwise we use values found in the Material Record or Customer record:
									If Not String.IsNullOrEmpty(plh.plds(i).sod.GLAccount) Then
										.GLAccount = plh.plds(i).sod.GLAccount
									ElseIf Not String.IsNullOrEmpty(plh.plds(i).sod.SalesCode) Then
										.GLAccount = plh.plds(i).sod.SalesCode
									ElseIf Not String.IsNullOrEmpty(cust.SalesCode) Then
										.GLAccount = cust.SalesCode
									ElseIf Not String.IsNullOrEmpty(plh.plds(i).sod.Matl.Sales_Code) Then
										.GLAccount = GetGLAccountFromSalesCode(plh.plds(i).sod.Matl.Sales_Code, cmd)
									End If

									If Not .AddDetail Then
										errs.LogError(SalesOrder, "Unable to add Invoice Detail: " & invdet.ClassErrors)
									End If
								End With
								hdr.InvDets.Add(invdet)
							Next
							'/ now post the Invoice, if possible:
							'/ note we cannot add Payments and Receipts unless the invoice is posted
							Dim c As New cSettings
							Dim post As Boolean = c.ReadSetting("PostInvoice") = "True"
							If post Then
								cmd.CommandText = "SELECT COUNT(*) FROM SO_Detail WHERE Sales_Order=" & AddString(SalesOrder) _
																& " AND (GL_Account IS NULL AND Sales_Code IS NULL)"
								If cmd.ExecuteScalar > 0 Then
									errs.LogError(SalesOrder, "Cannot Post Invoice for Sales Order " & SalesOrder & " - one or more Materials on the Sales Order do Not have a GL Account Code")
								Else
									'/ add 2 journal entries, add/modify Cost, add deposit, add receipt, add invoice receipt, update inv header
									Dim je As New cJournalEntry With {.cmd = cmd}
									cmd.CommandText = "SELECT * FROM usr_Woo_Data WHERE Sales_Order=" & AddString(SalesOrder)
									Dim dtr As DataRow
									Using dt As New DataTable
										dt.Load(cmd.ExecuteReader)
										If dt.Rows.Count = 0 Then
											errs.LogError(SalesOrder, "Could Not retrieve data from Woo import for Sales Order " & SalesOrder)
											Return False
										Else
											dtr = dt.Rows(0)
										End If
									End Using
									'/ add for AR account first:
									With je
										.GLAcct = finpref.ARAcct
										.Refernce = dtr("Payment_ID")
										.Source = plh.Customer.Customer
										.TransactionDate = dtr("Order_Date")
										.FiscalPeriod = finpref.CurrPeriod
										.Type = "AR-Receipt"
										.Amount = plh.soh.TotalPrice
										.CreationDate = Now.ToShortDateString
										.OriginatingTrans = plh.inv.JournalEntry
										.Quantity = 0
										.Partner = plh.Customer.Customer
										.CurrDef = finpref.BaseCurrency
										If Not .AddJournalEntry(False) Then
											errs.LogError(SalesOrder, "Unable to add Journal Entry: " & .ClassError)
											Return False
										End If
									End With
									'/ now add JE for each SO_Detail line:
									If hdr.InvDets Is Nothing Then
										errs.LogError(SalesOrder, "No Invoice Detail lines were found for Sales Order " & SalesOrder)
										Return False
									End If

									For i As Integer = 0 To hdr.InvDets.Count - 1
										Dim jed As New cJournalEntry With {.cmd = cmd}
										With jed
											.GLAcct = hdr.InvDets(i).sod.GLAccount
											If .GLAcct = "" Then
												errs.LogError(SalesOrder, "No Account Code found for Sales Order, Line " & hdr.InvDets(i).sod.SOLine)
												Return False
											End If
											.TransactionDate = Now
											.Refernce = hdr.Document
											.Type = "AR-Invoicing"
											.Source = hdr.cust.Customer
											.FiscalPeriod = hdr.FiscalPeriod
											.Amount = (hdr.InvDets(i).sod.LineTotal) * -1
											.CreationDate = Now
											.Quantity = hdr.InvDets(i).sod.Quantity
											.Unit = hdr.InvDets(i).sod.PriceUofM
											.OriginatingTrans = hdr.JournalEntry
											.Partner = hdr.cust.Customer
											.ItemID = hdr.InvDets(i).sod.Material
											.CurrDef = hdr.cust.CurrencyDef
											.Posted = True
											If Not .AddJournalEntry(False) Then
												errs.LogError(SalesOrder, "Unable to post Journal Entry for Sales Order, Line " & hdr.InvDets(i).sod.SOLine)
												Return False
											End If
										End With
									Next
									'/ now add payment and receipts:
									cmd.CommandText = "SELECT DISTINCT Payment_ID FROM usr_Woo_Data WHERE Sales_Order=" & AddString(SalesOrder)
									Dim pmt As Object
									pmt = cmd.ExecuteScalar
									If pmt Is Nothing Then
										errs.LogError(SalesOrder, "No Payment ID found for Sales Order " & SalesOrder)
										Return False
									End If

									Dim bank As New cBank
									If Not bank.LoadPrimaryBank(cmd) Then
										errs.LogError(SalesOrder, "Unable to load Primary Bank information, Sales Order " & SalesOrder)
										Return False
									End If
									'/ Add Deposit
									Dim dep As New cDeposit With {.cmd = cmd}
									With dep
										.Bank = bank.Bank
										.Status = "Un-Reconciled"
										.DepositDate = Now
										.TradeCurrency = bank.CurrencyDef
										.CurrencyConvRate = bank.CurrencyConvRate
										.TradeDate = Now
										.FixedRate = 1
										.BaseAmount = hdr.InvoiceTotal
										.TradeAmount = hdr.InvoiceTotal
										If Not .AddDeposit(False) Then
											errs.LogError(SalesOrder, "Unable to add Deposit for Sales Order " & SalesOrder)
											Return False
										End If
									End With
									'/ add Receipt:
									Dim rcp As New cReceipt With {.cmd = cmd}
									With rcp
										.Customer = hdr.cust.Customer
										If pmt.ToString.Length > 8 Then
											.Reference = pmt.ToString.Substring(0, 8)
										Else
											.Reference = pmt.ToString
										End If
										.Type = "R"
										.Amount = hdr.InvoiceTotal
										.ReceiptDate = Now
										.FiscalPeriod = hdr.FiscalPeriod
										.Deposit = dep.Deposit
										If Not .AddReceipt(False) Then
											errs.LogError(SalesOrder, "Unable to add Receipt for Sales Order " & SalesOrder)
											Return False
										End If
									End With
									'/ Add Invoice Receipt:
									Dim invrcp As New cInvoiceReceipt With {.cmd = cmd}
									With invrcp
										.Invoice = hdr.Document
										.Receipt = rcp.Receipt
										.Amount = hdr.InvoiceTotal
										.ApplicationType = 0
										If Not .AddInvoiceReceipt(False) Then
											errs.LogError(SalesOrder, "Unable to add Invoice Receipt for Sales Order " & SalesOrder)
											Return False
										End If
									End With
									'/ JE for A/R receipt:
									Dim jear As New cJournalEntry With {.cmd = cmd}
									With jear
										.GLAcct = finpref.ARAcct
										.Amount = hdr.InvoiceTotal * -1
										.Refernce = pmt.ToString
										.TransactionDate = Now
										.Source = hdr.cust.Customer
										.FiscalPeriod = hdr.FiscalPeriod
										.Type = "AR-Receipt"
										.Posted = False
										.CreationDate = Now
										.Quantity = 0
										.Partner = hdr.cust.Customer
										.CurrDef = hdr.cust.CurrencyDef
										If Not .AddJournalEntry(False) Then
											errs.LogError(SalesOrder, "Unable to create Journal Entry for Payment Receipt for Sales Order " & SalesOrder)
											Return False
										End If
									End With
									'/ add a journal entry for the Bank:
									Dim jeard As New cJournalEntry With {.cmd = cmd}
									With jeard
										.GLAcct = bank.GLAcct
										.Amount = hdr.InvoiceTotal
										.Refernce = pmt.ToString
										.TransactionDate = Now
										.Source = hdr.cust.Customer
										.FiscalPeriod = hdr.FiscalPeriod
										.Type = "AR-Receipt"
										.Posted = False
										.CreationDate = Now
										.Quantity = 0
										.Partner = hdr.cust.Customer
										.CurrDef = hdr.cust.CurrencyDef
										If Not .AddJournalEntry(False) Then
											errs.LogError(SalesOrder, "Unable to create Journal Entry for Payment Receipt for Sales Order " & SalesOrder)
											Return False
										End If
									End With
									'/ do Cost table for A/R account:
									Dim cost_hdr As New cCost With {.cmd = cmd}
									With cost_hdr
										.Account = je.GLAcct
										.Amount = hdr.InvoiceTotal
										.FiscalPeriod = hdr.FiscalPeriod
										If Not .AddCost(False) Then
											errs.LogError(SalesOrder, "Unable to update Cost table for A/R Account Sales Order " & SalesOrder)
											Return False
										End If
									End With
									'/ cost table for invoice details - this is done by GL account:
									cmd.CommandText = "SELECT SUM(id.Quantity * id.Unit_Price) AS Amt, uc.Text1 FROM Invoice_Detail id " _
										& " INNER JOIN User_Code uc ON id.AR_Code=uc.Code" _
										& " WHERE id.Document=" & AddString(hdr.Document) & " And uc.type='ARCodes'" _
										& " GROUP BY uc.Text1"
									Using dta As New DataTable
										For Each dtar As DataRow In dta.Rows
											Dim cost_det As New cCost With {.cmd = cmd}
											With cost_det
												.Account = dtar("Text1")
												.FiscalPeriod = hdr.FiscalPeriod
												.Amount = dtar("Amt")
												If Not .AddCost(False) Then
													errs.LogError(SalesOrder, "Unable to update Cost table for GL Account " & dtar("Text1") & " for Sales Order" & SalesOrder)
													Return False
												End If
											End With
										Next
									End Using

									If cust.CurrentBalance - dep.BaseAmount < 0 Then
										cmd.CommandText = "UPDATE Customer SET Curr_Balance=0, Last_Updated=getdate() WHERE Customer=" & AddString(cust.Customer)
									Else
										cmd.CommandText = "UPDATE Customer SET Curr_Balance=Curr_Balance-" & dep.BaseAmount _
											& ",Last_Updated=getdate() WHERE Customer=" & AddString(cust.Customer)
									End If
									cmd.ExecuteNonQuery()
									cmd.CommandText = "UPDATE Invoice_Header SET Open_Invoice_Amt=0,Open_Invoice_Amt_Curr_Per=0" _
										& ",Paid_Date=" & AddString(dep.DepositDate, DbType.Date) & ",Last_Updated=getdate()" _
										& " WHERE Document=" & AddString(hdr.Document) & " AND Customer=" & AddString(hdr.cust.Customer)
									cmd.ExecuteNonQuery()
								End If
							End If
							cmd.CommandText = "UPDATE usr_Woo_Data SET Processed=1,Date_Processed=getdate() WHERE Sales_Order=" & AddString(SalesOrder)
							cmd.ExecuteNonQuery()

						End Using
					End Using
					trn.Commit()
				End Using
			End Using
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.Main.FinishSalesOrderProcess Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in WooShipModule.Main.FinishSalesOrderProcess: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try

	End Function

	Function GetGLAccountFromSalesCode(SalesCode As String, cmd As SqlCommand) As String
		Try
			cmd.CommandText = "SELECT Text1 FROM User_Code WHERE Type='ARCodes' AND Code=" & AddString(SalesCode)
			Dim obj As Object
			obj = cmd.ExecuteScalar
			If obj Is Nothing Then
				Return ""
			Else
				Return obj.ToString
			End If
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.Main.GetGLAccountFromSalesCode Error")
			MessageBox.Show(String.Format("Error in WooShipModule.Main.GetGLAccountFromSalesCode: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return ""
		Finally

		End Try
	End Function
End Module
