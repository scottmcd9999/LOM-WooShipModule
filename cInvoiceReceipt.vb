Imports System.Data.SQLClient
Public Class cInvoiceReceipt
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
	Property Invoice As String
	Property Receipt As String
	Property Amount As Double
	Property InvoiceReceipt
	'/1=discount, 0 = invoice 
	Property ApplicationType As Integer
	Property cmd As SqlCommand
	Property ClassErrors As String

	Function AddInvoiceReceipt(IsAuto As Boolean) As Boolean
		Try
			Dim hdr As String = "INSERT INTO Invoice_Receipt(Invoice,Receipt,Amount_Applied,Last_Updated,Invoice_Receipt,Application_Type)"

			Dim vals As String = AddString(Invoice)
			vals = vals & "," & AddString(Receipt)
			vals = vals & "," & Amount
			vals = vals & "," & AddString(Now, DbType.DateTime)
			InvoiceReceipt = Guid.NewGuid.ToString.ToUpper
			vals = vals & "," & AddString(InvoiceReceipt)
			vals = vals & "," & ApplicationType

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.cInvoiceReceipt.AddInvoiceReceipt Error")
			ClassErrors = ex.ToString
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in WooShipModule.cInvoiceReceipt.AddInvoiceReceipt: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try


	End Function
End Class
