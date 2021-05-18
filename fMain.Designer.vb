<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class fMain
	Inherits DevExpress.XtraBars.Ribbon.RibbonForm

	'Form overrides dispose to clean up the component list.
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
		Me.components = New System.ComponentModel.Container()
		Dim DockingContainer2 As DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer = New DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer()
		Me.DocumentGroup1 = New DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup(Me.components)
		Me.ucSettingsDocument = New DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(Me.components)
		Me.ucSalesOrdersDocument = New DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(Me.components)
		Me.ucHelpDocument = New DevExpress.XtraBars.Docking2010.Views.Tabbed.Document(Me.components)
		Me.rcMain = New DevExpress.XtraBars.Ribbon.RibbonControl()
		Me.bbExit = New DevExpress.XtraBars.BarButtonItem()
		Me.bbFiles_Import = New DevExpress.XtraBars.BarButtonItem()
		Me.bbFiles_Refresh = New DevExpress.XtraBars.BarButtonItem()
		Me.bbHelp = New DevExpress.XtraBars.BarButtonItem()
		Me.bbSettings = New DevExpress.XtraBars.BarButtonItem()
		Me.rpProgram = New DevExpress.XtraBars.Ribbon.RibbonPage()
		Me.rpgProgram = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
		Me.rpActions = New DevExpress.XtraBars.Ribbon.RibbonPage()
		Me.rpgActions = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
		Me.rpHelp = New DevExpress.XtraBars.Ribbon.RibbonPage()
		Me.rpgHelp = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
		Me.rpSettings = New DevExpress.XtraBars.Ribbon.RibbonPage()
		Me.rpgSettings = New DevExpress.XtraBars.Ribbon.RibbonPageGroup()
		Me.RibbonStatusBar = New DevExpress.XtraBars.Ribbon.RibbonStatusBar()
		Me.dmDocMgr = New DevExpress.XtraBars.Docking2010.DocumentManager(Me.components)
		Me.tvMain = New DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView(Me.components)
		CType(Me.DocumentGroup1, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.ucSettingsDocument, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.ucSalesOrdersDocument, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.ucHelpDocument, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.rcMain, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.dmDocMgr, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.tvMain, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'DocumentGroup1
		'
		Me.DocumentGroup1.Items.AddRange(New DevExpress.XtraBars.Docking2010.Views.Tabbed.Document() {Me.ucSettingsDocument, Me.ucSalesOrdersDocument, Me.ucHelpDocument})
		'
		'ucSettingsDocument
		'
		Me.ucSettingsDocument.Caption = "ucSettings"
		Me.ucSettingsDocument.ControlName = "ucSettings"
		Me.ucSettingsDocument.ControlTypeName = "WooShipModule.ucSettings"
		'
		'ucSalesOrdersDocument
		'
		Me.ucSalesOrdersDocument.Caption = "ucSalesOrders"
		Me.ucSalesOrdersDocument.ControlName = "ucSalesOrders"
		Me.ucSalesOrdersDocument.ControlTypeName = "WooShipModule.ucSalesOrders"
		Me.ucSalesOrdersDocument.FloatLocation = New System.Drawing.Point(596, 310)
		Me.ucSalesOrdersDocument.FloatSize = New System.Drawing.Size(1154, 501)
		'
		'ucHelpDocument
		'
		Me.ucHelpDocument.Caption = "ucHelp"
		Me.ucHelpDocument.ControlName = "ucHelp"
		Me.ucHelpDocument.ControlTypeName = "WooShipModule.ucHelp"
		'
		'rcMain
		'
		Me.rcMain.ExpandCollapseItem.Id = 0
		Me.rcMain.Items.AddRange(New DevExpress.XtraBars.BarItem() {Me.rcMain.ExpandCollapseItem, Me.rcMain.SearchEditItem, Me.bbExit, Me.bbFiles_Import, Me.bbFiles_Refresh, Me.bbHelp, Me.bbSettings})
		Me.rcMain.Location = New System.Drawing.Point(0, 0)
		Me.rcMain.MaxItemId = 6
		Me.rcMain.Name = "rcMain"
		Me.rcMain.Pages.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPage() {Me.rpProgram, Me.rpActions, Me.rpHelp, Me.rpSettings})
		Me.rcMain.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Show
		Me.rcMain.Size = New System.Drawing.Size(1160, 151)
		Me.rcMain.StatusBar = Me.RibbonStatusBar
		'
		'bbExit
		'
		Me.bbExit.Caption = "Exit"
		Me.bbExit.Id = 1
		Me.bbExit.ImageOptions.Image = Global.WooShipModule.My.Resources.Resources._exit
		Me.bbExit.ImageOptions.LargeImage = Global.WooShipModule.My.Resources.Resources._exit
		Me.bbExit.Name = "bbExit"
		'
		'bbFiles_Import
		'
		Me.bbFiles_Import.Caption = "Import Order Files"
		Me.bbFiles_Import.Id = 2
		Me.bbFiles_Import.ImageOptions.Image = Global.WooShipModule.My.Resources.Resources.order_download
		Me.bbFiles_Import.ImageOptions.LargeImage = Global.WooShipModule.My.Resources.Resources.order_download
		Me.bbFiles_Import.Name = "bbFiles_Import"
		'
		'bbFiles_Refresh
		'
		Me.bbFiles_Refresh.Caption = "Refresh Sales Order List"
		Me.bbFiles_Refresh.Id = 3
		Me.bbFiles_Refresh.ImageOptions.Image = Global.WooShipModule.My.Resources.Resources.order_refresh
		Me.bbFiles_Refresh.ImageOptions.LargeImage = Global.WooShipModule.My.Resources.Resources.order_refresh
		Me.bbFiles_Refresh.Name = "bbFiles_Refresh"
		'
		'bbHelp
		'
		Me.bbHelp.Caption = "Help"
		Me.bbHelp.Id = 4
		Me.bbHelp.ImageOptions.Image = Global.WooShipModule.My.Resources.Resources.button_help
		Me.bbHelp.ImageOptions.LargeImage = Global.WooShipModule.My.Resources.Resources.button_help
		Me.bbHelp.Name = "bbHelp"
		'
		'bbSettings
		'
		Me.bbSettings.Caption = "Settings"
		Me.bbSettings.Id = 5
		Me.bbSettings.ImageOptions.Image = Global.WooShipModule.My.Resources.Resources.settings_options
		Me.bbSettings.ImageOptions.LargeImage = Global.WooShipModule.My.Resources.Resources.settings_options
		Me.bbSettings.Name = "bbSettings"
		'
		'rpProgram
		'
		Me.rpProgram.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.rpgProgram})
		Me.rpProgram.Name = "rpProgram"
		Me.rpProgram.Text = "Program"
		'
		'rpgProgram
		'
		Me.rpgProgram.ItemLinks.Add(Me.bbExit)
		Me.rpgProgram.Name = "rpgProgram"
		Me.rpgProgram.Text = "Program"
		'
		'rpActions
		'
		Me.rpActions.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.rpgActions})
		Me.rpActions.Name = "rpActions"
		Me.rpActions.Text = "Sales Orders"
		'
		'rpgActions
		'
		Me.rpgActions.ItemLinks.Add(Me.bbFiles_Refresh)
		Me.rpgActions.Name = "rpgActions"
		Me.rpgActions.Text = "Actions"
		'
		'rpHelp
		'
		Me.rpHelp.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.rpgHelp})
		Me.rpHelp.Name = "rpHelp"
		Me.rpHelp.Text = "Help"
		'
		'rpgHelp
		'
		Me.rpgHelp.ItemLinks.Add(Me.bbHelp)
		Me.rpgHelp.Name = "rpgHelp"
		Me.rpgHelp.Text = "Help"
		'
		'rpSettings
		'
		Me.rpSettings.Groups.AddRange(New DevExpress.XtraBars.Ribbon.RibbonPageGroup() {Me.rpgSettings})
		Me.rpSettings.Name = "rpSettings"
		Me.rpSettings.Text = "Settings"
		'
		'rpgSettings
		'
		Me.rpgSettings.ItemLinks.Add(Me.bbSettings)
		Me.rpgSettings.Name = "rpgSettings"
		Me.rpgSettings.Text = "Settings"
		'
		'RibbonStatusBar
		'
		Me.RibbonStatusBar.Location = New System.Drawing.Point(0, 682)
		Me.RibbonStatusBar.Name = "RibbonStatusBar"
		Me.RibbonStatusBar.Ribbon = Me.rcMain
		Me.RibbonStatusBar.Size = New System.Drawing.Size(1160, 21)
		'
		'dmDocMgr
		'
		Me.dmDocMgr.ContainerControl = Me
		Me.dmDocMgr.MenuManager = Me.rcMain
		Me.dmDocMgr.View = Me.tvMain
		Me.dmDocMgr.ViewCollection.AddRange(New DevExpress.XtraBars.Docking2010.Views.BaseView() {Me.tvMain})
		'
		'tvMain
		'
		Me.tvMain.DocumentGroupProperties.ShowTabHeader = False
		Me.tvMain.DocumentGroups.AddRange(New DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup() {Me.DocumentGroup1})
		Me.tvMain.Documents.AddRange(New DevExpress.XtraBars.Docking2010.Views.BaseDocument() {Me.ucSettingsDocument, Me.ucHelpDocument, Me.ucSalesOrdersDocument})
		DockingContainer2.Element = Me.DocumentGroup1
		Me.tvMain.RootContainer.Nodes.AddRange(New DevExpress.XtraBars.Docking2010.Views.Tabbed.DockingContainer() {DockingContainer2})
		'
		'fMain
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(1160, 703)
		Me.Controls.Add(Me.RibbonStatusBar)
		Me.Controls.Add(Me.rcMain)
		Me.Name = "fMain"
		Me.Ribbon = Me.rcMain
		Me.StatusBar = Me.RibbonStatusBar
		Me.Text = "Woo Commerce - Shipping Module"
		Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
		CType(Me.DocumentGroup1, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.ucSettingsDocument, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.ucSalesOrdersDocument, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.ucHelpDocument, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.rcMain, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.dmDocMgr, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.tvMain, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub

	Friend WithEvents rcMain As DevExpress.XtraBars.Ribbon.RibbonControl
	Friend WithEvents rpProgram As DevExpress.XtraBars.Ribbon.RibbonPage
	Friend WithEvents rpgProgram As DevExpress.XtraBars.Ribbon.RibbonPageGroup
	Friend WithEvents RibbonStatusBar As DevExpress.XtraBars.Ribbon.RibbonStatusBar
	Friend WithEvents dmDocMgr As DevExpress.XtraBars.Docking2010.DocumentManager
	Friend WithEvents tvMain As DevExpress.XtraBars.Docking2010.Views.Tabbed.TabbedView
	Friend WithEvents DocumentGroup1 As DevExpress.XtraBars.Docking2010.Views.Tabbed.DocumentGroup
	Friend WithEvents rpActions As DevExpress.XtraBars.Ribbon.RibbonPage
	Friend WithEvents rpgActions As DevExpress.XtraBars.Ribbon.RibbonPageGroup
	Friend WithEvents bbExit As DevExpress.XtraBars.BarButtonItem
	Friend WithEvents bbFiles_Import As DevExpress.XtraBars.BarButtonItem
	Friend WithEvents bbFiles_Refresh As DevExpress.XtraBars.BarButtonItem
	Friend WithEvents bbHelp As DevExpress.XtraBars.BarButtonItem
	Friend WithEvents bbSettings As DevExpress.XtraBars.BarButtonItem
	Friend WithEvents rpHelp As DevExpress.XtraBars.Ribbon.RibbonPage
	Friend WithEvents rpgHelp As DevExpress.XtraBars.Ribbon.RibbonPageGroup
	Friend WithEvents rpSettings As DevExpress.XtraBars.Ribbon.RibbonPage
	Friend WithEvents rpgSettings As DevExpress.XtraBars.Ribbon.RibbonPageGroup
	Friend WithEvents ucSettingsDocument As DevExpress.XtraBars.Docking2010.Views.Tabbed.Document
	Friend WithEvents ucSalesOrdersDocument As DevExpress.XtraBars.Docking2010.Views.Tabbed.Document
	Friend WithEvents ucHelpDocument As DevExpress.XtraBars.Docking2010.Views.Tabbed.Document
End Class
