<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class dlleditor
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
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

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意:  以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(dlleditor))
        Me.lvw1 = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.btnOpen = New System.Windows.Forms.Button()
        Me.txtString = New System.Windows.Forms.TextBox()
        Me.txtID = New System.Windows.Forms.TextBox()
        Me.btnModify = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.lblID = New System.Windows.Forms.Label()
        Me.lblString = New System.Windows.Forms.Label()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.lblFile = New System.Windows.Forms.Label()
        Me.btnHelp = New System.Windows.Forms.Button()
        Me.btnQuit = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lvw1
        '
        Me.lvw1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2})
        Me.lvw1.FullRowSelect = True
        Me.lvw1.GridLines = True
        Me.lvw1.HideSelection = False
        Me.lvw1.Location = New System.Drawing.Point(12, 12)
        Me.lvw1.MultiSelect = False
        Me.lvw1.Name = "lvw1"
        Me.lvw1.Size = New System.Drawing.Size(365, 236)
        Me.lvw1.TabIndex = 0
        Me.lvw1.UseCompatibleStateImageBehavior = False
        Me.lvw1.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "序号"
        Me.ColumnHeader1.Width = 61
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "字符串"
        Me.ColumnHeader2.Width = 298
        '
        'btnOpen
        '
        Me.btnOpen.Location = New System.Drawing.Point(383, 12)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(134, 23)
        Me.btnOpen.TabIndex = 1
        Me.btnOpen.Text = "打开语言 DLL 文件"
        Me.btnOpen.UseVisualStyleBackColor = True
        '
        'txtString
        '
        Me.txtString.Location = New System.Drawing.Point(384, 113)
        Me.txtString.Multiline = True
        Me.txtString.Name = "txtString"
        Me.txtString.Size = New System.Drawing.Size(232, 135)
        Me.txtString.TabIndex = 2
        '
        'txtID
        '
        Me.txtID.Location = New System.Drawing.Point(383, 62)
        Me.txtID.Name = "txtID"
        Me.txtID.Size = New System.Drawing.Size(100, 21)
        Me.txtID.TabIndex = 3
        '
        'btnModify
        '
        Me.btnModify.Location = New System.Drawing.Point(383, 255)
        Me.btnModify.Name = "btnModify"
        Me.btnModify.Size = New System.Drawing.Size(78, 37)
        Me.btnModify.TabIndex = 4
        Me.btnModify.Text = "修改"
        Me.btnModify.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(467, 255)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(76, 37)
        Me.btnAdd.TabIndex = 5
        Me.btnAdd.Text = "新增"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'lblID
        '
        Me.lblID.AutoSize = True
        Me.lblID.Location = New System.Drawing.Point(383, 47)
        Me.lblID.Name = "lblID"
        Me.lblID.Size = New System.Drawing.Size(29, 12)
        Me.lblID.TabIndex = 6
        Me.lblID.Text = "序号"
        '
        'lblString
        '
        Me.lblString.AutoSize = True
        Me.lblString.Location = New System.Drawing.Point(383, 95)
        Me.lblString.Name = "lblString"
        Me.lblString.Size = New System.Drawing.Size(41, 12)
        Me.lblString.TabIndex = 7
        Me.lblString.Text = "字符串"
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(549, 255)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(67, 37)
        Me.btnDelete.TabIndex = 8
        Me.btnDelete.Text = "删除"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(523, 12)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(93, 23)
        Me.btnSave.TabIndex = 9
        Me.btnSave.Text = "保存文件"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'lblFile
        '
        Me.lblFile.Location = New System.Drawing.Point(384, 299)
        Me.lblFile.Name = "lblFile"
        Me.lblFile.Size = New System.Drawing.Size(232, 49)
        Me.lblFile.TabIndex = 10
        '
        'btnHelp
        '
        Me.btnHelp.Location = New System.Drawing.Point(386, 347)
        Me.btnHelp.Name = "btnHelp"
        Me.btnHelp.Size = New System.Drawing.Size(75, 23)
        Me.btnHelp.TabIndex = 11
        Me.btnHelp.Text = "帮助"
        Me.btnHelp.UseVisualStyleBackColor = True
        '
        'btnQuit
        '
        Me.btnQuit.Location = New System.Drawing.Point(549, 347)
        Me.btnQuit.Name = "btnQuit"
        Me.btnQuit.Size = New System.Drawing.Size(67, 23)
        Me.btnQuit.TabIndex = 12
        Me.btnQuit.Text = "退出"
        Me.btnQuit.UseVisualStyleBackColor = True
        '
        'dlleditor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(644, 382)
        Me.Controls.Add(Me.btnQuit)
        Me.Controls.Add(Me.btnHelp)
        Me.Controls.Add(Me.lblFile)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.lblString)
        Me.Controls.Add(Me.lblID)
        Me.Controls.Add(Me.btnAdd)
        Me.Controls.Add(Me.btnModify)
        Me.Controls.Add(Me.txtID)
        Me.Controls.Add(Me.txtString)
        Me.Controls.Add(Me.btnOpen)
        Me.Controls.Add(Me.lvw1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "dlleditor"
        Me.ShowInTaskbar = False
        Me.Text = "帝国时代语言DLL编辑器"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lvw1 As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents btnOpen As System.Windows.Forms.Button
    Friend WithEvents txtString As System.Windows.Forms.TextBox
    Friend WithEvents txtID As System.Windows.Forms.TextBox
    Friend WithEvents btnModify As System.Windows.Forms.Button
    Friend WithEvents btnAdd As System.Windows.Forms.Button
    Friend WithEvents lblID As System.Windows.Forms.Label
    Friend WithEvents lblString As System.Windows.Forms.Label
    Friend WithEvents btnDelete As System.Windows.Forms.Button
    Friend WithEvents btnSave As System.Windows.Forms.Button
    Friend WithEvents lblFile As System.Windows.Forms.Label
    Friend WithEvents btnHelp As System.Windows.Forms.Button
    Friend WithEvents btnQuit As System.Windows.Forms.Button

End Class
