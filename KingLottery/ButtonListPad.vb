Public Class ButtonListPad
    Public Event Bolo_OK(bolo As Bola)
    Private Bolo As String
    Private TipoSorteo As Sorteos.Tipo

    Private Sub NumbersControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LabelDescription.Location = New Point(36, 16)
        LabelDescription.Parent = ButtonNumero
    End Sub
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
        Dim size = New Size(38, 27)
        Dim min As Integer
        Dim top As Integer
        Dim format = "00"

        FlowLayoutPanel1.Controls.Clear()

        Select Case tipoSorteo
            Case Sorteos.Tipo.LotoPool
                format = "00"
                min = 0
                top = 99
                LabelDescription.Text = $"{bolo}"
                '  ButtonNumero.Image = My.Resources.BolaVerde
        End Select

        For index As Integer = min To top
            Dim boton As New Button With {
                .Text = index.ToString(format),
                .Size = size,
                .TextAlign = ContentAlignment.MiddleCenter,
                .Font = New Font("Microsoft Sans Serif", 9.0F, FontStyle.Bold),
                .BackColor = SystemColors.Control,
                .Margin = New Padding(1, 1, 1, 1)
            }
            If numeroDeBoloUnico And bolosAnteriores IsNot Nothing Then
                For Each bola In bolosAnteriores
                    If bola IsNot Nothing Then
                        ' If bola.Resultado = index Then boton.Enabled = False
                        If bola.Resultado = index Then
                            boton.BackColor = Color.DarkOrange
                            boton.ForeColor = Color.White
                        End If
                    End If
                Next
            End If


            AddHandler boton.Click, AddressOf Me.Button_Click
            FlowLayoutPanel1.Controls.Add(boton)
        Next
    End Sub

    Private Sub Button_Click(ByVal sender As Button, ByVal e As System.EventArgs)
        ButtonNumero.Text = sender.Text
        VerifyText()
        TextBoxNumero.Focus()
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
        FlowLayoutPanel1.Controls.Clear()
        Bolo = ""
        ButtonNumero.Text = "  "
        TextBoxNumero.Text = ""
    End Sub


End Class


