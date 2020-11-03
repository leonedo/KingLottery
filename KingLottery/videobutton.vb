Imports System.Globalization
Imports StarDust.CasparCG.net.Models.Media

Public Class videobutton

    Private boton As Button
    Private mediaList


    Public Sub New(ByVal media As IList(Of MediaInfo), sender As Button, tipo As String)
        MyBase.New()
        'This call is required by the Windows Form Designer.
        InitializeComponent()
        Dim info As TextInfo = CultureInfo.InvariantCulture.TextInfo
        boton = sender
        mediaList = media
        For Each file In mediaList
            If file.FullName.Contains(tipo) Then ComboBox1.Items.Add(info.ToTitleCase(file.Name.ToString.ToLower))
        Next

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        For Each file In mediaList
            If file.FullName.Contains(ComboBox1.SelectedItem.ToString.ToUpper) Then
                boton.Text = ComboBox1.SelectedItem
                Me.Close()
            End If
        Next


    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Button1.Enabled = True
    End Sub
End Class