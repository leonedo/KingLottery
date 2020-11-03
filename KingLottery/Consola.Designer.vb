<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Consola
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ConsoleBox = New System.Windows.Forms.RichTextBox()
        Me.TextBoxSendCommand = New System.Windows.Forms.TextBox()
        Me.ButtonSendCommnad = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'ConsoleBox
        '
        Me.ConsoleBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ConsoleBox.BackColor = System.Drawing.SystemColors.Desktop
        Me.ConsoleBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.ConsoleBox.Font = New System.Drawing.Font("Lucida Console", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ConsoleBox.ForeColor = System.Drawing.SystemColors.ControlLightLight
        Me.ConsoleBox.Location = New System.Drawing.Point(12, 12)
        Me.ConsoleBox.Name = "ConsoleBox"
        Me.ConsoleBox.ReadOnly = True
        Me.ConsoleBox.Size = New System.Drawing.Size(436, 197)
        Me.ConsoleBox.TabIndex = 8
        Me.ConsoleBox.Text = ""
        '
        'TextBoxSendCommand
        '
        Me.TextBoxSendCommand.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxSendCommand.Font = New System.Drawing.Font("Microsoft Sans Serif", 10.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxSendCommand.Location = New System.Drawing.Point(12, 216)
        Me.TextBoxSendCommand.Name = "TextBoxSendCommand"
        Me.TextBoxSendCommand.Size = New System.Drawing.Size(352, 23)
        Me.TextBoxSendCommand.TabIndex = 6
        '
        'ButtonSendCommnad
        '
        Me.ButtonSendCommnad.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonSendCommnad.Location = New System.Drawing.Point(373, 215)
        Me.ButtonSendCommnad.Name = "ButtonSendCommnad"
        Me.ButtonSendCommnad.Size = New System.Drawing.Size(75, 24)
        Me.ButtonSendCommnad.TabIndex = 7
        Me.ButtonSendCommnad.Text = "Enviar"
        Me.ButtonSendCommnad.UseVisualStyleBackColor = True
        '
        'Consola
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(460, 245)
        Me.Controls.Add(Me.ConsoleBox)
        Me.Controls.Add(Me.TextBoxSendCommand)
        Me.Controls.Add(Me.ButtonSendCommnad)
        Me.Name = "Consola"
        Me.Text = "Consola"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ConsoleBox As RichTextBox
    Friend WithEvents TextBoxSendCommand As TextBox
    Friend WithEvents ButtonSendCommnad As Button
End Class
