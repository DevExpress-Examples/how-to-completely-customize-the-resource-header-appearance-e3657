Imports Microsoft.VisualBasic
Imports System.Data
Imports System.Data.OleDb
Imports System.Windows
Imports DevExpress.XtraScheduler

Namespace SchedulerResHeaderTemplateExWpf
	Partial Public Class MainWindow
		Inherits Window
		Private dataSet As New CarsDBDataSet()
		Private tableAdapterAppointments As New CarsDBDataSetTableAdapters.CarSchedulingTableAdapter()
		Private tableAdapterResources As New CarsDBDataSetTableAdapters.CarsTableAdapter()

		Public Sub New()
			InitializeComponent()

			tableAdapterAppointments.Fill(dataSet.CarScheduling)
			tableAdapterResources.Fill(dataSet.Cars)

			schedulerControl1.Storage.ResourceStorage.DataSource = dataSet.Cars
			schedulerControl1.Storage.AppointmentStorage.DataSource = dataSet.CarScheduling

			If schedulerControl1.Storage.AppointmentStorage.Count > 0 Then
				schedulerControl1.Start = schedulerControl1.Storage.AppointmentStorage(0).Start
			End If

			AddHandler schedulerControl1.Storage.AppointmentsInserted, AddressOf Storage_AppointmentsModified
			AddHandler schedulerControl1.Storage.AppointmentsChanged, AddressOf Storage_AppointmentsModified
			AddHandler schedulerControl1.Storage.AppointmentsDeleted, AddressOf Storage_AppointmentsModified

			AddHandler tableAdapterAppointments.Adapter.RowUpdated, AddressOf adapter_RowUpdated
		End Sub

		Private Sub Storage_AppointmentsModified(ByVal sender As Object, ByVal e As PersistentObjectsEventArgs)
			Me.tableAdapterAppointments.Adapter.Update(Me.dataSet)
			Me.dataSet.AcceptChanges()
		End Sub

		Private Sub adapter_RowUpdated(ByVal sender As Object, ByVal e As System.Data.OleDb.OleDbRowUpdatedEventArgs)
			If e.Status = UpdateStatus.Continue AndAlso e.StatementType = StatementType.Insert Then
				Dim id As Integer = 0
				Using cmd As New OleDbCommand("SELECT @@IDENTITY", tableAdapterAppointments.Connection)
					id = CInt(Fix(cmd.ExecuteScalar()))
				End Using
				e.Row("ID") = id
			End If
		End Sub
	End Class
End Namespace
