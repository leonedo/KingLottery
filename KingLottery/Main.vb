﻿Imports System.IO
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
Imports System.Xml
Imports Svt

Public Class Main
    Const Version = "1.0.2" '

    Public WithEvents CasparDevice As StarDust.CasparCG.net.Device.CasparDevice
    Public Canal_PGM As ChannelManager
    Public Canal_PVW As ChannelManager
    Public Canal_Ver_1 As ChannelManager
    Public Canal_Ver_2 As ChannelManager
    Public Canal_Hor_1 As ChannelManager
    Public Canal_Hor_2 As ChannelManager
    Public Canal_Hor_3 As ChannelManager
    'Public Canal_Ver_3 As ChannelManager
    Private WithEvents Oscserver As OscServer
    Public Process As Process
    Public ScannerProcess As Process
    Private Delegate Sub ProcessOutputDataCallback(sender As Object, e As DataReceivedEventArgs)
    Private Pick3_SXM(3) As Bola
    Private Pick4_SXM(4) As Bola
    Private Philipsburg(12) As Bola
    Private LotoPool(4) As Bola
    Private Talentos As New Talentos
    Private SorteoDeHoy As Integer = My.Settings.NumeroSorteo + 1
    Private SorteoActivo As Sorteos.Tipo = Sorteos.Tipo.Pick3_SXM
#Region "Channal and leyer configuration"
    Const PGM = 1
    Const PVW = 2
    Const VER1 = 3
    Const VER2 = 4
    Const HOR1 = 5
    Const HOR2 = 6
    Const HOR3 = 7

    Const LayerVideoLoops = 10
    Const LayerTemplates = 20
    Const LayerBumpers = 50
    Const LayerBumpersSorteos = 51
    Const LayerSeparadores = 60
    Const LayerVideoPlayer = 40
    Const LayerAudioPlayer = 70
#End Region

    Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupLabels()
        Dim login As New login
        If login.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK Then
            ' LabelModo.Text = $"Modo Ensayo: {SorteoDeHoy.ToString("0000")}"
            LabelVersion.Text = $"Version: {Version}"
            ConfigureIOC()
            CasparDevice = _container.Resolve(Of ICasparDevice)()
            Auth(login.ComboBox1.SelectedIndex) 'Ejecuta tareas segun Tipo de usuario
            LoadDataSource()
            LoadCrawl()
            SetupComboxes()
            CheckSavedConfig(SalidaPGMToolStripMenuItem, My.Settings.videoMode)
            CheckSavedConfig(LoglevelToolStripMenuItem, My.Settings.log)

            If TimeOfDay > #4:00:00 PM# Then
                RadioButtonPM.Checked = True
            End If
        Else
            Me.Close()
        End If
        login.Dispose()
    End Sub

    Private Sub Auth(user As Integer)

        If user = 1 Then
            LabelUser.Text = "Administrador"
            MenuStrip1.Visible = True
            StartCasparcgServer()
        ElseIf user = 2 Then
            LabelUser.Text = "Debug"
            TableLayoutPanel1.Enabled = True
        Else
            LabelUser.Text = "Operador"
            MenuStrip1.Visible = False
            StartCasparcgServer()
        End If
    End Sub

    Private Sub FormPrincipal_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        StopServer()
    End Sub

    Private Sub Main_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown
        Select Case True
            Case e.KeyCode = Keys.Escape
                Canal_PGM.CG.Stop(LayerTemplates, 1)
            Case e.KeyCode = Keys.F1
               ' RadioButton_PGM.Checked = True
            Case e.KeyCode = Keys.F2
               ' RadioButton_Verticales.Checked = True
            Case e.KeyCode = Keys.F4
                Button_Play_Separadores_Click(Button_Separador_1, Nothing)
            Case e.KeyCode = Keys.F5
                Button_Play_Separadores_Click(Button_Separador_2, Nothing)
            Case e.KeyCode = Keys.F6
                Button_Play_Separadores_Click(Button_Separador_3, Nothing)
            Case e.KeyCode = Keys.F7
                Button_Play_Separadores_Click(Button_Separador_4, Nothing)
            Case e.KeyCode = Keys.F8
                Button_Play_Separadores_Click(Button_Separador_5, Nothing)
            Case e.KeyCode = Keys.F9
                Button_Play_Separadores_Click(Button_Separador_6, Nothing)
            Case e.KeyCode = Keys.F10
                CheckBoxMosca.Checked = Not CheckBoxMosca.Checked
            Case e.KeyCode = Keys.Enter
                Select Case True
                    Case RB_Pick3.Checked
                        NumPad_Pick3.ButtonOk_Click(NumPad_Pick3.ButtonOk, Nothing)
                    Case RB_pick4.Checked
                        NumPad_Pick4.ButtonOk_Click(NumPad_Pick3.ButtonOk, Nothing)
                    Case RB_LotoPool.Checked
                        ButtonListPadLotoPool.ButtonOk_Click(NumPad_Pick3.ButtonOk, Nothing)
                    Case RB_Phillps.Checked
                        MultiNumberPadPhilipsburg.ButtonOk_Click(MultiNumberPadPhilipsburg.ButtonOk, Nothing)
                End Select

        End Select
    End Sub

    Private Sub SetupLabels()
        Label_SXM3_1.Parent = PictureBoxPick3
        Label_SXM3_1.BackColor = Color.Transparent
        Label_SXM3_2.Parent = PictureBoxPick3
        Label_SXM3_2.BackColor = Color.Transparent
        Label_SXM3_3.Parent = PictureBoxPick3
        Label_SXM3_3.BackColor = Color.Transparent

        Label_SXM4_1.Parent = PictureBoxPick4
        Label_SXM4_1.BackColor = Color.Transparent
        Label_SXM4_2.Parent = PictureBoxPick4
        Label_SXM4_2.BackColor = Color.Transparent
        Label_SXM4_3.Parent = PictureBoxPick4
        Label_SXM4_3.BackColor = Color.Transparent
        Label_SXM4_4.Parent = PictureBoxPick4
        Label_SXM4_4.BackColor = Color.Transparent

        Label_PHI4_1.Parent = PictureBoxPhillip
        Label_PHI4_1.BackColor = Color.Transparent
        Label_PHI4_2.Parent = PictureBoxPhillip
        Label_PHI4_2.BackColor = Color.Transparent
        Label_PHI4_3.Parent = PictureBoxPhillip
        Label_PHI4_3.BackColor = Color.Transparent
        Label_PHI4_4.Parent = PictureBoxPhillip
        Label_PHI4_4.BackColor = Color.Transparent

        Label_PHI4_5.Parent = PictureBoxPhillip
        Label_PHI4_5.BackColor = Color.Transparent
        Label_PHI4_6.Parent = PictureBoxPhillip
        Label_PHI4_6.BackColor = Color.Transparent
        Label_PHI4_7.Parent = PictureBoxPhillip
        Label_PHI4_7.BackColor = Color.Transparent
        Label_PHI4_8.Parent = PictureBoxPhillip
        Label_PHI4_8.BackColor = Color.Transparent

        Label_PHI4_9.Parent = PictureBoxPhillip
        Label_PHI4_9.BackColor = Color.Transparent
        Label_PHI4_10.Parent = PictureBoxPhillip
        Label_PHI4_10.BackColor = Color.Transparent
        Label_PHI4_11.Parent = PictureBoxPhillip
        Label_PHI4_11.BackColor = Color.Transparent
        Label_PHI4_12.Parent = PictureBoxPhillip
        Label_PHI4_12.BackColor = Color.Transparent

        Label_Pool1.Parent = PictureBoxLotoPool
        Label_Pool1.BackColor = Color.Transparent
        Label_Pool2.Parent = PictureBoxLotoPool
        Label_Pool2.BackColor = Color.Transparent
        Label_Pool3.Parent = PictureBoxLotoPool
        Label_Pool3.BackColor = Color.Transparent
        Label_Pool4.Parent = PictureBoxLotoPool
        Label_Pool4.BackColor = Color.Transparent


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
            Canal_PVW = CasparDevice.Channels.First(Function(x) x.ID = PVW)
            Canal_Ver_1 = CasparDevice.Channels.First(Function(x) x.ID = VER1)
            Canal_Ver_2 = CasparDevice.Channels.First(Function(x) x.ID = VER2)
            Canal_Hor_1 = CasparDevice.Channels.First(Function(x) x.ID = HOR1)
            Canal_Hor_2 = CasparDevice.Channels.First(Function(x) x.ID = HOR2)
            Canal_Hor_3 = CasparDevice.Channels.First(Function(x) x.ID = HOR3)
            LabelVersion.Text = $"{Version} / {CasparDevice.GetVersion.Substring(0, 5)}"
            TableLayoutPanel1.Enabled = True
        Catch ex As Exception
            MessageBox.Show($"{ex.Message} | El Server tiene :{CasparDevice.Channels.Count} canales configurados de los que espera el cliente.")
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


    Private Sub ServerConnected() Handles CasparDevice.ConnectionStatusChanged
        If CasparDevice.IsConnected Then
            'ReiniciarServidorToolStripMenuItem.Enabled = True
            ConfigOscServer()
            ConfiguraCanales()
            CasparDevice.GetMediafilesAsync()
            StatusLabel.Text = "Conectado al Servidor"
            PictureBoxStatus.Image = My.Resources.green

            SetMultiview()
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
                ProcceedMessage(message)
            End If
        Next
    End Sub

    Private Sub ProcceedMessage(ByRef message As OscMessage)


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
        Dim pb = Nothing

        Select Case layer
            Case LayerBumpers
                pb = ProgressBarBumpers
            Case LayerBumpersSorteos
                pb = ProgressBarBumpers
            Case LayerSeparadores
                pb = ProgressBarSeparadores
            Case LayerVideoPlayer
                pb = ProgressBarVideo
            Case LayerAudioPlayer
                pb = ProgressBarAudio
        End Select

        If pb IsNot Nothing Then pb.Invoke(Sub()
                                               UpdateProgressBar(pb, layer, message)
                                           End Sub)

    End Sub


    Public Sub UpdateProgressBar(progressBar As ProgressBar, layer As Integer, mensaje As OscMessage)

        progressBar.Maximum = CInt(mensaje.Data(1) * 100)

        If CDec(mensaje.Data(1)) > 0 AndAlso CDec(mensaje.Data(1) - mensaje.Data(0)) < 0.1 Then
            progressBar.Value = CInt(mensaje.Data(1) * 100)
            '  progressBar.Tag = "NO"
            StopPlayGeneral(layer)
            Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(progressBar, layer))
            thread.Start()

        Else 'If progressBar.Tag Is "READY" Then
            progressBar.Value = CInt(mensaje.Data(0) * 100)
        End If
    End Sub

#End Region

#Region "MenuBar"
    Private Sub ReloadMediaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ButtonReloadMedia.Click
        If CasparDevice.IsConnected Then
            CasparDevice.GetMediafilesAsync()
        Else
            MessageBox.Show("No Conectado al server")
        End If
    End Sub

    Private Sub RadioButtonAM_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonAM.CheckedChanged, RadioButtonPM.CheckedChanged
        If RadioButtonPM.Checked Then
            PictureBoxLotoPool.Enabled = True
            ButtonEntradaLotoPool.Enabled = True
            ButtonResultadoLotoPool.Enabled = True
            RB_LotoPool.Enabled = True
        Else
            PictureBoxLotoPool.Enabled = My.Settings.LotoPoolAM
            ButtonEntradaLotoPool.Enabled = My.Settings.LotoPoolAM
            ButtonResultadoLotoPool.Enabled = My.Settings.LotoPoolAM
            RB_LotoPool.Enabled = My.Settings.LotoPoolAM
        End If
    End Sub

    Private Sub LotoPoolEnAMToolStripMenuItem_CheckedChanged(sender As Object, e As EventArgs) Handles LotoPoolEnAMToolStripMenuItem.CheckedChanged
        My.Settings.LotoPoolAM = LotoPoolEnAMToolStripMenuItem.Checked
        RadioButtonAM_CheckedChanged(Nothing, Nothing)
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
                                                                                   Button_Separador_3.Click, Button_Separador_4.Click, Button_Separador_5.Click, Button_Separador_6.Click, Button_Separador_7.Click, Button_Separador_8.Click
        Dim video = New CasparPlayingInfoItem(LayerSeparadores, $"""Separadores/{sender.text}""")
        Canal_PGM.LoadBG(video, False)
        Canal_PGM.Play(LayerSeparadores)
    End Sub

    Private Sub Button_Separador_1_MouseClick(sender As Object, e As MouseEventArgs) Handles Button_Separador_1.MouseDown, Button_Separador_2.MouseDown,
                                                                                   Button_Separador_3.MouseDown, Button_Separador_4.MouseDown, Button_Separador_5.MouseDown, Button_Separador_6.MouseDown, Button_Separador_8.MouseDown, Button_Separador_7.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim configura As New videobutton(CasparDevice.Mediafiles, sender, "SEPARADORES")
            configura.ShowDialog()
        End If
    End Sub
    Private Sub StopPlayGeneral(layer As Integer)
        CasparDevice.Connection.SendString($"LOADBG {Canal_PGM.ID}-{layer} EMPTY MIX 5 AUTO")

    End Sub

    Private Sub Button_Stop_Separadores_Click(sender As Object, e As EventArgs) Handles Button_Stop_Separadores.Click
        CasparDevice.Connection.SendString($"PLAY {Canal_PGM.ID}-{LayerSeparadores} EMPTY MIX 5")
        Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(ProgressBarSeparadores, LayerSeparadores))
        thread.Start()
    End Sub

    Private Sub Button_Stop_Bumpers_Click(sender As Object, e As EventArgs) Handles Button_Stop_Bumpers.Click
        CasparDevice.Connection.SendString($"PLAY {Canal_PGM.ID}-{LayerBumpers} EMPTY MIX 5")
        Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(ProgressBarBumpers, LayerBumpers))
        thread.Start()
    End Sub

    Sub MyStopclearProgressThread(pb As ProgressBar, layer As Integer)
        Threading.Thread.Sleep(500)
        Try
            pb.Invoke(Sub()
                          pb.Value = 0
                          If layer = LayerBumpersSorteos Then EntradaSorteo(SorteoActivo)
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
            Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(ProgressBarVideo, LayerVideoPlayer))
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
        Dim thread = New Thread(Sub() Me.MyStopclearProgressThread(ProgressBarAudio, LayerAudioPlayer))
        thread.Start()
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Dim vol As Single = TrackBar1.Value / 10
        Canal_PGM.MixerManager.Volume(LayerAudioPlayer, vol, 10, StarDust.CasparCG.net.Models.Easing.Linear)
    End Sub

    'Private Sub Button1_Click(sender As Object, e As EventArgs)
    '    MonitorVer1.UpdateSourceWithComponents(Environment.MachineName, "PGM", True, 90)
    'End Sub

#End Region

#Region "Loops Verticales/Horizontaltes y Multiview"
    Private Sub ComboBoxVertical1_SelectedIndexChanged(sender As ComboBox, e As EventArgs) Handles ComboBoxVertical1.SelectedIndexChanged, ComboBoxVertical2.SelectedIndexChanged
        Dim canal As ChannelManager
        Select Case sender.Tag
            Case "2"
                canal = Canal_Ver_2
            Case Else
                canal = Canal_Ver_1
        End Select
        If sender.SelectedIndex > 0 Then
            canal.Stop(LayerTemplates)
            canal.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = $"""{sender.SelectedValue.FullName.ToString.Replace("\", "/")}""",
                         .VideoLayer = LayerVideoLoops,
                         .[Loop] = True
                         })
            canal.Play(LayerVideoLoops)
        Else
            canal.Clear()
        End If

    End Sub

    Private Sub ButtonPlaySyncVertical_Click(sender As Object, e As EventArgs) Handles ButtonPlaySyncVertical.Click

        If ComboBoxVertical1.SelectedIndex > 0 Then
            Canal_Ver_1.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = $"""{ComboBoxVertical1.SelectedValue.FullName.ToString.Replace("\", "/")}""",
                         .VideoLayer = LayerVideoLoops,
                         .[Loop] = True
                         })
        End If
        If ComboBoxVertical2.SelectedIndex > 0 Then
            Canal_Ver_2.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = $"""{ComboBoxVertical2.SelectedValue.FullName.ToString.Replace("\", "/")}""",
                         .VideoLayer = LayerVideoLoops,
                         .[Loop] = True
                         })
        End If

        Canal_Ver_1.Play(LayerVideoLoops)
        Canal_Ver_2.Play(LayerVideoLoops)
    End Sub

    Private Sub ButtonPlaySyncHorizontal_Click(sender As Object, e As EventArgs) Handles ButtonPlaySyncHorizontal.Click

        If ComboBoxHor1.SelectedIndex > 0 Then
            Canal_Hor_1.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = $"""{ComboBoxHor1.SelectedValue.FullName.ToString.Replace("\", "/")}""",
                         .VideoLayer = LayerVideoLoops,
                         .[Loop] = True
                         })
        End If
        If ComboBoxHor2.SelectedIndex > 0 Then
            Canal_Hor_2.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = $"""{ComboBoxHor2.SelectedValue.FullName.ToString.Replace("\", "/")}""",
                         .VideoLayer = LayerVideoLoops,
                         .[Loop] = True
                         })
        End If
        If ComboBoxHor3.SelectedIndex > 0 Then
            Canal_Hor_3.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = $"""{ComboBoxHor3.SelectedValue.FullName.ToString.Replace("\", "/")}""",
                         .VideoLayer = LayerVideoLoops,
                         .[Loop] = True
                         })
        End If

        Canal_Hor_1.Play(LayerVideoLoops)
        Canal_Hor_2.Play(LayerVideoLoops)
        Canal_Hor_3.Play(LayerVideoLoops)
    End Sub

    Private Sub ComboBoxHor_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxHor3.SelectedIndexChanged, ComboBoxHor2.SelectedIndexChanged, ComboBoxHor1.SelectedIndexChanged
        Dim canal As ChannelManager
        Select Case sender.Tag
            Case "2"
                canal = Canal_Hor_2
            Case "3"
                canal = Canal_Hor_3
            Case Else
                canal = Canal_Hor_1
        End Select
        If sender.SelectedIndex > 0 Then
            canal.Stop(LayerTemplates)
            canal.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = $"""{sender.SelectedValue.FullName.ToString.Replace("\", "/")}""",
                         .VideoLayer = LayerVideoLoops,
                         .[Loop] = True
                         })
            canal.Play(LayerVideoLoops)
        Else
            canal.Clear()
        End If
    End Sub

    Private Sub LoadMediaDataSource()
        Dim mediaVertical As New List(Of MediaInfo) From {
            New MediaInfo With {.Name = "-", .FullName = "STOP"} ' Fake Item to Stop the player
            }
        For Each item As MediaInfo In CasparDevice.Mediafiles
            If item.FullName.Contains("VERTICALES") Then
                mediaVertical.Add(item)
            End If
        Next
        ComboBoxVertical1.DataSource = New BindingSource(mediaVertical, "")
        ComboBoxVertical1.DisplayMember = "Name"
        ComboBoxVertical2.DataSource = New BindingSource(mediaVertical, "")
        ComboBoxVertical2.DisplayMember = "Name"

        Dim mediaHorizontal As New List(Of MediaInfo) From {
           New MediaInfo With {.Name = "-", .FullName = "STOP"} ' Fake Item to Stop the player
           }
        For Each item As MediaInfo In CasparDevice.Mediafiles
            If item.FullName.Contains("HORIZONTALES") Then
                mediaHorizontal.Add(item)
            End If
        Next
        ComboBoxHor1.DataSource = New BindingSource(mediaHorizontal, "")
        ComboBoxHor1.DisplayMember = "Name"
        ComboBoxHor2.DataSource = New BindingSource(mediaHorizontal, "")
        ComboBoxHor2.DisplayMember = "Name"
        ComboBoxHor3.DataSource = New BindingSource(mediaHorizontal, "")
        ComboBoxHor3.DisplayMember = "Name"


    End Sub

    Private Sub SetMultiview()
        CasparDevice.Connection.SendString($"MIXER {PVW}-{VER1} VOLUME 0")
        CasparDevice.Connection.SendString($"MIXER {PVW}-{VER2} VOLUME 0")
        CasparDevice.Connection.SendString($"MIXER {PVW}-{HOR1} VOLUME 0")
        CasparDevice.Connection.SendString($"MIXER {PVW}-{HOR2} VOLUME 0")
        CasparDevice.Connection.SendString($"MIXER {PVW}-{HOR3} VOLUME 0")

        CasparDevice.Connection.SendString($"MIXER {PGM} MASTERVOLUME 0.75")
        CasparDevice.Connection.SendString($"PLAY {PVW}-{PGM} route://{PGM}")
        CasparDevice.Connection.SendString($"PLAY {PVW}-{VER1} route://{VER1}")
        CasparDevice.Connection.SendString($"PLAY {PVW}-{VER2} route://{VER2}")
        CasparDevice.Connection.SendString($"PLAY {PVW}-{HOR1} route://{HOR1}")
        CasparDevice.Connection.SendString($"PLAY {PVW}-{HOR2} route://{HOR2}")
        CasparDevice.Connection.SendString($"PLAY {PVW}-{HOR3} route://{HOR3}")
        'CasparDevice.Connection.SendString($"MIXER {PVW} GRID 2")
        CasparDevice.Connection.SendString($"MIXER {PVW}-{PGM} FILL 0.0078125 0.0152778 0.596875 0.598611 0 Linear")
        CasparDevice.Connection.SendString($"Mixer {PVW}-{VER1} FILL 0.610937 0.6125 0.335938 0.333333 0 Linear")
        CasparDevice.Connection.SendString($"Mixer {PVW}-{VER1} ROTATION -90 0 Linear")
        CasparDevice.Connection.SendString($"Mixer {PVW}-{VER2} FILL 0.80625 0.6125 0.335938 0.333333 0 Linear")
        CasparDevice.Connection.SendString($"Mixer {PVW}-{VER2} ROTATION -90 0 Linear")
        CasparDevice.Connection.SendString($"Mixer {PVW}-{HOR1} FILL 0.0078125 0.643056 0.325 0.327778 0 Linear")
        CasparDevice.Connection.SendString($"Mixer {PVW}-{HOR2} FILL 0.3375 0.643056 0.325 0.327778 0 Linear")
        CasparDevice.Connection.SendString($"Mixer {PVW}-{HOR3} FILL 0.667969 0.643056 0.325 0.327778 0 Linear")

    End Sub


    Private Sub Reproducir_loops(sorteo As Sorteos.Tipo)

        Dim LoopVertical = "KingLottery"
        Dim LoopHorizontal = "KingLottery"

        Select Case sorteo
            Case Sorteos.Tipo.Pick3_SXM
                LoopVertical = "pick3"
                LoopHorizontal = "pick3"
            Case Sorteos.Tipo.Pick4_SXM
                LoopVertical = "pick4"
                LoopHorizontal = "pick4"
            Case Sorteos.Tipo.Philipsburg
                LoopVertical = "Philipsburg"
                LoopHorizontal = "Philipsburg"
            Case Sorteos.Tipo.LotoPool
                LoopVertical = "LotoPool"
                LoopHorizontal = "LotoPool"
        End Select



        Canal_Ver_1.LoadBG(New CasparPlayingInfoItem With {.Clipname = $"""Loops_Verticales/{LoopVertical}""", .VideoLayer = LayerVideoLoops, .[Loop] = True})
        Canal_Ver_2.LoadBG(New CasparPlayingInfoItem With {.Clipname = $"""Loops_Verticales/{LoopVertical}""", .VideoLayer = LayerVideoLoops, .[Loop] = True})
        Canal_Hor_1.LoadBG(New CasparPlayingInfoItem With {.Clipname = $"""Loops_Horizontales/{LoopHorizontal}""", .VideoLayer = LayerVideoLoops, .[Loop] = True})
        Canal_Hor_2.LoadBG(New CasparPlayingInfoItem With {.Clipname = $"""Loops_Horizontales/{LoopHorizontal}""", .VideoLayer = LayerVideoLoops, .[Loop] = True})
        Canal_Hor_3.LoadBG(New CasparPlayingInfoItem With {.Clipname = $"""Loops_Horizontales/{LoopHorizontal}""", .VideoLayer = LayerVideoLoops, .[Loop] = True})

        Canal_Ver_1.Play(LayerVideoLoops)
        Canal_Ver_2.Play(LayerVideoLoops)
        Canal_Hor_1.Play(LayerVideoLoops)
        Canal_Hor_2.Play(LayerVideoLoops)
        Canal_Hor_3.Play(LayerVideoLoops)
    End Sub

    Private Sub Reproducir_Loto_bumpers(sorteo As Sorteos.Tipo)

        Dim Bumper = "KingLottery"
        Select Case sorteo
            Case Sorteos.Tipo.Pick3_SXM
                Bumper = "pick3"
            Case Sorteos.Tipo.Pick4_SXM
                Bumper = "pick4"
            Case Sorteos.Tipo.Philipsburg
                Bumper = "Philipsburg"
            Case Sorteos.Tipo.LotoPool
                Bumper = "LotoPool"
        End Select
        Canal_PGM.LoadBG(New CasparPlayingInfoItem With {.Clipname = $"""Bumpers/{Bumper}""", .VideoLayer = LayerBumpersSorteos, .[Loop] = False})
        Canal_PGM.Play(LayerBumpersSorteos)

    End Sub


    'Private Sub RadioButton_PGM_CheckedChanged(sender As RadioButton, e As EventArgs) Handles RadioButton_Verticales.CheckedChanged, RadioButton_PGM.CheckedChanged
    '    If sender.Checked Then
    '        Select Case True
    '            Case sender Is RadioButton_PGM
    '                Canal_PVW.Clear()
    '                CasparDevice.Connection.SendString($"PLAY {PVW}-13 route://{PGM}")
    '            Case sender Is RadioButton_Verticales
    '                SetMultiview()
    '        End Select
    '    End If
    'End Sub
#End Region

#Region "sorteos"
    Private Function EntradaBolo(bolo As Bola) As Boolean
        If bolo.OK Then
            Dim CGdata As New CasparCGDataCollection From {
                {$"f{bolo.Bolo}", bolo.Resultado}
            }
            Canal_PGM.CG.Update(LayerTemplates, 1, CGdata)
            Canal_PGM.CG.Invoke(LayerTemplates, 1, $"bolo{bolo.Bolo}")

            Return True
        Else
            Return False
        End If
    End Function

    Private Sub SaveResultado(bola As Bola)
        Dim PanelSorteo As PictureBox
        Dim Sorteo
        If bola.OK Then
            Select Case bola.Sorteo
                Case Sorteos.Tipo.Pick3_SXM
                    PanelSorteo = PictureBoxPick3
                    Sorteo = Pick3_SXM
                Case Sorteos.Tipo.Pick4_SXM
                    PanelSorteo = PictureBoxPick4
                    Sorteo = Pick4_SXM
                Case Sorteos.Tipo.LotoPool
                    PanelSorteo = PictureBoxLotoPool
                    Sorteo = LotoPool
                Case Sorteos.Tipo.Philipsburg
                    PanelSorteo = PictureBoxPhillip
                    Sorteo = Philipsburg
                Case Else
                    PanelSorteo = PictureBoxPick3
                    Sorteo = Pick3_SXM
            End Select

            For Each control In PanelSorteo.Controls.OfType(Of Label)
                If control.Tag = bola.Bolo Then control.Text = bola.Resultado
            Next
            Sorteo(bola.Bolo - 1) = bola
        End If
    End Sub

    Private Sub RB_CheckedChanged(sender As Object, e As EventArgs) Handles RB_Resultados.CheckedChanged, RB_pick4.CheckedChanged, RB_Pick3.CheckedChanged, RB_Phillps.CheckedChanged, RB_LotoPool.CheckedChanged

        PanelPick3.BackColor = Color.FromArgb(0, 0, 64)
        PanelPick4.BackColor = Color.FromArgb(0, 0, 64)
        PanelPhill.BackColor = Color.FromArgb(0, 0, 64)
        PanelLotoPool.BackColor = Color.FromArgb(0, 0, 64)


        Select Case True
            Case RB_Pick3.Checked
                TabControl_Sorteo.SelectedTab = TabPagePick3
                PanelPick3.BackColor = Color.Red
            Case RB_pick4.Checked
                TabControl_Sorteo.SelectedTab = TabPagePick4
                PanelPick4.BackColor = Color.Red
            Case RB_Phillps.Checked
                TabControl_Sorteo.SelectedTab = TabPagePhill
                PanelPhill.BackColor = Color.Red
            Case RB_LotoPool.Checked
                TabControl_Sorteo.SelectedTab = TabPageLotoPool
                PanelLotoPool.BackColor = Color.Red
            Case RB_Resultados.Checked
                TabControl_Sorteo.SelectedTab = TabPageCrawlSlates

        End Select

    End Sub

    Private Sub EntradaSorteo(sorteo As Sorteos.Tipo)
        Select Case sorteo
            Case Sorteos.Tipo.Pick3_SXM
                MyBackgroundThreadPick3()
            Case Sorteos.Tipo.Pick4_SXM
                MyBackgroundThreadPick4()
            Case Sorteos.Tipo.LotoPool
                MyBackgroundThreadLotoPool()
            Case Sorteos.Tipo.Philipsburg
                MyBackgroundThreadPhilipsburg()
        End Select
        SorteoActivo = Sorteos.Tipo.Done
    End Sub

#Region "Pick3"

    Private Sub ButtonEntradaPick3_Click(sender As Object, e As EventArgs) Handles ButtonEntradaPick3.Click

        SorteoActivo = Sorteos.Tipo.Pick3_SXM
        Reproducir_Loto_bumpers(Sorteos.Tipo.Pick3_SXM)

        ''  Dim thread As New Thread(AddressOf MyBackgroundThreadPick3)
        ''  Thread.Start()
    End Sub

    Sub MyBackgroundThreadPick3()
        Dim CGdata As New CasparCGDataCollection From {
           {$"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"}
       }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/Pick3_SXM", True, CGdata)

        Label_SXM3_1.Enabled = True
        NumPad_Pick3.ConfiguraNumeros(Sorteos.Tipo.Pick3_SXM, "1")
        NumPad_Pick3.Enabled = True
        PanelPick3.Enabled = True

        Threading.Thread.Sleep(1000)
        Reproducir_loops(Sorteos.Tipo.Pick3_SXM)
    End Sub

    Private Sub ButtonResultadoPick3_Click(sender As Object, e As EventArgs) Handles ButtonResultadoPick3.Click
        Dim CGdata As New CasparCGDataCollection From {
                {"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"},
                {"f1", Label_SXM3_1.Text},
                {"f2", Label_SXM3_2.Text},
                {"f3", Label_SXM3_3.Text}
            }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/Pick3_SXM_final", True, CGdata)

    End Sub

    Private Sub Label_SXM3_1_Click_1(sender As Object, e As EventArgs) Handles Label_SXM3_3.Click, Label_SXM3_2.Click, Label_SXM3_1.Click
        NumPad_Pick3.ConfiguraNumeros(Sorteos.Tipo.Pick3_SXM, sender.tag)
    End Sub

    Private Sub BoloEventPick3(Bolo As Bola) Handles NumPad_Pick3.Bolo_OK
        If Bolo.Bolo = 1 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Label_SXM3_2.Enabled = True
            NumPad_Pick3.ConfiguraNumeros(Bolo.Sorteo, "2")
        End If
        If Bolo.Bolo = 2 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Label_SXM3_3.Enabled = True
            NumPad_Pick3.ConfiguraNumeros(Bolo.Sorteo, "3")
        End If
        If Bolo.Bolo = 3 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            NumPad_Pick3.Clear()
            NumPad_Pick3.Enabled = False
        End If
    End Sub


#End Region

#Region "Pick 4"
    Private Sub ButtonEntradaPick4_Click(sender As Object, e As EventArgs) Handles ButtonEntradaPick4.Click
        SorteoActivo = Sorteos.Tipo.Pick4_SXM
        Reproducir_Loto_bumpers(Sorteos.Tipo.Pick4_SXM)
    End Sub

    Private Sub MyBackgroundThreadPick4()
        Dim CGdata As New CasparCGDataCollection From {
         {$"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"}
     }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/Pick4_SXM", True, CGdata)
        Label_SXM4_1.Enabled = True
        NumPad_Pick4.ConfiguraNumeros(Sorteos.Tipo.Pick4_SXM, "1")
        NumPad_Pick4.Enabled = True
        PanelPick4.Enabled = True
        Threading.Thread.Sleep(1000)
        Reproducir_loops(Sorteos.Tipo.Pick4_SXM)
    End Sub

    Private Sub ButtonResultadoPick4_Click(sender As Object, e As EventArgs) Handles ButtonResultadoPick4.Click
        Dim CGdata As New CasparCGDataCollection From {
                {"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"},
                {"f1", Label_SXM4_1.Text},
                {"f2", Label_SXM4_2.Text},
                {"f3", Label_SXM4_3.Text},
                {"f4", Label_SXM4_4.Text}
            }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/Pick4_SXM_final", True, CGdata)
    End Sub

    Private Sub Label_SXM4_1_Click(sender As Object, e As EventArgs) Handles Label_SXM4_4.Click, Label_SXM4_3.Click, Label_SXM4_2.Click, Label_SXM4_1.Click
        NumPad_Pick4.ConfiguraNumeros(Sorteos.Tipo.Pick4_SXM, sender.tag)
    End Sub

    Private Sub BoloEventPick4(Bolo As Bola) Handles NumPad_Pick4.Bolo_OK
        If Bolo.Bolo = 1 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Label_SXM4_2.Enabled = True
            NumPad_Pick4.ConfiguraNumeros(Bolo.Sorteo, "2")
        End If
        If Bolo.Bolo = 2 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Label_SXM4_3.Enabled = True
            NumPad_Pick4.ConfiguraNumeros(Bolo.Sorteo, "3")
        End If
        If Bolo.Bolo = 3 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Label_SXM4_4.Enabled = True
            NumPad_Pick4.ConfiguraNumeros(Bolo.Sorteo, "4")
        End If
        If Bolo.Bolo = 4 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            NumPad_Pick4.Clear()
            NumPad_Pick4.Enabled = False
        End If
    End Sub
#End Region

#Region "LotoPool"
    Private Sub ButtonEntradaLotoPool_Click(sender As Object, e As EventArgs) Handles ButtonEntradaLotoPool.Click
        SorteoActivo = Sorteos.Tipo.LotoPool
        Reproducir_Loto_bumpers(Sorteos.Tipo.LotoPool)

    End Sub

    Private Sub MyBackgroundThreadLotoPool()
        Dim CGdata As New CasparCGDataCollection From {
       {$"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"}
   }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/LotoPool", True, CGdata)
        Label_Pool1.Enabled = True
        ButtonListPadLotoPool.ConfiguraNumeros(Sorteos.Tipo.LotoPool, "1")
        ButtonListPadLotoPool.Enabled = True
        PanelLotoPool.Enabled = True
        Threading.Thread.Sleep(1000)
        Reproducir_loops(Sorteos.Tipo.LotoPool)
    End Sub
    Private Sub ButtonResultadoLotoPool_Click(sender As Object, e As EventArgs) Handles ButtonResultadoLotoPool.Click
        Dim CGdata As New CasparCGDataCollection From {
               {"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"},
               {"f1", Label_Pool1.Text},
               {"f2", Label_Pool2.Text},
               {"f3", Label_Pool3.Text},
               {"f4", Label_Pool4.Text}
           }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/LotoPool_final", True, CGdata)
    End Sub

    Private Sub Label_Pool1_Click(sender As Object, e As EventArgs) Handles Label_Pool4.Click, Label_Pool3.Click, Label_Pool2.Click, Label_Pool1.Click
        ButtonListPadLotoPool.ConfiguraNumeros(Sorteos.Tipo.LotoPool, sender.tag)
    End Sub

    Private Sub BoloEventLotoPool(Bolo As Bola) Handles ButtonListPadLotoPool.Bolo_OK
        If Bolo.Bolo = 1 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Label_Pool2.Enabled = True
            ButtonListPadLotoPool.ConfiguraNumeros(Bolo.Sorteo, "2", True, LotoPool)
        End If
        If Bolo.Bolo = 2 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Label_Pool3.Enabled = True
            ButtonListPadLotoPool.ConfiguraNumeros(Bolo.Sorteo, "3", True, LotoPool)
        End If
        If Bolo.Bolo = 3 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Label_Pool4.Enabled = True
            ButtonListPadLotoPool.ConfiguraNumeros(Bolo.Sorteo, "4", True, LotoPool)
        End If
        If Bolo.Bolo = 4 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            ButtonListPadLotoPool.Clear()
            ButtonListPadLotoPool.Enabled = False
        End If
    End Sub



#End Region

#Region "Philipsburg"
    Private Sub ButtonEntradaPhillip_Click(sender As Object, e As EventArgs) Handles ButtonEntradaPhillip.Click
        SorteoActivo = Sorteos.Tipo.Philipsburg
        Reproducir_Loto_bumpers(Sorteos.Tipo.Philipsburg)


    End Sub

    Private Sub MyBackgroundThreadPhilipsburg()
        Dim CGdata As New CasparCGDataCollection From {
     {$"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"}
 }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/PHIL", True, CGdata)
        Label_PHI4_1.Enabled = True
        MultiNumberPadPhilipsburg.ConfiguraNumeros(Sorteos.Tipo.Philipsburg, "1", Philipsburg)
        MultiNumberPadPhilipsburg.Enabled = True
        PanelPhill.Enabled = True
        Threading.Thread.Sleep(1000)
        Reproducir_loops(Sorteos.Tipo.Philipsburg)
    End Sub

    Private Sub ButtonResultadosPhillips_Click(sender As Object, e As EventArgs) Handles ButtonResultadosPhillips.Click
        Dim CGdata As New CasparCGDataCollection
        For Each bolo In Philipsburg
            CGdata.Add($"f{bolo?.Bolo}", bolo?.Resultado)
        Next
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/PHIL_final", True, CGdata)
    End Sub

    Private Sub BoloEventPhillipsburg(Bolo As Bola) Handles MultiNumberPadPhilipsburg.Bolo_OK
        If Bolo.Bolo < 12 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Dim nextBolo = CInt(Bolo.Bolo) + 1
            For Each control In PictureBoxPhillip.Controls.OfType(Of Label)
                If control.Tag = nextBolo Then control.Enabled = True
            Next
            MultiNumberPadPhilipsburg.ConfiguraNumeros(Bolo.Sorteo, nextBolo.ToString, Philipsburg)
        ElseIf Bolo.Bolo = 12 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            MultiNumberPadPhilipsburg.Clear()
            MultiNumberPadPhilipsburg.Enabled = False
        End If

    End Sub


#End Region

#End Region

    Private Sub ButtonQuinielaFullSrceen(sender As Object, e As EventArgs) Handles ButtonQuinielaFullScreen.Click
        Dim CGdata As New CasparCGDataCollection From {
                {"f1", $"{Label_SXM3_2.Text}{Label_SXM3_3.Text}"},
                {"f2", $"{Label_SXM4_1.Text}{Label_SXM4_2.Text}"},
                {"f3", $"{Label_SXM4_3.Text}{Label_SXM4_4.Text}"}
            }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/quiniela", True, CGdata)
    End Sub
    Private Sub ButtonResultados_Click(sender As Object, e As EventArgs) Handles ButtonResultados.Click
        Dim CGdata As New CasparCGDataCollection From {
                {"f1", Label_SXM3_1.Text},
                {"f2", Label_SXM3_2.Text},
                {"f3", Label_SXM3_3.Text},
                {"f4", Label_SXM4_1.Text},
                {"f5", Label_SXM4_2.Text},
                {"f6", Label_SXM4_3.Text},
                {"f7", Label_SXM4_4.Text},
                {"f8", $"{Philipsburg(0)?.Resultado} {Philipsburg(1)?.Resultado} {Philipsburg(2)?.Resultado} {Philipsburg(3)?.Resultado}"},
                {"f9", $"{Philipsburg(4)?.Resultado} {Philipsburg(5)?.Resultado} {Philipsburg(6)?.Resultado} {Philipsburg(7)?.Resultado}"},
                {"f10", $"{Philipsburg(8)?.Resultado} {Philipsburg(9)?.Resultado} {Philipsburg(10)?.Resultado} {Philipsburg(11)?.Resultado}"},
                {"f11", Label_Pool1.Text},
                {"f12", Label_Pool2.Text},
                {"f13", Label_Pool3.Text},
                {"f14", Label_Pool4.Text},
                {"f15", $"{Label_SXM3_2.Text}{Label_SXM3_3.Text}"},
                {"f16", $"{Label_SXM4_1.Text}{Label_SXM4_2.Text}"},
                {"f17", $"{Label_SXM4_3.Text}{Label_SXM4_4.Text}"}
            }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/Resultados_QN", True, CGdata)
    End Sub

    Private Sub ButtonGeneral_Click(sender As Object, e As EventArgs) Handles ButtonGeneral.Click
        Dim CGdata As New CasparCGDataCollection From {
               {"f0", TextBoxGenerico1.Text},
               {"f1", TextBoxGenerico2.Text}
               }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/General", True, CGdata)

    End Sub


    Private Sub CheckBoxMosca_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxMosca.CheckedChanged
        Dim bug = "bug1"
        If RadioButtonBug2.Checked Then
            bug = "bug2"
        End If

        If sender.Checked Then
            Canal_PGM.CG.Add(99, 1, $"King/{bug}", True)
            sender.Text = "Ocultar Bug"
        Else
            Canal_PGM.CG.Stop(99, 1)
            sender.Text = "Mostrar Bug"
        End If
    End Sub

    Private Sub Button_Capturas_Click(sender As Object, e As EventArgs) Handles Button_Capturas.Click
        If CasparDevice.IsConnected Then
            CasparDevice.Connection.SendString($"ADD 1 IMAGE Capturas/Sorteo_{SorteoDeHoy:0000}_{Date.Now:dd-MM-yy_hh-mm}")
        End If
    End Sub

    Private Sub CheckBoxRec_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxRec.CheckedChanged
        If CasparDevice.IsConnected Then
            If CheckBoxRec.Checked Then
                If TimerGrabacion.Enabled = False Then
                    min = "00"
                    sec = "00"
                    TimerGrabacion.Enabled = True
                    aa = Val(Now.Second.ToString) 'new code
                End If

                Dim params = ".mp4 -codec:v libx264 -profile:v high444p -pixel_format:v yuv444p-s 1280x720 -preset:v default"
                If CasparDevice.Version.StartsWith("2.3") Then
                    ''  params = ".mp4 -codec:v libx264 -crf:v 23 -preset:v veryfast -filter:v format=pix_fmts=yuv422p -flags:v +ildct+ilme -codec:a aac -b:a 128k -ar:a 48k -filter:a pan=stereo|c0=c0|c1=c2"

                    '' If True Then
                    params = ".mxf -b:v 20000000 -codec:a pcm_s16le -codec:v mpeg2video -filter:v format=yuv422p  -minrate:v 20000k -maxrate:v 20000k -color_primaries:v bt709 -color_trc:v 1 -colorspace:v 1 -filter:a pan=stereo|c0=c0|c1=c1"
                    ''  End If
                End If



                Dim result = CasparDevice.Connection.SendStringWithResult($"ADD 1-100 FILE  Grabaciones/Sorteo_{SorteoDeHoy:0000}_{Date.Now:dd-MM-yy_hh-mm}{params}", New TimeSpan(0, 0, 2))
                CheckBoxRec.Text = "Grabando..."
                LabelRecTimer.BackColor = Color.IndianRed
                '  MessageBox.Show(result)
            Else
                TimerGrabacion.Enabled = False
                LabelRecTimer.Text = $"{min}:{sec}"
                Dim result = CasparDevice.Connection.SendStringWithResult("REMOVE 1-100", New TimeSpan(0, 0, 5))
                '  MessageBox.Show(result)
                CheckBoxRec.Text = "Iniciar Grabacion"
                LabelRecTimer.BackColor = SystemColors.ControlDarkDark
            End If
        Else
            CheckBoxRec.Checked = False
        End If
    End Sub

    Dim aa As Integer
    Dim sec As String
    Dim min As String = "00"
    Dim puntos = 0
    Private Sub TimerGrabacion_Tick(sender As Object, e As EventArgs) Handles TimerGrabacion.Tick
        On Error Resume Next
        Dim bb = Val(Now.Second.ToString)
        Dim dif As Integer = (bb - aa)
        aa = bb
        If dif < 0 Then dif = dif + 60
        sec = Format(Val(sec + dif), "00")
        If sec > 59 Then
            sec = "00"
            min = Format(Val(min + 1), "0")
        End If

        If puntos > 1 Then
            LabelRecTimer.Text = $"{min}:{sec}"
            puntos = 0
        Else
            LabelRecTimer.Text = $"{min} {sec}"
        End If
        puntos += 1
    End Sub

    Private Sub PresentadoresYJuecesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PresentadoresYJuecesToolStripMenuItem.Click
        Dim configAutoridades As New PersonasForm
        If configAutoridades.ShowDialog = DialogResult.OK Then
            LoadDataSource()
            SetupComboxes()
        End If
    End Sub
    Private Sub LoadDataSource()
        If File.Exists("JuecesYpresentadores.json") Then
            Dim json = File.ReadAllText("JuecesYpresentadores.json")
            Talentos = JsonConvert.DeserializeObject(Of Talentos)(json)
        End If
        BindingSourceJueces.DataSource = Talentos.Jueces
        BindingSourcePresentadores.DataSource = Talentos.Presentadores

    End Sub

    Private Sub SetupComboxes()
        ComboBoxPresentador.DataSource = New BindingSource(Talentos, "Presentadores")
        ComboBoxPresentador.DisplayMember = "Nombre"
        ComboBoxPresentador.ValueMember = "Titulo"

        ComboBoxJurado1.DataSource = New BindingSource(Talentos, "Jueces")
        ComboBoxJurado1.DisplayMember = "Nombre"
        ComboBoxJurado1.ValueMember = "Titulo"

        ComboBoxJurado2.DataSource = New BindingSource(Talentos, "Jueces")
        ComboBoxJurado2.DisplayMember = "Nombre"
        ComboBoxJurado2.ValueMember = "Titulo"

        ComboBoxJurado3.DataSource = New BindingSource(Talentos, "Jueces")
        ComboBoxJurado3.DisplayMember = "Nombre"
        ComboBoxJurado3.ValueMember = "Titulo"

        ComboBoxJurado4.DataSource = New BindingSource(Talentos, "Jueces")
        ComboBoxJurado4.DisplayMember = "Nombre"
        ComboBoxJurado4.ValueMember = "Titulo"
    End Sub

    Private Sub ComboBoxJurado1_SelectedIndexChanged(sender As ComboBox, e As EventArgs) Handles _
       ComboBoxJurado1.SelectedIndexChanged,
       ComboBoxJurado2.SelectedIndexChanged,
       ComboBoxJurado3.SelectedIndexChanged,
       ComboBoxPresentador.SelectedIndexChanged,
       ComboBoxJurado4.SelectedIndexChanged
        Dim item As Personas = sender.SelectedItem
        Dim listaTextbox = sender.Parent.Controls.OfType(Of TextBox)
        For Each tbox As TextBox In listaTextbox
            If tbox.Tag = "Nombre" Then tbox.Text = item?.Nombre
            If tbox.Tag = "Titulo" Then tbox.Text = item?.Titulo
        Next

    End Sub

    Private Sub ButtonJuez1_Click(sender As Object, e As EventArgs) Handles ButtonJuez1.Click
        Dim graficoJueces = "King/General"
        Dim CGdata As New CasparCGDataCollection From {
            {"f0", TextBoxJf0.Text},
            {"f1", TextBoxjf1.Text}
        }
        Canal_PGM.CG.Add(LayerTemplates, 1, graficoJueces, True, CGdata)

    End Sub
    Private Sub Button_update_jurado_1_Click(sender As Object, e As EventArgs) Handles Button_update_jurado_1.Click
        Dim CGdata As New CasparCGDataCollection From {
           {"f0", TextBoxJf0.Text},
           {"f1", TextBoxjf1.Text}
       }
        Canal_PGM.CG.Update(LayerTemplates, 1, CGdata)
    End Sub

    Private Sub Button_Juez2_Click(sender As Object, e As EventArgs) Handles Button_Juez2.Click
        Dim graficoJueces = "King/General"
        Dim CGdata As New CasparCGDataCollection From {
            {"f0", TextBoxJf2.Text},
            {"f1", TextBoxJf3.Text}
        }
        Canal_PGM.CG.Add(LayerTemplates, 1, graficoJueces, True, CGdata)
    End Sub

    Private Sub Button_update_jurado_2_Click(sender As Object, e As EventArgs) Handles Button_update_jurado_2.Click
        Dim CGdata As New CasparCGDataCollection From {
           {"f0", TextBoxJf2.Text},
           {"f1", TextBoxJf3.Text}
       }
        Canal_PGM.CG.Update(LayerTemplates, 1, CGdata)
    End Sub

    Private Sub Button_Juez3_Click(sender As Object, e As EventArgs) Handles Button_Juez3.Click
        Dim graficoJueces = "King/General"
        Dim CGdata As New CasparCGDataCollection From {
            {"f0", TextBoxJf4.Text},
            {"f1", TextBoxJf5.Text}
        }
        Canal_PGM.CG.Add(LayerTemplates, 1, graficoJueces, True, CGdata)
    End Sub
    Private Sub Button_update_jurado_3_Click(sender As Object, e As EventArgs) Handles Button_update_jurado_3.Click
        Dim CGdata As New CasparCGDataCollection From {
           {"f0", TextBoxJf4.Text},
           {"f1", TextBoxJf5.Text}
       }
        Canal_PGM.CG.Update(LayerTemplates, 1, CGdata)
    End Sub
    Private Sub Button_Juez4_Click(sender As Object, e As EventArgs) Handles Button_Juez4.Click
        Dim graficoJueces = "King/General"
        Dim CGdata As New CasparCGDataCollection From {
            {"f0", TextBoxJf6.Text},
            {"f1", TextBoxJf7.Text}
        }
        Canal_PGM.CG.Add(LayerTemplates, 1, graficoJueces, True, CGdata)
    End Sub
    Private Sub Button_update_jurado_4_Click(sender As Object, e As EventArgs) Handles Button_update_jurado_4.Click
        Dim CGdata As New CasparCGDataCollection From {
           {"f0", TextBoxJf6.Text},
           {"f1", TextBoxJf7.Text}
       }
        Canal_PGM.CG.Update(LayerTemplates, 1, CGdata)
    End Sub

    Private Sub ConsolaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConsolaToolStripMenuItem.Click
        Dim consola As New Consola
        consola.Show()
    End Sub

    Private Sub Templates_salida_Click(sender As Object, e As EventArgs) Handles templates_salida.Click
        Canal_PGM.CG.Stop(LayerTemplates, 1)
    End Sub

    Private Sub ButtonClearAll_Click(sender As Object, e As EventArgs) Handles ButtonClearAll.Click
        Canal_PGM.Clear()
    End Sub

    Private Sub Button_ModoSorteo_Click(sender As Object, e As EventArgs) Handles Button_ModoSorteo.Click
        If MessageBox.Show($"Inicio del Sorteo de hoy: {Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO"))} {vbCrLf}Una vez iniciado el sorteo, Se guarda el número del sorteo y no se puede regresar al modo ensayo", "Iniciar Sorteo de Hoy?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.OK Then
            My.Settings.NumeroSorteo = SorteoDeHoy
            LabelModo.Text = $"Modo Sorteo: {SorteoDeHoy:0000}"
            '  Button_ensayo.Enabled = False
            Button_ModoSorteo.Enabled = False
        Else
            ' CheckBoxSorteo.Checked = False
        End If
    End Sub

    Private Sub ButtonGuardar_Click(sender As Object, e As EventArgs) Handles ButtonGuardar.Click
        SaveFileDialog1.Filter = "Json Files (*.Json*)|*.Json"
        SaveFileDialog1.FileName = $"LotoKing_{Date.Now.Year}_{Date.Now.Month.ToString("00")}_{Date.Today.Day.ToString("00")}_{SorteoDeHoy.ToString("0000")}"
        If SaveFileDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim jurados As New List(Of Personas) From {
                New Personas With {.Nombre = TextBoxJf0.Text, .Titulo = TextBoxjf1.Text},
                New Personas With {.Nombre = TextBoxJf2.Text, .Titulo = TextBoxJf3.Text},
                New Personas With {.Nombre = TextBoxJf4.Text, .Titulo = TextBoxJf5.Text},
                New Personas With {.Nombre = TextBoxJf6.Text, .Titulo = TextBoxJf7.Text}
            }

            Dim presentador = New Personas With {.Nombre = TextBoxGenerico1.Text, .Titulo = TextBoxGenerico2.Text}

            Dim Resultados = New Resultados With {.Fecha = Today, .SorteoId = SorteoDeHoy.ToString("0000"), .Jueces = jurados, .Presentador = presentador,
                                                  .LotoPool = LotoPool, .Philipsburg = Philipsburg, .Pick3_SXM = Pick3_SXM, .Pick4_SXM = Pick4_SXM}
            Dim json = JsonConvert.SerializeObject(Resultados, New JsonSerializerSettings With {.Formatting = Newtonsoft.Json.Formatting.Indented, .NullValueHandling = NullValueHandling.Ignore})
            My.Computer.FileSystem.WriteAllText(SaveFileDialog1.FileName, json, False)
        End If
    End Sub

    Private Sub ButtonCargar_Click(sender As Object, e As EventArgs) Handles ButtonCargar.Click
        Try
            If OpenFileDialog1.ShowDialog = DialogResult.OK Then
                Dim json = File.ReadAllText(OpenFileDialog1.FileName)
                Dim Resultados = JsonConvert.DeserializeObject(Of Resultados)(json)
                Pick3_SXM = Resultados.Pick3_SXM
                Pick4_SXM = Resultados.Pick4_SXM
                Philipsburg = Resultados.Philipsburg
                LotoPool = Resultados.LotoPool
                '' Aca creamos un array con los tres panales de sorteos y luego obtenemos todos lso botones dentro de cada panel, 
                '' asiganmemos  resultados que tenemos para ese boton/bolo al text del boton correspondiente. 
                Dim paneles = {PictureBoxPick3, PictureBoxPick4, PictureBoxPhillip, PictureBoxLotoPool}
                Dim result As Integer
                For Each panel In paneles
                    For Each control In panel.Controls.OfType(Of Label)
                        If Not control.Tag = "" Then
                            Select Case panel.Name
                                Case PictureBoxPick3.Name
                                    If Integer.TryParse(control.Tag, result) Then control.Text = Pick3_SXM(result - 1)?.Resultado
                                Case PictureBoxPick4.Name
                                    If Integer.TryParse(control.Tag, result) Then control.Text = Pick4_SXM(result - 1)?.Resultado
                                Case PictureBoxPhillip.Name
                                    If Integer.TryParse(control.Tag, result) Then control.Text = Philipsburg(result - 1)?.Resultado
                                Case PictureBoxLotoPool.Name
                                    If Integer.TryParse(control.Tag, result) Then control.Text = LotoPool(result - 1)?.Resultado
                            End Select
                        End If
                    Next
                Next
                ComboBoxPresentador.SelectedIndex = ComboBoxPresentador.FindStringExact(Resultados.Presentador.Nombre)
                ComboBoxJurado1.SelectedIndex = ComboBoxJurado1.FindStringExact(Resultados.Jueces(0).Nombre)
                ComboBoxJurado2.SelectedIndex = ComboBoxJurado1.FindStringExact(Resultados.Jueces(1).Nombre)
                ComboBoxJurado3.SelectedIndex = ComboBoxJurado1.FindStringExact(Resultados.Jueces(2).Nombre)
                ComboBoxJurado4.SelectedIndex = ComboBoxJurado1.FindStringExact(Resultados.Jueces(3).Nombre)

            End If
        Catch ex As Exception
            MessageBox.Show($"Error Cargando datos:{vbCrLf}{ex.Message}")
        End Try
    End Sub

    Private Sub ButtonLimpiar_Click(sender As Object, e As EventArgs) Handles ButtonLimpiar.Click
        Array.Clear(Pick3_SXM, 0, Pick3_SXM.Length)
        Array.Clear(Pick4_SXM, 0, Pick4_SXM.Length)
        Array.Clear(Philipsburg, 0, Philipsburg.Length)
        Array.Clear(LotoPool, 0, LotoPool.Length)
        Dim paneles = {PictureBoxPick3, PictureBoxPick4, PictureBoxPhillip, PictureBoxLotoPool}
        For Each panel In paneles
            For Each control In panel.Controls.OfType(Of Label)
                If Not control.Tag = "" Then
                    control.Text = ""
                    control.Enabled = False
                End If
            Next
        Next
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Dim url = My.Settings.RTMP_URL
            Dim vBitrate = My.Settings.StreamVideoBitrate
            Dim pic_size = My.Settings.StreamPictureSize
            Dim stream = $"ADD {PGM} STREAM {url}-500 -codec:a aac -strict -2  -b:a 128k -ar:a 48000 -b:v {vBitrate} -filter:v format=pix_fmts=yuv422p,scale={pic_size},fps=30 -filter:a pan=stereo|c0=c0|c1=c1 -format flv"
            CasparDevice.Connection.SendString(stream)
            CheckBox1.Text = "Streaming..."
        Else
            CasparDevice.Connection.SendString($"REMOVE {PGM}-500")
            CheckBox1.Text = "Iniciar Stream..."
        End If
    End Sub



    Private Sub StreamToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StreamToolStripMenuItem.Click
        Dim stream As New StreamForm
        stream.ShowDialog()
    End Sub

    Private Sub UsuariosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UsuariosToolStripMenuItem1.Click
        Dim Ventana As New UsersForm
        Ventana.ShowDialog()
        Ventana.Dispose()
    End Sub

    Private Sub LabelVersion_DoubleClick(sender As Object, e As EventArgs) Handles LabelVersion.DoubleClick
        If MessageBox.Show("Reiniciar el Servidor Grafico", "Seguro desea reiniciar el servidor grafico??", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) = DialogResult.OK Then
            Try
                Oscserver.Stop()
            Catch ex As Exception

            End Try
            StartCasparcgServer()
        End If

    End Sub

    Private Sub P5994ToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles P5994ToolStripMenuItem.Click, P2997ToolStripMenuItem.Click
        CheckMenuItem(SalidaPGMToolStripMenuItem, sender)
        UpdateVideoMode(sender.Tag)
    End Sub

    Private Sub DebugToolStripMenuItem_Click(sender As ToolStripMenuItem, e As EventArgs) Handles WarningToolStripMenuItem.Click, InfoToolStripMenuItem.Click, FatalToolStripMenuItem.Click, ErrorToolStripMenuItem.Click, DebugToolStripMenuItem.Click
        CheckMenuItem(LoglevelToolStripMenuItem, sender)
        UpdateLogLevel(sender.Tag)
    End Sub
    Private Sub CheckMenuItem(ByVal mnu As ToolStripMenuItem,
    ByVal checked_item As ToolStripMenuItem)
        ' Uncheck the menu items except checked_item.
        For Each item As ToolStripItem In mnu.DropDownItems
            If (TypeOf item Is ToolStripMenuItem) Then
                Dim menu_item As ToolStripMenuItem =
                DirectCast(item, ToolStripMenuItem)
                menu_item.Checked = (menu_item Is checked_item)
            End If
        Next item
    End Sub

    Private Sub CheckSavedConfig(menu As ToolStripMenuItem, saved_setting As String)
        For Each item As ToolStripMenuItem In menu.DropDownItems
            If (TypeOf item Is ToolStripMenuItem) Then
                item.Checked = String.Equals(item.Tag, saved_setting)
            End If
        Next item
    End Sub

    Private Sub UpdateVideoMode(mode)
        Try

            Dim xmlDoc As New XmlDocument
            xmlDoc.Load("server\casparcg.config")
            Dim node_Canales As XmlNode = xmlDoc.SelectSingleNode("/configuration/channels") '/channel/consumers/decklink
            ' Dim node_PGM As XmlNode = node_Canales.FirstChild()

            For Each node In node_Canales
                node.SelectSingleNode("video-mode").InnerText = mode
            Next
            xmlDoc.Save("server\casparcg.config")

            My.Settings.videoMode = mode
            If MessageBox.Show($"Server Configurado con Video mode: {mode}, Reiniciar el server para implementar los cambios?") = DialogResult.OK Then
                LabelVersion_DoubleClick(Nothing, Nothing)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub

    Private Sub UpdateLogLevel(level)
        Try
            Dim xmlDoc As New XmlDocument
            xmlDoc.Load("server\casparcg.config")
            Dim logLevel As XmlNode = xmlDoc.SelectSingleNode("/configuration/log-level") '/channel/consumers/decklink
            logLevel.InnerText = level
            xmlDoc.Save("server\casparcg.config")

            My.Settings.log = level
            If MessageBox.Show($"Server Configurado con Video mode: {level}, Reiniciar el server para implementar los cambios?") = DialogResult.OK Then
                LabelVersion_DoubleClick(Nothing, Nothing)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub


    Private Sub InternalKeyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InternalKeyToolStripMenuItem.CheckedChanged
        Try
            My.Settings.KeyerInternal = InternalKeyToolStripMenuItem.Checked
            Dim keyer = "external_separate_device"
            If InternalKeyToolStripMenuItem.Checked Then keyer = "internal"
            Dim xmlDoc As New XmlDocument
            xmlDoc.Load("server\casparcg.config")
            Dim node_Canales As XmlNode = xmlDoc.SelectSingleNode("/configuration/channels") '/channel/consumers/decklink
            Dim node_PGM As XmlNode = node_Canales.FirstChild()


            node_PGM.SelectSingleNode("consumers/decklink/keyer").InnerText = keyer
            xmlDoc.Save("server\casparcg.config")
            If MessageBox.Show($"Server Configurado con keyer: {keyer}, Reiniciar el server para implementar los cambios?") = DialogResult.OK Then
                LabelVersion_DoubleClick(Nothing, Nothing)
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

    End Sub


#Region "Crawl"
    Private Sub ButtonEliminarItem_Click(sender As Object, e As EventArgs)

    End Sub



    Private Sub ButtonExport_Click(sender As Object, e As EventArgs) Handles ButtonExport.Click
        Try
            SaveFileDialog1.FileName = "Crawl"
            SaveFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
            If (SaveFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK) Then

                Dim xmlsetting As New XmlWriterSettings With {.Indent = True}
                Using writer As XmlWriter = XmlWriter.Create(SaveFileDialog1.FileName, xmlsetting)
                    writer.WriteStartDocument()
                    writer.WriteStartElement("Crawl") ' Root.
                    For Each row As DataGridViewRow In DataGridView_Crawl.Rows
                        If row.Cells("CR_Texto").Value IsNot Nothing Then
                            writer.WriteStartElement("Item")
                            writer.WriteStartAttribute("Texto")
                            writer.WriteValue(row.Cells("CR_Texto").Value)
                            writer.WriteEndAttribute()
                            writer.WriteStartAttribute("Active")
                            If row.Cells("CR_active").Value IsNot Nothing Then
                                writer.WriteValue(row.Cells("CR_active").Value)
                            Else
                                writer.WriteValue("false")
                            End If
                            writer.WriteEndAttribute()
                            writer.WriteEndElement()
                        End If
                    Next
                    writer.WriteEndElement()
                    writer.WriteEndDocument()
                End Using
            End If
        Catch ex As Exception
            MsgBox("Error generando el archivo XML " & ex.Message)
        End Try
    End Sub

    Private Sub ButtonImport_Click(sender As Object, e As EventArgs) Handles ButtonImport.Click
        Try
            OpenFileDialog1.Filter = "xml files (*.xml)|*.xml|All files (*.*)|*.*"
            If (OpenFileDialog1.ShowDialog() = Windows.Forms.DialogResult.OK) Then
                Dim response As New Xml.XmlDocument
                response.Load(OpenFileDialog1.FileName)
                DataGridView_Crawl.Rows.Clear()
                For Each instance As Xml.XmlElement In response.GetElementsByTagName("Item")
                    '  AgregarItemAlRundownTool(StringToType(instance.GetAttribute("Tipo")), instance.GetAttribute("Nombre"), instance.GetAttribute("Auto"))
                    DataGridView_Crawl.Rows.Add({instance.GetAttribute("Texto"), instance.GetAttribute("Active")})
                Next
            End If
        Catch ex As Exception
            MsgBox("Error cargando el archivo XML de guardado " & ex.Message)
        End Try
    End Sub

    Private Sub Button_Crawl_Entrada_Click(sender As Object, e As EventArgs) Handles Button_Crawl_Entrada.Click
        Dim file = "King/Separador.PNG"
        Dim separador = $"<img vspace=""0"" hspace=""0"" height=""40"" width=""30"" src=""{file}"">"
        Dim texto As String = ""
        For Each row As DataGridViewRow In DataGridView_Crawl.Rows
            If row.Cells("CR_active").Value = True Then
                texto = $"{texto} {separador} {row.Cells("CR_texto").Value.ToString.Replace(vbCr, "").Replace(vbLf, "")}"
            End If
        Next
        texto = $"{texto} {separador}"
        ticker(texto)
        ' activeTemplate = Templates.crawl
    End Sub

    Public Sub ticker(text As String)
        Dim CGdata As New Svt.Caspar.CasparCGDataCollection
        CGdata.SetData("scrolldata", text)
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/Crawl", True, CGdata.ToAMCPEscapedXml)

    End Sub

    Private Sub Button_crawl_update_Click(sender As Object, e As EventArgs) Handles Button_crawl_update.Click
        Dim file = "King/Separador.PNG"
        Dim separador = $"<img vspace=""0"" hspace=""0"" height=""40"" width=""30"" src=""{file}"">"
        'Dim separador = "¦"
        Dim texto As String = ""
        For Each row As DataGridViewRow In DataGridView_Crawl.Rows
            If row.Cells("CR_active").Value = True Then
                texto = $"{texto} {separador} {row.Cells("CR_texto").Value.ToString.Replace(vbCr, "").Replace(vbLf, "")}"
            End If
        Next
        texto = $"{texto} {separador}"
        Dim CGdata As New Svt.Caspar.CasparCGDataCollection
        CGdata.SetData("scrolldata", texto)
        Canal_PGM.CG.Update(LayerTemplates, 1, CGdata.ToAMCPEscapedXml)
    End Sub
    Private Sub ButtonActivar_Click(sender As Object, e As EventArgs) Handles ButtonActivar.Click
        Try
            Dim xmlsetting As New XmlWriterSettings With {.Indent = True}
            Using writer As XmlWriter = XmlWriter.Create("Crawl.XML", xmlsetting)
                writer.WriteStartDocument()
                writer.WriteStartElement("Crawl") ' Root.
                For Each row As DataGridViewRow In DataGridView_Crawl.Rows
                    If row.Cells("CR_Texto").Value IsNot Nothing Then
                        writer.WriteStartElement("Item")
                        writer.WriteStartAttribute("Texto")
                        writer.WriteValue(row.Cells("CR_Texto").Value)
                        writer.WriteEndAttribute()
                        writer.WriteStartAttribute("Active")
                        If row.Cells("CR_active").Value IsNot Nothing Then
                            writer.WriteValue(row.Cells("CR_active").Value)
                        Else
                            writer.WriteValue("false")
                        End If
                        writer.WriteEndAttribute()
                        writer.WriteEndElement()
                    End If
                Next
                writer.WriteEndElement()
                writer.WriteEndDocument()
            End Using
        Catch ex As Exception
            MsgBox("Error generando el archivo XML " & ex.Message)
        End Try
    End Sub

    Private Sub LoadCrawl()
        Try
            If File.Exists("Crawl.XML") Then
                Dim response As New Xml.XmlDocument
                response.Load("Crawl.XML")
                DataGridView_Crawl.Rows.Clear()
                For Each instance As Xml.XmlElement In response.GetElementsByTagName("Item")
                    '  AgregarItemAlRundownTool(StringToType(instance.GetAttribute("Tipo")), instance.GetAttribute("Nombre"), instance.GetAttribute("Auto"))
                    DataGridView_Crawl.Rows.Add({instance.GetAttribute("Texto"), instance.GetAttribute("Active")})
                Next
            End If
        Catch ex As Exception
            MsgBox("Error cargando el archivo XML de guardado " & ex.Message)
        End Try
    End Sub

    Private Sub TrackBar2_Scroll(sender As Object, e As EventArgs) Handles TrackBar_crawlSpeed.Scroll
        Dim CGdata As New CasparCGDataCollection From {
              {"speed", TrackBar_crawlSpeed.Value}
              }
        Canal_PGM.CG.Update(LayerTemplates, 1, CGdata)
    End Sub

#End Region

    Private Sub ButtonClaquetaPick3_Click(sender As Object, e As EventArgs) Handles ButtonClaquetaPick4.Click, ButtonClaquetaPick3.Click, ButtonClaquetaPhil.Click, ButtonClaquetaLotoPool.Click
        Select Case sender.tag
            Case "Pick3_SXM"
                MuestraClaqueta(Sorteos.Tipo.Pick3_SXM)
            Case "Pick4_SXM"
                MuestraClaqueta(Sorteos.Tipo.Pick4_SXM)
            Case "Philipsburg"
                MuestraClaqueta(Sorteos.Tipo.Philipsburg)
            Case "LotoPool"
                MuestraClaqueta(Sorteos.Tipo.LotoPool)
        End Select
    End Sub
    Private Sub MuestraClaqueta(sorteo As Sorteos.Tipo)
        If CasparDevice.IsConnected Then


            Dim titulo = ""
            Dim sorteoSlate = ""
            Select Case sorteo
                Case Sorteos.Tipo.Pick3_SXM
                    titulo = "PICK 3 SXM"
                    sorteoSlate = My.Settings.SlatePick3
                Case Sorteos.Tipo.Pick4_SXM
                    titulo = "PICK 4 SXM"
                    sorteoSlate = My.Settings.SlatePick4
                Case Sorteos.Tipo.Philipsburg
                    titulo = "PHILIPSBURG"
                    sorteoSlate = My.Settings.SlatePhilipsburg
                Case Sorteos.Tipo.LotoPool
                    titulo = "LOTO POOL"
                    sorteoSlate = My.Settings.SlateLotoPool
            End Select

            Dim CGdata As New CasparCGDataCollection From {
           {"f0", $"{titulo}"},
           {"f1", $"Sorteo: {sorteoSlate}"}
       }
            Canal_PGM.CG.Add(LayerTemplates, 1, "King/Slate", True, CGdata)
        End If
    End Sub


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
    Public Property Philipsburg As Bola()
    Public Property LotoPool As Bola()
    Public Property Presentador As Personas
    Public Property Jueces As List(Of Personas)
End Class

Public Class Sorteos
    Enum Tipo
        Pick3_SXM
        Pick4_SXM
        Philipsburg
        LotoPool
        Done
    End Enum
End Class

#End Region
