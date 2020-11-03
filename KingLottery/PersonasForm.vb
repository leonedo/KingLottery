Imports System.IO
Imports Newtonsoft.Json

Public Class PersonasForm
    Private Talentos As New Talentos
    Private Sub PersonasForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If File.Exists("JuecesYpresentadores.json") Then
            Dim json = File.ReadAllText("JuecesYpresentadores.json")
            Talentos = JsonConvert.DeserializeObject(Of Talentos)(json)
        End If
        BindingSourceJueces.DataSource = Talentos.Jueces
        BindingSourcePresentadores.DataSource = Talentos.Presentadores

    End Sub

    Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click
        Dim json = JsonConvert.SerializeObject(Talentos)
        My.Computer.FileSystem.WriteAllText("JuecesYpresentadores.json", json, False)
    End Sub


End Class