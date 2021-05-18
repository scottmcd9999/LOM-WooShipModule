Imports System.Data.SqlClient
Imports DevExpress.XtraEditors
Public Class cBank
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Event Loaded()
	Event Saved()
	Event Dirty()
	Event Invalid()


	Property Bank As String
	Property CurrencyDef As Integer
	Property CurrencyConvRate As Integer
	Property GLAcct As String
	Property CheckFormat As String
	Property LastStatementDate As Date
	Property AccountNumber As String
	Property RoutingNumber As String
	Property CompanyLegalName As String
	Property CompanyID As String
	Property TaxNumber As String
	Property BalancedFile As Boolean
	Property BlockedFile As Boolean
	Property ClientNumber As String
	Property DestinationDataCenter As String
	Property BankLegalName As String
	Property IsRBC As Boolean
	Property PrenoteCode As String
	Property PrenoteAmount As Double
	Property ACHFormat As String
	Property OutputFormat As String
	Property ACHID As String
	Property BankID As String
	Property BankCountry As String
	Property BankCurrencyName As String
	Property BankCode As String
	Property LocationCode As String
	Property BranchCode As String
	Property NextCheck As Integer
	Property IsDirty As Boolean
	Property IsNew As Boolean
	Property IsAuto As Boolean
	ReadOnly Property Errors As String
		Get
			If errs Is Nothing Then
				Return String.Empty
			Else
				Return errs.ToString()
			End If

		End Get
	End Property
	Private errs As System.Text.StringBuilder

	Function LoadClass(dtr As DataRow, cmd As SqlCommand) As Boolean
		Bank = dtr("Code")
		CurrencyDef = dtr("Numeric1")
		GLAcct = dtr("Text1")
		CheckFormat = dtr("Text2")
		If Not IsDBNull(dtr("Date1")) Then
			LastStatementDate = dtr("Date1")
		End If
		cmd.CommandText = "SELECT Currency_Name FROM Currency_Def WHERE Currency_Def=" & CurrencyDef
		Dim obj As Object = cmd.ExecuteScalar
		If Not obj Is Nothing Then
			BankCurrencyName = obj.ToString
		Else
			BankCurrencyName = "USD"
		End If
		'Dim ce As New cEncrypt("rbg119!")
		'cmd.CommandText = "SELECT * FROM ACH_Banks WHERE Bank=" & AddString(BankName, DbType.String)
		'Using dt As New DataTable
		'	dt.Load(cmd.ExecuteReader)
		'	If dt.Rows.Count > 0 Then
		'		Dim dtr As DataRow = dt.Rows(0)
		'		AccountNumber = ce.DecryptData(dtr("Account_Number"))
		'		RoutingNumber = ce.DecryptData(dtr("Routing_Number"))
		'		CompanyLegalName = dtr("Company_Name")
		'		TaxNumber = ce.DecryptData(dtr("Tax_Number"))
		'		ClientNumber = ce.DecryptData(dtr("Client_Number"))
		'		DestinationDataCenter = ce.DecryptData(dtr("Destination_Data_Center"))
		'		BankLegalName = dtr("Bank_Name")
		'		BalancedFile = dtr("Balanced_File")
		'		BlockedFile = dtr("Blocked_File")
		'		IsRBC = dtr("RBC")
		'		PrenoteCode = dtr("Prenote_Code")
		'		PrenoteAmount = dtr("Prenote_Amount")
		'		ACHFormat = dtr("EFT_Format") & ""
		'		OutputFormat = dtr("Output_Format")
		'		BankCode = dtr("Bank_Code") & ""
		'		BranchCode = dtr("Branch_Code") & ""
		'		LocationCode = dtr("Location_Code") & ""
		'		BankCountry = dtr("Country_Code") & ""
		'		ACHID = dtr("ACH_ID") & ""
		'		BankID = dtr("Bank_ID") & ""
		'		CompanyID = dtr("Company_ID") & ""
		'		If Not IsDBNull(dtr("Starting_Check_Number")) Then
		'			NextCheck = dtr("Starting_Check_Number")
		'		End If

		'	Else
		'		logger.Info("Could not load ACH info for bank " & BankName)
		'		Return False
		'	End If
		'End Using
	End Function
	Function LoadPrimaryBank(cmd As SqlCommand) As Boolean
		Try
			cmd.CommandText = "SELECT * FROM User_Code WHERE Type='Banks' AND Numeric3=1"
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count = 0 Then
					Return False
				Else
					LoadClass(dt.Rows(0), cmd)
				End If
			End Using
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.cBank.LoadPrimaryBank Error")
			errs.AppendLine(ex.ToString)
			If Not IsAuto Then
				XtraMessageBox.Show(String.Format("Error in WooShipModule.cBank.LoadPrimaryBank: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try


	End Function
	Function Load(BankName As String) As Boolean
		Try
			logger.Info("BankName: " & BankName)
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand
					cmd.Connection = con
					cmd.CommandText = "SELECT * FROM User_Code WHERE Type='Banks' AND Code=" & AddString(BankName, DbType.String)
					Using dt As New DataTable
						dt.Load(cmd.ExecuteReader)
						If dt.Rows.Count > 0 Then
							Bank = BankName
							CurrencyDef = dt.Rows(0).Item("Numeric1")
							GLAcct = dt.Rows(0).Item("Text1")
							CheckFormat = dt.Rows(0).Item("Text2")
							If Not IsDBNull(dt.Rows(0).Item("Date1")) Then
								LastStatementDate = dt.Rows(0).Item("Date1")
							End If
							cmd.CommandText = "SELECT Currency_Name FROM Currency_Def WHERE Currency_Def=" & CurrencyDef
							Dim obj As Object = cmd.ExecuteScalar
							If Not obj Is Nothing Then
								BankCurrencyName = obj.ToString
							Else
								BankCurrencyName = "USD"
							End If
						Else
							logger.Info("Could not load for Bank " & BankName)
							Return False
						End If
					End Using
					'Dim ce As New cEncrypt("rbg119!")
					'cmd.CommandText = "SELECT * FROM ACH_Banks WHERE Bank=" & AddString(BankName, DbType.String)
					'Using dt As New DataTable
					'	dt.Load(cmd.ExecuteReader)
					'	If dt.Rows.Count > 0 Then
					'		Dim dtr As DataRow = dt.Rows(0)
					'		AccountNumber = ce.DecryptData(dtr("Account_Number"))
					'		RoutingNumber = ce.DecryptData(dtr("Routing_Number"))
					'		CompanyLegalName = dtr("Company_Name")
					'		TaxNumber = ce.DecryptData(dtr("Tax_Number"))
					'		ClientNumber = ce.DecryptData(dtr("Client_Number"))
					'		DestinationDataCenter = ce.DecryptData(dtr("Destination_Data_Center"))
					'		BankLegalName = dtr("Bank_Name")
					'		BalancedFile = dtr("Balanced_File")
					'		BlockedFile = dtr("Blocked_File")
					'		IsRBC = dtr("RBC")
					'		PrenoteCode = dtr("Prenote_Code")
					'		PrenoteAmount = dtr("Prenote_Amount")
					'		ACHFormat = dtr("EFT_Format") & ""
					'		OutputFormat = dtr("Output_Format")
					'		BankCode = dtr("Bank_Code") & ""
					'		BranchCode = dtr("Branch_Code") & ""
					'		LocationCode = dtr("Location_Code") & ""
					'		BankCountry = dtr("Country_Code") & ""
					'		ACHID = dtr("ACH_ID") & ""
					'		BankID = dtr("Bank_ID") & ""
					'		CompanyID = dtr("Company_ID") & ""
					'		If Not IsDBNull(dtr("Starting_Check_Number")) Then
					'			NextCheck = dtr("Starting_Check_Number")
					'		End If

					'	Else
					'		logger.Info("Could not load ACH info for bank " & BankName)
					'		Return False
					'	End If
					'End Using
				End Using
			End Using

			RaiseEvent Loaded()
			Return True

		Catch ex As Exception
			logger.Error(ex.ToString, "JobbossACH.cBank.Load Error")
			XtraMessageBox.Show(String.Format("Error in Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try

	End Function

	Function Validate() As Boolean
		errs = New System.Text.StringBuilder
		If String.IsNullOrEmpty(Bank) Then
			errs.AppendLine("  -- Jobboss Bank not selected")
		End If
		'If String.IsNullOrEmpty(AccountNumber) Then
		'	errs.AppendLine("  -- Account Number not entered")
		'End If
		'If String.IsNullOrEmpty(RoutingNumber) Then
		'	errs.AppendLine("  -- Routing Number not entered")
		'End If
		'If String.IsNullOrEmpty(CompanyLegalName) Then
		'	errs.AppendLine("  -- Company Legal Name not entered")
		'End If
		'If String.IsNullOrEmpty(TaxNumber) Then
		'	errs.AppendLine("  -- Tax Number not entered")
		'End If
		'If IsCanadian() Then
		'	If String.IsNullOrEmpty(ClientNumber) Then
		'		errs.AppendLine("  -- Client Number not entered")
		'	End If
		'	If String.IsNullOrEmpty(DestinationDataCenter) Then
		'		errs.AppendLine("  -- Destination Data Center not entered")
		'	End If
		'	If String.IsNullOrEmpty(ACHFormat) Then
		'		errs.AppendLine("  -- No EFT Format selected")
		'	End If
		'End If

		'If ACHFormat = "ISO 20022 format" Then
		'	If String.IsNullOrEmpty(BankCode) Then
		'		errs.AppendLine(" -- No Bank Code (20022 format)")
		'	End If
		'	If String.IsNullOrEmpty(BankCountry) Then
		'		errs.AppendLine(" -- No Bank Country Code (20022 format)")
		'	End If
		'	If String.IsNullOrEmpty(LocationCode) Then
		'		errs.Append(" -- No Location Code (20022 format)")
		'	End If
		'	If String.IsNullOrEmpty(ACHID) Then
		'		errs.Append(" -- No ACH ID value provided (20022 format)")
		'	End If
		'	If String.IsNullOrEmpty(BankID) Then
		'		errs.Append(" -- No Bank ID value provided (20022 format)")
		'	End If
		'End If

		Return errs.Length = 0
	End Function

	Function Save() As Boolean
		Try
			'If Not IsDirty Then
			'	Return True
			'End If
			'If Not Validate() Then
			'	RaiseEvent Invalid()
			'	Return False
			'End If
			''Dim ce As New cEncrypt("rbg119!")
			'Using con As New SqlConnection(JBConnection)
			'	con.Open()
			'	Using cmd As New SqlCommand
			'		cmd.Connection = con
			'		cmd.CommandText = "SELECT COUNT(*) FROM ACH_Banks WHERE Bank=" & AddString(Bank)
			'		If cmd.ExecuteScalar = 0 Then
			'			'/ adding a new bank:
			'			Dim vals As String
			'			Dim hdr As String = "INSERT INTO ACH_Banks([Bank],[Routing_Number],[Account_Number],[Company_Name],[Tax_Number],[Balanced_File]," _
			'				& "[Blocked_File],[Client_Number],[Destination_Data_Center],[Bank_Name],[RBC],[Prenote_Code],[Prenote_Amount],[EFT_Format], [Output_Format], " _
			'				& "Bank_Code,Branch_Code,Location_Code,Country_Code,ACH_ID,Bank_ID,Company_ID,Starting_Check_Number)"

			'			'vals = vals & "," & AddString(dtr("ID"), DbType.Double)
			'			vals = AddString(Bank, DbType.String, "NULL")
			'			vals = vals & "," & AddString(ce.EncryptData(RoutingNumber), DbType.String, "NULL")
			'			vals = vals & "," & AddString(ce.EncryptData(AccountNumber), DbType.String, "NULL")
			'			vals = vals & "," & AddString(CompanyLegalName, DbType.String, "NULL")
			'			vals = vals & "," & AddString(ce.EncryptData(TaxNumber), DbType.String, "NULL")
			'			vals = vals & "," & AddString(Convert.ToInt32(BalancedFile), DbType.Double)
			'			vals = vals & "," & AddString(Convert.ToInt32(BlockedFile), DbType.Double)
			'			vals = vals & "," & AddString(ce.EncryptData(ClientNumber), DbType.String, "NULL")
			'			vals = vals & "," & AddString(ce.EncryptData(DestinationDataCenter), DbType.String, "NULL")
			'			vals = vals & "," & AddString(BankLegalName, DbType.String, "NULL")
			'			vals = vals & "," & AddString(Convert.ToInt32(IsRBC), DbType.Double)
			'			vals = vals & "," & AddString(PrenoteCode, DbType.String, "0000")
			'			vals = vals & "," & AddString(PrenoteAmount, DbType.Double, "0")
			'			vals = vals & "," & AddString(ACHFormat, DbType.String, "NULL")
			'			vals = vals & "," & AddString(OutputFormat, DbType.String, AddString("Credit"))
			'			vals = vals & "," & AddString(BankCode, DbType.String, "NULL")
			'			vals = vals & "," & AddString(BranchCode, DbType.String, "NULL")
			'			vals = vals & "," & AddString(LocationCode, DbType.String, "NULL")
			'			vals = vals & "," & AddString(BankCountry, DbType.String, "NULL")
			'			vals = vals & "," & AddString(ACHID, DbType.String, "NULL")
			'			vals = vals & "," & AddString(BankID, DbType.String, "NULL")
			'			vals = vals & "," & AddString(CompanyID, DbType.String, "NULL")
			'			vals = vals & "," & AddString(NextCheck, DbType.Double, "0")
			'			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			'			'logger.Info(cmd.CommandText)
			'			cmd.ExecuteNonQuery()
			'		Else
			'			Dim vals As String
			'			Dim hdr As String = "UPDATE ACH_Banks SET "

			'			'vals = vals & ",[ID]=" & AddString(dtr("ID"), DbType.Double)
			'			'vals = "[Bank]=" & AddString(Bank, DbType.String, "NULL")
			'			vals = vals & "[Routing_Number]=" & AddString(ce.EncryptData(RoutingNumber), DbType.String, "NULL")
			'			vals = vals & ",[Account_Number]=" & AddString(ce.EncryptData(AccountNumber), DbType.String, "NULL")
			'			vals = vals & ",[Company_Name]=" & AddString(CompanyLegalName, DbType.String, "NULL")
			'			vals = vals & ",[Tax_Number]=" & AddString(ce.EncryptData(TaxNumber), DbType.String, "NULL")
			'			vals = vals & ",[Balanced_File]=" & AddString(Convert.ToInt32(BalancedFile), DbType.Double)
			'			vals = vals & ",[Blocked_File]=" & AddString(Convert.ToInt32(BlockedFile), DbType.Double)
			'			vals = vals & ",[Client_Number]=" & AddString(ce.EncryptData(ClientNumber), DbType.String, "NULL")
			'			vals = vals & ",[Destination_Data_Center]=" & AddString(ce.EncryptData(DestinationDataCenter), DbType.String, "NULL")
			'			vals = vals & ",[Bank_Name]=" & AddString(BankLegalName, DbType.String, "NULL")
			'			vals = vals & ",[Status]='Active'"
			'			vals = vals & ",[RBC]=" & AddString(Convert.ToInt32(IsRBC), DbType.Double)
			'			vals = vals & ",[Prenote_Code]=" & AddString(PrenoteCode, DbType.String, "0000")
			'			vals = vals & ",[Prenote_Amount]=" & AddString(PrenoteAmount, DbType.Double, "0")
			'			vals = vals & ",[EFT_Format]=" & AddString(ACHFormat, DbType.String, "NULL")
			'			vals = vals & ",[Output_Format]=" & AddString(OutputFormat, DbType.String, AddString("Credit"))
			'			vals = vals & ",Bank_Code=" & AddString(BankCode, DbType.String, "NULL")
			'			vals = vals & ",Branch_Code=" & AddString(BranchCode, DbType.String, "NULL")
			'			vals = vals & ",Location_Code=" & AddString(LocationCode, DbType.String, "NULL")
			'			vals = vals & ",Country_Code=" & AddString(BankCountry, DbType.String, "NULL")
			'			vals = vals & ",ACH_ID=" & AddString(ACHID, DbType.String, "NULL")
			'			vals = vals & ",Bank_ID=" & AddString(BankID, DbType.String, "NULL")
			'			vals = vals & ",Company_ID=" & AddString(CompanyID, DbType.String, "NULL")
			'			vals = vals & ",Starting_Check_Number=" & AddString(NextCheck, DbType.Double, "0")
			'			'logger.Info(cmd.CommandText)
			'			cmd.CommandText = hdr & vals & " WHERE Bank=" & AddString(Bank)
			'			cmd.ExecuteNonQuery()
			'		End If
			'	End Using
			'End Using

			'RaiseEvent Saved()
			'Return True

		Catch ex As Exception
			logger.Error(ex.ToString, "Save Error")
			XtraMessageBox.Show(String.Format("Error in Save: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function

	Function RemoveBank() As Boolean
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand
					cmd.Connection = con
					cmd.CommandText = "UPDATE ACH_Banks SET Status='Inactive' WHERE Bank=" & AddString(Bank)
					cmd.ExecuteNonQuery()
					Return True
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "RemoveBank Error")
			xtraMessageBox.Show(String.Format("Error in RemoveBank: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function
End Class
