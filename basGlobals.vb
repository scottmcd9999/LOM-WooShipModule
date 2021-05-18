Imports System.Data.SqlClient
Imports LogicNP.CryptoLicensing
Imports Microsoft.VisualBasic.Interaction

Imports DevExpress.XtraEditors
Module basGlobals
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Public bIsAuto As Boolean
	Public con As SqlClient.SqlConnection
	Public conEDI As SqlClient.SqlConnection
	Public conSupport As SqlClient.SqlConnection

	Public cDemand_Need As String
	Public cDemand_Good As String
	Public cDemand_Header As String

	Public oDemand_Need As Object
	Public oDemand_Good As Object
	Public oDemand_Header As Object

	Public Const cSecPerHr As Double = 0.00027777777777777778
	Public Const cMinPerHr As Double = 0.016666666666666666

	'Public sVer As String = "2.0.13"
	Public sCustomer As String = ""

	Public gucSalesOrders As ucSalesOrders
	Public gucHelp As ucHelp
	Public gucSettings As ucSettings

	Public appDataPath As String = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) & "\ConcurIntegration"
	Public appFile As String = System.IO.Path.Combine(appDataPath, "app_log.log")


	Public ShowMatrix As Boolean
	Public SilentDBUpdate As Boolean
	Dim oCmn As Object

	Function crlf() As String
		Return Environment.NewLine & Environment.NewLine
	End Function
	Sub ShowMessage(Message As String, Caption As String, AutoClose As Integer)
		Try
			Dim args As New XtraMessageBoxArgs
			args.Caption = Caption
			args.Text = Message

			If AutoClose > 0 Then
				args.AutoCloseOptions.Delay = AutoClose * 1000
			End If

			args.Buttons = New DialogResult() {DialogResult.OK, DialogResult.Cancel}

			XtraMessageBox.Show(args).ToString()
		Catch ex As Exception
			logger.Error(ex.ToString, "SalesOrderImport.basGlobals.ShowMessage Error")
			XtraMessageBox.Show(String.Format("Error in SalesOrderImport.basGlobals.ShowMessage: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try

	End Sub

	Function GetDateStampForFile() As String
		Return Now.Year & Now.Month.ToString("00") & Now.Day.ToString("00")
	End Function
	Function GetTimeStampForFile() As String
		Return Now.Year & Now.Month.ToString("00") & Now.Day.ToString("00") & Now.Hour.ToString("00") & Now.Minute.ToString("00") & Now.Second.ToString("00")

	End Function

	Sub ExnHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
		Dim ex As Exception
		ex = e.ExceptionObject
		logger.Info(ex.StackTrace)
		ShowMessage("An error occurred in the Sales Order Import utility:" & Environment.NewLine & Environment.NewLine & ex.StackTrace, "Error", 0)
	End Sub
	Sub ThreadHandler(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
		ShowMessage("An error occurred in the Sales Order Import utility:" & Environment.NewLine & Environment.NewLine & e.Exception.StackTrace, "Error", 0)
	End Sub
	Function AppName() As String
		Try
			Dim ver As FileVersionInfo = FileVersionInfo.GetVersionInfo(Application.ExecutablePath)
			Return ver.FileDescription
		Catch ex As Exception
			logger.Error(ex.ToString, "SalesOrderImport.basGlobals.AppName Error")
			'XtraMessageBox.Show(String.Format("Error in SalesOrderImport.basGlobals.AppName: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return "<unknown>"
		Finally

		End Try
	End Function

	Function SaveConfigSetting(SettingName As String, SettingValue As Object, ConfigSection As String, ConfigID As Integer, cmd As SqlCommand, Optional DataType As System.Data.DbType = DbType.Boolean)

		Try
			If SettingValue Is Nothing Then
				Return True
			End If

			cmd.CommandText = "SELECT COUNT(*) FROM usr_Configuration_Settings WHERE Config_ID=" & ConfigID & " AND Config_Section=" _
				& AddString(ConfigSection) & " AND Option_Name=" & AddString(SettingName)
			If cmd.ExecuteScalar > 0 Then
				cmd.CommandText = "UPDATE usr_Configuration_Settings"
				Dim vals As String = "Option_Value=" & AddString(SettingValue)
				vals = vals & ", Last_Updated=" & AddString(Now, DbType.DateTime)
				cmd.CommandText = cmd.CommandText & " SET " & vals & " WHERE Config_ID=" & ConfigID _
					& " And Config_Section=" & AddString(ConfigSection) & " And Option_Name=" & AddString(SettingName)
			Else
				cmd.CommandText = "INSERT INTO usr_Configuration_Settings(Config_ID,Option_Name,Option_Value,Option_DataType,Config_Section,Last_Updated)"

				Dim vals As String = ConfigID
				vals = vals & "," & AddString(SettingName)
				vals = vals & "," & AddString(SettingValue)
				vals = vals & "," & AddString(DataType.ToString)
				vals = vals & "," & AddString(ConfigSection)
				vals = vals & "," & AddString(Now, DbType.DateTime)

				cmd.CommandText = cmd.CommandText & " VALUES(" & vals & ")"
			End If

			cmd.ExecuteNonQuery()

		Catch ex As Exception
			logger.Error(ex.ToString, "SaveConfigSetting Error")
			MessageBox.Show(String.Format("Error In SaveConfigSetting {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally
		End Try
	End Function

	Function GetConfigSetting(SettingName As String, ConfigSection As String, ConfigID As Integer, cmd As SqlCommand, Optional DefaultValue As String = "False") As String
		Try
			cmd.CommandText = "SELECT Option_Value, Option_DataType FROM usr_Configuration_Settings WHERE Option_Name=" & AddString(SettingName) _
					& " And Config_Section=" & AddString(ConfigSection) & " And Config_ID=" & ConfigID

			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count > 0 Then
					Dim dtr As DataRow = dt.Rows(0)
					Return dtr("Option_Value")

					'Select Case dtr("Option_DataType").ToString
					'	Case "Boolean"
					'		Return System.Int32.Parse(dtr("Option_Value")).ToString
					'	Case "String"
					'		If Not String.IsNullOrEmpty(dtr("Option_Value")) Then
					'			Return dtr("Option_Value")
					'		Else
					'			Return String.Empty
					'		End If

					'End Select
				Else
					Return DefaultValue
				End If
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "GetConfigSetting Error")
			MessageBox.Show(String.Format("Error in GetConfigSetting: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally
		End Try

	End Function
	Public Function GetUserFolder() As String
		If Not System.IO.Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ACH")) Then
			System.IO.Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ACH"))
		End If
		Return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ACH")
	End Function
	Public Function JBRound(ValueIn As Double, DecPlaces As Integer) As Double
		Try
			If oCmn Is Nothing Then
				oCmn = CreateObject("jbrpthlp.cls_Common")
			End If

			Return oCmn.JBRound(ValueIn, DecPlaces)
		Catch ex As Exception
			Return System.Math.Round(Convert.ToDecimal(ValueIn), DecPlaces)
		End Try
	End Function
	Public Function SaveLicenseKey(Key As String) As Boolean
		Try
			Dim val As String = String.Empty
			Dim basekey As Microsoft.Win32.RegistryKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64)
			Dim regkey = basekey.OpenSubKey("SOFTWARE\Jobboss Sales Order Import")
			If Not regkey Is Nothing Then
				regkey.SetValue("License", Key)
			Else
				basekey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32)
				regkey = basekey.OpenSubKey("SOFTWARE\Jobboss Sales Order Import")
				If Not regkey Is Nothing Then
					regkey.SetValue("License", Key)
				End If
			End If
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "SalesOrderImport.basGlobals.SaveLicenseKey Error")
			'XtraMessageBox.Show(String.Format("Error in SalesOrderImport.basGlobals.SaveLicenseKey: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally

		End Try
	End Function
	Public Function GetLicenseKey() As String
		Try
			Dim val As String = String.Empty
			Dim basekey As Microsoft.Win32.RegistryKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64)
			Dim regkey = basekey.OpenSubKey("SOFTWARE\Jobboss Sales Order Import")
			If Not regkey Is Nothing Then
				val = regkey.GetValue("License")
			End If

			If String.IsNullOrEmpty(val) Then
				'/ look in the other reg:
				basekey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry32)
				regkey = basekey.OpenSubKey("SOFTWARE\Jobboss Sales Order Import")
				If Not regkey Is Nothing Then
					val = regkey.GetValue("License")
				End If
			End If

			Return val

		Catch ex As Exception
			Return String.Empty
		End Try
	End Function
	Public Function CheckJobbossConnection(UserName As String, Password As String) As Boolean
		Try
			Using con As New SqlConnection
				con.ConnectionString = "Server=" & JBServer() & ";Database=" & JBDatabase() & ";User ID=" & UserName & ";Password=" & Password & ";Trusted_Connection=False;"
				Try
					con.Open()
				Catch ex As SqlException
					Return False
					'XtraMessageBox.Show("Error in  JobbossACH.fDBLogin.cmLogin_Click: " & Environment.NewLine & Environment.NewLine & ex.ToString, "Error")
				End Try
				Using cmd As New SqlCommand
					cmd.Connection = con
					cmd.CommandText = "SELECT COUNT(*) FROM Job"
					Try
						cmd.ExecuteScalar()
					Catch ex As SqlException
						Return False
					End Try
				End Using
			End Using
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "CheckJobbossConnection Error")
			XtraMessageBox.Show(String.Format("Error in CheckJobbossConnection: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function

	Public Function LoadCustomerShipTo(Customer As String, Combo As DevExpress.XtraEditors.LookUpEdit)
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				cmd.CommandText = "SELECT Ship_To_ID FROM Address WHERE Customer=" & AddString(Customer) & " ORDER BY Ship_To_ID"
				Using dt As New DataTable
					dt.Load(cmd.ExecuteReader)
					With Combo
						.Properties.DataSource = dt
						.Properties.ValueMember = "Ship_To_ID"
						.Properties.DisplayMember = "Ship_To_ID"
					End With
				End Using
			End Using
		End Using
	End Function
	Public Function CustomerName() As String
		Try
			Dim val As String = String.Empty
			Dim basekey As Microsoft.Win32.RegistryKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64)
			Dim regkey = basekey.OpenSubKey("SOFTWARE\Jobboss ACH")
			If Not regkey Is Nothing Then
				val = regkey.GetValue("Customer")
			End If

			Return val

		Catch ex As Exception
			Return String.Empty
		End Try
	End Function
	'Public Function IsCanadian() As Boolean
	'	Try
	'		'/ 06-15-2017, version 2.0.16: change to get the System_base_Currency value:
	'		Dim pref As New cFinPrefs
	'		If pref.GetFinancialPreferences() Then
	'			Dim curr As New cCurrencyDef
	'			If curr.LoadCurrency(pref.BaseCurrency) Then
	'				'/ Currency Name is ALWAYS the 3 digit code, like USD or CAD:
	'				If curr.CurrencyName = "USD" Then
	'					Return False
	'				Else
	'					Return True
	'				End If
	'			Else
	'				Return False
	'			End If
	'		Else
	'			Return False
	'		End If
	'	Catch ex As Exception
	'		Return False
	'	End Try
	'End Function
	Public Sub SetColors()

		Try
			Dim ccv As New ColorConverter

			cDemand_Need = ccv.ConvertToString(Color.Tomato)
			cDemand_Good = ccv.ConvertToString(Color.LightGoldenrodYellow)
			cDemand_Header = ccv.ConvertToString(Color.LightGray)

			oDemand_Good = ccv.ConvertFromString(cDemand_Good)
			oDemand_Need = ccv.ConvertFromString(cDemand_Need)
			oDemand_Header = ccv.ConvertFromString(cDemand_Header)

		Catch ex As Exception
			XtraMessageBox.Show("Error in  basGlobals.SetColors: " & Environment.NewLine & Environment.NewLine & ex.ToString, "Error")
		End Try
	End Sub
	Public Sub SaveDataGrid(dg As DataGridView)
		Try
			'/ move the focus off the current cell, then back on, to force commit
			dg.NotifyCurrentCellDirty(True)
			Dim OrigCellAddress As Point = New Point(dg.CurrentCellAddress.X, dg.CurrentCellAddress.Y)
			dg.CurrentCell = Nothing
			dg.CurrentCell = dg.Rows(OrigCellAddress.Y).Cells(OrigCellAddress.X)
		Catch ex As Exception
			'logger.Error(ex.ToString, "MagentoSync.basGlobals.SaveDataGrid Error")
			'xtraMessagebox.Show(String.Format("Error in SaveDataGrid: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally
		End Try
	End Sub

	Public Function JBServer() As String
		Dim s As Object = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\ODBC\ODBC.INI\jobboss32", "Server", Nothing)
		If s Is Nothing Then
			JBServer = ""
		Else
			JBServer = s.ToString
		End If

	End Function

	Public Function JBDatabase() As String
		Dim s As Object = My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\ODBC\ODBC.INI\jobboss32", "Database", Nothing)
		If s Is Nothing Then
			JBDatabase = ""
		Else
			JBDatabase = s.ToString
		End If

	End Function
	'Public Function InstallConnection() As String
	'	Return "Server=" & JBServer() & ";Database=" & JBDatabase() & ";User ID=" & mSAUser & ";Password=" & mSAPass & ";Trusted_Connection=False;"
	'End Function
	Public Function SupportConnection() As String
		Return "Server=" & JBServer() & ";Database=" & JBDatabase() & ";User ID=Support;Password=lonestar;Trusted_Connection=False;"
	End Function
	Public Function DevelopConnection() As String
		Return "Server=" & JBServer() & ";Database=" & JBDatabase() & ";User ID=Develop;Password=Congo;Trusted_Connection=False;"
	End Function
	Public Function JBConnection() As String
		Return "Server=" & JBServer() & ";Database=" & JBDatabase() & ";User ID=jobboss;Password=Bali;Trusted_Connection=False;"
	End Function

	Public Function JobbossName() As String
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand
					cmd.Connection = con
					cmd.CommandText = "SELECT a.Name FROM Address a INNER JOIN RptPref rp ON a.Address=rp.Address"
					Using dt As New DataTable
						dt.Load(cmd.ExecuteReader)
						If dt.Rows.Count > 0 Then
							Return dt.Rows(0).Item("Name").ToString
						Else
							Return ""
						End If
					End Using
				End Using
			End Using
		Catch ex As Exception
			'logger.Error(ex.ToString, "JobbossName Error")
			XtraMessageBox.Show(String.Format("Error in JobbossName: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally
		End Try
	End Function
	Public Function JobbossVersion() As String
		Try
			Using con As New SqlConnection
				con.ConnectionString = "Server=" & JBServer() & ";Database=JBSettings;User ID=jobboss;Password=Bali;Trusted_Connection=False;"
				con.Open()

				Using cmd As New SqlCommand
					cmd.Connection = con
					cmd.CommandText = "SELECT TOP(1) * FROM Version ORDER BY Version_Date DESC"
					Using dt As New DataTable
						dt.Load(cmd.ExecuteReader)
						If dt.Rows.Count > 0 Then
							Return dt.Rows(0).Item("Version_Number")
						Else
							Return ""
						End If
					End Using
				End Using
			End Using
		Catch ex As Exception
			'logger.Error(ex.ToString, "JobbossVersion Error")
			'XtraMessageBox.Show(String.Format("Error in JobbossVersion: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return ""
		Finally
		End Try
	End Function
	Public Function EDIDatabase() As String
		EDIDatabase = "EDI"
	End Function

	Public Function EDIConnection() As String
		Dim s As String = "Server=" & JBServer() & ";Database=" & EDIDatabase() & ";User ID=jobboss;Password=Bali;Trusted_Connection=False;"
		EDIConnection = s
	End Function
	Public Function GetSupportCon() As SqlClient.SqlConnection
		If conSupport Is Nothing Then
			conSupport = New SqlClient.SqlConnection
		End If

		If conSupport.State <> ConnectionState.Open Then
			conSupport = New SqlClient.SqlConnection
			conSupport.ConnectionString = "Server=" & JBServer() & ";Database=" & JBDatabase() & ";User ID=support;Password=lonestar;Trusted_Connection=False;"
			conSupport.Open()
		End If

		GetSupportCon = conSupport
	End Function

	'Public Function GetCon() As SqlClient.SqlConnection
	'	If con Is Nothing Then
	'		con = New SqlClient.SqlConnection
	'	End If

	'	If con.State <> ConnectionState.Open Then
	'		con = New SqlClient.SqlConnection
	'		con.ConnectionString = JBConnection()
	'		con.Open()
	'	End If

	'	GetCon = con

	'End Function

	Function CreateVendorEmailDatatable() As DataTable
		Dim dt As New DataTable
		With dt
			.Columns.Add("Select", GetType(Boolean))
			.Columns.Add("Vendor", GetType(String))
			.Columns.Add("Contact_ID", GetType(Int32))
			.Columns.Add("JB_Contact_ID", GetType(Int32))
			.Columns.Add("Contact_Name", GetType(String))
			.Columns.Add("Contact_Email", GetType(String))
		End With
		Return dt
	End Function
	Function CreateSOIMappingDataset() As DataTable
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				cmd.CommandText = "SELECT * FROM usr_Configuration_Import_Map WHERE 1=0"
				Dim dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				Dim r As DataRow = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Customer"
				r("Required") = 0
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Order Date"
				r("Required") = 1
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Purchase Order"
				r("Required") = 0
				r("Last_Updated") = Now
				dt.Rows.Add(r)
				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Ship To"
				r("Required") = 0
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Ship Via"
				r("Required") = 0
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Detail Line Number"
				r("Required") = 0
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Material"
				r("Required") = 1
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Revision"
				r("Required") = 0
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Promised Date"
				r("Required") = 1
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Order Quantity"
				r("Required") = 1
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Order Unit of Measure"
				r("Required") = 0
				r("Last_Updated") = Now
				dt.Rows.Add(r)

				r = dt.NewRow
				r("Config_ID") = 0
				r("Jobboss_Column") = "Unit Price"
				r("Required") = 1
				r("Last_Updated") = Now
				dt.Rows.Add(r)
				Return dt
			End Using
		End Using
	End Function
	Function CreateMappingDataset() As DataTable
		Dim dt As New DataTable

		With dt
			.Columns.Add("Item Name", GetType(String))
			.Columns.Add("Start Position", GetType(Integer))
			.Columns.Add("Length", GetType(Integer))
			'.Columns.Add("JobbossColumn", GetType(String))
		End With

		Dim dtr As DataRow
		dtr = dt.NewRow
		dtr("Item Name") = "Account Number"
		dtr("Start Position") = "0"
		dtr("Length") = "10"

		dt.Rows.Add(dtr)

		dtr = dt.NewRow
		dtr("Item Name") = "Transaction Code"
		dtr("Start Position") = "11"
		dtr("Length") = "1"

		dt.Rows.Add(dtr)

		dtr = dt.NewRow
		dtr("Item Name") = "Check Number"
		dtr("Start Position") = "13"
		dtr("Length") = "10"

		dt.Rows.Add(dtr)

		dtr = dt.NewRow
		dtr("Item Name") = "Amount"
		dtr("Start Position") = "24"
		dtr("Length") = "12"

		dt.Rows.Add(dtr)

		dtr = dt.NewRow
		dtr("Item Name") = "Check Date"
		dtr("Start Position") = "37"
		dtr("Length") = "8"

		dt.Rows.Add(dtr)

		dtr = dt.NewRow
		dtr("Item Name") = "Vendor"
		dtr("Start Position") = "46"
		dtr("Length") = 80 - 46
		dt.Rows.Add(dtr)

		Return dt
	End Function

	Public Function formatSDKDate(ByVal dte As String) As String
		Try
			Dim result As String = ""
			Dim currdate As Date
			Dim ccyy As String
			Dim mm As String
			Dim dd As String

			currdate = CDate(dte)
			ccyy = currdate.Year.ToString
			mm = currdate.Month.ToString.PadLeft(2).Replace(" ", "0")
			dd = currdate.Day.ToString.PadLeft(2).Replace(" ", "0")

			result = ccyy & mm & dd
			formatSDKDate = result
		Catch ex As Exception
			XtraMessageBox.Show(ex.ToString)
			Return ""
		End Try


	End Function

	Function FormatEDIDate(ByVal dte As String) As String
		Try
			Dim result As String = ""
			Dim currdate As Date
			Dim ccyy As String
			Dim mm As String
			Dim dd As String

			currdate = CDate(dte)
			ccyy = currdate.Year.ToString
			mm = currdate.Month.ToString.PadLeft(2).Replace(" ", "0")
			dd = currdate.Day.ToString.PadLeft(2).Replace(" ", "0")

			result = ccyy & "-" & mm & "-" & dd
			Return result
		Catch ex As Exception
			XtraMessageBox.Show(ex.ToString)
			Return ""
		End Try

	End Function

	Function FormatStringAsCurrency(StringIn As String) As String
		Try

			If StringIn = "0" Then
				Return "$0.00"
			End If

			Dim dAmt As Double = CDbl(StringIn)
			Dim sAmt As String = StringIn
			Dim sDec As String = "00"
			If dAmt - Math.Truncate(dAmt) <> 0 Then
				sDec = sAmt.Split(".")(1)

				If sDec.Length > 2 Then
					sDec = sDec.Substring(0, 2)
				Else
					If sDec.Length = 1 Then
						sDec = sDec & "0"
					End If
				End If
			End If

			sAmt = sAmt.Split(".")(0)
			If sAmt.Length = 0 Then
				sAmt = "0"
			End If
			sAmt = sAmt & "." & sDec
			sAmt = Convert.ToDouble(sAmt).ToString("###,###,###.00")
			'sAmt = dAmt.ToString

			Return "$" & sAmt '& "." & sDec

		Catch ex As Exception
			Return ""
		End Try
	End Function
	Function FormatEDITime(tim As String) As String
		Dim result As String = ""
		Dim currdate As Date
		Dim hh As String
		Dim mm As String
		Dim ss As String

		currdate = CDate(tim)
		hh = currdate.Hour.ToString.PadRight(2, "0")
		mm = currdate.Minute.ToString.PadLeft(2, "0") '.Replace(" ", "0")
		ss = currdate.Second.ToString.PadLeft(2, "0") ').Replace(" ", "0")

		result = hh & mm & ss
		Return result
	End Function

	Function CreateEmailDataset() As DataTable
		Dim dt As New DataTable
		With dt
			.Columns.Add("Vendor", GetType(String))
			.Columns.Add("Email", GetType(String))
			.Columns.Add("Invoice", GetType(String))
			.Columns.Add("InvoiceAmount", GetType(Double))
			.Columns.Add("CheckNumber", GetType(String))
			.Columns.Add("CheckAmount", GetType(Double))
			.Columns.Add("CheckDate", GetType(Date))
			.Columns.Add("InvoiceDate", GetType(Date))
			.Columns.Add("TotalCheckAmount", GetType(Double))
			.Columns.Add("DiscountAmount", GetType(Double))
			.Columns.Add("DiscountDate", GetType(Date))
		End With
		Return dt
	End Function

	Function CreateSODataset() As DataTable
		Dim dt As New DataTable
		With dt
			.Columns.Add("SalesOrder", GetType(String))
			.Columns.Add("Customer", GetType(String))
			.Columns.Add("PO", GetType(String))
			.Columns.Add("ShipTo", GetType(String))
			.Columns.Add("Material", GetType(String))
		End With
		Return dt
	End Function
	Function CreateACHDataset() As DataTable
		Dim dt As New DataTable
		With dt

			Dim sel As New DataColumn
			sel = .Columns.Add("Select")
			sel.DataType = GetType(Boolean)

			Dim apdoc As New DataColumn
			apdoc = .Columns.Add("AP_Document")
			apdoc.DataType = GetType(String)

			Dim doc As New DataColumn
			doc = .Columns.Add("Document")
			doc.DataType = GetType(String)

			Dim apchk As New DataColumn
			apchk = .Columns.Add("AP_Check")
			apchk.DataType = GetType(String)

			Dim chknbr As New DataColumn
			chknbr = .Columns.Add("Check_Number")
			chknbr.DataType = GetType(String)

			Dim vend As New DataColumn
			vend = .Columns.Add("Vendor")
			vend.DataType = GetType(String)

			Dim bank As New DataColumn
			bank = .Columns.Add("Bank")
			bank.DataType = GetType(String)

			Dim chkdate As New DataColumn
			chkdate = .Columns.Add("Check_Date")
			chkdate.DataType = GetType(Date)

			Dim chkstatus As New DataColumn
			chkstatus = .Columns.Add("Status")
			chkstatus.DataType = GetType(String)

			Dim chkposted As New DataColumn
			chkposted = .Columns.Add("Posted")
			chkposted.DataType = GetType(Integer)

			Dim docdate As New DataColumn
			docdate = .Columns.Add("Document_Date")
			docdate.DataType = GetType(DateTime)

			Dim amt As New DataColumn
			amt = .Columns.Add("Check_Amt")
			amt.DataType = GetType(Double)

			Dim openamt As New DataColumn
			openamt = .Columns.Add("Open_Amt")
			openamt.DataType = GetType(Double)

			'Dim discdate As New DataColumn
			'discdate.DataType = GetType(Date)
			'discdate.ColumnName = "Discount_Date"
			'.Columns.Add(discdate)

			Dim discamt As New DataColumn
			discamt.ColumnName = "Discount_Amt"
			discamt.DataType = GetType(Double)
			.Columns.Add(discamt)

			Dim duedate As New DataColumn
			duedate = .Columns.Add("Due_Date")
			duedate.DataType = GetType(DateTime)

			Dim remit As New DataColumn
			remit = .Columns.Add("Remit_To")
			remit.DataType = GetType(Double)

			Dim stat As New DataColumn
			stat = .Columns.Add("Error")
			stat.DataType = GetType(String)


			.Columns.Add("Log_AP_Check", GetType(String))
			.Columns.Add("International", GetType(Boolean))
			.Columns.Add("Trade_Currency", GetType(Integer))
			'Dim apcheck As New DataColumn
			'apcheck = .Columns.Add("AP_Check")
			'apcheck.DataType = GetType(String)

		End With

		Return dt

	End Function
	Function CreateVendorTable() As DataTable

		Dim dtVendor As New DataTable
		With dtVendor
			Dim col As New DataColumn

			col = .Columns.Add("ID")
			col.DataType = GetType(Integer)

			col = .Columns.Add("Vendor")
			col.DataType = GetType(String)

			col = .Columns.Add("Vendor_ID_Number")
			col.DataType = GetType(String)

			col = .Columns.Add("Vendor_Legal_Name")
			col.DataType = GetType(String)

			col = .Columns.Add("Routing_Number")
			col.DataType = GetType(String)

			col = .Columns.Add("Account_Number")
			col.DataType = GetType(String)

			col = .Columns.Add("Date_Added")
			col.DataType = GetType(Date)

			.Columns.Add("Use_Default_Email")
			.Columns.Add("Use_Contact_Title")
			.Columns.Add("Use_Selected_Contacts")
			.Columns.Add("International", GetType(Boolean))
			.Columns.Add("Bank_Name")
			.Columns.Add("Bank_Country")
			.Columns.Add("Bank_ID_Number")
		End With

		Return dtVendor


	End Function

	Function CreateDemandDataset() As DataTable
		Dim dt As New DataTable
		With dt
			Dim col As New DataColumn
			col = .Columns.Add("Part")
			col.DataType = GetType(String)

			col = .Columns.Add("Demand")
			col.DataType = GetType(String)

			col = .Columns.Add("OnHandQty")
			col.DataType = GetType(String)

			col = .Columns.Add("OnOrderQty")
			col.DataType = GetType(String)

			col = .Columns.Add("JobDue")
			col.DataType = GetType(String)

			col = .Columns.Add("PODue")
			col.DataType = GetType(String)

			col = .Columns.Add("Allocated")
			col.DataType = GetType(String)

			col = .Columns.Add("OrderPoint")
			col.DataType = GetType(String)

			col = .Columns.Add("ReorderQty")
			col.DataType = GetType(String)


			'col = .Columns.Add("Total")
			'col.DataType = GetType(String)

		End With

		Return dt

	End Function
	Function CreateJobDataset() As DataTable
		Dim dt As New DataTable
		With dt
			Dim col As New DataColumn
			'"SELECT 0 AS [Select], Job, Order_Date, Status, Part_Number, Rev, Description, (Order_Quantity + Extra_Quantity) AS [Job Qty], " _
			'                  & " In_Production_Quantity AS [In Production Qty], Completed_Quantity AS [Completed Qty], Order_Quantity, Extra_Quantity " _
			col = .Columns.Add("Select")
			col.DataType = System.Type.GetType("System.Boolean")
			col = .Columns.Add("Job")
			col.DataType = GetType(String)
			col.DefaultValue = ""
			col = .Columns.Add("Order Date")
			col.DataType = GetType(DateTime)
			col.DefaultValue = DBNull.Value
			col = .Columns.Add("Status")
			col.DataType = GetType(String)
			col.DefaultValue = ""
			col = .Columns.Add("Part_Number")
			col.DataType = GetType(String)
			col.DefaultValue = ""
			col = .Columns.Add("Rev")
			col.DataType = GetType(String)
			col.DefaultValue = ""
			col = .Columns.Add("Description")
			col.DataType = GetType(String)
			col.DefaultValue = ""
			col = .Columns.Add("Job Qty")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("In Production Qty")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("Completed Qty")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("Order_Quantity")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("Extra_Quantity")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("Customer")
			col.DataType = GetType(String)
			col = .Columns.Add("Customer_PO")
			col.DataType = GetType(String)
			col = .Columns.Add("PO_Line")
			col.DataType = GetType(String)
		End With

		Return dt
	End Function


	Function CreateMPDataset() As DataTable
		Dim dt As New DataTable

		With dt
			Dim col As New DataColumn
			'col = .Columns.Add("Process")
			'col.DataType = System.Type.GetType("System.Boolean")

			col = .Columns.Add("Material")
			col.DataType = GetType(String)
			col.DefaultValue = ""
			col = .Columns.Add("Description")
			col = .Columns.Add("Demand")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col.DataType = GetType(String)
			col = .Columns.Add("JobDueToStock")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("PODueToStock")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("OnHandQty")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("OnOrderQty")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("Allocated")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("OrderPoint")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("ReorderQty")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("Balance")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0
			col = .Columns.Add("ActualQty")
			col.DataType = GetType(Integer)
			col.DefaultValue = 0


		End With

		Return dt
	End Function


	Public Function DTAdd(Datain As Object, DataType As System.Data.DbType, Optional ValueIfNull As String = "") As Object

		Try
			If IsDBNull(Datain) Then
				Return DBNull.Value
			Else
				If String.IsNullOrEmpty(Datain) Then
					Return DBNull.Value
				Else
					Select Case DataType
						Case DbType.String
							Return Datain.ToString
						Case DbType.Boolean
							If Datain = True Then
								Return "True"
							Else
								Return "False"
							End If
							Return Datain
						Case DbType.DateTime
							If IsDate(Datain) Then
								Return "'" & FormatISODate(Datain) & "'"
							Else
								Return DBNull.Value
							End If
						Case DbType.Date
							If IsDate(Datain) Then
								Dim year As String
								Dim month As String
								Dim day As String

								year = CDate(Datain).Year
								month = CDate(Datain).Month.ToString.PadLeft(2, "0")
								day = CDate(Datain).Day.ToString.PadLeft(2, "0")

								Return "'" & year & "-" & month & "-" & day & "'"
							Else
								Return DBNull.Value
							End If

						Case DbType.Double
							Return Datain
						Case Else
							Return Datain
					End Select
				End If
			End If
		Catch ex As Exception
			Return DBNull.Value
		End Try

	End Function
	Public Function AddString(DataIn As Object, Optional DataType As System.Data.DbType = DbType.String, Optional ValueIfNull As String = "", Optional IncludeDelimiters As Boolean = True) As String
		Try

			Dim sDelim As String

			If IncludeDelimiters Then
				sDelim = "'"
			Else
				sDelim = ""
			End If

			If IsDBNull(DataIn) Then
				If ValueIfNull <> "" Then
					Return ValueIfNull
				Else
					Select Case DataType
						Case DbType.String
							Return "NULL"
						Case DbType.Boolean
							Return "False"
						Case DbType.Date
							Return "NULL"
						Case DbType.Double
							Return ""
						Case Else
							Return ""
					End Select

				End If
			Else
				If String.IsNullOrEmpty(DataIn) Then
					If ValueIfNull <> "" Then
						Select Case DataType
							Case DbType.String
								If ValueIfNull = "NULL" Then
									Return ValueIfNull
								Else
									Return "'" & ValueIfNull & "'"
								End If
							Case DbType.Boolean
								Return ValueIfNull
							Case DbType.Date
								Return "'" & ValueIfNull & "'"
							Case DbType.Double
								Return ValueIfNull
							Case Else
						End Select
						Return ValueIfNull
					Else
						Select Case DataType
							Case DbType.String
								Return "''"
							Case DbType.Boolean
								Return "False"
							Case DbType.Date
								Return "NULL"
							Case DbType.Double
								Return "0"
							Case Else
								Return ""
						End Select

					End If
				Else
					Select Case DataType
						Case DbType.String
							Return sDelim & DataIn.ToString.Replace("'", "''") & sDelim
						Case DbType.Boolean
							If DataIn = True Then
								Return "True"
							Else
								Return "False"
							End If
							Return DataIn
						Case DbType.DateTime
							If IsDate(DataIn) Then
								Return "'" & FormatISODate(DataIn) & "'"
							Else
								Return "NULL"
							End If
						Case DbType.Date
							If IsDate(DataIn) Then
								Dim year As String = CDate(DataIn).Year
								Dim month As String = CDate(DataIn).Month.ToString.PadLeft(2, "0")
								Dim day As String = CDate(DataIn).Day.ToString.PadLeft(2, "0")

								Return "'" & year & "-" & month & "-" & day & "'"
							Else
								Return "NULL"
							End If

						Case DbType.Double
							Return DataIn
						Case Else
							Return DataIn
					End Select
				End If
			End If
		Catch ex As Exception
			Return ""
			'System.Windows.Forms.xtraMessagebox.Show(ex.ToString)
		End Try
	End Function
	Public Function FormatISODate(DateIn As String) As String
		Try

			Dim ld As DateTime = CDate(DateIn) '/ CDate(ds.Tables("SO").Rows(0).Item("Last_Updated").ToString)

			'YYYY-MM-DDThh:mm:ss
			Dim dd As String = ld.Day.ToString.PadLeft(2).Replace(" ", "0")
			Dim mm As String = ld.Month.ToString.PadLeft(2).Replace(" ", "0")
			Dim yyyy As String = ld.Year.ToString

			Dim hh As String = ld.Hour.ToString.PadLeft(2).Replace(" ", "0")
			Dim min As String = ld.Minute.ToString.PadLeft(2).Replace(" ", "0")
			Dim ss As String = ld.Second.ToString.PadLeft(2).Replace(" ", "0")

			Return yyyy & "-" & mm & "-" & dd & "T" & hh & ":" & min & ":" & ss
		Catch ex As Exception
			XtraMessageBox.Show(ex.ToString)
			Return ""
		End Try

	End Function
	Function GetDefaultRemitAddress(Vendor As String, cmd As SqlCommand) As Integer
		cmd.CommandText = "SELECT Address FROM Address WHERE Vendor=" & AddString(Vendor) & " AND Type LIKE '_1_'"
		Dim obj As Object
		obj = cmd.ExecuteScalar
		If obj Is Nothing Then
			Return 0
		Else
			Return obj
		End If
	End Function
	Function GetCurrencyCode(CurrencyCode As Integer, cmd As SqlCommand) As String
		cmd.CommandText = "SELECT Currency_Name FROM Currency_Def WHERE Currency_Def=" & CurrencyCode
		Dim obj As Object
		obj = cmd.ExecuteScalar
		If Not obj Is Nothing Then
			Return obj.ToString
		Else
			Return ""
		End If
	End Function
	Function GetOrderQuantity(SODetail As Integer) As Integer
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				cmd.CommandText = "SELECT Order_Qty FROM SO_Detail WHERE SO_Detail=" & SODetail
				Dim obj As Object = cmd.ExecuteScalar
				If obj Is Nothing Then
					Return 0
				Else
					Return obj
				End If
			End Using
		End Using
	End Function
	Function GetShippedQuantity(SODetail As Integer) As Integer
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				cmd.CommandText = "SELECT Shipped_Qty FROM SO_Detail WHERE SO_Detail=" & SODetail
				Dim obj As Object = cmd.ExecuteScalar
				If obj Is Nothing Then
					Return 0
				Else
					Return obj
				End If
			End Using
		End Using
	End Function
	Function GetPromisedDate(SODetail As Integer) As String
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				'/ Get the ShipTo from the mapping table:
				cmd.CommandText = "SELECT Promised_Date FROM SO_Detail WHERE SO_Detail=" & SODetail
				Dim obj As Object = cmd.ExecuteScalar
				If obj Is Nothing Then
					Return ""
				Else
					Dim promdate As String = obj.ToString

					Return obj.ToString
				End If
			End Using
		End Using
	End Function
	Function ValidateShipToForVendor(ShipTo As String, Vendor As String, cmd As SqlCommand) As Boolean
		cmd.CommandText = "SELECT COUNT(*) FROM Address WHERE Vendor=" & AddString(Vendor) & " AND Ship_To_ID=" & AddString(ShipTo) _
			& " AND Billable=1"
		Return cmd.ExecuteScalar

	End Function
	Function GetAddressFromCustomerShipTo(Customer As String, ShipTo As String, ConfigID As Integer) As Integer
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				'/ Get the ShipTo from the mapping table:
				cmd.CommandText = "SELECT Jobboss_Value FROM usr_Import_Jobboss_Maps WHERE Map_Type='ShipTo' AND Customer= " & AddString(Customer) _
					& " AND Import_Value=" & AddString(ShipTo) & " AND Config_ID=" & ConfigID
				Dim obj As Object = cmd.ExecuteScalar
				If obj Is Nothing Then
					'/ the ship to was not mapped, so see if we have one with the same name for the Customer:
					cmd.CommandText = "SELECT Address FROM Address WHERE Customer=" & AddString(Customer) & " AND Ship_To_ID=" & AddString(ShipTo)
					Return cmd.ExecuteScalar
				Else
					Return Convert.ToInt32(obj.ToString)
				End If
			End Using
		End Using

	End Function
	Function GetAddressNameFromCustomerShipTo(Customer As String, ShipTo As String, ConfigID As Integer) As String
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				'/ Get the ShipTo from the mapping table:
				cmd.CommandText = "SELECT Jobboss_Value FROM usr_Import_Jobboss_Maps WHERE Map_Type='ShipTo' AND Customer= " & AddString(Customer) _
					& " AND Import_Value=" & AddString(ShipTo) & " AND Config_ID=" & ConfigID
				Dim obj As Object = cmd.ExecuteScalar
				If obj Is Nothing Then
					'/ the ship to was not mapped, so see if we have one with the same name for the Customer:
					cmd.CommandText = "SELECT COUNT(*) FROM Address WHERE Customer=" & AddString(Customer) & " AND Ship_To_ID=" & AddString(ShipTo)
					If cmd.ExecuteScalar > 0 Then
						Return ShipTo.ToUpper
					Else
						Return String.Empty
					End If

				Else
					cmd.CommandText = "SELECT Ship_To_ID FROM Address WHERE Address=" & Convert.ToInt32(obj.ToString)
					Dim ship As Object = cmd.ExecuteScalar
					If ship Is Nothing Then
						Return String.Empty
					Else
						Return ship.ToString.ToUpper
					End If

				End If
			End Using
		End Using

	End Function
	Function GetFiscalPeriod(DateIn As Date) As String
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				'cmd.CommandText = "SELECT Fiscal_Period FROM Fiscal_Period WHERE Start_Date>='" & FormatISODate(DateIn) & "' ANd End_Date<='" & FormatISODate(DateIn) & "'"

				cmd.CommandText = "SELECT TOP 1 * FROM Fiscal_Period WHERE Start_Date<='" & FormatEDIDate(DateIn) & "' ORDER BY Start_Date DESC"
				Using dt As New DataTable
					dt.Load(cmd.ExecuteReader)
					If dt.Rows.Count > 0 Then
						'/ there will be only one:
						Return dt.Rows(0).Item("Fiscal_Period")
					Else
						Return ""
					End If
				End Using
			End Using
		End Using
	End Function
	Function NextPacklistNumber(cmd As SqlCommand, IsAuto As Boolean) As String
		Try
			Dim so As String = "1"
			cmd.CommandText = "SELECT Last_Nbr FROM Auto_Number WHERE Type='Packlist'"

			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count > 0 Then
					so = dt.Rows(0).Item("Last_Nbr")
				End If
			End Using

			Dim ok As Boolean

			Do Until ok = True
				cmd.CommandText = "SELECT COUNT(*) FROM Packlist_Header WHERE Packlist='" & so & "'"
				If cmd.ExecuteScalar = 0 Then
					ok = True
				Else
					so = (CDbl(so) + 1).ToString
				End If
			Loop

			Return so
		Catch ex As Exception
			logger.Error(ex.ToString, "NextPacklistNumber Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in NextPacklistNumber: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If

			Return "0"
		Finally
		End Try
	End Function
	Function NextARInvoiceNumber(cmd As SqlCommand, IsAuto As Boolean) As String
		Try
			Dim so As String = "1"
			cmd.CommandText = "SELECT Last_Nbr FROM Auto_Number WHERE Type='Invoice'"

			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count > 0 Then
					so = dt.Rows(0).Item("Last_Nbr")
				End If
			End Using

			Dim ok As Boolean

			Do Until ok = True
				cmd.CommandText = "SELECT COUNT(*) FROM Invoice_Header WHERE Document='" & so & "'"
				If cmd.ExecuteScalar = 0 Then
					ok = True
				Else
					so = (CDbl(so) + 1).ToString
				End If
			Loop

			Return so
		Catch ex As Exception
			logger.Error(ex.ToString, "NexARInvoiceNumber Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in NextArInvoiceNumber: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If

			Return "0"
		Finally
		End Try
	End Function
	Function NextSONumber(cmd As SqlCommand) As String
		Try
			Dim so As String = "1"
			cmd.CommandText = "SELECT Last_Nbr FROM Auto_Number WHERE Type='SalesOrder'"

			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count > 0 Then
					so = dt.Rows(0).Item("Last_Nbr")
				End If
			End Using

			Dim ok As Boolean

			Do Until ok = True
				cmd.CommandText = "SELECT COUNT(*) FROM SO_Header WHERE Sales_Order='" & so & "'"
				If cmd.ExecuteScalar = 0 Then
					ok = True
				Else
					so = (CDbl(so) + 1).ToString
				End If
			Loop

			Return so
		Catch ex As Exception
			logger.Error(ex.ToString, "MMT_VOI_SP.basGlobals.NextSONumber Error")
			MessageBox.Show(String.Format("Error in NextSONumber: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return "0"
		Finally
		End Try
	End Function
	'Sub ConfigureUpdates()

	'	Dim updateModulePath As String = System.IO.Path.Combine(Application.StartupPath, "updater.exe")
	'	Dim proc As Process = Process.Start(updateModulePath, "/configure")

	'End Sub

	'Sub CheckForUpdates()
	'	Try
	'		'/ we don't allow expired systems to update:
	'		Dim lic As New cLicensing
	'		lic.LoadLicense()

	'		If Not lic.LicenseLoaded Then
	'			XtraMessageBox.Show("The ACH system was not able to load your license key. Try restarting the ACH system, and if that does not work, contact Jobboss support.", "Unable to Load License", MessageBoxButtons.OK, MessageBoxIcon.Error)
	'			End
	'		End If

	'		Dim updateModulePath As String = System.IO.Path.Combine(Application.StartupPath, "updater.exe")
	'		Dim url As String
	'		If lic.IsEvaluation Then
	'			If lic.LicenseStatus = LicenseStatus.EvaluationExpired Or lic.LicenseStatus = LicenseStatus.EvaluationlTampered Or lic.DaysRemaining = 0 Then
	'				XtraMessageBox.Show("Your Jobboss ACH Evaluation has expired. Please contact Jobboss to purchase this software", "Evaluation Period Expired", MessageBoxButtons.OK, MessageBoxIcon.Information)
	'				End
	'			End If
	'			'/ if the license if not expired, we get the update from the evaluation URL:
	'			url = "http://www.infotrakker.com/ach/updates/trial/ACH30TrialUpdates.txt"
	'		Else
	'			url = "http://www.infotrakker.com/ach/updates/ACH30Updates.txt"
	'		End If


	'		Dim proc As Process = Process.Start(updateModulePath, "/checknow -showwaitdialog -url " & url)
	'		proc.WaitForExit()
	'		Dim ret As Integer = proc.ExitCode

	'		MsgBox(ret.ToString)



	'		'Dim val As String = String.Empty
	'		'Dim basekey As Microsoft.Win32.RegistryKey = Microsoft.Win32.RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, Microsoft.Win32.RegistryView.Registry64)
	'		'Dim regkey = basekey.OpenSubKey("SOFTWARE\Jobboss ACH")
	'		'If Not regkey Is Nothing Then
	'		'	val = regkey.GetValue("UpdateURL", String.Empty)
	'		'End If

	'		'If lic.IsEvaluation Then
	'		'	If lic.LicenseStatus = LicenseStatus.EvaluationExpired Or lic.LicenseStatus = LicenseStatus.EvaluationlTampered Or lic.DaysRemaining = 0 Then
	'		'		XtraMessageBox.Show("Your Jobboss ACH Evaluation has expired. Please contact Jobboss to purchase this software", "Evaluation Period Expired", MessageBoxButtons.OK, MessageBoxIcon.Information)
	'		'		End
	'		'	End If
	'		'	'/ if the license if not expired, we get the update from the evaluation URL:
	'		'	If val.Length > 0 Then
	'		'		'/ change the updates directory for Eval versions:
	'		'		val = val & "/evaluation"
	'		'	End If
	'		'End If

	'		'Dim updateFileName As String = "Jobboss ACHSetup.txt"
	'		'updateFileName = updateFileName.Replace(" ", "%20")
	'		''/ do we have the twuxW.exe file?
	'		'If IO.File.Exists(System.IO.Path.Combine(Application.StartupPath, "twuxW.exe")) Then
	'		'	'/ got the Update URL, so we try the update:
	'		'	If val.Length > 0 Then
	'		'		val = val & "/" & updateFileName
	'		'		Dim bbq As Process = Process.GetCurrentProcess '.GetProcessesByName("BBQScoring")
	'		'		'MsgBox(System.IO.Path.Combine(Application.StartupPath, "twuxW.exe"))
	'		'		Dim prc As New ProcessStartInfo
	'		'		prc.FileName = System.IO.Path.Combine(Application.StartupPath, "twuxW.exe")
	'		'		prc.Arguments = " /w:" & bbq.MainWindowHandle.ToString & " " & val
	'		'		prc.WindowStyle = ProcessWindowStyle.Normal

	'		'		Dim p As Process = Process.Start(prc)
	'		'	End If

	'		'End If
	'	Catch ex As Exception
	'		XtraMessageBox.Show("Error in  JobbossACH.basGlobals.CheckForUpdates: " & Environment.NewLine & Environment.NewLine & ex.ToString, "Error")
	'	End Try

	'End Sub

	Function ACHTableBuild() As String

		'/ 04-26-2016: Added ACH_Vendor_Contacts, and Date_Added column to ach_Vendors table
		'/ 07-11-2016: Added Return_Account_Number, Bank_Account_Number and Client_Number to ACH_Settings

		Dim s As String = "IF OBJECT_ID('dbo.ACH_Log', 'U') IS NOT NULL DROP TABLE dbo.ACH_Log;" _
						  & " IF OBJECT_ID('dbo.ACH_Vendors', 'U') IS NOT NULL DROP TABLE dbo.ACH_Vendors;" _
						  & " IF OBJECT_ID('dbo.ACH_Settings', 'U') IS NOT NULL DROP TABLE dbo.ACH_Settings;" _
						  & " IF OBJECT_ID('dbo.ACH_Vendor_Contacts', 'U') IS NOT NULL DROP TABLE dbo.ACH_Vendor_Contacts;"

		s = s & " CREATE TABLE [dbo].[ACH_Log]([ID] [int] IDENTITY(1,1) NOT NULL,[Date_Processed] [datetime] NULL," _
			& " [AP_Check] [varchar](50) NULL,[Vendor] [varchar](50) NULL,[Check_Number] [varchar](50) NULL," _
			& " [Check_Date] [date] NULL,[Check_Amount] [money] NULL,[Email_Sent] [bit] NULL,[Bank] [varchar](50) NULL," _
			& " CONSTRAINT [PK_ACH_Log] PRIMARY KEY CLUSTERED ([ID] Asc) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF," _
			& " IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY];" _
			& " ALTER TABLE [dbo].[ACH_Log] ADD  CONSTRAINT [DF_ACH_Log_Date_Processed]  DEFAULT (getdate()) FOR [Date_Processed];" _
			& " ALTER TABLE [dbo].[ACH_Log] ADD  CONSTRAINT [DF_ACH_Log_Email_Sent]  DEFAULT ((0)) FOR [Email_Sent]"


		s = s & "CREATE TABLE dbo.ACH_Vendors (" _
			& " ID Int IDENTITY," _
			& " Vendor varchar(max) NULL," _
			& " Vendor_Legal_Name varchar(max) NULL," _
			& " Vendor_ID_Number varchar(max) NULL," _
			& " Routing_Number varchar(max) NULL," _
			& " Account_Number varchar(max) NULL," _
			& " Use_Default_Email bit NULL CONSTRAINT DF_ACH_Vendors_Use_Default_Email DEFAULT (0)," _
			& " Use_Contact_Title varchar(50) NULL," _
			& " Use_Selected_Contacts bit NULL CONSTRAINT DF_ACH_Vendors_Use_Selected_Contacts DEFAULT (0)," _
			& " Date_Added DateTime NULL CONSTRAINT DF_ACH_Vendors_Date_Added DEFAULT (getdate())," _
			& " International bit NULL DEFAULT (0)," _
			& " Bank_Name varchar(max) NULL," _
			& " Bank_Country varchar(max) NULL," _
			& " Bank_ID_Number varchar(max) NULL" _
			& " )" _
			& " ON [PRIMARY]" _
			& " TEXTIMAGE_ON [PRIMARY];"

		s = s & "CREATE TABLE [dbo].[ACH_Version](" _
			& " [ID] [Int] IDENTITY(1, 1) Not NULL," _
			& "	[Version] [VARCHAR](50) NULL," _
			& " [Updated_On] [DateTime] NULL," _
			& " Constraint [PK_ACH_Version] PRIMARY KEY CLUSTERED " _
			& " ([ID] Asc) WITH (PAD_INDEX = OFF, " _
			& " STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]) ON [PRIMARY];"


		s = s & "ALTER TABLE [dbo].[ACH_Version] ADD  CONSTRAINT [DF_ACH_Version_Updated_On]  DEFAULT (GETDATE()) FOR [Updated_On];"

		's = s & " CREATE TABLE [dbo].[ACH_Vendors]([ID] [int] IDENTITY(1,1) NOT NULL,[Vendor] [varchar](50) NULL,[Vendor_Legal_Name] [varchar](50) NULL," _
		'	& " [Vendor_ID_Number] [varchar](50) NULL,[Routing_Number] [varchar](MAX) NULL,[Account_Number] [varchar](MAX) NULL," _
		'	& " [Use_Default_Email] [BIT] NULL CONSTRAINT [DF_ACH_Vendors_Use_Default_Email]  DEFAULT ((0))," _
		'	& " [Use_Contact_Title] [VARCHAR](50) NULL," _
		'	& " [Use_Selected_Contacts] [BIT] NULL CONSTRAINT [DF_ACH_Vendors_Use_Selected_Contacts] DEFAULT ((0))," _
		'	& " [Date_Added] [DATETIME] NULL CONSTRAINT DF_ACH_Vendors_Date_Added DEFAULT (getdate())," _
		'	& " [Bank_Name] [varchar](50) NULL,[Bank_Country] [varchar](50) NULL,[Bank_ID_Number] [varchar](50) NULL," _
		'	& " [International] [BIT] NULL DEFAULT (0))" _
		'	& " ON PRIMARY;"

		s = s & " CREATE TABLE [dbo].[ACH_Vendor_Contacts]([ID] [INT] IDENTITY(1,1) NOT NULL,[ACH_Vendor_ID] [INT] NULL,[Contact] [INT] NULL," _
			& " CONSTRAINT [PK_ACH_Vendor_Contacts] PRIMARY KEY CLUSTERED ([ID] ASC)" _
			& " WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]); "

		s = s & " CREATE TABLE [dbo].[ACH_Settings]([ID] [int] IDENTITY(1,1) NOT NULL,[User_Name] [varchar](50) NULL,[Password] [varchar](250) NULL," _
			& " [SMTP_Server] [varchar](max) NULL,[SMTP_Port] [varchar](max) NULL,[SMTP_Requires_Login] [bit] NULL,[SMTP_Login] [varchar](max) NULL," _
			& " [SMTP_Password] [varchar](max) NULL,[SMTP_Email_From] [varchar](max) NULL,[SMTP_CC] [varchar](max) NULL,[Company_Legal_Name] [varchar](max) NULL," _
			& " [Company_FEIN] [varchar](max) NULL,[Bank_Legal_Name] [varchar](max) NULL,[Bank_Routing_Number] [varchar](max) NULL," _
			& " [Return_Account_Number] [varchar](max) NULL,[Bank_Account_Number] [varchar](max) NULL,[Client_Number] [varchar](max) NULL," _
			& " [Data_Center] [varchar](50) NULL," _
			& " CONSTRAINT [PK_ACH_Settings] PRIMARY KEY CLUSTERED ([ID] Asc)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF," _
			& " IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]) ON [PRIMARY];"

		s = s & " GRANT CONTROL ON ACH_Log TO JB_User;GRANT CONTROL ON ACH_Vendors TO JB_User;" _
			& "GRANT CONTROL ON ACH_Settings TO JB_User;GRANT CONTROL ON ACH_Vendor_Contacts TO JB_User;" _
			& "GRANT CONTROL ON ACH_Version TO JB_User"


		Return s
	End Function

	Function DeleteFile(path As String, Optional maxAttempts As Integer = 0) As Boolean
		Dim fileDeleted = False
		Dim attempts = 0

		Do
			Try
				System.IO.File.Delete(path)
				fileDeleted = True
			Catch ex As System.IO.IOException
				attempts += 1

				If attempts = maxAttempts Then
					Exit Do
				End If

				'The file is open so pause before trying again.
				System.Threading.Thread.Sleep(1000)
			End Try
		Loop Until fileDeleted

		Return fileDeleted
	End Function
End Module
