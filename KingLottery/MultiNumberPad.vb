Public Class MultiNumberPad
    Private Sub MultiNumberPad_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label1.Location = New Point(18, 64)
        Label2.Location = New Point(51, 64)
        Label3.Location = New Point(84, 64)
        Label4.Location = New Point(117, 64)
        Label1.Parent = PictureBox1
        Label2.Parent = PictureBox1
        Label3.Parent = PictureBox1
        Label4.Parent = PictureBox1
    End Sub

    Public Event Bolo_OK(bolo As Bola)
    Private Bolo As String
    Private TipoSorteo As Sorteos.Tipo
    Private BolosAnteriores

    Private Sub TextBoxNumero_TextChanged(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles TextBoxNumero.KeyPress
        e.Handled = Not (Char.IsDigit(e.KeyChar) Or Asc(e.KeyChar) = 8)
    End Sub
    Private Sub TextBoxNumero_TextChanged(sender As Object, e As EventArgs) Handles TextBoxNumero.TextChanged, TextBoxNumero.KeyPress
        VerifyText()
    End Sub

    Sub VerifyText()
        If String.Equals(LabelNumero.Text, TextBoxNumero.Text) Then
            ButtonOk.Enabled = True
            ButtonOk.BackColor = Color.GreenYellow
        Else
            ButtonOk.Enabled = False
            ButtonOk.BackColor = SystemColors.Control
        End If
    End Sub

    Public Sub ConfiguraNumeros(tipoSorteo As Sorteos.Tipo, bolo As String, bolosAnteriores() As Bola)
        Clear()
        Me.Bolo = bolo
        Me.BolosAnteriores = bolosAnteriores
        Me.TipoSorteo = tipoSorteo
        If tipoSorteo = Sorteos.Tipo.Phillipsburg Then
            FillLabel()
        End If
    End Sub

    Private Sub FillLabel()
        Select Case CInt(Bolo)
            Case 1 To 4
                Label1.Text = BolosAnteriores(0)?.Resultado
                Label2.Text = BolosAnteriores(1)?.Resultado
                Label3.Text = BolosAnteriores(2)?.Resultado
                Label4.Text = BolosAnteriores(3)?.Resultado
                LabelGrupo.Text = "1"
            Case 5 To 8
                Label1.Text = BolosAnteriores(4)?.Resultado
                Label2.Text = BolosAnteriores(5)?.Resultado
                Label3.Text = BolosAnteriores(6)?.Resultado
                Label4.Text = BolosAnteriores(7)?.Resultado
                LabelGrupo.Text = "2"
            Case 9 To 12
                Label1.Text = BolosAnteriores(8)?.Resultado
                Label2.Text = BolosAnteriores(9)?.Resultado
                Label3.Text = BolosAnteriores(10)?.Resultado
                Label4.Text = BolosAnteriores(11)?.Resultado
                LabelGrupo.Text = "3"
        End Select
    End Sub


    Private Sub FormaNumber_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Enter And ButtonOk.Enabled Then
            RaiseEvent Bolo_OK(New Bola With {.Bolo = Bolo, .Resultado = LabelNumero.Text, .OK = True, .Sorteo = TipoSorteo})
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
            RaiseEvent Bolo_OK(New Bola With {.Bolo = Bolo, .Resultado = LabelNumero.Text, .OK = True, .Sorteo = TipoSorteo})
            'Clear()
        End If
    End Sub

    Public Sub Clear()
        Bolo = ""
        LabelNumero.Text = "  "
        TextBoxNumero.Text = ""
    End Sub

    Private Sub Button1_Click(sender As Button, e As EventArgs) Handles Button9.Click, Button8.Click, Button7.Click, Button6.Click, Button5.Click, Button4.Click, Button3.Click, Button2.Click, Button10.Click, Button1.Click
        LabelNumero.Text = sender.Tag
        Select Case CInt(Bolo)
            Case 1, 5, 9
                Label1.Text = sender.Tag
            Case 2, 6, 10
                Label2.Text = sender.Tag
            Case 3, 7, 11
                Label3.Text = sender.Tag
            Case 4, 8, 12
                Label4.Text = sender.Tag
        End Select

        VerifyText()
        TextBoxNumero.Focus()
    End Sub
End Class
