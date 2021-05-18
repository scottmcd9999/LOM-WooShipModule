Imports System.Data.SqlClient

Public Class cFinPrefs
	Property BaseCurrency As Integer
	Property ARAcct As String
	Property CurrPeriod As String
	Property NextPeriod As String
	Property PostInvoice As Boolean
	Property CurrPeriodStart As Date
	Property CurrPeriodEnd As Date
	Property NextPeriodStart As Date
	Property NextPeriodEnd As Date
	Property CurrFiscalYear As String
	Property NextFiscalYear As String
	Property ARDiscountAcct As String
	Property APAcct As String
	Property APDiscountAcct As String
	Property CurrAPPeriod As String
	Property NextAPPeriod As String
	Property CurrAPStartDate As Date
	Property CurrAPEndDate As Date
	Property CurrAPYear As String
	Property NextAPStartDate As Date
	Property NextAPEndDate As Date
	Property NextAPYear As String
	Property APGainLossForeignExchAcct As String

	Function Load(cmd As SqlCommand) As Boolean
		Try

			cmd.CommandText = "SELECT * FROM FinPref WHERE Preferences=1"
			Using rdr = cmd.ExecuteReader
				rdr.Read()
				CurrPeriod = rdr("AR_Fiscal_Per") & ""
				NextPeriod = rdr("AR_Next_Fiscal_Per") & ""
				ARAcct = rdr("AR_Account") & ""
				If Not IsDBNull(rdr("AR_Discount_Code")) Then
					ARDiscountAcct = rdr("AR_Discount_Code")
				End If
				APAcct = rdr("AP_Account") & ""
				If Not IsDBNull(rdr("AP_Discount")) Then
					APDiscountAcct = rdr("AP_Discount")
				End If
				CurrAPPeriod = rdr("AP_Fiscal_Per") & ""
				NextAPPeriod = rdr("AP_Next_Fiscal_Per") & ""
				If Not IsDBNull(rdr("AP_GLFE_Account")) Then
					APGainLossForeignExchAcct = rdr("AP_GLFE_Account")
				End If
			End Using

			cmd.CommandText = "SELECT * FROM Fiscal_Period WHERE Fiscal_Period='" & CurrPeriod & "'"
			Using rdr = cmd.ExecuteReader
				If rdr.HasRows Then
					rdr.Read()
					CurrPeriodEnd = rdr("End_Date")
					CurrPeriodStart = rdr("Start_Date")
					CurrFiscalYear = rdr("Fiscal_Year")
				End If
			End Using

			cmd.CommandText = "SELECT * FROM Fiscal_Period WHERE Fiscal_Period='" & NextPeriod & "'"
			Using rdr = cmd.ExecuteReader
				If rdr.HasRows Then
					rdr.Read()
					NextPeriodEnd = rdr("End_Date")
					NextPeriodStart = rdr("Start_Date")
					NextFiscalYear = rdr("Fiscal_Year")
				End If
			End Using

			cmd.CommandText = "SELECT * FROM Fiscal_Period WHERE Fiscal_Period='" & CurrAPPeriod & "'"
			Using rdr = cmd.ExecuteReader
				If rdr.HasRows Then
					rdr.Read()
					CurrAPEndDate = rdr("End_Date")
					CurrAPStartDate = rdr("Start_Date")
					CurrAPYear = rdr("Fiscal_Year")
				End If
			End Using
			cmd.CommandText = "SELECT * FROM Fiscal_Period WHERE Fiscal_Period='" & NextAPPeriod & "'"
			Using rdr = cmd.ExecuteReader
				If rdr.HasRows Then
					rdr.Read()
					NextAPEndDate = rdr("End_Date")
					NextAPStartDate = rdr("Start_Date")
					NextAPYear = rdr("Fiscal_Year")
				End If
			End Using

			cmd.CommandText = "SELECT * FROM Preferences"
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count > 0 Then
					Dim dtr As DataRow = dt.Rows(0)
					If Not IsDBNull(dtr("System_Base_Currency")) Then
						BaseCurrency = dtr("System_Base_Currency")
					End If
				End If
			End Using



			Return True
		Catch ex As Exception
			MessageBox.Show("Error in cFinPrefs.GetFinancialPreferences" & Environment.NewLine & Environment.NewLine & ex.ToString, "Error")
			Return False
		End Try

	End Function

	Function GetFinancialPreferences()
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand With {.Connection = con}
				Return Load(cmd)
			End Using
		End Using

	End Function
	Function GetFinancialPreferences(cmd As SqlCommand)
		Return Load(cmd)
	End Function

End Class
