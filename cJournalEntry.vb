Imports System.Data.SqlClient

Public Class cJournalEntry
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property GLAcct As String
	Property Refernce As String
	Property Source As String
	Property TransactionDate As Date
	Property FiscalPeriod As String
	Property Type As String
	Property Amount As Double
	Property Posted As Boolean
	Property CreationDate As Date
	Property OriginatingTrans As String
	Property Quantity As Double
	Property Partner As String
	Property CurrDef As Integer
	Property Unit As String
	Property ItemID As String
	Property cmd As SqlCommand
	Property ClassError As String

	Function AddJournalEntry(IsAuto As Boolean) As Boolean
		Try

			Dim finpref As New cFinPrefs
			If Not finpref.GetFinancialPreferences(cmd) Then
				ClassError = "Could not load Financial Preferences"
				Return False
			End If

			FiscalPeriod = GetFiscalPeriod(TransactionDate)

			Dim hdr As String = "INSERT INTO Journal_Entry (GL_Account, Reference, Source, Transaction_Date, Period, Type, Amount, Posted," _
						& " Creation_Date, Last_Updated, Originating_Transaction, Statement_Date, Line_Description, Quantity, Unit, Order_Number," _
						& " Partner_Type, Cust_Trans_Type, Partner, Item_ID, Item_Description, Currency_Def, Holder_Type, MFG_Holder_Type, MFG_Holder_ID)"

			Dim vals As String = AddString(GLAcct)
			vals = vals & "," & AddString(Refernce)
			vals = vals & "," & AddString(Source)
			vals = vals & "," & AddString(TransactionDate, DbType.Date)
			vals = vals & "," & AddString(FiscalPeriod)
			vals = vals & "," & AddString(Type)
			vals = vals & "," & Amount
			vals = vals & "," & Convert.ToInt32(Posted)
			vals = vals & "," & AddString(CreationDate, DbType.DateTime)
			vals = vals & "," & AddString(Now, DbType.DateTime)
			vals = vals & "," & AddString(OriginatingTrans)
			vals = vals & ",NULL,NULL" 'statement date, line desc
			vals = vals & "," & Quantity
			vals = vals & "," & AddString(Unit, DbType.String, "NULL")
			vals = vals & ",NULL,0,0" 'order num, part type, cust trans type
			vals = vals & "," & AddString(Partner)
			vals = vals & "," & AddString(ItemID, DbType.String, "NULL")
			vals = vals & ",NULL" 'item desc 
			vals = vals & "," & CurrDef
			vals = vals & ",NULL,NULL,NULL" 'holder type, mfg holder type, mfg holderid

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "ConcurIntegration.cJournalEntry.AddJournalEntry Error")
			ClassError = ex.ToString
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in ConcurIntegration.cJournalEntry.AddJournalEntry: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try

	End Function

End Class
