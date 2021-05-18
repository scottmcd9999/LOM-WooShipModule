Imports System.Data.SqlClient
Public Class cPacklist_Header
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property Packlist As String
	Property PacklistDate As Date
	Property PacklistType As String
	Property PromisedDate As Date
	Property ShipTo As Integer
	Property ShipToID As Integer
	Property ShipVia As String
	Property Customer As cCustomer ' String
	Property Contact As String
	Property Notes As String
	Property cmd As SqlCommand
	Property PLHOID As String
	Property soh As cSalesOrder
	Property inv As cInvoiceHeader
	Property plds As List(Of cPacklistDetail)


	Private errs As New System.Text.StringBuilder

	Public Sub New()
		plds = New List(Of cPacklistDetail)
	End Sub

	ReadOnly Property ClassErrors As String
		Get
			Return errs.ToString
		End Get
	End Property

	Private Sub AddError(msg As String)
		errs.AppendLine(" -- " & msg)
	End Sub
	Function AddPacklistHeader(IsAuto As Boolean)

		Try
			Dim hdr As String = "INSERT INTO Packlist_Header(Packlist,Customer_Vendor,Invoice,Ship_To,Ship_Via,Contact,Packlist_Date,Type,Invoiced," _
					& " Comment,Last_Updated,Freight_Amt,BOL_Number,Carrier_Bill_To,BOL_Terms,Third_Party_Acct,ObjectID)"

			Packlist = NextPacklistNumber(cmd, IsAuto)
			If Packlist = "0" Then
				AddError("Unable to get new Packlist Number")
				Return False
			End If

			If ShipTo = 0 Then
				ShipTo = Customer.DefaultShipTo
			End If

			Dim vals As String = AddString(Packlist)
			vals = vals & "," & AddString(Customer.Customer)
			vals = vals & ",0"
			vals = vals & "," & ShipTo
			vals = vals & "," & AddString(ShipVia,, "NULL")
			vals = vals & "," & AddString(Contact, DbType.Double, "NULL")
			vals = vals & "," & AddString(PacklistDate, DbType.Date)
			vals = vals & "," & AddString(PacklistType,, "SOShip")
			vals = vals & ",0"
			vals = vals & ",'Woo Integration'"
			vals = vals & "," & AddString(Now, DbType.DateTime)
			vals = vals & ",0,NULL,NULL,NULL,NULL"
			PLHOID = System.Guid.NewGuid.ToString.ToUpper
			vals = vals & "," & AddString(PLHOID)

			cmd.CommandText = hdr & " VALUES(" & vals & ")"
			cmd.ExecuteNonQuery()

			cmd.CommandText = "UPDATE Auto_Number SET Last_Nbr='" & Packlist & "' WHERE Type='Packlist'"
			cmd.ExecuteNonQuery()

			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.cPacklist_Header.AddPacklistHeader Error")
			AddError(ex.ToString)
			MessageBox.Show(String.Format("Error in WooShipModule.cPacklist_Header.AddPacklistHeader: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Function
	Function GetPacklistDetail(PacklistDetail As Integer) As cPacklistDetail
		Try
			If plds Is Nothing Then
				Return Nothing
			End If
			If plds.Count = 0 Then
				Return Nothing
			End If
			For i As Integer = 0 To plds.Count - 1
				If plds(i).PacklistDetail = PacklistDetail Then
					Return plds(i)
				End If
			Next
			Return Nothing
		Catch ex As Exception
			logger.Error(ex.ToString, "WooShipModule.cPacklist_Header.GetPacklistDetail Error")
			AddError(ex.ToString)
			MessageBox.Show(String.Format("Error in WooShipModule.cPacklist_Header.GetPacklistDetail: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
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
End Class
