using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HijamaClinicManagement.ReportsViews
{
    /// <summary>
    /// Interaction logic for ExpensesReport.xaml
    /// </summary>
    public partial class ExpensesReport : UserControl
    {
        public ExpensesReport()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Expences_DateFromField.SelectedDate = null;
            Expences_DateToField.SelectedDate = null;
            Export_ExpencesR();
        }

        // Function For Export Expences Reports .....
        private void Expences_Export_Reports_Button_Click(object sender, RoutedEventArgs e)
        {
            // Queryable if  tow date in view  null
            if (Expences_DateFromField.Text == string.Empty && Expences_DateToField.Text == string.Empty)
            {
                MessageBox.Show("أحد حقول التاريخ يجب أن تكون ممتلئة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            // Queryable if tow date in view not null
            if (Expences_DateFromField.Text != string.Empty && Expences_DateToField.Text != string.Empty)
            {
                // Patients_Report.Load += Export_PatientTowDate;
                Export_ExpencesR("TowDate");
            }
            // Queryable if one of  tow date in view not null
            if (Expences_DateFromField.Text != string.Empty && Expences_DateToField.Text == string.Empty)
            {
                // Patients_Report.Load += Export_PatientOneDate;
                Export_ExpencesR("OneDate");
            }
            // Queryable if one of  tow date in view not null
            if (Expences_DateFromField.Text == string.Empty && Expences_DateToField.Text != string.Empty)
            {
                // Patients_Report.Load += Export_PatientOneDate;
                Export_ExpencesR("OneDate");
            }
        }

        private void Export_ExpencesR()
        {
            // intialize inctance for DataSource reporting s1
            Microsoft.Reporting.WinForms.ReportDataSource reportdatasource = new Microsoft.Reporting.WinForms.ReportDataSource();
            // intialize inctance for DataSet reporting s2
            HijamaCenterDbDataSet2 dataset = new HijamaCenterDbDataSet2();
            //Intialize DataSet
            dataset.BeginInit();
            //Detected Name of DataSet Thate be used with reportdatasource instance 
            reportdatasource.Name = "DataSet1";
            //Detected Path of report with reportview component in view
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Resources\ExpencesReport.rdlc");
            Expences_Report.LocalReport.ReportPath = path;
            //Expences_Report.LocalReport.ReportPath = @"C:\Users\DELL\source\repos\HijamaClinicManagement\HijamaClinicManagement\Resources\ExpencesReport.rdlc";
            HijamaCenterDbDataSet2TableAdapters.ExpensesTableAdapter expencestable = new HijamaCenterDbDataSet2TableAdapters.ExpensesTableAdapter();
            expencestable.ClearBeforeFill = true;
            Expences_Report.LocalReport.DataSources.Clear();
            //Call Fill method in billtable 
            expencestable.Fill(dataset.Expenses);
            //Detected table of dataset Thate be used with reportdatasource instance 
            reportdatasource.Value = expencestable.GetData();
            //Fill this reportdatasource with reportviewr
            Expences_Report.LocalReport.DataSources.Add(reportdatasource);
            // Update reportviewer with new data
            Expences_Report.RefreshReport();
            //Close dataset1
            dataset.EndInit();

        }

        private void Export_ExpencesR(string report_type)
        {
            // intialize inctance for DataSource reporting s1
            Microsoft.Reporting.WinForms.ReportDataSource reportdatasource = new Microsoft.Reporting.WinForms.ReportDataSource();
            // intialize inctance for DataSet reporting s2
            HijamaCenterDbDataSet2 dataset = new HijamaCenterDbDataSet2();
            //Intialize DataSet
            dataset.BeginInit();
            //Detected Name of DataSet Thate be used with reportdatasource instance 
            reportdatasource.Name = "DataSet1";
            //Detected Path of report with reportview component in view
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Resources\ExpencesReport.rdlc");
            Expences_Report.LocalReport.ReportPath = path;
            //Expences_Report.LocalReport.ReportPath = @"C:\Users\DELL\source\repos\HijamaClinicManagement\HijamaClinicManagement\Resources\ExpencesReport.rdlc";
            HijamaCenterDbDataSet2TableAdapters.ExpensesTableAdapter expencestable = new HijamaCenterDbDataSet2TableAdapters.ExpensesTableAdapter();
            expencestable.ClearBeforeFill = true;

            if (report_type.Equals("TowDate"))
            {
                Expences_Report.LocalReport.DataSources.Clear();
                //Call Fill method in billtable 
                expencestable.FilterTowDate(dataset.Expenses, Expences_DateFromField.Text, Expences_DateToField.Text);
                //Detected table of dataset Thate be used with reportdatasource instance 
                reportdatasource.Value = expencestable.GetDataBy(Expences_DateFromField.Text, Expences_DateToField.Text);
                //Fill this reportdatasource with reportviewr
                Expences_Report.LocalReport.DataSources.Add(reportdatasource);
                // Update reportviewer with new data
                Expences_Report.RefreshReport();
                //Close dataset1
                dataset.EndInit();

            }
            if (report_type.Equals("OneDate"))
            {
                if (Expences_DateFromField.Text != string.Empty && Expences_DateToField.Text == string.Empty)
                {
                    Expences_Report.LocalReport.DataSources.Clear();
                    //Call Fill method in billtable 
                    expencestable.FilterOneDate(dataset.Expenses, Expences_DateFromField.Text);
                    //Detected table of dataset Thate be used with reportdatasource instance 
                    reportdatasource.Value = expencestable.GetDataBy1(Expences_DateFromField.Text);
                    //Fill this reportdatasource with reportviewr
                    Expences_Report.LocalReport.DataSources.Add(reportdatasource);
                    // Update reportviewer with new data
                    Expences_Report.RefreshReport();
                    //Close dataset1
                    dataset.EndInit();

                }
                if (Expences_DateFromField.Text == string.Empty && Expences_DateToField.Text != string.Empty)
                {
                    Expences_Report.LocalReport.DataSources.Clear();
                    //Call Fill method in billtable 
                    expencestable.FilterOneDate(dataset.Expenses, Expences_DateToField.Text);
                    //Detected table of dataset Thate be used with reportdatasource instance 
                    reportdatasource.Value = expencestable.GetDataBy1(Expences_DateToField.Text);
                    //Fill this reportdatasource with reportviewr
                    Expences_Report.LocalReport.DataSources.Add(reportdatasource);
                    // Update reportviewer with new data
                    Expences_Report.RefreshReport();
                    //Close dataset1
                    dataset.EndInit();

                }
            }

        }

        private void Clear_Date_ER_Click(object sender, RoutedEventArgs e)
        {
            Clear_ERDate();
        }

        // Function Clear
        private void Clear_ERDate()
        {
            Expences_DateFromField.SelectedDate = null;
            Expences_DateToField.SelectedDate = null;
            Expences_Report.Clear();
        }
    }
}
