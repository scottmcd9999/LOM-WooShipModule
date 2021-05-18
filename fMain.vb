Imports System.Data.SqlClient
Imports DevExpress.XtraBars.Docking2010.Views
Imports DevExpress.XtraBars.Ribbon

Public Class fMain
	Sub New()

		InitializeComponent()
		AddHandler Me.tvMain.QueryControl, AddressOf tvMain_QueryControl
		AddHandler Me.tvMain.QueryControl, AddressOf tvMain_QueryControl
	End Sub

	Private logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger

	Private Sub tvMain_DocumentActivated(sender As Object, e As DocumentEventArgs) Handles tvMain.DocumentActivated
		Try
			'/ sync the ribbon with the active document
			Select Case e.Document.ControlName
				Case "docSettings"
					rcMain.SelectedPage = rpSettings
				Case "docHelp"
					rcMain.SelectedPage = rpHelp
				Case "docSalesOrders"
					rcMain.SelectedPage = rpActions
				Case "ucFiles"
					'rcMain.SelectedPage = rpFile
				Case Else
					'rcMain.SelectedPage = rpFile
			End Select
		Catch ex As Exception
			logger.Error(ex.ToString, "Jobboss_Woo_Integration.fMain.tvMain_DocumentActivated Error")
			MessageBox.Show(String.Format("Error in Jobboss_Woo_Integration.fMain.tvMain_DocumentActivated: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
		Finally

		End Try
	End Sub

	'Private Sub tvMain_QueryControl(sender As Object, e As QueryControlEventArgs) Handles tvMain.QueryControl
	'	Try
	'		If e.Document Is ucFilesDocument Then
	'			e.Control = New ucFiles
	'			gucFile = TryCast(e.Control, ucFiles)
	'		ElseIf e.Document Is ucHelpDocument Then
	'			e.Control = New ucHelp
	'			gucHelp = TryCast(e.Control, ucHelp)
	'		ElseIf e.Document Is ucLogDocument Then
	'			e.Control = New ucLog
	'			gucLog = TryCast(e.Control, ucLog)
	'		ElseIf e.Document Is ucSetupDocument Then
	'			e.Control = New ucSetup
	'			gucSetup = TryCast(e.Control, ucSetup)
	'		Else
	'			e.Control = New System.Windows.Forms.Control
	'		End If
	'	Catch ex As Exception
	'		logger.Error(ex.ToString, "Jobboss_Woo_Integration.fMain.tvMain_QueryControl Error")
	'		XtraMessageBox.Show(String.Format("Error in Jobboss_Woo_Integration.fMain.tvMain_QueryControl: {0}{1}{2}", Environment.NewLine, Environment.NewLine, ex), "Error")
	'	Finally

	'	End Try
	'End Sub

	'Private Sub tvMain_QueryControl(sender As Object, e As QueryControlEventArgs) Handles tvMain.QueryControl
	'	If e.Document Is docSalesOrders Then
	'		e.Control = New ucSalesOrders
	'		gucSalesOrders = TryCast(e.Control, ucSalesOrders)
	'	End If
	'	If e.Document Is docHelp Then
	'		e.Control = New ucHelp
	'		gucHelp = TryCast(e.Control, ucHelp)
	'	End If
	'	If e.Document Is docSettings Then
	'		e.Control = New ucHelp
	'		gucSettings = New ucSettings
	'	End If
	'End Sub


	Private Sub bbExit_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbExit.ItemClick
		Close()

	End Sub

	Private Sub bbFiles_Refresh_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbFiles_Refresh.ItemClick
		gucSalesOrders.LoadSalesOrders()
	End Sub

	Private Sub bbFiles_Import_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs) Handles bbFiles_Import.ItemClick

	End Sub

	Private Sub rcMain_SelectedPageChanging(sender As Object, e As RibbonPageChangingEventArgs) Handles rcMain.SelectedPageChanging
		Select Case e.Page.Name
			Case "rpSettings"
				tvMain.Controller.Activate(ucSettingsDocument) ' docSettings)
				'tvMain.Controller.Select(docSettings)
			Case "rpActions"
				tvMain.Controller.Activate(ucSalesOrdersDocument) ' docSalesOrders)
				'tvMain.Controller.Select(docSalesOrders)
			Case "rpHelp"
				tvMain.Controller.Activate(ucHelpDocument) ' ucdocHelp)
				'tvMain.Controller.Select(docHelp)
		End Select
	End Sub



	Sub tvMain_QueryControl(sender As Object, e As QueryControlEventArgs) Handles tvMain.QueryControl
		If e.Document Is ucSettingsDocument Then
			e.Control = New WooShipModule.ucSettings()
		End If
		If e.Document Is ucSalesOrdersDocument Then
			e.Control = New WooShipModule.ucSalesOrders()
		End If
		If e.Document Is ucHelpDocument Then
			e.Control = New WooShipModule.ucHelp()
		End If
		If e.Control Is Nothing Then
			e.Control = New System.Windows.Forms.Control()
		End If
	End Sub

	Private Sub fMain_Load(sender As Object, e As EventArgs) Handles Me.Load
		rcMain.SelectedPage = rpActions
	End Sub
End Class