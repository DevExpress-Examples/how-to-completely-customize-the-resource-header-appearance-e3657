using System.Data;
using System.Data.OleDb;
using System.Windows;
using DevExpress.XtraScheduler;

namespace SchedulerResHeaderTemplateExWpf {
    public partial class MainWindow : Window {
        private CarsDBDataSet dataSet = new CarsDBDataSet();
        private CarsDBDataSetTableAdapters.CarSchedulingTableAdapter tableAdapterAppointments = new CarsDBDataSetTableAdapters.CarSchedulingTableAdapter();
        private CarsDBDataSetTableAdapters.CarsTableAdapter tableAdapterResources = new CarsDBDataSetTableAdapters.CarsTableAdapter();

        public MainWindow() {
            InitializeComponent();
            
            tableAdapterAppointments.Fill(dataSet.CarScheduling);
            tableAdapterResources.Fill(dataSet.Cars);

            schedulerControl1.Storage.ResourceStorage.DataSource = dataSet.Cars;
            schedulerControl1.Storage.AppointmentStorage.DataSource = dataSet.CarScheduling;

            if (schedulerControl1.Storage.AppointmentStorage.Count > 0)
                schedulerControl1.Start = schedulerControl1.Storage.AppointmentStorage[0].Start;

            schedulerControl1.Storage.AppointmentsInserted +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);
            schedulerControl1.Storage.AppointmentsChanged +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);
            schedulerControl1.Storage.AppointmentsDeleted +=
                new PersistentObjectsEventHandler(Storage_AppointmentsModified);

            tableAdapterAppointments.Adapter.RowUpdated +=
                new System.Data.OleDb.OleDbRowUpdatedEventHandler(adapter_RowUpdated);
        }

        void Storage_AppointmentsModified(object sender, PersistentObjectsEventArgs e) {
            this.tableAdapterAppointments.Adapter.Update(this.dataSet);
            this.dataSet.AcceptChanges();
        }

        private void adapter_RowUpdated(object sender, System.Data.OleDb.OleDbRowUpdatedEventArgs e) {
            if (e.Status == UpdateStatus.Continue && e.StatementType == StatementType.Insert) {
                int id = 0;
                using (OleDbCommand cmd = new OleDbCommand("SELECT @@IDENTITY", tableAdapterAppointments.Connection)) {
                    id = (int)cmd.ExecuteScalar();
                }
                e.Row["ID"] = id;
            }
        }
    }
}
