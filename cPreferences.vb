Imports System.Data.SqlClient
Public Class cPreferences
	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Property TransferCostMethod As String
	Property InventoryCostMethod As String
	Property BaseCurrency As Integer
	Property FreightClass As String
	Property cmd As SqlCommand

	Function Load() As Boolean
		Try
			'Using con As New SqlConnection(JBConnection)
			'	con.Open()
			'	Using cmd As New SqlCommand
			'		cmd.Connection = con
			cmd.CommandText = "SELECT * FROM Preferences"
			Using dt As New DataTable
				dt.Load(cmd.ExecuteReader)
				If dt.Rows.Count > 0 Then
					Dim dtr As DataRow = dt.Rows(0)
					TransferCostMethod = dtr("Transfer_Cost_Method")
					InventoryCostMethod = dtr("Inv_Cost_Method")
					BaseCurrency = dtr("System_Base_Currency")
					FreightClass = dtr("Freight_Class") & ""
				End If
			End Using
			'	End Using
			'End Using
			Return True
		Catch ex As Exception
			logger.Error(ex.ToString, "cPreferences.Load Error")
			MessageBox.Show(String.Format("Error in Load: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
			Return False
		Finally
		End Try

	End Function

End Class
