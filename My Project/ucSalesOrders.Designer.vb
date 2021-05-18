<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ucSalesOrders
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
		Dim GridLevelNode1 As DevExpress.XtraGrid.GridLevelNode = New DevExpress.XtraGrid.GridLevelNode()
		Dim GridLevelNode2 As DevExpress.XtraGrid.GridLevelNode = New DevExpress.XtraGrid.GridLevelNode()
		Dim GridLevelNode3 As DevExpress.XtraGrid.GridLevelNode = New DevExpress.XtraGrid.GridLevelNode()
		Me.scSalesOrders = New DevExpress.XtraEditors.SplitContainerControl()
		Me.GroupControl3 = New DevExpress.XtraEditors.GroupControl()
		Me.grSalesOrders = New DevExpress.XtraGrid.GridControl()
		Me.gvSalesOrders = New DevExpress.XtraGrid.Views.Grid.GridView()
		Me.SplitContainerControl1 = New DevExpress.XtraEditors.SplitContainerControl()
		Me.gpSOLines = New DevExpress.XtraEditors.GroupControl()
		Me.grSODetails = New DevExpress.XtraGrid.GridControl()
		Me.gvSODetails = New DevExpress.XtraGrid.Views.Grid.GridView()
		Me.gpSerialNums = New DevExpress.XtraEditors.GroupControl()
		Me.grSerialNumbers = New DevExpress.XtraGrid.GridControl()
		Me.gvSerialNumbers = New DevExpress.XtraGrid.Views.Grid.GridView()
		CType(Me.scSalesOrders, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.scSalesOrders.SuspendLayout()
		CType(Me.GroupControl3, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.GroupControl3.SuspendLayout()
		CType(Me.grSalesOrders, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.gvSalesOrders, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SplitContainerControl1.SuspendLayout()
		CType(Me.gpSOLines, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.gpSOLines.SuspendLayout()
		CType(Me.grSODetails, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.gvSODetails, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.gpSerialNums, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.gpSerialNums.SuspendLayout()
		CType(Me.grSerialNumbers, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.gvSerialNumbers, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'scSalesOrders
		'
		Me.scSalesOrders.Dock = System.Windows.Forms.DockStyle.Fill
		Me.scSalesOrders.Location = New System.Drawing.Point(0, 0)
		Me.scSalesOrders.Name = "scSalesOrders"
		Me.scSalesOrders.Panel1.Controls.Add(Me.GroupControl3)
		Me.scSalesOrders.Panel1.Text = "Panel1"
		Me.scSalesOrders.Panel2.Controls.Add(Me.SplitContainerControl1)
		Me.scSalesOrders.Panel2.Text = "Panel2"
		Me.scSalesOrders.Size = New System.Drawing.Size(1238, 700)
		Me.scSalesOrders.SplitterPosition = 618
		Me.scSalesOrders.TabIndex = 0
		'
		'GroupControl3
		'
		Me.GroupControl3.Controls.Add(Me.grSalesOrders)
		Me.GroupControl3.Dock = System.Windows.Forms.DockStyle.Fill
		Me.GroupControl3.GroupStyle = DevExpress.Utils.GroupStyle.Card
		Me.GroupControl3.Location = New System.Drawing.Point(0, 0)
		Me.GroupControl3.Name = "GroupControl3"
		Me.GroupControl3.Size = New System.Drawing.Size(618, 700)
		Me.GroupControl3.TabIndex = 0
		Me.GroupControl3.Text = "Sales Orders - Ready to Ship"
		'
		'grSalesOrders
		'
		Me.grSalesOrders.Dock = System.Windows.Forms.DockStyle.Fill
		GridLevelNode1.RelationName = "Level1"
		Me.grSalesOrders.LevelTree.Nodes.AddRange(New DevExpress.XtraGrid.GridLevelNode() {GridLevelNode1})
		Me.grSalesOrders.Location = New System.Drawing.Point(2, 21)
		Me.grSalesOrders.MainView = Me.gvSalesOrders
		Me.grSalesOrders.Name = "grSalesOrders"
		Me.grSalesOrders.Size = New System.Drawing.Size(614, 677)
		Me.grSalesOrders.TabIndex = 0
		Me.grSalesOrders.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gvSalesOrders})
		'
		'gvSalesOrders
		'
		Me.gvSalesOrders.GridControl = Me.grSalesOrders
		Me.gvSalesOrders.Name = "gvSalesOrders"
		Me.gvSalesOrders.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.[False]
		Me.gvSalesOrders.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.[False]
		Me.gvSalesOrders.OptionsView.ShowGroupPanel = False
		'
		'SplitContainerControl1
		'
		Me.SplitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill
		Me.SplitContainerControl1.Horizontal = False
		Me.SplitContainerControl1.Location = New System.Drawing.Point(0, 0)
		Me.SplitContainerControl1.Name = "SplitContainerControl1"
		Me.SplitContainerControl1.Panel1.Controls.Add(Me.gpSOLines)
		Me.SplitContainerControl1.Panel1.Text = "Panel1"
		Me.SplitContainerControl1.Panel2.Controls.Add(Me.gpSerialNums)
		Me.SplitContainerControl1.Panel2.Text = "Panel2"
		Me.SplitContainerControl1.Size = New System.Drawing.Size(608, 700)
		Me.SplitContainerControl1.SplitterPosition = 322
		Me.SplitContainerControl1.TabIndex = 0
		'
		'gpSOLines
		'
		Me.gpSOLines.Controls.Add(Me.grSODetails)
		Me.gpSOLines.Dock = System.Windows.Forms.DockStyle.Fill
		Me.gpSOLines.GroupStyle = DevExpress.Utils.GroupStyle.Card
		Me.gpSOLines.Location = New System.Drawing.Point(0, 0)
		Me.gpSOLines.Name = "gpSOLines"
		Me.gpSOLines.Size = New System.Drawing.Size(608, 322)
		Me.gpSOLines.TabIndex = 0
		Me.gpSOLines.Text = "Sales Order Lines"
		'
		'grSODetails
		'
		Me.grSODetails.Dock = System.Windows.Forms.DockStyle.Fill
		GridLevelNode2.RelationName = "Level1"
		Me.grSODetails.LevelTree.Nodes.AddRange(New DevExpress.XtraGrid.GridLevelNode() {GridLevelNode2})
		Me.grSODetails.Location = New System.Drawing.Point(2, 21)
		Me.grSODetails.MainView = Me.gvSODetails
		Me.grSODetails.Name = "grSODetails"
		Me.grSODetails.Size = New System.Drawing.Size(604, 299)
		Me.grSODetails.TabIndex = 0
		Me.grSODetails.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gvSODetails})
		'
		'gvSODetails
		'
		Me.gvSODetails.GridControl = Me.grSODetails
		Me.gvSODetails.Name = "gvSODetails"
		Me.gvSODetails.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.[False]
		Me.gvSODetails.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.[False]
		Me.gvSODetails.OptionsBehavior.Editable = False
		Me.gvSODetails.OptionsBehavior.ReadOnly = True
		Me.gvSODetails.OptionsView.ShowGroupPanel = False
		'
		'gpSerialNums
		'
		Me.gpSerialNums.Controls.Add(Me.grSerialNumbers)
		Me.gpSerialNums.Dock = System.Windows.Forms.DockStyle.Fill
		Me.gpSerialNums.GroupStyle = DevExpress.Utils.GroupStyle.Card
		Me.gpSerialNums.Location = New System.Drawing.Point(0, 0)
		Me.gpSerialNums.Name = "gpSerialNums"
		Me.gpSerialNums.Size = New System.Drawing.Size(608, 366)
		Me.gpSerialNums.TabIndex = 0
		Me.gpSerialNums.Text = "Serial Numbers and Pick Locations"
		'
		'grSerialNumbers
		'
		Me.grSerialNumbers.Dock = System.Windows.Forms.DockStyle.Fill
		GridLevelNode3.RelationName = "Level1"
		Me.grSerialNumbers.LevelTree.Nodes.AddRange(New DevExpress.XtraGrid.GridLevelNode() {GridLevelNode3})
		Me.grSerialNumbers.Location = New System.Drawing.Point(2, 21)
		Me.grSerialNumbers.MainView = Me.gvSerialNumbers
		Me.grSerialNumbers.Name = "grSerialNumbers"
		Me.grSerialNumbers.Size = New System.Drawing.Size(604, 343)
		Me.grSerialNumbers.TabIndex = 0
		Me.grSerialNumbers.ViewCollection.AddRange(New DevExpress.XtraGrid.Views.Base.BaseView() {Me.gvSerialNumbers})
		'
		'gvSerialNumbers
		'
		Me.gvSerialNumbers.GridControl = Me.grSerialNumbers
		Me.gvSerialNumbers.Name = "gvSerialNumbers"
		Me.gvSerialNumbers.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.[True]
		Me.gvSerialNumbers.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.[True]
		Me.gvSerialNumbers.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top
		Me.gvSerialNumbers.OptionsView.ShowGroupPanel = False
		'
		'ucSalesOrders
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.Controls.Add(Me.scSalesOrders)
		Me.Name = "ucSalesOrders"
		Me.Size = New System.Drawing.Size(1238, 700)
		CType(Me.scSalesOrders, System.ComponentModel.ISupportInitialize).EndInit()
		Me.scSalesOrders.ResumeLayout(False)
		CType(Me.GroupControl3, System.ComponentModel.ISupportInitialize).EndInit()
		Me.GroupControl3.ResumeLayout(False)
		CType(Me.grSalesOrders, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.gvSalesOrders, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.SplitContainerControl1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.SplitContainerControl1.ResumeLayout(False)
		CType(Me.gpSOLines, System.ComponentModel.ISupportInitialize).EndInit()
		Me.gpSOLines.ResumeLayout(False)
		CType(Me.grSODetails, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.gvSODetails, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.gpSerialNums, System.ComponentModel.ISupportInitialize).EndInit()
		Me.gpSerialNums.ResumeLayout(False)
		CType(Me.grSerialNumbers, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.gvSerialNumbers, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents scSalesOrders As DevExpress.XtraEditors.SplitContainerControl
	Friend WithEvents grSalesOrders As DevExpress.XtraGrid.GridControl
	Friend WithEvents gvSalesOrders As DevExpress.XtraGrid.Views.Grid.GridView
	Friend WithEvents SplitContainerControl1 As DevExpress.XtraEditors.SplitContainerControl
	Friend WithEvents grSODetails As DevExpress.XtraGrid.GridControl
	Friend WithEvents gvSODetails As DevExpress.XtraGrid.Views.Grid.GridView
	Friend WithEvents GroupControl3 As DevExpress.XtraEditors.GroupControl
	Friend WithEvents gpSOLines As DevExpress.XtraEditors.GroupControl
	Friend WithEvents gpSerialNums As DevExpress.XtraEditors.GroupControl
	Friend WithEvents grSerialNumbers As DevExpress.XtraGrid.GridControl
	Friend WithEvents gvSerialNumbers As DevExpress.XtraGrid.Views.Grid.GridView
End Class
