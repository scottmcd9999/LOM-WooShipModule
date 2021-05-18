Imports System.Data.SqlClient

Public Class cCurrencyDef
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
	Property CurrencyDef As String
	Property CurrencyName As String
	Property Symbol As String
	Property Description As String
	Property Base_Currency As Integer
	'Property Rate As Integer
	Property cmd As SqlCommand

	Function LoadCurrency(Currency As Integer, cmd As SqlCommand) As Boolean
		Try
			cmd.CommandText = "SELECT * FROM Currency_Def WHERE Currency_Def=" & AddString(Currency, DbType.Double)
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count > 0 Then
					CurrencyDef = dt.Rows(0).Item("Currency_Def")
					CurrencyName = dt.Rows(0).Item("Currency_Name")
					'Rate = dt.Rows(0).Item("Rate")
					Return True
				Else
					Return False
				End If
			End Using

		Catch ex As Exception
			logger.Error(ex.ToString, "JobbossACH.cCurrencyDef.LoadCurrency Error")
			MessageBox.Show(String.Format("Error in LoadCurrency: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function
	Function LoadCurrency(Currency As Integer) As Boolean
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand
					cmd.Connection = con
					cmd.CommandText = "SELECT * FROM Currency_Def WHERE Currency_Def=" & AddString(Currency, DbType.Double)
					Using dt As New DataTable
						dt.Load(cmd.ExecuteReader)
						If dt.Rows.Count > 0 Then
							CurrencyDef = dt.Rows(0).Item("Currency_Def")
							CurrencyName = dt.Rows(0).Item("Currency_Name")
							'Rate = dt.Rows(0).Item("Rate")
							Return True
						Else
							Return False
						End If
					End Using
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "JobbossACH.cCurrencyDef.LoadCurrency Error")
			MessageBox.Show(String.Format("Error in LoadCurrency: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function
	Function ConversionRate(SourceCurrency As Integer, TargetCurrency As Integer, cmd As SqlCommand) As Double
		Try
			cmd.CommandText = "SELECT TOP 1 * FROM Currency_Rate WHERE Source_Currency=" & SourceCurrency & " AND Target_Currency=" & TargetCurrency _
				& " ORDER BY Effective_Date DESC"
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count > 0 Then
					Return dt.Rows(0).Item("Rate")
				Else
					Return 1
				End If
			End Using

		Catch ex As Exception
			logger.Error(ex.ToString, "JobbossACH.cCurrencyDef.ConversionRate Error")
			MessageBox.Show(String.Format("Error in ConversionRate: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return 1
		Finally
		End Try
	End Function
	Function ConversionRate(SourceCurrency As Integer, TargetCurrency As Integer) As Double
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand
					cmd.Connection = con
					cmd.CommandText = "SELECT TOP 1 * FROM Currency_Rate WHERE Source_Currency=" & SourceCurrency & " AND Target_Currency=" & TargetCurrency _
						& " ORDER BY Effective_Date DESC"
					Using dt As New DataTable
						dt.Load(cmd.ExecuteReader)
						If dt.Rows.Count > 0 Then
							Return dt.Rows(0).Item("Rate")
						Else
							Return 1
						End If
					End Using
				End Using
			End Using

		Catch ex As Exception
			logger.Error(ex.ToString, "JobbossACH.cCurrencyDef.ConversionRate Error")
			MessageBox.Show(String.Format("Error in ConversionRate: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return 1
		Finally
		End Try
	End Function
End Class
