Imports System.Data.SqlClient
Public Class cErrors
    Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

    Function LogError(SourceDocument As String, Errors As String) As Boolean
        Using con As New SqlConnection(JBConnection)
            con.Open()
            Using cmd As New SqlCommand
                cmd.Connection = con
                Return AddToErrorLog(SourceDocument, Errors, cmd)
            End Using
        End Using
    End Function

    Function LogError(SourceDocument As String, Errors As String, cmd As SqlCommand) As Boolean
        Return LogError(SourceDocument, Errors, cmd)
    End Function
    Private Function AddToErrorLog(SourceDocument As String, Errors As String, cmd As SqlCommand) As Boolean
        Try
            '/ get the FileName only:
            Dim filename As String = IO.Path.GetFileNameWithoutExtension(SourceDocument)
            cmd.CommandText = "INSERT INTO usr_API_Errors(Source_Document,Error_Msg,Last_Updated) VALUES(" _
                        & AddString(filename) & "," & AddString(Errors) & "," & AddString(Now, DbType.DateTime) & ")"
            cmd.ExecuteNonQuery()
            Return True
        Catch ex As Exception
            logger.Error(ex.ToString, "AmtexImport.cErrors.AddToErrorLog Error")
            'XtraMessageBox.Show(String.Format("Error in AmtexImport.cErrors.AddToErrorLog: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
            Return False
        Finally

        End Try
    End Function

    Function GetErrors(SourceDocument As String) As DataTable

        Dim filename As String = IO.Path.GetFileNameWithoutExtension(SourceDocument)
        Using con As New SqlConnection(JBConnection)
            con.Open()
            Using cmd As New SqlCommand
                cmd.Connection = con
                cmd.CommandText = "SELECT * FROM usr_API_Errors WHERE Source_Document=" & AddString(filename)
                Using dte As New DataTable
                    dte.Load(cmd.ExecuteReader)
                    If dte.Rows.Count > 0 Then
                        Return dte
                    Else
                        Return Nothing
                    End If
                End Using
            End Using
        End Using
    End Function

End Class
