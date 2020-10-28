Public Class NumPad
    Public Event Bolo_OK(bolo As Bola)
    Private Bolo As String
    Private TipoSorteo As Sorteos.Tipo

    Private Sub TextBoxNumero_TextChanged(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBoxNumero.KeyPress
        e.Handled = Not (Char.IsDigit(e.KeyChar) Or Asc(e.KeyChar) = 8)
    End Sub
    Private Sub TextBoxNumero_TextChanged(sender As Object, e As EventArgs) Handles TextBoxNumero.TextChanged, TextBoxNumero.KeyPress
        VerifyText()
    End Sub

    Sub VerifyText()
        If String.Equals(ButtonNumero.Text, TextBoxNumero.Text) Then
            ButtonOk.Enabled = True
            ButtonOk.BackColor = Color.GreenYellow
        Else
            ButtonOk.Enabled = False
            ButtonOk.BackColor = SystemColors.Control
        End If
    End Sub

    Public Sub ConfiguraNumeros(tipoSorteo As Sorteos.Tipo, bolo As String, Optional numeroDeBoloUnico As Boolean = False, Optional bolosAnteriores() As Bola = Nothing)
        Clear()
        Me.Bolo = bolo
        Me.TipoSorteo = tipoSorteo

        Select Case tipoSorteo
            Case Sorteos.Tipo.Pick3_SXM
                LabelDescription.Text = $"Pick3 Bolo # {bolo}"
            Case Sorteos.Tipo.Pick4_SXM
                LabelDescription.Text = $"Pick4 Bolo # {bolo}"

        End Select
    End Sub


    Private Sub FormaNumber_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter And ButtonOk.Enabled Then
            RaiseEvent Bolo_OK(New Bola With {.Bolo = Bolo, .Resultado = ButtonNumero.Text, .OK = True, .Sorteo = TipoSorteo})
            'Clear()
        Else
            If Not TextBoxNumero.Focused Then
                TextBoxNumero.Focus()
                SendKeys.Send(e.KeyCode.ToString)
            End If
        End If
    End Sub

    Public Sub ButtonOk_Click(sender As Object, e As EventArgs) Handles ButtonOk.Click
        If ButtonOk.Enabled Then
            RaiseEvent Bolo_OK(New Bola With {.Bolo = Bolo, .Resultado = ButtonNumero.Text, .OK = True, .Sorteo = TipoSorteo})
            'Clear()
        End If
    End Sub

    Public Sub Clear()
        Bolo = ""
        ButtonNumero.Text = "  "
        TextBoxNumero.Text = ""
    End Sub

    Private Sub Button1_Click(sender As Button, e As EventArgs) Handles Button9.Click, Button8.Click, Button7.Click, Button6.Click, Button5.Click, Button4.Click, Button3.Click, Button2.Click, Button10.Click, Button1.Click
        ButtonNumero.Text = sender.Tag
        VerifyText()
        TextBoxNumero.Focus()
    End Sub

    Private Sub NumPad_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
