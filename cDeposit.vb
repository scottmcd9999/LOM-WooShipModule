Imports System.Data.SQLClient
Public Class cDeposit
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property Deposit As String
	Property Bank As String
	Property Status As String
	Property DepositDate As Date
	Property StatementDate As Date
	Property TradeCurrency As Integer
	Property CurrencyConvRate As Integer
	Property TradeDate As Date
	Property FixedRate As Integer
	Property BaseAmount As Double
	Property TradeAmount As Double
	Property cmd As SqlCommand
	Property ClassErrors As String

	Function AddDeposit(IsAuto As Boolean) As Boolean
		Try
			Dim hdr As String = "INSERT INTO Deposit(Deposit,Bank,Status,Deposit_Date,Statement_Date,Trade_Currency,Currency_Conv_Rate," _
						& "Trade_Date,Fixed_Rate,Base_Amount,Trade_Amount,Last_Updated)"

			Deposit = Guid.NewGuid.ToString.ToUpper
			Dim vals As String = AddString(Deposit)
			vals = vals & "," & AddString(Bank,, "NULL")
			vals = vals & "," & AddString(Status,, "Un-Reconciled")
			vals = vals & "," & AddString(DepositDate, DbType.Date)
			If StatementDate = Date.MinValue Then
				vals = vals & ",NULL"
			Else
				vals = vals & "," & AddString(StatementDate, DbType.Date, "NULL")
			End If
			vals = vals & "," & TradeCurrency
			vals = vals & "," & CurrencyConvRate
			If TradeDate = Date.MinValue Then
				vals = vals & ",NULL"
			Else
				vals = vals & "," & AddString(TradeDate, DbType.Date, "NULL")
			End If

			vals = vals & "," & FixedRate
			vals = vals & "," & BaseAmount
			vals = vals & "," & TradeAmount
			vals = vals & "," & AddString(Now, DbType.DateTime)

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.cDeposit.AddDeposit Error")
			ClassErrors = ex.ToString
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in WooShipModule.cDeposit.AddDeposit: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try


	End Function
End Class
