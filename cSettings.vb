Imports System.Data.SqlClient
Imports DevExpress.XtraEditors
Public Class cSettings

	Function WriteSetting(SettingName As String, SettingValue As String, Optional Encrypt As Boolean = False) As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using trn As SqlTransaction = con.BeginTransaction
				Using cmd As New SqlCommand
					cmd.Connection = con
					cmd.Transaction = trn
					Dim value As String = SettingValue
					If Encrypt Then
						Dim ce As New cEncrypt("rgb119$")
						value = ce.EncryptData(SettingValue)
					End If
					cmd.CommandText = "SELECT COUNT(*) FROM usr_Woo_Settings WHERE Setting_Name=" & AddString(SettingName, DbType.String)
					If cmd.ExecuteScalar = 0 Then
						cmd.CommandText = "INSERT INTO usr_Woo_Settings(Setting_Name, Setting_Value, Encrypted) VALUES(" & AddString(SettingName) _
							& "," & AddString(SettingValue) & "," & AddString(Convert.ToInt32(Encrypt), DbType.Double) & ")"
					Else
						cmd.CommandText = "UPDATE usr_Woo_Settings SET Setting_Value=" & AddString(value) & ", Last_Updated=" & AddString(Now, DbType.DateTime) _
							& ",Encrypted=" & AddString(Convert.ToInt32(Encrypt), DbType.Double) & " WHERE Setting_Name=" & AddString(SettingName)
					End If
					cmd.ExecuteNonQuery()
				End Using
				trn.Commit()
				Return True
			End Using
		End Using
	End Function
	Function ReadSetting(SettingName As String) As String
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				cmd.CommandText = "SELECT Setting_Value, Encrypted FROM usr_Woo_Settings WHERE Setting_Name=" & AddString(SettingName)
				Using dt As New DataTable
					dt.Load(cmd.ExecuteReader)
					If dt.Rows.Count > 0 Then
						Dim dtr As DataRow = dt.Rows(0)
						Dim val As String = dtr("Setting_Value")
						If dtr("Encrypted") = True Then
							Dim ce As New cEncrypt("rgb119$")
							val = ce.DecryptData(dtr("Setting_Value"))
						End If
						Return val
					Else
						Return String.Empty
					End If
				End Using
			End Using
		End Using
	End Function
End Class
