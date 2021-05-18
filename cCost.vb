Imports System.Data.SqlClient
Public Class cCost
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property Account As String
	Property FiscalPeriod As String
	Property Amount As Double
	Property ClassError As String
	Property cmd As SqlCommand
	Function AddCost(IsAuto As Boolean) As Boolean
		Try
			cmd.CommandText = "SELECT COUNT(*) FROM Cost WHERE Account=" & AddString(Account) & " AND Period=" & AddString(FiscalPeriod)
			If cmd.ExecuteScalar = 0 Then
				cmd.CommandText = "INSERT INTO Cost(Account, Period, MTD_Amt, Last_Updated) VALUES(" & AddString(Account) & "," _
					& AddString(FiscalPeriod) & "," & Amount & "," & AddString(Now, DbType.DateTime) & ")"
			Else
				cmd.CommandText = "SELECT ISNULL(MTD_Amt,0) AS MtdAmt FROM Cost WHERE Account=" & AddString(Account) & " AND Period=" & AddString(FiscalPeriod)
				Dim amt As Double = cmd.ExecuteScalar
				cmd.CommandText = "UPDATE Cost SET MTD_Amt=" & amt + Amount & " WHERE Account=" & AddString(Account) & " AND Period=" & AddString(FiscalPeriod)
			End If
			cmd.ExecuteNonQuery()
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "ConcurIntegration.cCost.AddCost Error")
			ClassError = ex.ToString
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in ConcurIntegration.cCost.AddCost: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try
	End Function
End Class
