Imports System.Data.SqlClient
Public Class cDelivery
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger
	Property Delivery As Integer
	Property SODetail As Integer
	Property PromisedQuantity As Double
	Property ShippedQuantity As Double
	Property RemainingQuantity As Double

	Property PromisedDate As Date
	Property ShippedDate As Date
	Property DockCode As String
	Property Comment As String
	Property DeliveryOID As String
	Property IsAuto As Boolean
	Property Errors As String
	Private Sub AddError(msg As String)
		If Errors.Length = 0 Then
			Errors = "-- " & msg
		Else
			Errors = Errors & Environment.NewLine & "-- " & msg
		End If
	End Sub
	Function Load(SODetail As Integer) As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				Return LoadClass(SODetail, cmd)
			End Using
		End Using
	End Function
	Function Load(SODetail As Integer, cmd As SqlCommand) As Boolean
		Return LoadClass(SODetail, cmd)
	End Function
	Private Function LoadClass(SODetail As Integer, cmd As SqlCommand) As Boolean
		cmd.CommandText = "SELECT * FROM Delivery WHERE SO_Detail=" & SODetail
		Using dt As New DataTable
			dt.Load(cmd.ExecuteReader)
			If dt.Rows.Count = 0 Then
				Return False
			Else
				Dim dtr As DataRow = dt.Rows(0)
				Delivery = dtr("Delivery")
				PromisedQuantity = dtr("Promised_Quantity")
				ShippedQuantity = dtr("Shipped_Quantity")
				RemainingQuantity = dtr("Remaining_Quantity")
				DeliveryOID = dtr("ObjectID").ToString
				Comment = dtr("Comment") & ""
				PromisedDate = dtr("Promised_Date")
				If Not IsDBNull(dtr("Shipped_Date")) Then
					ShippedDate = dtr("Shipped_Date")
				End If

				Return True
			End If
		End Using
	End Function
	Function AddDeliveryForSOD(cmd As SqlCommand) As Boolean
		Try
			Return AddDelivery(cmd)
		Catch ex As Exception
			logger.Error(ex.ToString, "AddDeliveryForSOD Error")
			If IsAuto Then
				MessageBox.Show(String.Format("Error in AddDeliveryForSOD: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If

			Return False
		Finally
		End Try
	End Function
	Function AddDeliveryForSOD() As Boolean
		Try
			Using con As New SqlConnection(JBConnection)
				con.Open()
				Using cmd As New SqlCommand
					cmd.Connection = con
					Return AddDeliveryForSOD(cmd)
				End Using
			End Using
		Catch ex As Exception
			logger.Error(ex.ToString, "AddDeliveryForSOD Error")
			If IsAuto Then
				MessageBox.Show(String.Format("Error in AddDeliveryForSOD: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
			Return False
		Finally
		End Try
	End Function

	Private Function AddDelivery(cmd As SqlCommand) As Boolean
		Try
			Dim vals As String
			Dim hdr As String = "INSERT INTO Delivery([Packlist],[Job],[Invoice],[CrMemo],[Invoice_Line],[CrMemo_Line],[SO_Detail],[Requested_Date],[Promised_Date],[Shipped_Date]," _
				& "[Invoice_Date],[Returned_Date],[CrMemo_Date],[Promised_Quantity],[Shipped_Quantity],[Remaining_Quantity],[Returned_Quantity],[NCP_Quantity],[Return_Document]," _
				& "[Comment],[Last_Updated],[ObjectID],[Job_OID],[Last_Updated_By]) "

			vals = "NULL" '(dtr("Packlist"), DbType.String, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("Job"), DbType.String, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("Invoice"), DbType.String, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("CrMemo"), DbType.String, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("Invoice_Line"), DbType.String, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("CrMemo_Line"), DbType.String, "NULL")
			vals = vals & "," & AddString(SODetail, DbType.Double, "NULL")
			vals = vals & "," & AddString(PromisedDate, DbType.DateTime, "NULL") 'requested date
			vals = vals & "," & AddString(PromisedDate, DbType.DateTime) 'promise date
			vals = vals & ",NULL" '& AddString(dtr("Shipped_Date"), DbType.DateTime, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("Invoice_Date"), DbType.DateTime, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("Returned_Date"), DbType.DateTime, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("CrMemo_Date"), DbType.DateTime, "NULL")
			vals = vals & "," & PromisedQuantity
			vals = vals & "," & ShippedQuantity
			vals = vals & "," & RemainingQuantity
			vals = vals & ",0" '& AddString(dtr("Returned_Quantity"), DbType.Double, "NULL")
			vals = vals & ",0" '& AddString(dtr("NCP_Quantity"), DbType.Double, "NULL")
			vals = vals & ",NULL" '& AddString(dtr("Return_Document"), DbType.String, "NULL")
			vals = vals & "," & AddString(Comment, DbType.String, "NULL")
			vals = vals & "," & AddString(Now, DbType.DateTime)
			DeliveryOID = System.Guid.NewGuid.ToString.ToUpper
			vals = vals & "," & AddString(DeliveryOID, DbType.String)
			vals = vals & ",NULL" '& AddString(newGUID1, DbType.String)
			vals = vals & "," & AddString("SYSADM", DbType.String, "NULL")

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "SELECT Delivery FROM Delivery WHERE DeliveryKey=(SELECT SCOPE_IDENTITY())"
			Delivery = cmd.ExecuteScalar
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "AddDelivery Error")
			AddError(ex.ToString)
			If IsAuto Then
				MessageBox.Show(String.Format("Error in AddDelivery: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If

			Return False
		Finally
		End Try
	End Function


End Class
