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
    /// Interaction logic for BillsReport.xaml
    /// </summary>
    public partial class BillsReport : UserControl
    {
        public BillsReport()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Bill_DateFromField.SelectedDate = null;
            Bill_DateToField.SelectedDate = null;
            Export_BillsR();
        }

        // Function For Export Bills Reports .....
        private void Bill_Export_Reports_Button_Click(object sender, RoutedEventArgs e)
        {
            // Queryable if  tow date in view  null
            if (Bill_DateFromField.Text == string.Empty && Bill_DateToField.Text == string.Empty)
            {
                MessageBox.Show("أحد حقول التاريخ يجب أن تكون ممتلئة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            // Queryable if tow date in view not null
            if (Bill_DateFromField.Text != string.Empty && Bill_DateToField.Text != string.Empty)
            {
                // Patients_Report.Load += Export_PatientTowDate;
                Export_BillsR("TowDate");
            }
            // Queryable if one of  tow date in view not null
            if (Bill_DateFromField.Text != string.Empty && Bill_DateToField.Text == string.Empty)
            {
                // Patients_Report.Load += Export_PatientOneDate;
                Export_BillsR("OneDate");
            }
            // Queryable if one of  tow date in view not null
            if (Bill_DateFromField.Text == string.Empty && Bill_DateToField.Text != string.Empty)
            {
                // Patients_Report.Load += Export_PatientOneDate;
                Export_BillsR("OneDate");
            }
        }

        private void Export_BillsR()
        {

            // intialize inctance for DataSource reporting s1
            Microsoft.Reporting.WinForms.ReportDataSource reportdatasource = new Microsoft.Reporting.WinForms.ReportDataSource();
            // intialize inctance for DataSet reporting s2
            HijamaCenterDbDataSet1 dataset = new HijamaCenterDbDataSet1();
            //Intialize DataSet
            dataset.BeginInit();
            //Detected Name of DataSet Thate be used with reportdatasource instance 
            reportdatasource.Name = "DataSet1";
            // reportdatasource.Value = dataset.Bill;
            //Detected Path of report with reportview component in view
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Resources\BillsReport.rdlc");
            Bills_Report.LocalReport.ReportPath = path;
            // Bills_Report.LocalReport.ReportPath = @"C:\Users\DELL\source\repos\HijamaClinicManagement\HijamaClinicManagement\Resources\BillsReport.rdlc";
            //fill data into HijamaCenterDbDataSet1
            HijamaCenterDbDataSet1TableAdapters.BillTableAdapter billtable = new HijamaCenterDbDataSet1TableAdapters.BillTableAdapter();
            billtable.ClearBeforeFill = true;
            //Clear DataSource befor fill in reportviewr with fill method
            Bills_Report.LocalReport.DataSources.Clear();
            //Call Fill method in billtable 
            billtable.Fill(dataset.Bill);
            //Detected table of dataset Thate be used with reportdatasource instance 
            reportdatasource.Value = billtable.GetData();
            //Fill this reportdatasource with reportviewr
            Bills_Report.LocalReport.DataSources.Add(reportdatasource);
            // Update reportviewer with new data
            Bills_Report.RefreshReport();
            //Close dataset1
            dataset.EndInit();
        }

        private void Export_BillsR(string report_type)
        {
            // intialize inctance for DataSource reporting s1
            Microsoft.Reporting.WinForms.ReportDataSource reportdatasource = new Microsoft.Reporting.WinForms.ReportDataSource();
            // intialize inctance for DataSet reporting s2
            HijamaCenterDbDataSet1 dataset = new HijamaCenterDbDataSet1();
            //Intialize DataSet
            dataset.BeginInit();
            //Detected Name of DataSet Thate be used with reportdatasource instance 
            reportdatasource.Name = "DataSet1";
            // reportdatasource.Value = dataset.Bill;
            //Detected Path of report with reportview component in view
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Resources\BillsReport.rdlc");
            Bills_Report.LocalReport.ReportPath = path;
            //Bills_Report.LocalReport.ReportPath = @"C:\Users\PcMax\source\repos\HijamaClinicManagement\HijamaClinicManagement\BillsReport.rdlc";
            //fill data into HijamaCenterDbDataSet1
            HijamaCenterDbDataSet1TableAdapters.BillTableAdapter billtable = new HijamaCenterDbDataSet1TableAdapters.BillTableAdapter();
            billtable.ClearBeforeFill = true;

            if (report_type.Equals("TowDate"))
            {
                //Clear DataSource befor fill in reportviewr with fill method
                Bills_Report.LocalReport.DataSources.Clear();
                //Call Fill method in billtable 
                billtable.FilterTowDate(dataset.Bill, Bill_DateFromField.Text, Bill_DateToField.Text);
                //Detected table of dataset Thate be used with reportdatasource instance 
                reportdatasource.Value = billtable.GetDataBy(Bill_DateFromField.Text, Bill_DateToField.Text);
                //Fill this reportdatasource with reportviewr
                Bills_Report.LocalReport.DataSources.Add(reportdatasource);
                // Update reportviewer with new data
                Bills_Report.RefreshReport();
                //Close dataset1
                dataset.EndInit();

            }
            if (report_type.Equals("OneDate"))
            {
                if (Bill_DateFromField.Text != string.Empty && Bill_DateToField.Text == string.Empty)
                {
                    //Clear DataSource befor fill in reportviewr with fill method
                    Bills_Report.LocalReport.DataSources.Clear();
                    //Call Fill method in billtable 
                    billtable.FilterOneDate(dataset.Bill, Bill_DateFromField.Text);
                    //Detected table of dataset Thate be used with reportdatasource instance 
                    reportdatasource.Value = billtable.GetDataBy1(Bill_DateFromField.Text);
                    //Fill this reportdatasource with reportviewr
                    Bills_Report.LocalReport.DataSources.Add(reportdatasource);
                    // Update reportviewer with new data
                    Bills_Report.RefreshReport();
                    //Close dataset1
                    dataset.EndInit();

                }

                if (Bill_DateFromField.Text == string.Empty && Bill_DateToField.Text != string.Empty)
                {
                    //Clear DataSource befor fill in reportviewr with fill method
                    Bills_Report.LocalReport.DataSources.Clear();
                    //Call Fill method in billtable 
                    billtable.FilterOneDate(dataset.Bill, Bill_DateToField.Text);
                    //Detected table of dataset Thate be used with reportdatasource instance 
                    reportdatasource.Value = billtable.GetDataBy1(Bill_DateToField.Text);
                    //Fill this reportdatasource with reportviewr
                    Bills_Report.LocalReport.DataSources.Add(reportdatasource);
                    // Update reportviewer with new data
                    Bills_Report.RefreshReport();
                    //Close dataset1
                    dataset.EndInit();

                }
            }
        }

        private void Clear_Date_Reports_Button_Click(object sender, RoutedEventArgs e)
        {
            Clear_BRDate();
        }

        //Function Clear 
        private void Clear_BRDate()
        {
            Bill_DateFromField.SelectedDate = null;
            Bill_DateToField.SelectedDate = null;
            Bills_Report.Clear();
        }
    }
}
