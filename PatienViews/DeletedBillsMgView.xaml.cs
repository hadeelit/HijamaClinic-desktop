using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for DeletedBillsMgView.xaml
    /// </summary>
    public partial class DeletedBillsMgView : UserControl
    {
        DBHandler.DeletedBillsAdapter deletedb_adapter = new DBHandler.DeletedBillsAdapter();

        public DeletedBillsMgView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("  فضلا قم بتفريغ الجدول عند الامتلاء للحفاظ على مساحة القاعدة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            Get_AllDeletedBillsRecords();
        }

        // Deleted Bills Management  View .....
        private void Get_AllDeletedBillsRecords()
        {
            SqlDataReader sdr = deletedb_adapter.Get_DeletedBillsRecords();
            if (sdr.HasRows == false)
            {
                DBHandler.ConnectionDB.CloseConnection();
                MessageBox.Show("لايوجد سجلات فواتير محذوفة", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                DataTable dt_deletedbills = new DataTable();
                dt_deletedbills.Load(sdr);
                DataViewPatient_DeletedBills.ItemsSource = dt_deletedbills.DefaultView;
                DBHandler.ConnectionDB.CloseConnection();
            }

        }

        private void ClearButton_DBills_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.DialogResult dialog1 = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد حذف جميع سجلات الفواتير المحذوفة مسبقاً  ", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
            if (((int)dialog1) == 6)
            {
                bool result = deletedb_adapter.Delete_DeletedBillsRecords();
                if (result)
                {
                    MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    DataViewPatient_DeletedBills.ItemsSource = null;
                    deletedb_adapter.Edit_Identity_DeletedBillsRecords();
                }
                else
                {
                    MessageBox.Show("فشلت عملية الحذف", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("تم الغاء الحذف ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }
    }
}
