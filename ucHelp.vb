Imports DevExpress.XtraEditors
Imports System.Drawing.Printing
Public Class ucHelp

	Dim checkprint As Integer
	Private Sub tsbPrintHelp_Click(sender As Object, e As EventArgs) Handles tsbPrint.Click
		Try
			If ptPrintDialog.ShowDialog = DialogResult.OK Then
				ptDocument.Print()
			End If
		Catch ex As Exception
			'logger.Error(ex.ToString, "tsbPrintHelp_Click Error")
			XtraMessageBox.Show(String.Format("Error in tsbPrintHelp_Click: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Sub

	Private Sub pdDocument_BeginPrint(sender As Object, e As PrintEventArgs) Handles ptDocument.BeginPrint
		checkprint = 0
	End Sub

	Private Sub pdDocument_PrintPage(sender As Object, e As PrintPageEventArgs) Handles ptDocument.PrintPage
		Try
			' Print the content of the RichTextBox. Store the last character printed.
			checkprint = rtHelp.Print(checkprint, rtHelp.TextLength, e)

			' Look for more pages
			If checkprint < rtHelp.TextLength Then
				e.HasMorePages = True
			Else
				e.HasMorePages = False
			End If
		Catch ex As Exception
			'logger.Error(ex.ToString, "pdDocument_PrintPage Error")
			XtraMessageBox.Show(String.Format("Error in pdDocument_PrintPage: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally
		End Try
	End Sub

	Private Sub tsbPageSetup_Click(sender As Object, e As EventArgs) Handles tsbPageSetup.Click
		Try
			ptPageSetupDialog.ShowDialog()
		Catch ex As Exception
			'logger.Error(ex.ToString, "tsbPageSetup_Click Error")
			XtraMessageBox.Show(String.Format("Error in tsbPageSetup_Click: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally
		End Try
	End Sub

	Private Sub tsbPrintPreview_Click(sender As Object, e As EventArgs) Handles tsbPreview.Click
		Try
			ptPrintPreviewDialog.ShowDialog()
		Catch ex As Exception
			'logger.Error(ex.ToString, "tsbPrintPreview_Click Error")
			XtraMessageBox.Show(String.Format("Error in tsbPrintPreview_Click: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally
		End Try
	End Sub

	Private Sub ucHelp_Load(sender As Object, e As EventArgs) Handles Me.Load
		'/ show the Help file if found
		Using ms As New System.IO.MemoryStream
			Dim buffer As Byte() = System.Text.Encoding.UTF8.GetBytes(My.Resources.Help)
			ms.Write(buffer, 0, buffer.Length)
			ms.Seek(0, System.IO.SeekOrigin.Begin)
			rtHelp.LoadFile(ms, RichTextBoxStreamType.RichText)
		End Using
	End Sub
End Class
