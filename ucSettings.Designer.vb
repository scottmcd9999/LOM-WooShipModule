<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucSettings
	Inherits DevExpress.XtraEditors.XtraUserControl

	'UserControl overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing AndAlso components IsNot Nothing Then
			components.Dispose()
		End If
		MyBase.Dispose(disposing)
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.GroupControl1 = New DevExpress.XtraEditors.GroupControl()
		Me.ckPostInvoices = New DevExpress.XtraEditors.CheckEdit()
		CType(Me.GroupControl1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.GroupControl1.SuspendLayout()
		CType(Me.ckPostInvoices.Properties, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'GroupControl1
		'
		Me.GroupControl1.Controls.Add(Me.ckPostInvoices)
		Me.GroupControl1.Location = New System.Drawing.Point(5, 5)
		Me.GroupControl1.Name = "GroupControl1"
		Me.GroupControl1.Size = New System.Drawing.Size(452, 56)
		Me.GroupControl1.TabIndex = 0
		Me.GroupControl1.Text = "Program Settings"
		'
		'ckPostInvoices
		'
		Me.ckPostInvoices.Location = New System.Drawing.Point(29, 24)
		Me.ckPostInvoices.Name = "ckPostInvoices"
		Me.ckPostInvoices.Properties.Caption = "Post Invoices"
		Me.ckPostInvoices.Size = New System.Drawing.Size(130, 19)
		Me.ckPostInvoices.TabIndex = 0
		'
		'ucSettings
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.GroupControl1)
		Me.Name = "ucSettings"
		Me.Size = New System.Drawing.Size(930, 612)
		CType(Me.GroupControl1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.GroupControl1.ResumeLayout(False)
		CType(Me.ckPostInvoices.Properties, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents GroupControl1 As DevExpress.XtraEditors.GroupControl
	Friend WithEvents ckPostInvoices As DevExpress.XtraEditors.CheckEdit
End Class
