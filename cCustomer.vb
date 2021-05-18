Imports System.Data.SqlClient
Public Class cCustomer
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Event MissingData()
	Event Saved()
	Event Loaded()
	Event DeleteFail()
	Event DeleteSuccess()
	Property Customer As String
	Property CustomerName As String
	Property DefaultShipVia As String
	Property DefaultShipTo As Integer
	Property DefaultBillingAddress As Integer
	Property Terms As String
	Property SalesRep As String
	Property SalesCode As String
	Property TaxCode As String
	Property CurrencyDef As String
	Property VOILocation As String
	Property VOIShipTo As String
	Property VOIShipToAddressID As Integer
	Property VOIShipVia As String
	Property VOIShipType As String
	Property VOIProcessType As String
	Property IsNew As Boolean
	Property ShowInList As Boolean
	Property LeadDays As Integer
	Property RequiresPO As Boolean
	Property CurrentBalance As Double
	Property DiscountLevel As Integer
	Property DiscountPct As Decimal
	Property ConversionRate As Double

	Property MainAddr As cAddress
	Property ShipAddr As cAddress
	Property RemitAddr As cAddress
	Property Contact As cContact
	Property cmd As SqlCommand
	Property Errors As String
	Private Sub AddError(msg As String)
		If String.IsNullOrEmpty(Errors) Then
			Errors = "-- " & msg
		Else
			Errors = Errors & Environment.NewLine & "-- " & msg
		End If
	End Sub
	Private Function LoadClass(Customer As String, cmd As SqlCommand) As Boolean
		Try
			cmd.CommandText = "SELECT * FROM Customer WHERE Customer='" & Customer & "'"
			'MsgBox(Customer)
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				Dim dtr As DataRow = dt.Rows(0)
				Me.Customer = dtr("Customer")
				If Not IsDBNull(dtr("Name")) Then
					CustomerName = dtr("Name")
				End If
				If Not IsDBNull(dtr("Ship_Via")) Then
					DefaultShipVia = dtr("Ship_Via")
				End If

				Terms = dtr("Terms") & ""
				SalesCode = dtr("Sales_Code") & ""
				TaxCode = dtr("Tax_Code") & ""
				SalesRep = dtr("Sales_Rep") & ""
				CurrentBalance = dtr("Curr_Balance")
				If Not IsDBNull(dtr("Currency_Def")) Then
					CurrencyDef = dtr("Currency_Def")
				End If

				LeadDays = dtr("Ship_Lead_Days")

				Dim pref As New cPreferences With {.cmd = cmd}
				If Not pref.Load() Then
					AddError("Could not load Preferences")
					Return False
				End If

				If pref.BaseCurrency <> CurrencyDef Then
					Dim curdef As New cCurrencyDef
					If Not curdef.LoadCurrency(CurrencyDef, cmd) Then
						AddError("Could not load Currency Definition")
						Return False
					Else
						ConversionRate = curdef.ConversionRate(pref.BaseCurrency, CurrencyDef)
					End If
				End If

				cmd.CommandText = "SELECT Address FROM Address WHERE Customer=" & AddString(Customer) & " AND Type LIKE '__1'"
				Using dtST As New DataTable
					dtST.Load(cmd.ExecuteReader)
					If dtST.Rows.Count > 0 Then
						Dim shipad As New cAddress
						shipad.Address = dtST.Rows(0).Item("Address")
						If shipad.Load(cmd) Then
							ShipAddr = shipad
						End If
						DefaultShipTo = dtST.Rows(0).Item("Address")
					End If
				End Using

				cmd.CommandText = "SELECT Address FROM Address WHERE Customer=" & AddString(Customer) & " AND Type LIKE '_1_'"
				Using dtDB As New DataTable
					dtDB.Load(cmd.ExecuteReader)
					If dtDB.Rows.Count > 0 Then
						Dim billad As New cAddress
						billad.Address = dtDB.Rows(0).Item("Address")
						If billad.Load(cmd) Then
							RemitAddr = billad
						End If
						DefaultBillingAddress = dtDB.Rows(0).Item("Address")
					End If
				End Using

				cmd.CommandText = "SELECT Address FROM Address WHERE Customer=" & AddString(Customer) & " AND Type LIKE '1__'"
				Using dtDB As New DataTable
					dtDB.Load(cmd.ExecuteReader)
					If dtDB.Rows.Count > 0 Then
						Dim billad As New cAddress
						billad.Address = dtDB.Rows(0).Item("Address")
						If billad.Load(cmd) Then
							MainAddr = billad
						End If
						DefaultBillingAddress = dtDB.Rows(0).Item("Address")
					End If
				End Using

				'cmd.CommandText = "SELECT * FROM voi_Customers WHERE Customer='" & Customer & "'"
				'Using dtv As New DataTable
				'	dtv.Load(cmd.ExecuteReader)
				'	If dtv.Rows.Count > 0 Then
				'		Dim dtvr As DataRow = dtv.Rows(0)
				'		'/ dont use for MinMax:
				'		'If Not IsDBNull(dtvr("VOI_Location")) Then
				'		'	VOILocation = dtvr("VOI_Location")
				'		'End If
				'		If Not IsDBNull(dtvr("Ship_To")) Then
				'			VOIShipTo = dtvr("Ship_To")
				'			cmd.CommandText = "SELECT Address FROM Address WHERE Ship_To_ID='" & dtvr("Ship_To") & "' AND Customer='" & Customer & "'"
				'			VOIShipToAddressID = cmd.ExecuteScalar
				'		End If

				'		If Not IsDBNull(dtvr("Ship_Via")) Then
				'			VOIShipVia = dtvr("Ship_Via")
				'		End If

				'		If Not IsDBNull(dtvr("Shipment_Type")) Then
				'			VOIShipType = dtvr("Shipment_Type")
				'		End If

				'		If Not IsDBNull(dtvr("Process_Type")) Then
				'			VOIProcessType = dtvr("Process_Type")
				'		End If
				'		LeadDays = dtvr("Lead_Days")
				'		RequiresPO = dtvr("Require_PO")
				'	End If
				'End Using

				If Not IsDBNull(dtr("Pricing_Level")) Then
					cmd.CommandText = "SELECT * FROM User_Code WHERE Type='Discount' AND Code='" & dtr("Pricing_Level") & "'"
					Using dtp As New DataTable
						dtp.Load(cmd.ExecuteReader)
						If dtp.Rows.Count > 0 Then
							'/ numeric1 values are like 20, 10 etc. 
							DiscountLevel = dtr("Pricing_Level")
							DiscountPct = dtp.Rows(0).Item("Numeric1")
						End If
					End Using
				Else
					DiscountLevel = 0
					DiscountPct = 0
				End If

			End Using
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "LoadClass Error")
			MessageBox.Show(String.Format("Error in LoadClass: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try
	End Function

	Function Load(Customer As String, cmd As SqlCommand) As Boolean
		Try
			If Customer.Length = 0 Then
				Return False
			Else
				Me.Customer = Customer
				Return LoadClass(Customer, cmd)
			End If
		Catch ex As Exception
			logger.Error(ex.ToString, "Load Error")
			MessageBox.Show(String.Format("Error in Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try

	End Function
	Function Load(Customer As String) As Boolean
		Try
			If Customer.Length = 0 Then
				Return False
			Else
				Using con As New SqlConnection(JBConnection)
					con.Open()
					Using cmd As New SqlCommand
						cmd.Connection = con
						Return LoadClass(Customer, cmd)
					End Using
				End Using
			End If
		Catch ex As Exception
			logger.Error(ex.ToString, "cCustomer.Load Error")
			MessageBox.Show(String.Format("Error in Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try

	End Function
	'Function Load(Customer As String) As Boolean
	'	Try
	'		If Customer Is Nothing Then
	'			Return False
	'		End If

	'		If Customer.Length = 0 Then
	'			Return False
	'		End If
	'		'Using con As New SqlConnection(JBConnection)
	'		'    con.Open()
	'		'    Using cmd As New SqlCommand
	'		'        cmd.Connection = con
	'		cmd.CommandText = "SELECT * FROM Customer WHERE Customer='" & Customer & "'"

	'		Using dt As New DataTable
	'			dt.Load(cmd.ExecuteReader)
	'			Dim dtr As DataRow = dt.Rows(0)
	'			Me.Customer = dtr("Customer")
	'			If Not IsDBNull(dtr("Name")) Then
	'				CustomerName = dtr("Name")
	'			End If
	'			If Not IsDBNull(dtr("Ship_Via")) Then
	'				DefaultShipVia = dtr("Ship_Via")
	'			End If

	'			Terms = dtr("Terms") & ""
	'			SalesCode = dtr("Sales_Code") & ""
	'			TaxCode = dtr("Tax_Code") & ""
	'			CurrencyDef = dtr("Currency_Def") & ""
	'			cmd.CommandText = "SELECT Address FROM Address WHERE Customer='" & Customer & "' AND Type LIKE '__1'"
	'			Using dtST As New DataTable
	'				dtST.Load(cmd.ExecuteReader)
	'				If dtST.Rows.Count > 0 Then
	'					DefaultShipTo = dtST.Rows(0).Item("Address")
	'				End If
	'			End Using

	'			cmd.CommandText = "SELECT Address FROM Address WHERE Customer='" & Customer & "' AND Type LIKE '_1_'"
	'			Using dtDB As New DataTable
	'				dtDB.Load(cmd.ExecuteReader)
	'				If dtDB.Rows.Count > 0 Then
	'					DefaultBillingAddress = dtDB.Rows(0).Item("Address")
	'				End If
	'			End Using

	'			'cmd.CommandText = "SELECT * FROM voi_Customers WHERE Customer='" & Customer & "'"
	'			'Using dtv As New DataTable
	'			'	dtv.Load(cmd.ExecuteReader)
	'			'	If dtv.Rows.Count > 0 Then
	'			'		Dim dtvr As DataRow = dtv.Rows(0)
	'			'		'/ dont use for MinMax:
	'			'		'If Not IsDBNull(dtvr("VOI_Location")) Then
	'			'		'	VOILocation = dtvr("VOI_Location")
	'			'		'End If
	'			'		If Not IsDBNull(dtvr("Ship_To")) Then
	'			'			VOIShipTo = dtvr("Ship_To")
	'			'			cmd.CommandText = "SELECT Address FROM Address WHERE Ship_To_ID='" & dtvr("Ship_To") & "' AND Customer='" & Customer & "'"
	'			'			VOIShipToAddressID = cmd.ExecuteScalar
	'			'		End If

	'			'		If Not IsDBNull(dtvr("Ship_Via")) Then
	'			'			VOIShipVia = dtvr("Ship_Via")
	'			'		End If

	'			'		If Not IsDBNull(dtvr("Shipment_Type")) Then
	'			'			VOIShipType = dtvr("Shipment_Type")
	'			'		End If

	'			'		If Not IsDBNull(dtvr("Process_Type")) Then
	'			'			VOIProcessType = dtvr("Process_Type")
	'			'		End If
	'			'		LeadDays = dtvr("Lead_Days")
	'			'		RequiresPO = dtvr("Require_PO")
	'			'	End If
	'			'End Using

	'			If Not IsDBNull(dtr("Pricing_Level")) Then
	'				cmd.CommandText = "SELECT * FROM User_Code WHERE Type='Discount' AND Code='" & dtr("Pricing_Level") & "'"
	'				Using dtp As New DataTable
	'					dtp.Load(cmd.ExecuteReader)
	'					If dtp.Rows.Count > 0 Then
	'						DiscountLevel = dtr("Pricing_Level")
	'						DiscountPct = dtp.Rows(0).Item("Numeric1")
	'					End If
	'				End Using
	'			Else
	'				DiscountLevel = 0
	'				DiscountPct = 0
	'			End If



	'			'Dim cs As New clsSettings
	'			'Select Case Customer
	'			'	Case "CESSNA ACC"
	'			'		VOILocation = cs.ReadSetting("CessnaVOILocation")
	'			'	Case "SPIRIT AER"
	'			'		VOILocation = cs.ReadSetting("SpiritVOILocation")
	'			'	Case Else
	'			'		VOILocation = ""
	'			'End Select

	'		End Using
	'		'    End Using
	'		'End Using

	'		Return True
	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "cCustomer.Load Error")
	'		MessageBox.Show(String.Format("Error in Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'		Return False
	'	Finally
	'	End Try

	'End Function

	Function SaveCustomer() As Boolean
		'/ removed from below, not needed for min-max:
		'/ Or String.IsNullOrEmpty(VOILocation)
		If String.IsNullOrEmpty(Customer) Or String.IsNullOrEmpty(VOIShipTo) Or String.IsNullOrEmpty(VOIShipVia) _
			Or String.IsNullOrEmpty(VOIShipType) Then
			RaiseEvent MissingData()
			Return False
		End If

		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				Dim sql As String = ""
				If IsNew Then
					sql = "INSERT INTO voi_Customers(Customer, VOI_Location, Ship_To, Ship_Via, Lead_Days, Require_PO, Shipment_Type, Process_Type) VALUES('" _
						& Customer & "',NULL,'" & VOIShipTo & "','" & VOIShipVia & "'," & LeadDays & "," & Convert.ToInt32(RequiresPO) _
						& ",'" & VOIShipType & "','" & VOIProcessType & "')"
				Else
					sql = "UPDATE voi_Customers SET Ship_To='" & VOIShipTo _
						& "', Ship_Via='" & VOIShipVia & "', Show_In_List=" & Convert.ToInt32(ShowInList) _
						& ", Lead_Days=" & LeadDays & ", Require_PO=" & Convert.ToInt32(RequiresPO) & ", Last_Updated='" & FormatISODate(Now) _
						& "', Shipment_Type='" & VOIShipType & "', Process_Type='" & VOIProcessType & "' WHERE Customer='" & Customer & "'"
				End If

				cmd.CommandText = sql
				cmd.ExecuteNonQuery()
			End Using
		End Using

		Return True

	End Function

	Function DeleteCustomer() As Boolean

		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				cmd.CommandText = "SELECT COUNT(*) FROM voi_MM_Sales_Orders WHERE Customer='" & Customer & "'"
				If cmd.ExecuteScalar > 0 Then
					RaiseEvent MissingData()
				End If
			End Using
		End Using

	End Function
End Class
