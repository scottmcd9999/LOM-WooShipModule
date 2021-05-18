Imports System.Data.SqlClient
Public Class cInvoiceDetail
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property InvoiceDetail As Integer
	Property inv As cInvoiceHeader
	Property pld As cPacklistDetail
	Property sod As cSalesOrder_Detail
	Property DocLine As Integer
	Property SourceType As Integer
	Property GLAccount As String
	Property IsAuto As Boolean
	Property cmd As SqlCommand
	Property ClassErrors As String
	Private Sub AddError(ErrorMsg As String)
		If ClassErrors Is Nothing Then
			ClassErrors = ErrorMsg
		Else
			ClassErrors = ClassErrors & crlf() & ErrorMsg
		End If
	End Sub
	Function AddDetail() As Boolean
		Try
			Dim hdr As String = "INSERT INTO Invoice_Detail(Document,Document_Line,AR_Code,Ship_Date,Quantity,Unit_Price,Price_UofM," _
				& "Amount,Sales_Rep,Commission_Pct,Commissionable,Prepaid_Amt,Prepaid_Code,Reference,Note_Text,Last_Updated," _
				& "Order_Unit,Price_Unit_Conv,Tax_Code,Prepaid_Tax_Amount,CommissionIncluded,Packlist,SO_Detail,Job,Source_Type)"

			Dim vals As String = AddString(inv.Document)
			vals = vals & "," & DocLine
			vals = vals & "," & AddString(GLAccount,, "NULL")
			If sod.Delivery.ShippedDate = Date.MinValue Then
				vals = vals & ",NULL"
			Else
				vals = vals & "," & AddString(sod.Delivery.ShippedDate, DbType.Date, "NULL")
			End If

			vals = vals & "," & pld.Quantity
			vals = vals & "," & sod.UnitPrice
			vals = vals & "," & AddString(sod.PriceUofM)
			vals = vals & "," & pld.Quantity * sod.UnitPrice
			vals = vals & ",NULL,0,0,0,NULL,NULL,'Woo Integration'"
			vals = vals & "," & AddString(Now, DbType.DateTime)
			vals = vals & "," & AddString(sod.PriceUofM)
			vals = vals & ",1"
			vals = vals & "," & AddString(sod.TaxCode,, "NULL")
			vals = vals & ",0,0"
			vals = vals & "," & AddString(pld.Packlist,, "NULL")
			vals = vals & "," & AddString(sod.SODetail,, "NULL")
			vals = vals & "," & AddString(sod.SalesOrder,, "NULL")
			vals = vals & "," & SourceType

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "SELECT Invoice_Detail FROM Invoice_Detail WHERE Invoice_DetailKey=(SELECT SCOPE_IDENTITY())"
			InvoiceDetail = cmd.ExecuteScalar
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "Jobboss_Woo_Integration.cInvoiceDetail.AddDetail Error")
			AddError(ex.ToString)
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in Jobboss_Woo_Integration.cInvoiceDetail.AddDetail: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try

	End Function

End Class
