Imports System.Data.SqlClient
Public Class cInventory
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property Material As String
	Property LocationID As String
	Property QtyIssued As Double
	Property Lot As String
	Property Errors As String
	Property IsAuto As Boolean

	Private Sub AddError(err As String)
		Try
			If Errors Is Nothing Then
				Errors = err
			Else
				If Errors.Length > 0 Then
					Errors = Errors & Environment.NewLine
				End If

				Errors = Errors & " - " & err
			End If
		Catch ex As Exception
			logger.Error(ex.ToString, "JobbossACH.cPayment.AddError Error")

			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in AddError: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If
		Finally
		End Try
	End Sub
	Function RelieveInventory(cmd As SqlCommand) As Boolean
		Return InventoryMod(cmd)
	End Function

	Function RelieveInventory() As Boolean
		Using con As New SqlConnection(JBConnection)
			con.Open()
			Using cmd As New SqlCommand
				cmd.Connection = con
				Return InventoryMod(cmd)
			End Using
		End Using
	End Function

	Private Function InventoryMod(cmd As SqlCommand) As Boolean
		Try
			Dim where As String = "WHERE Location_ID=" & AddString(LocationID) & " AND Material=" & AddString(Material)

			If Not String.IsNullOrEmpty(Lot) Then
				where += " AND Lot=" & AddString(Lot)
			Else
				where += " AND Lot IS NULL"
			End If

			cmd.CommandText = "SELECT SUM(On_Hand_Qty) AS MatQty FROM Material_Location " & where
			Dim qoh As Double = cmd.ExecuteScalar

			If qoh < QtyIssued Then
				'/ we can't perform the operation:
				AddError(" Insufficient Material Quantity for " & Material & " in " & LocationID)
				Return False
			Else
				If qoh = QtyIssued Then
					cmd.CommandText = "DELETE FROM Material_Location " & where
				Else
					'/ qoh is > qty issued:
					cmd.CommandText = "UPDATE Material_Location SET On_Hand_Qty=On_Hand_Qty - " & QtyIssued & " " & where
				End If
				cmd.ExecuteNonQuery()
			End If

			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "cInventory.InventoryMod Error")
			If Not IsAuto Then
				MessageBox.Show(String.Format("Error in KardexIntegrate.cInventory.InventoryMod: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			End If

			AddError(ex.ToString)
			Return False
		Finally

		End Try

	End Function

End Class
