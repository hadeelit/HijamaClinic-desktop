using Grpc.Core;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HijamaClinicManagement.ReportsViews
{
    /// <summary>
    /// Interaction logic for PatientsReport.xaml
    /// </summary>
    public partial class PatientsReport : UserControl
    {
        public PatientsReport()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DateFromField.SelectedDate = null;
            DateToField.SelectedDate = null;
            Export_PatientsR();
        }

        // Function For Export Patients Reports ....
        private void Export_Reports_Button_Click(object sender, RoutedEventArgs e)
        {

            // Queryable if  tow date in view  null
            if (DateFromField.Text == string.Empty && DateToField.Text == string.Empty)
            {
                MessageBox.Show("أحد حقول التاريخ يجب أن تكون ممتلئة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            // Queryable if tow date in view not null
            if (DateFromField.Text != string.Empty && DateToField.Text != string.Empty)
            {
                // Patients_Report.Load += Export_PatientTowDate;
                Export_PatientsR("TowDate");
            }
            // Queryable if one of  tow date in view not null
            if (DateFromField.Text != string.Empty && DateToField.Text == string.Empty)
            {
                // Patients_Report.Load += Export_PatientOneDate;
                Export_PatientsR("OneDate");
            }
            // Queryable if one of  tow date in view not null
            if (DateFromField.Text == string.Empty && DateToField.Text != string.Empty)
            {
                // Patients_Report.Load += Export_PatientOneDate;
                Export_PatientsR("OneDate");
            }

        }

        private void Export_PatientsR()
        {

            Microsoft.Reporting.WinForms.ReportDataSource reportdatasource = new Microsoft.Reporting.WinForms.ReportDataSource();
            HijamaCenterDbDataSet dataset = new HijamaCenterDbDataSet();
            //Intialize dataset
            dataset.BeginInit();
            reportdatasource.Name = "DataSet1";
            reportdatasource.Value = dataset.Patient;
            // Patients_Report.LocalReport.DataSources.Add(reportdatasource);
            string filename = "PatientsReport.rdlc";
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Resources\", filename);
            //Patients_Report.LocalReport.ReportPath = @"C:\Users\DELL\source\repos\HijamaClinicManagement\HijamaClinicManagement\Resources\PatientsReport.rdlc";
            Patients_Report.LocalReport.ReportPath = path;


            //fill data into HijamaCenterDbDataSet
            HijamaCenterDbDataSetTableAdapters.PatientTableAdapter patienttable = new HijamaCenterDbDataSetTableAdapters.PatientTableAdapter();
            patienttable.ClearBeforeFill = true;
            Patients_Report.LocalReport.DataSources.Clear();
            patienttable.Fill(dataset.Patient);
            reportdatasource.Value = patienttable.GetData();
            Patients_Report.LocalReport.DataSources.Add(reportdatasource);
            Patients_Report.RefreshReport();
            // Close dataSet
            dataset.EndInit();
        }

        private void Export_PatientsR(string report_type)
        {
            Microsoft.Reporting.WinForms.ReportDataSource reportdatasource = new Microsoft.Reporting.WinForms.ReportDataSource();
            HijamaCenterDbDataSet dataset = new HijamaCenterDbDataSet();
            //Intialize dataset
            dataset.BeginInit();
            reportdatasource.Name = "DataSet1";
            reportdatasource.Value = dataset.Patient;
            // Patients_Report.LocalReport.DataSources.Add(reportdatasource);
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Resources\PatientsReport.rdlc");
            Patients_Report.LocalReport.ReportPath = path;
            // Patients_Report.LocalReport.ReportPath = @"C:\Users\PcMax\source\repos\HijamaClinicManagement\HijamaClinicManagement\PatientsReport.rdlc";


            //fill data into HijamaCenterDbDataSet
            HijamaCenterDbDataSetTableAdapters.PatientTableAdapter patienttable = new HijamaCenterDbDataSetTableAdapters.PatientTableAdapter();
            patienttable.ClearBeforeFill = true;

            if (report_type.Equals("TowDate"))
            {
                //MessageBox.Show("Tow");
                Patients_Report.LocalReport.DataSources.Clear();
                patienttable.FilterTowDate(dataset.Patient, DateFromField.Text, DateToField.Text);
                reportdatasource.Value = patienttable.GetDataBy(DateFromField.Text, DateToField.Text);
                Patients_Report.LocalReport.DataSources.Add(reportdatasource);
                Patients_Report.RefreshReport();
                // Close dataSet
                dataset.EndInit();

            }
            if (report_type.Equals("OneDate"))
            {
                if (DateFromField.Text != string.Empty && DateToField.Text == string.Empty)
                {
                    // MessageBox.Show("From");
                    Patients_Report.LocalReport.DataSources.Clear();
                    patienttable.FilterOneDate(dataset.Patient, DateFromField.Text);
                    reportdatasource.Value = patienttable.GetDataBy1(DateFromField.Text);
                    Patients_Report.LocalReport.DataSources.Add(reportdatasource);
                    Patients_Report.RefreshReport();
                    // Close dataSet
                    dataset.EndInit();

                }
                if (DateToField.Text != string.Empty && DateFromField.Text == string.Empty)
                {
                    //MessageBox.Show("To");
                    Patients_Report.LocalReport.DataSources.Clear();
                    patienttable.FilterOneDate(dataset.Patient, DateToField.Text);
                    reportdatasource.Value = patienttable.GetDataBy1(DateToField.Text);
                    Patients_Report.LocalReport.DataSources.Add(reportdatasource);
                    Patients_Report.RefreshReport();
                    // Close dataSet
                    dataset.EndInit();

                }



            }
        }

        //Clear dates from Patients report
        private void Clear_Date_PR_Click(object sender, RoutedEventArgs e)
        {
            Clear_PRDate();
        }

        //Function for Clear Dates in Reports 
        private void Clear_PRDate()
        {
            DateFromField.SelectedDate = null;
            DateToField.SelectedDate = null;
            Patients_Report.Clear();
        }

       
    }
}
