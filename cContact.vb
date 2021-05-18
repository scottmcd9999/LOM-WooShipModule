Imports System.Data.SqlClient
Public Class cContact
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property Contact As Integer
	Property Customer As String
	Property Vendor As String
	Property Address As Integer
	Property ContactName As String
	Property Title As String
	Property Phone As String
	Property PhoneExt As String
	Property Fax As String
	Property Email As String
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
	Function AddContact() As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				Return AddNewContact(cmd)
			End Using
		End Using
	End Function
	Function AddContact(cmd As SqlCommand) As Boolean
		Return AddNewContact(cmd)
	End Function
	Private Function AddNewContact(cmd As SqlCommand) As Boolean
		Try
			Dim hdr As String = "INSERT INTO Contact (Customer,Vendor,Address,Contact_Name,Title,Phone,Phone_Ext,Fax,Email_Address,Last_Updated," _
						& "Cell_Phone,NET1_Contact_ID,Status,Default_Invoice_Contact)"

			Dim vals As String = AddString(Customer,, "NULL")
			vals = vals & "," & AddString(Vendor,, "NULL")
			vals = vals & "," & Address
			vals = vals & "," & AddString(ContactName,, "NULL")
			vals = vals & ",NULL"
			vals = vals & "," & AddString(Phone,, "NULL")
			vals = vals & "," & AddString(PhoneExt, , "NULL")
			vals = vals & ",NULL" 'fax
			vals = vals & "," & AddString(Email,, "NULL")
			vals = vals & "," & AddString(Now, DbType.DateTime)
			vals = vals & "," & AddString(CellPhone,, "NULL")
			vals = vals & ",NULL,1,0"

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "SELECT Contact FROM Contact WHERE ContactKey=(SELECT SCOPE_IDENTITY())"
			Contact = cmd.ExecuteScalar
			Return True
		Catch ex As Exception
			AddError(ex.ToString)
			logger.Error(ex.ToString, "Jobboss_Woo_Integration.cContact.AddNewContact Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in Jobboss_Woo_Integration.cContact.AddNewContact: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally

		End Try


	End Function
End Class
