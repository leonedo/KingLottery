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
    Const Version = "0.0.5" '

    Public WithEvents CasparDevice As CasparDevice
    Public Canal_PGM As ChannelManager
    ' Public Canal_PVW As ChannelManager
    Public Canal_Ver_1 As ChannelManager
    Public Canal_Ver_2 As ChannelManager
    'Public Canal_Ver_3 As ChannelManager
    Private WithEvents Oscserver As OscServer
    Public Process As Process
    Public ScannerProcess As Process
    Private Delegate Sub ProcessOutputDataCallback(sender As Object, e As DataReceivedEventArgs)
    Private Pick3_SXM(3) As Bola
    Private Pick4_SXM(4) As Bola
    Private Phillipsburg(12) As Bola
    Private LotoPool(4) As Bola
    Private Talentos As New Talentos
    Private SorteoDeHoy As Integer = My.Settings.NumeroSorteo + 1

#Region "Channal and leyer configuration"
    Const PGM = 1
    ' Const PVW = 2
    Const VER1 = 2
    Const VER2 = 3
    'Const VER3 = 5
    Const LayerVideoVertical = 10
    Const LayerTemplates = 20
    Const LayerBumpers = 50
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
            'LoadDataSource()
            'SetupComboxes()
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
            StartCasparcgServer()
        ElseIf user = 2 Then
            LabelUser.Text = "Debug"
            TableLayoutPanel1.Enabled = True
        Else
            LabelUser.Text = "Operador"
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
              '  CheckBoxMosca.Checked = Not CheckBoxMosca.Checked
            Case e.KeyCode = Keys.Enter
                Select Case True
                    Case RB_Pick3.Checked
                        NumPad_Pick3.ButtonOk_Click(NumPad_Pick3.ButtonOk, Nothing)
                    Case RB_pick4.Checked
                        NumPad_Pick4.ButtonOk_Click(NumPad_Pick3.ButtonOk, Nothing)
                    Case RB_LotoPool.Checked
                        ButtonListPadLotoPool.ButtonOk_Click(NumPad_Pick3.ButtonOk, Nothing)
                    Case RB_Phillps.Checked
                        MultiNumberPadPhillipsburg.ButtonOk_Click(MultiNumberPadPhillipsburg.ButtonOk, Nothing)
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
            '   Canal_PVW = CasparDevice.Channels.First(Function(x) x.ID = PVW)
            Canal_Ver_1 = CasparDevice.Channels.First(Function(x) x.ID = VER1)
            Canal_Ver_2 = CasparDevice.Channels.First(Function(x) x.ID = VER2)
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

    'Private Sub Button1_Click(sender As Object, e As EventArgs)
    '    MonitorVer1.UpdateSourceWithComponents(Environment.MachineName, "PGM", True, 90)
    'End Sub

#End Region


#Region "Clips Verticales"
    Private Sub ComboBoxVertical1_SelectedIndexChanged(sender As ComboBox, e As EventArgs) Handles ComboBoxVertical1.SelectedIndexChanged, ComboBoxVertical2.SelectedIndexChanged
        Dim canal As ChannelManager
        Select Case sender.Tag
            Case "2"
                canal = Canal_Ver_2
                '  Case "3"
                '  canal = Canal_Ver_3
            Case Else
                canal = Canal_Ver_1
        End Select
        If sender.SelectedIndex > 0 Then
            canal.Stop(LayerTemplates)
            canal.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = sender.SelectedValue.FullName.ToString.Replace("\", "/"),
                         .VideoLayer = LayerVideoVertical,
                         .[Loop] = True
                         })
            canal.Play(LayerVideoVertical)
        End If

    End Sub

    Private Sub ButtonPlaySyncVertical_Click(sender As Object, e As EventArgs) Handles ButtonPlaySyncVertical.Click

        If ComboBoxVertical1.SelectedIndex > 0 Then
            Canal_Ver_1.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = ComboBoxVertical1.SelectedValue.FullName.ToString.Replace("\", "/"),
                         .VideoLayer = LayerVideoVertical,
                         .[Loop] = True
                         })
        End If
        If ComboBoxVertical2.SelectedIndex > 0 Then
            Canal_Ver_2.LoadBG(New CasparPlayingInfoItem With {
                         .Clipname = ComboBoxVertical2.SelectedValue.FullName.ToString.Replace("\", "/"),
                         .VideoLayer = LayerVideoVertical,
                         .[Loop] = True
                         })
        End If
        'If ComboBoxVertical3.SelectedIndex > 0 Then
        '    Canal_Ver_3.LoadBG(New CasparPlayingInfoItem With {
        '                 .Clipname = ComboBoxVertical3.SelectedValue.FullName.ToString.Replace("\", "/"),
        '                 .VideoLayer = LayerVideoVertical,
        '                 .[Loop] = True
        '                 })
        'End If
        Canal_Ver_1.Play(LayerVideoVertical)
        Canal_Ver_2.Play(LayerVideoVertical)
        ' Canal_Ver_3.Play(LayerVideoVertical)
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
        ComboBoxVertical1.DataSource = New BindingSource(media, "")
        ComboBoxVertical1.DisplayMember = "Name"
        ComboBoxVertical2.DataSource = New BindingSource(media, "")
        ComboBoxVertical2.DisplayMember = "Name"


    End Sub

    Private Sub SetMultiview()
        MonitorVer1.UpdateSourceWithComponents(Environment.MachineName, "VER1", True, 90)
        MonitorVer2.UpdateSourceWithComponents(Environment.MachineName, "VER2", True, 90)
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

            ''Para grafico de Sorteo vertical
            'Dim CGdata1 As New CasparCGDataCollection From {{$"f0", bolo.Resultado}}
            'Select Case bolo.Bolo
            '    Case 1
            '        Canal_Ver_1.CG.Update(LayerTemplates, 1, CGdata1)
            '        Canal_Ver_1.CG.Next(LayerTemplates, 1)
            '    Case 2
            '        Canal_Ver_2.CG.Update(LayerTemplates, 1, CGdata1)
            '        Canal_Ver_2.CG.Next(LayerTemplates, 1)
            '    Case 3
            '        Canal_Ver_3.CG.Update(LayerTemplates, 1, CGdata1)
            '        Canal_Ver_3.CG.Next(LayerTemplates, 1)
            'End Select

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
                Case Sorteos.Tipo.Phillipsburg
                    PanelSorteo = PictureBoxPhillip
                    Sorteo = Phillipsburg
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
                TabControl_Sorteo.SelectedTab = TabPageResultados

        End Select

    End Sub
#Region "Pick3"

    Private Sub ButtonEntradaPick3_Click(sender As Object, e As EventArgs) Handles ButtonEntradaPick3.Click
        Dim CGdata As New CasparCGDataCollection From {
            {$"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"}
        }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/Pick3_SXM", True, CGdata)

        Label_SXM3_1.Enabled = True
        NumPad_Pick3.ConfiguraNumeros(Sorteos.Tipo.Pick3_SXM, "1")
        NumPad_Pick3.Enabled = True
        PanelPick3.Enabled = True
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
        Dim CGdata As New CasparCGDataCollection From {
           {$"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"}
       }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/Pick4_SXM", True, CGdata)
        Label_SXM4_1.Enabled = True
        NumPad_Pick4.ConfiguraNumeros(Sorteos.Tipo.Pick4_SXM, "1")
        NumPad_Pick4.Enabled = True
        PanelPick4.Enabled = True
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
        Dim CGdata As New CasparCGDataCollection From {
        {$"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"}
    }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/LotoPool", True, CGdata)
        Label_Pool1.Enabled = True
        ButtonListPadLotoPool.ConfiguraNumeros(Sorteos.Tipo.LotoPool, "1")
        ButtonListPadLotoPool.Enabled = True
        PanelLotoPool.Enabled = True
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

#Region "Phillipsburg"
    Private Sub ButtonEntradaPhillip_Click(sender As Object, e As EventArgs) Handles ButtonEntradaPhillip.Click
        Dim CGdata As New CasparCGDataCollection From {
      {$"f0", $"{Date.Now.ToString("D", CultureInfo.CreateSpecificCulture("es-DO")).ToUpper}"}
  }
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/PHIL", True, CGdata)
        Label_PHI4_1.Enabled = True
        MultiNumberPadPhillipsburg.ConfiguraNumeros(Sorteos.Tipo.Phillipsburg, "1", Phillipsburg)
        MultiNumberPadPhillipsburg.Enabled = True
        PanelPhill.Enabled = True
    End Sub

    Private Sub ButtonResultadosPhillips_Click(sender As Object, e As EventArgs) Handles ButtonResultadosPhillips.Click
        Dim CGdata As New CasparCGDataCollection
        For Each bolo In Phillipsburg
            CGdata.Add($"f{bolo?.Bolo}", bolo?.Resultado)
        Next
        Canal_PGM.CG.Add(LayerTemplates, 1, "King/PHIL_final", True, CGdata)
    End Sub

    Private Sub BoloEventPhillipsburg(Bolo As Bola) Handles MultiNumberPadPhillipsburg.Bolo_OK
        If Bolo.Bolo < 12 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            Dim nextBolo = CInt(Bolo.Bolo) + 1
            For Each control In PictureBoxPhillip.Controls.OfType(Of Label)
                If control.Tag = nextBolo Then control.Enabled = True
            Next
            MultiNumberPadPhillipsburg.ConfiguraNumeros(Bolo.Sorteo, nextBolo.ToString, Phillipsburg)
        ElseIf Bolo.Bolo = 12 And Bolo.OK Then
            EntradaBolo(Bolo)
            SaveResultado(Bolo)
            MultiNumberPadPhillipsburg.Clear()
            MultiNumberPadPhillipsburg.Enabled = False
        End If

    End Sub

    Private Sub RadioButtonAM_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonAM.CheckedChanged, RadioButtonPM.CheckedChanged
        If RadioButtonPM.Checked Then
            PictureBoxLotoPool.Enabled = True
            ButtonEntradaLotoPool.Enabled = True
            ButtonResultadoLotoPool.Enabled = True
        Else
            PictureBoxLotoPool.Enabled = False
            ButtonEntradaLotoPool.Enabled = False
            ButtonResultadoLotoPool.Enabled = False
        End If
    End Sub

#End Region

#End Region
    Private Sub ButtonResultados_Click(sender As Object, e As EventArgs) Handles ButtonResultados.Click
        Dim CGdata As New CasparCGDataCollection From {
                {"f1", Label_SXM3_1.Text},
                {"f2", Label_SXM3_2.Text},
                {"f3", Label_SXM3_3.Text},
                {"f4", Label_SXM4_1.Text},
                {"f5", Label_SXM4_2.Text},
                {"f6", Label_SXM4_3.Text},
                {"f7", Label_SXM4_4.Text},
                {"f8", $"{Phillipsburg(0)?.Resultado} {Phillipsburg(1)?.Resultado} {Phillipsburg(2)?.Resultado} {Phillipsburg(3)?.Resultado}"},
                {"f9", $"{Phillipsburg(4)?.Resultado} {Phillipsburg(5)?.Resultado} {Phillipsburg(6)?.Resultado} {Phillipsburg(7)?.Resultado}"},
                {"f10", $"{Phillipsburg(8)?.Resultado} {Phillipsburg(9)?.Resultado} {Phillipsburg(10)?.Resultado} {Phillipsburg(11)?.Resultado}"},
                {"f11", Label_Pool1.Text},
                {"f12", Label_Pool2.Text},
                {"f13", Label_Pool3.Text},
                {"f14", Label_Pool4.Text}
            }

        If RadioButtonAM.Checked Then
            Canal_PGM.CG.Add(LayerTemplates, 1, "King/Resultados_AM", True, CGdata)
        Else
            Canal_PGM.CG.Add(LayerTemplates, 1, "King/Resultados_PM", True, CGdata)
        End If
    End Sub

    Private Sub ButtonGeneral_Click(sender As Object, e As EventArgs) Handles ButtonGeneral.Click
        Dim CGdata As New CasparCGDataCollection From {
               {"f0", TextBox1.Text},
               {"f1", TextBox2.Text}
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
    Public Property Phillipsburg As Bola()
    Public Property LotoPool As Bola()
    Public Property Presentador As Personas
    Public Property Jueces As List(Of Personas)
    Public Property Invidentes As List(Of Personas)
End Class

Public Class Sorteos
    Enum Tipo
        Pick3_SXM
        Pick4_SXM
        Phillipsburg
        LotoPool
    End Enum
End Class

#End Region
