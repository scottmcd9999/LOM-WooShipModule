Imports System.Data.SqlClient
Public Class cSODetail_Woo
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property ID As Integer
	Property WooDataID As Integer
	Property SODetail As Integer
	Property SerialNumber As String
	Property Location As String
	Property Quantity As Integer
	Property Processed As Boolean
	Property DateProcessed As Date

	Property cmd As SqlCommand
	Function Load(ID As Integer) As Boolean

	End Function
	Function Save() As Boolean
		If WooDataID = 0 Or SODetail = 0 Or Quantity = 0 Or String.IsNullOrEmpty(SerialNumber) Or String.IsNullOrEmpty(Location) Then
			Return False
		End If

		Dim hdr As String = "INSERT INTO usr_Woo_SO_Detail(Woo_Data_ID,SO_Detail,Serial_Number,Location,Quantity,Processed,Date_Processed)"
		Dim vals As String = WooDataID
		vals = vals & "," & SODetail
		vals = vals & "," & AddString(SerialNumber)
		vals = vals & "," & AddString(Location)
		vals = vals & "," & Quantity
		vals = vals & "," & Convert.ToInt32(Processed)
		vals = vals & "," & AddString(DateProcessed, DbType.DateTime, "NULL")

		cmd.CommandText = hdr & " VALUES(" & vals & ")"
		cmd.ExecuteNonQuery()
		Return True
	End Function

End Class
