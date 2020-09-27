Imports System.IO
Imports StarDust.CasparCG.net.Connection
Imports StarDust.CasparCG.net.AmcpProtocol
Imports StarDust.CasparCG.net.Datas
Imports StarDust.CasparCG.net.Device
Imports StarDust.CasparCG.net.Models
Imports StarDust.CasparCG.net.Models.Media
Imports StarDust.CasparCG.net.Models.Mixer
Imports StarDust.CasparCG.net.Models.Info
Imports Bespoke.Common.Osc
Imports System.Net
Imports Unity
Imports Unity.Lifetime
Imports Newtonsoft.Json
Imports System.Globalization
Imports System.Timers
Imports System.Threading

Public Class Main
    Const Version = "0.0.2" '

    Public WithEvents CasparDevice As CasparDevice
    Public Canal_PGM As ChannelManager
    Public Canal_PVW As ChannelManager
    Public Canal_Ver_1 As ChannelManager
    Public Canal_Ver_2 As ChannelManager
    Public Canal_Ver_3 As ChannelManager
    Private WithEvents Oscserver As OscServer
    Public Process As Process
    Public ScannerProcess As Process
    Private Delegate Sub ProcessOutputDataCallback(sender As Object, e As DataReceivedEventArgs)
    Private Pick3_SXM(9) As Bola
    Private Pick4_SXM(4) As Bola
    Private Pick4_PHI(4) As Bola
    Private Talentos As New Talentos
    Private SorteoDeHoy As Integer = My.Settings.NumeroSorteo + 1

#Region "Channal and leyer configuration"
    Const PGM = 1
    Const PVW = 2
    Const VER1 = 3
    Const VER2 = 4
    Const VER3 = 5
    Const LayerVideoVertical = 10
    Const LayerTemplates = 20
    Const LayerBumpers = 50
    Const LayerSeparadores = 60
    Const LayerVideoPlayer = 40
    Const LayerAudioPlayer = 70
#End Region

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '  CheckAndKillExistingCasparProcess()
        ' Dim login As New FormaLogin
        'If login.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
        ' LabelModo.Text = $"Modo Ensayo: {SorteoDeHoy.ToString("0000")}"
        LabelVersion.Text = $"Version: {Version}"
            ConfigureIOC()
            CasparDevice = _container.Resolve(Of ICasparDevice)()
        '   If Not login.ComboBox1.SelectedIndex = 2 Then 
        StartCasparcgServer()
        '  If login.ComboBox1.SelectedIndex = 0 Then AdminToolStripMenuItem.Visible = False
        ' Auth(login.ComboBox1.SelectedIndex)
        'LoadDataSource()
        'SetupComboxes()
        'Else
        'Me.Close()
        'End If
        'login.Dispose()
    End Sub

    Private Sub FormPrincipal_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        StopServer()
    End Sub

#Region "CasparCg"
    Private _container As IUnityContainer
    Private Sub ConfigureIOC()
        _container = New UnityContainer()
        _container.RegisterInstance(Of IServerConnection)(New ServerConnection(New CasparCGConnectionSettings("127.0.0.1")))
        _container.RegisterType(GetType(IAMCPTcpParser), GetType(AmcpTCPParser))
        _container.RegisterSingleton(Of IDataParser, CasparCGDataParser)()
        _container.RegisterType(GetType(IAMCPProtocolParser), GetType(AMCPProtocolParser))
        _container.RegisterType(Of ICasparDevice, CasparDevice)(New ContainerControlledLifetimeManager())

    End Sub

    Sub StartCasparcgServer()
        Try
            CheckAndKillExistingCasparProcess()

            If File.Exists(My.Settings.ScannerPath) Then
                My.Settings.UseScanner = True
            Else
                My.Settings.UseScanner = False

            End If

            Process = New Process With {
                .EnableRaisingEvents = True
            }
            Process.StartInfo.FileName = My.Settings.ServerPath
            Process.StartInfo.WorkingDirectory = Path.GetDirectoryName(My.Settings.ServerPath)   'Importante para el funcionamiento del server
            Process.StartInfo.CreateNoWindow = True
            Process.StartInfo.UseShellExecute = False
            Process.StartInfo.RedirectStandardOutput = True
            Process.StartInfo.RedirectStandardInput = True

            Process.Start()
            AddHandler Process.OutputDataReceived, AddressOf OnProcessOutputData
            Process.BeginOutputReadLine()

            If My.Settings.UseScanner Then
                ScannerProcess = New Process With {
                    .EnableRaisingEvents = True
                }
                ScannerProcess.StartInfo.FileName = My.Settings.ScannerPath
                ScannerProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(My.Settings.ScannerPath)   'Importante para el funcionamiento del server
                ScannerProcess.StartInfo.CreateNoWindow = True
                ScannerProcess.StartInfo.UseShellExecute = False
                ScannerProcess.StartInfo.RedirectStandardOutput = True
                ScannerProcess.StartInfo.RedirectStandardInput = True
                ScannerProcess.Start()
                AddHandler ScannerProcess.OutputDataReceived, AddressOf OnProcessOutputDataScanner
                ScannerProcess.BeginOutputReadLine()
            End If

            Threading.Thread.Sleep(500)
            TimerCasparConnect.Start()
        Catch ex As Exception
            MessageBox.Show("Error Iniciando Servidor Grafico")
        End Try
    End Sub

    Private Sub StopServer()
        Try
            TableLayoutPanel1.Enabled = False
            CasparDevice.Connection.SendString("kill")

            Threading.Thread.Sleep(500)
            CasparDevice.Disconnect()
            Threading.Thread.Sleep(500)


            If ScannerProcess IsNot Nothing AndAlso Not ScannerProcess.HasExited Then
                RemoveHandler ScannerProcess.OutputDataReceived, AddressOf OnProcessOutputDataScanner
                ScannerProcess.Kill()
                ScannerProcess.WaitForExit()
                ScannerProcess.Dispose()
            End If

            If Process IsNot Nothing AndAlso Not Process.HasExited Then
                RemoveHandler Process.OutputDataReceived, AddressOf OnProcessOutputData
                Process.Kill()
            End If
            Process.WaitForExit()
            Process.Dispose()
        Catch ex As Exception

        End Try
    End Sub

    Sub CheckAndKillExistingCasparProcess()
        For Each proc As Process In Process.GetProcessesByName("casparcg")
            proc.CloseMainWindow() 'ask the process to exit.
            proc.WaitForExit(100) 'wait up to 0.1 seconds.
            If Not proc.HasExited Then
                proc.Kill() 'force the process to exit.
            End If
        Next proc

        For Each proc As Process In Process.GetProcessesByName("scanner")
            proc.CloseMainWindow() 'ask the process to exit.
            proc.WaitForExit(100) 'wait up to 0.1 seconds.
            If Not proc.HasExited Then
                proc.Kill() 'force the process to exit.
            End If
        Next proc

    End Sub

    Private Sub ConfiguraCanales()
        Try
            Canal_PGM = CasparDevice.Channels.First(Function(x) x.ID = PGM)
            ' Canal_PVW = CasparDevice.Channels.First(Function(x) x.ID = PVW)
            ' Canal_Ver_1 = CasparDevice.Channels.First(Function(x) x.ID = VER1)
            ' Canal_Ver_2 = CasparDevice.Channels.First(Function(x) x.ID = VER2)
            ' Canal_Ver_3 = CasparDevice.Channels.First(Function(x) x.ID = VER3)
            LabelVersion.Text = $"{Version} / {CasparDevice.GetVersion.Substring(0, 5)}"
            TableLayoutPanel1.Enabled = True
        Catch ex As Exception
            MessageBox.Show($"{ex.Message} | El Server tiene :{CasparDevice.Channels.Count} canales configurados de los 5 que espera el cliente.")
        End Try
    End Sub

    Private Sub OnProcessOutputData(sender As Object, e As DataReceivedEventArgs)
        If (Me.InvokeRequired) Then
            BeginInvoke(New ProcessOutputDataCallback(AddressOf OnProcessOutputData), sender, e)
        Else
            'ConsoleBox.AppendText(e.Data + vbCrLf)
            'ConsoleBox.SelectionStart = ConsoleBox.Text.Length
            'ConsoleBox.ScrollToCaret()
        End If
    End Sub

    Private Sub OnProcessOutputDataScanner(sender As Object, e As DataReceivedEventArgs)
        If (Me.InvokeRequired) Then
            BeginInvoke(New ProcessOutputDataCallback(AddressOf OnProcessOutputDataScanner), sender, e)
        Else
            'ScannerBox.AppendText(e.Data + vbCrLf)
            'ScannerBox.SelectionStart = ScannerBox.Text.Length
            'ScannerBox.ScrollToCaret()
        End If
    End Sub


    Private Sub TimerCasparConnect_Tick(sender As Object, e As EventArgs) Handles TimerCasparConnect.Tick
        '  Timer siempre activo que verifica la conexion con el servidor de casparCG
        Try
            If CasparDevice.IsConnected = True AndAlso CasparDevice.Channels.Count > 0 Then
                StatusLabel.Text = "Conectado al Servidor"
                PictureBoxStatus.Image = My.Resources.green

            Else
                If CasparDevice.IsConnected Then CasparDevice.GetInfo()
                StatusLabel.Text = "No Conectado al Servidor"
                PictureBoxStatus.Image = My.Resources.red
                CasparDevice.Connect()
            End If
        Catch ex As Exception
            System.Diagnostics.Debug.WriteLine(ex.Message)
        End Try
    End Sub

    Public Sub MediaRefreshComplete() Handles CasparDevice.MediafilesUpdated

        DataGridViewPlayout.Invoke(Sub()
                                       DataGridViewPlayout.Rows.Clear()
                                       DataGridViewAudio.Rows.Clear()
                                       For Each file In CasparDevice.Mediafiles
                                           If file.FullName.Contains("VIDEOSERVER") Then DataGridViewPlayout.Rows.Add({file.FullName.Replace("\", "/"), (file.Frames / file.Fps).ToString("F")})
                                           If file.FullName.Contains("AUDIOSERVER") Then DataGridViewAudio.Rows.Add({file.FullName.Replace("\", "/"), (file.Frames / file.Fps).ToString("F")})
                                       Next
                                   End Sub)
        LoadMediaDataSource()

    End Sub

    Private Sub LoadMediaDataSource()
        Dim media As New List(Of MediaInfo) From {
            New MediaInfo With {.Name = "-", .FullName = "STOP"} ' Fake Item to Stop the player
            }
        For Each item As MediaInfo In CasparDevice.Mediafiles
            If item.FullName.Contains("VERTICALES") Then
                media.Add(item)
            End If
        Next
        'ComboBoxVertical1.DataSource = New BindingSource(media, "")
        'ComboBoxVertical1.DisplayMember = "Name"
        'ComboBoxVertical2.DataSource = New BindingSource(media, "")
        'ComboBoxVertical2.DisplayMember = "Name"
        'ComboBoxVertical3.DataSource = New BindingSource(media, "")
        'ComboBoxVertical3.DisplayMember = "Name"

    End Sub

    Private Sub ServerConnected() Handles CasparDevice.ConnectionStatusChanged
        If CasparDevice.IsConnected Then
            'ReiniciarServidorToolStripMenuItem.Enabled = True
            ConfigOscServer()
            ConfiguraCanales()
            CasparDevice.GetMediafilesAsync()
            StatusLabel.Text = "Conectado al Servidor"
            PictureBoxStatus.Image = My.Resources.green

            'RadioButton_Verticales.Checked = True 'SetMultiview()
            '  CasparDevice.RefreshTemplates()
        End If
    End Sub
#End Region

#Region "OSC Server"
    Private Sub ConfigOscServer()   '
        Try
            Oscserver = New OscServer(TransportType.Udp, IPAddress.Loopback, 6250)
            '  For i = 1 To 2
            '     Oscserver.RegisterMethod($"/channel/{i}/profiler/time")
            ' Next
            Oscserver.Start()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub SOscServer_BundleReceived(ByVal sender As Object, ByVal e As OscBundleReceivedEventArgs) Handles Oscserver.BundleReceived
        For Each message In e.Bundle.Messages
            If message.Address.Contains("stage") Then
                procceedMessage(message)
            End If
        Next
    End Sub

    Private Sub procceedMessage(ByRef message As OscMessage)


        Dim dataString As String() = message.Address.ToString.Split("/")
        Dim Canal = dataString(2)
        Dim Layer = dataString(5)

        Select Case Canal = Canal_PGM.ID.ToString


            Case message.Address Like $"/channel/{Canal_PGM.ID}/stage/layer/*/foreground/file/time"
                TimeMessageFromOSC(CInt(Layer), message)
                Return
            'Case message.Address = $"/channel/{Canal_PGM}/stage/layer/{videoLayer}/foreground/file/path"
            '    CurrentClip = message.Data(0).ToString
            '    Return
            'Case message.Address = $"/channel/{Canal_PGM}/stage/layer/{videoLayer}/foreground/paused"
            '    Paused = CBool(message.Data(0))
            '    Return
                '' Legacy Part for 2.0.7 ''

            Case message.Address Like $"/channel/{Canal_PGM.ID}/stage/layer/*/file/time"  ''legacy 2.0.7
                TimeMessageFromOSC(CInt(Layer), message)
                Return
                'Case message.Address = $"/channel/{Canal_PGM}/stage/layer/{videoLayer}/file/path"  ''legacy 2.0.7
                '    CurrentClip = message.Data(0).ToString
                '    Return
                'Case message.Address = $"/channel/{Canal_PGM}/stage/layer/{videoLayer}/paused"    'legacy 2.0.7
                '    Paused = CBool(message.Data(0))
                '    Return
        End Select
    End Sub

    Private Sub TimeMessageFromOSC(layer As Integer, message As OscMessage)
        Dim pb = ProgressBarVideo
        Select Case layer
            Case LayerBumpers
                pb = ProgressBarBumpers
            Case LayerSeparadores
                pb = ProgressBarSeparadores
            Case LayerVideoPlayer
                pb = ProgressBarVideo
            Case LayerAudioPlayer
                pb = ProgressBarAudio
        End Select

        pb.Invoke(Sub()
                      UpdateProgressBar(pb, layer, message)
                  End Sub)

    End Sub


    Public Sub UpdateProgressBar(progressBar As ProgressBar, layer As Integer, mensaje As OscMessage)

        progressBar.Maximum = CInt(mensaje.Data(1) * 100)

        If CDec(mensaje.Data(1)) > 0 AndAlso CDec(mensaje.Data(1) - mensaje.Data(0)) < 0.1 Then
            progressBar.Value = CInt(mensaje.Data(1) * 100)
            '  progressBar.Tag = "NO"
            StopPlayGeneral(layer)
            Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(progressBar))
            thread.Start()

        Else 'If progressBar.Tag Is "READY" Then
            progressBar.Value = CInt(mensaje.Data(0) * 100)
        End If
    End Sub

#End Region

#Region "MenuBar"
    Private Sub ReloadMediaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReloadMediaToolStripMenuItem.Click
        If CasparDevice.IsConnected Then
            CasparDevice.GetMediafilesAsync()
        Else
            MessageBox.Show("No Conectado al server")
        End If
    End Sub
#End Region

#Region "Bumpers generales y players"
    Private Sub Button_Play_Bumpers_Click(sender As Object, e As EventArgs) Handles Button_Bumper_1.Click, Button_Bumper_2.Click, Button_Bumper_3.Click, Button_Bumper_4.Click,
                                                                                    Button_Bumper_5.Click, Button_Bumper_6.Click
        Dim bumper = New CasparPlayingInfoItem(LayerBumpers, $"""Bumpers/{sender.text}""")
        Canal_PGM.LoadBG(bumper, False)
        Canal_PGM.Play(LayerBumpers)
        ProgressBarBumpers.Tag = sender.text
    End Sub

    Private Sub Button_Bumpers_MouseClick(sender As Object, e As MouseEventArgs) Handles Button_Bumper_1.MouseDown, Button_Bumper_2.MouseDown, Button_Bumper_3.MouseDown, Button_Bumper_4.MouseDown,
                                                                                         Button_Bumper_5.MouseDown, Button_Bumper_6.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim configura As New videobutton(CasparDevice.Mediafiles, sender, "BUMPERS")
            configura.ShowDialog()
        End If
    End Sub

    Private Sub Button_Play_Separadores_Click(sender As Object, e As MouseEventArgs) Handles Button_Separador_1.Click, Button_Separador_2.Click,
                                                                                   Button_Separador_3.Click, Button_Separador_4.Click, Button_Separador_5.Click, Button_Separador_6.Click
        Dim video = New CasparPlayingInfoItem(LayerSeparadores, $"""Separadores/{sender.text}""")
        Canal_PGM.LoadBG(video, False)
        Canal_PGM.Play(LayerSeparadores)
    End Sub

    Private Sub Button_Separador_1_MouseClick(sender As Object, e As MouseEventArgs) Handles Button_Separador_1.MouseDown, Button_Separador_2.MouseDown,
                                                                                   Button_Separador_3.MouseDown, Button_Separador_4.MouseDown, Button_Separador_5.MouseDown, Button_Separador_6.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim configura As New videobutton(CasparDevice.Mediafiles, sender, "SEPARADORES")
            configura.ShowDialog()
        End If
    End Sub
    Private Sub StopPlayGeneral(layer As Integer)
        CasparDevice.Connection.SendString($"LOADBG {Canal_PGM.ID}-{layer} EMPTY MIX 5 AUTO")

    End Sub

    Private Sub Button_Stop_Separadores_Click(sender As Object, e As EventArgs) Handles Button_Stop_Separadores.Click
        Canal_PGM.Stop(LayerSeparadores)
        Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(ProgressBarSeparadores))
        thread.Start()
    End Sub

    Private Sub Button_Stop_Bumpers_Click(sender As Object, e As EventArgs) Handles Button_Stop_Bumpers.Click
        Canal_PGM.Stop(LayerBumpers)
        Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(ProgressBarBumpers))
        thread.Start()
    End Sub

    Sub MyStopclearProgressThread(pb As ProgressBar)
        Threading.Thread.Sleep(500)
        Try
            pb.Invoke(Sub()
                          pb.Value = 0
                      End Sub)
        Catch ex As Exception

        End Try

    End Sub

    Private Sub ButtonPlayoutPlay_Click(sender As Object, e As EventArgs) Handles ButtonPlayoutPlay.Click
        If CasparDevice.IsConnected Then
            Dim video = New CasparPlayingInfoItem With {.VideoLayer = LayerVideoPlayer, .Clipname = $"""{DataGridViewPlayout.CurrentRow.Cells(0).Value}""", .[Loop] = CheckBoxPlayoutLoop.Checked}
            Canal_PGM.LoadBG(video, True)
            Canal_PGM.Play(LayerVideoPlayer)
        End If
    End Sub

    Private Sub ButtonPlayoutStop_Click(sender As Object, e As EventArgs) Handles ButtonPlayoutStop.Click
        If CasparDevice.IsConnected Then
            CasparDevice.Connection.SendString($"PLAY {Canal_PGM.ID}-{LayerVideoPlayer} EMPTY MIX 5")
            Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(ProgressBarVideo))
            thread.Start()
        End If
    End Sub

    Private Sub ButtonPlayAudio_Click(sender As Object, e As EventArgs) Handles ButtonPlayAudio.Click
        If CasparDevice.IsConnected Then
            Dim audio = New CasparPlayingInfoItem With {.VideoLayer = LayerAudioPlayer, .Clipname = $"""{DataGridViewAudio.CurrentRow.Cells(0).Value}""", .[Loop] = CheckBoxLoopAudio.Checked}
            Canal_PGM.LoadBG(audio, True)
            Canal_PGM.Play(LayerAudioPlayer)
        End If
    End Sub

    Private Sub ButtonStopAudio_Click(sender As Object, e As EventArgs) Handles ButtonStopAudio.Click
        CasparDevice.Connection.SendString($"PLAY {Canal_PGM.ID}-{LayerAudioPlayer} EMPTY MIX 5")
        Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(ProgressBarAudio))
        thread.Start()
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Dim vol As Single = TrackBar1.Value / 10
        Canal_PGM.MixerManager.Volume(LayerAudioPlayer, vol, 10, StarDust.CasparCG.net.Models.Easing.Linear)
    End Sub





#End Region

End Class

#Region "Clases de soporte"
Public Class Bola
    Public Bolo As Integer
    Public Resultado As String
    Public Sorteo
    Public OK As Boolean
End Class
Public Class Personas
    Public Property Nombre As String
    Public Property Titulo As String
End Class
Public Class Talentos
    Public Property Presentadores As New List(Of Personas)
    Public Property Jueces As New List(Of Personas)
    Public Property Invidentes As New List(Of Personas)
End Class
Public Class Resultados
    Public Property SorteoId As String
    Public Property Fecha As Date
    Public Property Pick3_SXM As Bola()
    Public Property Pick4_SXM As Bola()
    Public Property Pick4_PHI As Bola()
    Public Property Presentador As Personas
    Public Property Jueces As List(Of Personas)
    Public Property Invidentes As List(Of Personas)
End Class

Public Class Sorteos
    Enum Tipo
        Pick3_SXM
        Pick4_SXM
        Pick4_PHI
    End Enum
End Class

#End Region
