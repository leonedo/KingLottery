Public Class Consola
    Private Sub ButtonSendCommnad_Click(sender As Object, e As EventArgs) Handles ButtonSendCommnad.Click
        Dim reply = Main.CasparDevice.Connection.SendStringWithResult(TextBoxSendCommand.Text, New TimeSpan(0, 0, 5))
        ConsoleBox.AppendText("> " + TextBoxSendCommand.Text + vbCrLf + "< " + reply + vbCrLf)
        TextBoxSendCommand.Clear()
        ConsoleBox.SelectionStart = ConsoleBox.Text.Length
        ConsoleBox.ScrollToCaret()
    End Sub

    Private Sub Consola_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case True
            Case e.KeyCode = Keys.Enter
                ButtonSendCommnad_Click(Nothing, Nothing)
        End Select
    End Sub
End Class