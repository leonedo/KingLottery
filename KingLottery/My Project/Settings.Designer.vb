﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On


Namespace My
    
    <Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.7.0.0"),  _
     Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
    Partial Friend NotInheritable Class MySettings
        Inherits Global.System.Configuration.ApplicationSettingsBase
        
        Private Shared defaultInstance As MySettings = CType(Global.System.Configuration.ApplicationSettingsBase.Synchronized(New MySettings()),MySettings)
        
#Region "My.Settings Auto-Save Functionality"
#If _MyType = "WindowsForms" Then
    Private Shared addedHandler As Boolean

    Private Shared addedHandlerLockObject As New Object

    <Global.System.Diagnostics.DebuggerNonUserCodeAttribute(), Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)> _
    Private Shared Sub AutoSaveSettings(sender As Global.System.Object, e As Global.System.EventArgs)
        If My.Application.SaveMySettingsOnExit Then
            My.Settings.Save()
        End If
    End Sub
#End If
#End Region
        
        Public Shared ReadOnly Property [Default]() As MySettings
            Get
                
#If _MyType = "WindowsForms" Then
               If Not addedHandler Then
                    SyncLock addedHandlerLockObject
                        If Not addedHandler Then
                            AddHandler My.Application.Shutdown, AddressOf AutoSaveSettings
                            addedHandler = True
                        End If
                    End SyncLock
                End If
#End If
                Return defaultInstance
            End Get
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1")>  _
        Public Property NumeroSorteo() As Integer
            Get
                Return CType(Me("NumeroSorteo"),Integer)
            End Get
            Set
                Me("NumeroSorteo") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("server\casparcg.exe")>  _
        Public Property ServerPath() As String
            Get
                Return CType(Me("ServerPath"),String)
            End Get
            Set
                Me("ServerPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property UseScanner() As Boolean
            Get
                Return CType(Me("UseScanner"),Boolean)
            End Get
            Set
                Me("UseScanner") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("server\scanner.exe")>  _
        Public Property ScannerPath() As String
            Get
                Return CType(Me("ScannerPath"),String)
            End Get
            Set
                Me("ScannerPath") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("BPT9mH4EF+mdySaXHwMYQ1EApqe84pwxBhJcde+a7DDNI+jHa+z9WmsKaFo8bagAlrWMfN69RZwUXGV32"& _ 
            "IEwRg==")>  _
        Public Property OpPsHs() As String
            Get
                Return CType(Me("OpPsHs"),String)
            End Get
            Set
                Me("OpPsHs") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("6n!Jo]9oXQN0n8wVH)M9bA8*SydJnhsq5JJ1iitEUT^%2fvM+6L@l=b5x+y740BeC8=dy]b@kIf6N+OaD"& _ 
            "GZyMyE@_qEo1%l^Sv#+")>  _
        Public Property OpSa() As String
            Get
                Return CType(Me("OpSa"),String)
            End Get
            Set
                Me("OpSa") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("SWZYGm0pF5nC8b7hur6DLpiJfS5aB7WpUxUmZFSvfFdbpbi9iYvpxzepeFIJJDP1/dqECZfJHSHBdForb"& _ 
            "o+u+w==")>  _
        Public Property AdPsHs() As String
            Get
                Return CType(Me("AdPsHs"),String)
            End Get
            Set
                Me("AdPsHs") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("YGXGsEAPL[KK69PDbofk})<vStqLb}Bj4)6)QV1N=hKf2xMuy5D#Ht#0H]w*0*bN=$s^DXv34Ip195+&r"& _ 
            "%UU=bbKHF#U+#VEF[k*")>  _
        Public Property AdSa() As String
            Get
                Return CType(Me("AdSa"),String)
            End Get
            Set
                Me("AdSa") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Opening")>  _
        Public Property Bumper1() As String
            Get
                Return CType(Me("Bumper1"),String)
            End Get
            Set
                Me("Bumper1") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Intermedio largo")>  _
        Public Property Bumper2() As String
            Get
                Return CType(Me("Bumper2"),String)
            End Get
            Set
                Me("Bumper2") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Loop")>  _
        Public Property Bumper3() As String
            Get
                Return CType(Me("Bumper3"),String)
            End Get
            Set
                Me("Bumper3") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Bumper corto")>  _
        Public Property Bumper4() As String
            Get
                Return CType(Me("Bumper4"),String)
            End Get
            Set
                Me("Bumper4") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Bumper corto")>  _
        Public Property Bumper5() As String
            Get
                Return CType(Me("Bumper5"),String)
            End Get
            Set
                Me("Bumper5") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Bumper corto")>  _
        Public Property Bumper6() As String
            Get
                Return CType(Me("Bumper6"),String)
            End Get
            Set
                Me("Bumper6") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Separador_1")>  _
        Public Property Separador1() As String
            Get
                Return CType(Me("Separador1"),String)
            End Get
            Set
                Me("Separador1") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Separador_2")>  _
        Public Property Separador2() As String
            Get
                Return CType(Me("Separador2"),String)
            End Get
            Set
                Me("Separador2") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Separador_3")>  _
        Public Property Separador3() As String
            Get
                Return CType(Me("Separador3"),String)
            End Get
            Set
                Me("Separador3") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Separador_4")>  _
        Public Property Separador4() As String
            Get
                Return CType(Me("Separador4"),String)
            End Get
            Set
                Me("Separador4") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Separador_5")>  _
        Public Property Separador5() As String
            Get
                Return CType(Me("Separador5"),String)
            End Get
            Set
                Me("Separador5") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Phillipsburg1")>  _
        Public Property Separador6() As String
            Get
                Return CType(Me("Separador6"),String)
            End Get
            Set
                Me("Separador6") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("rtmp://a.rtmp.youtube.com/live2/Steam_Key_here")>  _
        Public Property RTMP_URL() As String
            Get
                Return CType(Me("RTMP_URL"),String)
            End Get
            Set
                Me("RTMP_URL") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("1280x720")>  _
        Public Property StreamPictureSize() As String
            Get
                Return CType(Me("StreamPictureSize"),String)
            End Get
            Set
                Me("StreamPictureSize") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("2750")>  _
        Public Property StreamVideoBitrate() As Decimal
            Get
                Return CType(Me("StreamVideoBitrate"),Decimal)
            End Get
            Set
                Me("StreamVideoBitrate") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("True")>  _
        Public Property LotoPoolAM() As Boolean
            Get
                Return CType(Me("LotoPoolAM"),Boolean)
            End Get
            Set
                Me("LotoPoolAM") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Phillipsburg2")>  _
        Public Property Separador7() As String
            Get
                Return CType(Me("Separador7"),String)
            End Get
            Set
                Me("Separador7") = value
            End Set
        End Property
        
        <Global.System.Configuration.UserScopedSettingAttribute(),  _
         Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
         Global.System.Configuration.DefaultSettingValueAttribute("Phillipsburg3")>  _
        Public Property Separador8() As String
            Get
                Return CType(Me("Separador8"),String)
            End Get
            Set
                Me("Separador8") = value
            End Set
        End Property
    End Class
End Namespace

Namespace My
    
    <Global.Microsoft.VisualBasic.HideModuleNameAttribute(),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Friend Module MySettingsProperty
        
        <Global.System.ComponentModel.Design.HelpKeywordAttribute("My.Settings")>  _
        Friend ReadOnly Property Settings() As Global.KingLottery.My.MySettings
            Get
                Return Global.KingLottery.My.MySettings.Default
            End Get
        End Property
    End Module
End Namespace
