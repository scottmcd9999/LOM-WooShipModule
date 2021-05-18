<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucHelp
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ucHelp))
		Me.tsPrint = New System.Windows.Forms.ToolStrip()
		Me.tsbPrint = New System.Windows.Forms.ToolStripButton()
		Me.tsbPageSetup = New System.Windows.Forms.ToolStripButton()
		Me.tsbPreview = New System.Windows.Forms.ToolStripButton()
		Me.ptPageSetupDialog = New System.Windows.Forms.PageSetupDialog()
		Me.ptDocument = New System.Drawing.Printing.PrintDocument()
		Me.ptPrintDialog = New System.Windows.Forms.PrintDialog()
		Me.ptPrintPreviewDialog = New System.Windows.Forms.PrintPreviewDialog()
		Me.rtHelp = New WooShipModule.RichTextBoxPrintCtrl.RichTextBoxPrintCtrl()
		Me.tsPrint.SuspendLayout()
		Me.SuspendLayout()
		'
		'tsPrint
		'
		Me.tsPrint.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbPrint, Me.tsbPageSetup, Me.tsbPreview})
		Me.tsPrint.Location = New System.Drawing.Point(0, 0)
		Me.tsPrint.Name = "tsPrint"
		Me.tsPrint.Size = New System.Drawing.Size(1286, 25)
		Me.tsPrint.TabIndex = 0
		Me.tsPrint.Text = "ToolStrip1"
		'
		'tsbPrint
		'
		Me.tsbPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbPrint.Image = Global.WooShipModule.My.Resources.Resources.printer
		Me.tsbPrint.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbPrint.Name = "tsbPrint"
		Me.tsbPrint.Size = New System.Drawing.Size(23, 22)
		Me.tsbPrint.Text = "ToolStripButton1"
		'
		'tsbPageSetup
		'
		Me.tsbPageSetup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbPageSetup.Image = Global.WooShipModule.My.Resources.Resources.print_setup
		Me.tsbPageSetup.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbPageSetup.Name = "tsbPageSetup"
		Me.tsbPageSetup.Size = New System.Drawing.Size(23, 22)
		Me.tsbPageSetup.Text = "ToolStripButton2"
		'
		'tsbPreview
		'
		Me.tsbPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.tsbPreview.Image = Global.WooShipModule.My.Resources.Resources.print_preview
		Me.tsbPreview.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.tsbPreview.Name = "tsbPreview"
		Me.tsbPreview.Size = New System.Drawing.Size(23, 22)
		Me.tsbPreview.Text = "ToolStripButton3"
		'
		'ptPageSetupDialog
		'
		Me.ptPageSetupDialog.Document = Me.ptDocument
		'
		'ptDocument
		'
		'
		'ptPrintDialog
		'
		Me.ptPrintDialog.Document = Me.ptDocument
		Me.ptPrintDialog.UseEXDialog = True
		'
		'ptPrintPreviewDialog
		'
		Me.ptPrintPreviewDialog.AutoScrollMargin = New System.Drawing.Size(0, 0)
		Me.ptPrintPreviewDialog.AutoScrollMinSize = New System.Drawing.Size(0, 0)
		Me.ptPrintPreviewDialog.ClientSize = New System.Drawing.Size(359, 300)
		Me.ptPrintPreviewDialog.Document = Me.ptDocument
		Me.ptPrintPreviewDialog.Enabled = True
		Me.ptPrintPreviewDialog.Icon = CType(resources.GetObject("ptPrintPreviewDialog.Icon"), System.Drawing.Icon)
		Me.ptPrintPreviewDialog.Name = "ptPrintPreviewDialog"
		Me.ptPrintPreviewDialog.Visible = False
		'
		'rtHelp
		'
		Me.rtHelp.BorderStyle = System.Windows.Forms.BorderStyle.None
		Me.rtHelp.Dock = System.Windows.Forms.DockStyle.Fill
		Me.rtHelp.Location = New System.Drawing.Point(0, 25)
		Me.rtHelp.Name = "rtHelp"
		Me.rtHelp.Size = New System.Drawing.Size(1286, 689)
		Me.rtHelp.TabIndex = 1
		Me.rtHelp.Text = ""
		'
		'ucHelp
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.rtHelp)
		Me.Controls.Add(Me.tsPrint)
		Me.Name = "ucHelp"
		Me.Size = New System.Drawing.Size(1286, 714)
		Me.tsPrint.ResumeLayout(False)
		Me.tsPrint.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Friend WithEvents tsPrint As ToolStrip
	Friend WithEvents tsbPrint As ToolStripButton
	Friend WithEvents tsbPageSetup As ToolStripButton
	Friend WithEvents tsbPreview As ToolStripButton
	Friend WithEvents ptPageSetupDialog As PageSetupDialog
	Friend WithEvents ptPrintDialog As PrintDialog
	Friend WithEvents ptDocument As Printing.PrintDocument
	Friend WithEvents ptPrintPreviewDialog As PrintPreviewDialog
	Friend WithEvents rtHelp As RichTextBoxPrintCtrl.RichTextBoxPrintCtrl
End Class
