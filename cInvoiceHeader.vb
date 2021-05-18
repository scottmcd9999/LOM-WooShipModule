Imports System.Data.SqlClient
Public Class cInvoiceHeader
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Public Sub New()
		InvDets = New List(Of cInvoiceDetail)

	End Sub

	Property plh As cPacklist_Header
	Property soh As cSalesOrder
	Property cust As cCustomer
	Property Document As String
	Property DocumentDate As Date
	Property FiscalPeriod As String
	Property CurrencyConvRate As Double
	Property JournalEntry As String
	Property Customer As String
	Property TaxCode As String
	Property ShipTo As Integer
	Property ShipVia As String
	Property IsAuto As Boolean
	Property InvDets As List(Of cInvoiceDetail)
	ReadOnly Property InvoiceTotal As Double
		Get
			If InvDets Is Nothing Then
				Return 0
			End If
			If InvDets.Count = 0 Then
				Return 0
			End If
			Dim invtotal As Double
			For i As Integer = 0 To InvDets.Count - 1
				invtotal = invtotal + InvDets(i).sod.LineTotal
			Next
			Return invtotal
		End Get
	End Property
	Property cmd As SqlCommand
	Property ClassErrors As String
	Private Sub AddError(ErrorMsg As String)
		If ClassErrors Is Nothing Then
			ClassErrors = ErrorMsg
		Else
			ClassErrors = ClassErrors & crlf() & ErrorMsg
		End If
	End Sub

	Function AddInvoiceHeader() As Boolean
		Try
			Dim hdr As String = "INSERT INTO Invoice_Header(Document,Customer,Ship_To,Contact,Ship_Via,Terms,Tax_Code,Document_Date,AR_Description,Due_Date," _
						& "Orig_Invoice_Amt,Open_Invoice_Amt,Open_Invoice_Amt_Curr_Per,Source,Type,Status,Period,Taxable_Amt,Currency_Conv_Rate,Trade_Currency," _
						& "Fixed_Rate,Trade_Date,Print_Date,Comment,Last_Updated,Bill_To,User_Values,Invoiced_By,Journal_Entry,Paid_Date)"

			Document = NextARInvoiceNumber(cmd, IsAuto)
			If String.IsNullOrEmpty(Document) Then
				AddError("Unable to get next AR Invoice Number")
				Return False
			End If

			Dim finpref As New cFinPrefs
			If Not finpref.GetFinancialPreferences(cmd) Then
				AddError("Unable to get Financial Preferences from Jobboss")
				Return False
			End If
			'/ Document Date must be in the Current or Next AR period:
			If Date.Compare(DocumentDate, finpref.CurrPeriodEnd) <= 0 Then
				'/ doc date is EARLIER
				If Date.Compare(DocumentDate, finpref.CurrPeriodStart) < 0 Then
					'/ doc date is BEFORE the start of the current period - can't do that, so we set it to the curr period end:
					DocumentDate = finpref.CurrPeriodEnd
					FiscalPeriod = finpref.CurrPeriod
				Else
					FiscalPeriod = finpref.CurrPeriod
					'/ document date can stay where it is
				End If
			Else
				'/ doc date is LATER than curr period end: is it inthe NEXT period?
				If Date.Compare(DocumentDate, finpref.NextPeriodEnd) <= 0 Then
					'/ doc date is less that the next period end date, so we can use the doc date as is
					FiscalPeriod = finpref.NextPeriod
				Else
					'/ the document date is past the next period end date, so we reset it to the curr period end date
					FiscalPeriod = finpref.CurrPeriod
					DocumentDate = finpref.CurrPeriodEnd
				End If
			End If

			Dim vals As String = AddString(Document)
			vals = vals & "," & AddString(plh.Customer.Customer)
			vals = vals & "," & plh.ShipTo
			vals = vals & "," & AddString(plh.Contact, DbType.Double, "NULL")
			vals = vals & "," & AddString(plh.ShipVia,, "NULL")
			vals = vals & "," & AddString(cust.Terms,, "NULL")
			vals = vals & "," & AddString(TaxCode,, "NULL")
			vals = vals & "," & AddString(DocumentDate, DbType.Date)
			vals = vals & ",NULL"
			vals = vals & "," & AddString(DocumentDate, DbType.Date)
			vals = vals & ",0,0,0,'N','INV','UnPosted'"
			vals = vals & "," & AddString(FiscalPeriod)
			vals = vals & ",0" 'taxable amount = set to zero, reset after adding details
			If finpref.BaseCurrency <> cust.CurrencyDef Then
				Dim curdef As New cCurrencyDef
				CurrencyConvRate = curdef.ConversionRate(finpref.BaseCurrency, cust.CurrencyDef)
			Else
				CurrencyConvRate = 1
			End If
			vals = vals & "," & CurrencyConvRate
			vals = vals & "," & cust.CurrencyDef
			vals = vals & ",1"
			vals = vals & "," & AddString(Now, DbType.Date) 'trade date
			vals = vals & ",NULL" 'print date
			vals = vals & ",'Imported by Woo Integration'" 'comment
			vals = vals & "," & AddString(Now, DbType.DateTime) 'last udated
			vals = vals & "," & cust.RemitAddr.Address 'bill to
			vals = vals & ",NULL,'SYSADM'" 'user vals, invoiced by
			JournalEntry = System.Guid.NewGuid.ToString.ToUpper
			vals = vals & "," & AddString(JournalEntry)
			vals = vals & ",NULL"

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "Jobboss_Woo_Integration.cInvoiceHeader.AddInvoiceHeader Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in Jobboss_Woo_Integration.cInvoiceHeader.AddInvoiceHeader: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
		Finally

		End Try


	End Function
End Class
