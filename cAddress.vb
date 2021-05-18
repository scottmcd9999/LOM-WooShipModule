Imports System.Data.SqlClient
Public Class cAddress
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property Address As Integer
	Property Vendor As String
	Property Customer As String
	Property ShipToID As String
	Property Name As String
	Property Line1 As String
	Property Line2 As String
	Property City As String
	Property State As String
	Property Zip As String
	Property Country As String
	Property AddrType As String
	Property PrimaryShip As Boolean
	Property PrimaryRemit As Boolean
	Property PrimaryMain As Boolean
	Property Phone As String
	Property Billable As Boolean
	Property Shippable As Boolean
	Property CellPhone As String
	Property IsAuto As Boolean

	Property ClassErrors As String
	Private Sub AddError(ErrorMsg As String)
		If ClassErrors Is Nothing Then
			ClassErrors = ErrorMsg
		Else
			ClassErrors = ClassErrors & crlf() & ErrorMsg
		End If
	End Sub
	Function AddAddress() As Boolean
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand
					cmd.Connection = con
					Return AddNewAddress(cmd)
				End Using
			End Using
		Catch ex As Exception
			AddError(ex.ToString)
			logger.Error(ex.ToString, "Jobboss_Woo_Integration.cAddress.AddAddress Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in Jobboss_Woo_Integration.cAddress.AddAddress: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try

	End Function
	Function AddAddress(cmd As SqlCommand) As Boolean
		Return AddNewAddress(cmd)
	End Function
	Private Function AddNewAddress(cmd As SqlCommand) As Boolean
		Try
			Dim hdr As String = "INSERT INTO Address(Customer,Vendor,Status,Type,Ship_To_ID,Line1,Line2,City,State,Zip,Name,Country,Phone" _
						& ",Billable,Shippable,Cell_Phone,Last_Updated)"

			Dim vals As String = AddString(Customer, , "NULL")
			vals = vals & "," & AddString(Vendor, , "NULL")
			vals = vals & ",'Active'"
			If PrimaryMain Then
				AddrType = "1"
			Else
				AddrType = "0"
			End If
			If PrimaryRemit Then
				AddrType = AddrType & "1"
			Else
				AddrType = AddrType & "0"
			End If
			If PrimaryShip Then
				AddrType = AddrType & "1"
			Else
				AddrType = AddrType & "0"
			End If
			vals = vals & "," & AddString(AddrType)
			vals = vals & "," & AddString(ShipToID,, "NULL")
			vals = vals & "," & AddString(Line1,, "NULL")
			vals = vals & "," & AddString(Line2,, "NULL")
			vals = vals & "," & AddString(City,, "NULL")
			vals = vals & "," & AddString(State, , "NULL")
			vals = vals & "," & AddString(Zip,, "NULL")
			vals = vals & "," & AddString(Name,, "NULL")
			vals = vals & "," & AddString(Country,, "NULL")
			vals = vals & "," & AddString(Phone,, "NULL")
			vals = vals & "," & Convert.ToInt32(Billable)
			vals = vals & "," & Convert.ToInt32(Shippable)
			vals = vals & "," & AddString(CellPhone,, "NULL")
			vals = vals & "," & AddString(Now, DbType.DateTime)

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()
			cmd.CommandText = "SELECT Address FROM Address WHERE AddressKey=(SELECT Scope_Identity())"
			Address = cmd.ExecuteScalar

			Return True
		Catch ex As Exception
			AddError(ex.ToString)
			logger.Error(ex.ToString, "Jobboss_Woo_Integration.cAddress.AddNewAddress Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in Jobboss_Woo_Integration.cAddress.AddNewAddress: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try
	End Function
	Function Load() As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				Return LoadAddress(cmd)
			End Using
		End Using
	End Function
	Function Load(cmd As SqlCommand) As Boolean
		Return LoadAddress(cmd)
	End Function

	Private Function LoadAddress(cmd As SqlCommand) As Boolean
		Try
			If Address <> 0 Then
				cmd.CommandText = "SELECT * FROM Address WHERE Address=" & Address
			ElseIf Not String.IsNullOrEmpty(ShipToID) And Not String.IsNullOrEmpty(Vendor) Then
				cmd.CommandText = "SELECT * FROM Address WHERE Vendor=" & AddString(Vendor) & " AND Ship_To_ID=" & AddString(ShipToID)
			Else
				Return False
			End If

			logger.Info(cmd.CommandText)
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count = 0 Then
					Return False
				End If

				Dim dtr As DataRow = dt.Rows(0)

				Vendor = dtr("Vendor") & ""
				ShipToID = dtr("Ship_To_ID") & ""
				Line1 = dtr("Line1") & ""
				Line2 = dtr("Line2") & ""
				City = dtr("City") & ""
				Zip = dtr("Zip") & ""
				State = dtr("State") & ""
				Country = dtr("Country") & ""

				Return True

			End Using
		Catch ex As Exception
			AddError(ex.ToString)
			logger.Error(ex.ToString, "ConcurIntegration.cAddress.LoadAddress Error")
			'MessageBox.Show(String.Format("Error in ConcurIntegration.cAddress.LoadAddress: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally

		End Try



	End Function
End Class
