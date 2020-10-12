Imports System.Security.Cryptography
Imports System.Text
Public Class login
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim salt As String
        Dim pahs As String
        If ComboBox1.SelectedIndex = 0 Then
            salt = My.Settings.OpSa
            pahs = My.Settings.OpPsHs
        Else
            salt = My.Settings.AdSa
            pahs = My.Settings.AdPsHs
        End If

        If pahs = Hash(TextBox1.Text, salt) Then
            DialogResult = DialogResult.OK
        Else
            MsgBox("Contraseña Incorrecta")
        End If
    End Sub

    Private Sub MainLoginForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
        AcceptButton = Button1
    End Sub

    Public Function Hash(password As String, salt As String) As String
        Dim convertedToBytes As Byte() = Encoding.UTF8.GetBytes(password & salt)
        Dim hashType As HashAlgorithm = New SHA512Managed()
        Dim hashBytes As Byte() = hashType.ComputeHash(convertedToBytes)
        Dim hashedResult As String = Convert.ToBase64String(hashBytes)
        Return hashedResult
    End Function

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex = 2 Then
            Label2.Visible = True
        Else
            Label2.Visible = False
        End If
    End Sub
End Class