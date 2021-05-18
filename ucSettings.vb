Public Class ucSettings
    Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    Private Sub ucSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim c As New cSettings
        ckPostInvoices.Checked = c.ReadSetting("PostInvoices") = "True"
    End Sub
    Private Sub ckPostInvoices_CheckedChanged(sender As Object, e As EventArgs) Handles ckPostInvoices.CheckedChanged
        Dim c As New cSettings
        c.WriteSetting("PostInvoices", ckPostInvoices.Checked)
    End Sub

End Class
