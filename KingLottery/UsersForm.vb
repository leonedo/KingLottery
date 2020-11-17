Imports System.Text

Public Class UsersForm
    Private Sub FormaSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim salt As String = ""
        Dim pahs As String = ""
        Select Case ComboBox1.SelectedIndex
            Case 0
                salt = My.Settings.OpSa
                pahs = My.Settings.OpPsHs
            Case 1
                salt = My.Settings.AdSa
                pahs = My.Settings.AdPsHs
        End Select

        If pahs = login.Hash(TextBox1.Text, salt) Then
            If String.Equals(TextBox2.Text, TextBox3.Text) And TextBox2.Text.Length > 0 Then
                Select Case ComboBox1.SelectedIndex
                    Case 0
                        My.Settings.OpSa = CreateRandomSalt()
                        My.Settings.OpPsHs = login.Hash(TextBox2.Text, My.Settings.OpSa)
                    Case 1
                        My.Settings.AdSa = CreateRandomSalt()
                        My.Settings.AdPsHs = login.Hash(TextBox2.Text, My.Settings.AdSa)
                End Select
                My.Settings.Save()
                MsgBox("Contraseña Actualizada con éxito")
            Else
                MsgBox("Contraseñas nuevas no coinciden")
            End If
        Else
            MsgBox("Contraseña Incorrecta")
        End If
    End Sub

    Public Function CreateRandomSalt() As String
        'the following is the string that will hold the salt charachters
        Dim mix As String = "ABCDEFGHIJKLMNOZabcdefghijklmnopqrsP+=][}{<>QRSTUVWXYtuvwxyz0123456789!@#$%^&*()_"
        Dim salt As String = ""
        Dim rnd As New Random
        Dim sb As New StringBuilder
        For i As Integer = 1 To 100 'Length of the salt
            Dim x As Integer = rnd.Next(0, mix.Length - 1)
            salt &= (mix.Substring(x, 1))
        Next
        Return salt
    End Function
End Class