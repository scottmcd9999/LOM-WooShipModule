Imports System.Data.SQLClient
Public Class cReceipt
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
	Property Receipt As String
	Property Customer As String
	Property Bank As String
	Property ARCode As String
	Property Reference As String
	Property Type As String
	Property Amount As Double
	Property ReceiptDate As Date
	Property DepositDate As Date
	Property FiscalPeriod As String
	Property GLAccount As String
	Property Deposit As String
	Property Payer As String
	Property CustomerReceipt As String
	Property Notes As String
	Property cmd As SqlCommand
	Property ClassError As String
	Function AddReceipt(IsAuto As Boolean) As Boolean
		Try
			Dim hdr As String = "INSERT INTO Receipt(Receipt,Customer,Reference,Type,Amount,Deposit_Date,Receipt_Date,Payer,Last_Updated," _
						& "Period,GL_Account,Customer_Receipt,Note_Text)"

			Receipt = Guid.NewGuid.ToString.ToUpper
			Dim vals As String = AddString(Receipt)
			vals = vals & "," & AddString(Customer,, "NULL")
			vals = vals & "," & AddString(Reference,, "NULL")
			vals = vals & "," & AddString(Type,, "NULL")
			vals = vals & "," & AddString(Amount, DbType.Double)
			vals = vals & "," & AddString(DepositDate, DbType.Date, "NULL")
			vals = vals & "," & AddString(ReceiptDate, DbType.Date)
			vals = vals & "," & AddString(Payer,, "NULL")
			vals = vals & "," & AddString(Now, DbType.DateTime)
			vals = vals & "," & AddString(FiscalPeriod)
			vals = vals & "," & AddString(GLAccount,, "NULL")
			vals = vals & "," & AddString(CustomerReceipt,, "NULL")
			vals = vals & "," & AddString(Notes,, "NULL")

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			Return True

		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.cReceipt.AddReceipt Error")
			ClassError = ex.ToString
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in WooShipModule.cReceipt.AddReceipt: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
		Finally

		End Try


	End Function

End Class
