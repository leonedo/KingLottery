<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ButtonListPad
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.ButtonNumero = New System.Windows.Forms.Label()
        Me.ButtonOk = New System.Windows.Forms.Button()
        Me.TextBoxNumero = New System.Windows.Forms.TextBox()
        Me.LabelDescription = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.SuspendLayout()
        '
        'ButtonNumero
        '
        Me.ButtonNumero.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonNumero.Font = New System.Drawing.Font("Kanit SemiBold", 20.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonNumero.ForeColor = System.Drawing.SystemColors.ActiveCaptionText
        Me.ButtonNumero.Image = Global.KingLottery.My.Resources.Resources.bolo81
        Me.ButtonNumero.ImageAlign = System.Drawing.ContentAlignment.TopCenter
        Me.ButtonNumero.Location = New System.Drawing.Point(311, 19)
        Me.ButtonNumero.Name = "ButtonNumero"
        Me.ButtonNumero.Size = New System.Drawing.Size(87, 124)
        Me.ButtonNumero.TabIndex = 142
        Me.ButtonNumero.Text = "  "
        Me.ButtonNumero.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'ButtonOk
        '
        Me.ButtonOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonOk.BackColor = System.Drawing.SystemColors.Control
        Me.ButtonOk.Enabled = False
        Me.ButtonOk.Font = New System.Drawing.Font("Microsoft Sans Serif", 16.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonOk.Location = New System.Drawing.Point(313, 211)
        Me.ButtonOk.Name = "ButtonOk"
        Me.ButtonOk.Size = New System.Drawing.Size(87, 43)
        Me.ButtonOk.TabIndex = 141
        Me.ButtonOk.Text = "OK"
        Me.ButtonOk.UseVisualStyleBackColor = False
        '
        'TextBoxNumero
        '
        Me.TextBoxNumero.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBoxNumero.Font = New System.Drawing.Font("Kanit", 26.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TextBoxNumero.Location = New System.Drawing.Point(323, 145)
        Me.TextBoxNumero.MaxLength = 2
        Me.TextBoxNumero.Name = "TextBoxNumero"
        Me.TextBoxNumero.Size = New System.Drawing.Size(65, 60)
        Me.TextBoxNumero.TabIndex = 140
        Me.TextBoxNumero.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'LabelDescription
        '
        Me.LabelDescription.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.LabelDescription.AutoSize = True
        Me.LabelDescription.BackColor = System.Drawing.Color.Transparent
        Me.LabelDescription.Font = New System.Drawing.Font("Kanit", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LabelDescription.ForeColor = System.Drawing.SystemColors.ControlText
        Me.LabelDescription.Location = New System.Drawing.Point(347, 32)
        Me.LabelDescription.Name = "LabelDescription"
        Me.LabelDescription.Size = New System.Drawing.Size(16, 24)
        Me.LabelDescription.TabIndex = 143
        Me.LabelDescription.Text = "1"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.FlowLayoutPanel1.AutoScroll = True
        Me.FlowLayoutPanel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.FlowLayoutPanel1.ForeColor = System.Drawing.SystemColors.ControlText
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Padding = New System.Windows.Forms.Padding(5, 5, 0, 0)
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(307, 257)
        Me.FlowLayoutPanel1.TabIndex = 144
        '
        'ButtonListPad
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(0, Byte), Integer), CType(CType(64, Byte), Integer))
        Me.Controls.Add(Me.FlowLayoutPanel1)
        Me.Controls.Add(Me.LabelDescription)
        Me.Controls.Add(Me.ButtonNumero)
        Me.Controls.Add(Me.ButtonOk)
        Me.Controls.Add(Me.TextBoxNumero)
        Me.Name = "ButtonListPad"
        Me.Size = New System.Drawing.Size(403, 257)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ButtonNumero As Label
    Friend WithEvents ButtonOk As Button
    Friend WithEvents TextBoxNumero As TextBox
    Friend WithEvents LabelDescription As Label
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
End Class
