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

namespace HijamaClinicManagement.DashBoardView
{
    /// <summary>
    /// Interaction logic for DashBoardView.xaml
    /// </summary>
    public partial class DashBoardView : UserControl
    {
        DBHandler.DashBoardAdapter dashBoard_adapter = new DBHandler.DashBoardAdapter();
        public DashBoardView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DashBoardQueries();
        }

        // Get Statistics For DahBoard
        public void DashBoardQueries()
        {
            // Get Current Date 
            DateTime now = DateTime.Now.Date;
            string current_date = now.ToString();

            int NewFiles = dashBoard_adapter.Get_NewFilesCount(current_date);
            int DailyAppoint = dashBoard_adapter.Get_DailyAppointCount(current_date);
            int DailyDeletedBills = dashBoard_adapter.Get_DailyDeletedBills(current_date);
            int DailyBills = dashBoard_adapter.Get_DailyBills(current_date);

            NewPFileNum.Content = NewFiles.ToString();
            DailyAppointmentsNum.Content = DailyAppoint.ToString();
            DailyBillsNum.Content = DailyBills.ToString();
            DailyDelBillsNum.Content = DailyDeletedBills.ToString();

        }
    }
}
