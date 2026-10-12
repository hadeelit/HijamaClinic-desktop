using Microsoft.Win32;
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
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace HijamaClinicManagement.UsersView
{
    /// <summary>
    /// Interaction logic for AccountsManagement.xaml
    /// </summary>
    public partial class AccountsManagement : UserControl
    {
        DBHandler.UsersAdapter user_adapter = new DBHandler.UsersAdapter();
        public static string imgfilename = "";
        private string User_id;

        public AccountsManagement()
        {
            InitializeComponent();
        }

        //Intialization UserControl
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_User();
            LoadUsersGridViw();
           //UserNameField_AccountsMang.IsReadOnly = true;
        }

        //Load Data from Users Table to AccountsManagementsGridView
        private void LoadUsersGridViw()
        {
            SqlDataReader sdr = user_adapter.Get_Users();
            DataTable dt = new DataTable("Users");
            dt.Load(sdr);
            DataViewFromUsersTable.ItemsSource = dt.DefaultView;
            DBHandler.ConnectionDB.CloseConnection();
        }

        // Control of UsersGridView Record and Get Data From UsersGridView To Fields When Click on Row 
        private void DataViewFromUsersTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cell1 = DataViewFromUsersTable.SelectedCells[1];
            var cell2 = DataViewFromUsersTable.SelectedCells[2];
            var cell3 = DataViewFromUsersTable.SelectedCells[3];
            var cell4 = DataViewFromUsersTable.SelectedCells[4];

            string username = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
            string pass = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
            string email = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
            string role = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;

            Clear_User();
            UserNameField_AccountsMang.Text = username;
            PasswordField_AccountsMang.Text = pass;
            EmailField_AccountsMang.Text = email;
            TypesRoles.SelectedItem = role;
            switch (role)
            {
                case "Doctor":
                    TypesRoles.SelectedIndex = 0;
                    break;
                case "Receptionist":
                    TypesRoles.SelectedIndex = 1;
                    break;
                case "Other":
                    TypesRoles.SelectedIndex = 2;
                    break;
            }

        }

        //Add Delete Edie Button For AccountsManagment Views
        private void AddButton_AccountsMang_Click(object sender, RoutedEventArgs e)
        {
            if (Accounts_Isvalid())
            {
                //Test if the user already exists or not
                bool check_user = user_adapter.Check_UserName(UserNameField_AccountsMang.Text);
                if (check_user == false)
                {
                    bool result = user_adapter.Insert_User(UserNameField_AccountsMang.Text, PasswordField_AccountsMang.Text, TypesRoles.Text, EmailField_AccountsMang.Text);
                    if (result)
                    {
                        MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_User();
                        LoadUsersGridViw();
                    }
                    else
                    {
                        MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_User();
                    }
                }
                else
                {
                    MessageBox.Show("اسم المستخدم المدخل موجود مسبقاً ! أدخل اسم اّخر", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    UserNameField_AccountsMang.Focus();
                }
            }
        }

        //Delete Button
        private void DeleteButton_AccountsMang_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromUsersTable.SelectedItem != null)
            {
                if (UserNameField_AccountsMang.Text == string.Empty || PasswordField_AccountsMang.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد الحذف بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        var cell0 = DataViewFromUsersTable.SelectedCells[0];
                        string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                        bool result = user_adapter.Delete_user(id);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadUsersGridViw();
                            Clear_User();
                            user_adapter.Edit_Identity();
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
            else
            {
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        //Edit Button
        private void EditButton_AccountsMang_Click(object sender, RoutedEventArgs e)
        {

            // Detect if one of rows in GridView Selection Or not
            if (DataViewFromUsersTable.SelectedItem != null)
            {
                // If Not Detect Row Correct
                if (UserNameField_AccountsMang.Text == string.Empty || PasswordField_AccountsMang.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    MessageBox.Show("يجب ادخال  حقلي اسم المستخدم وكلمة المرور ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    if (Accounts_Isvalid())
                    {
                        // Step 1 : Fetch Info For Specefic cell
                        var cell0 = DataViewFromUsersTable.SelectedCells[0];
                        // Step 2: get the content for this info Cell (User_id Cell)
                        string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;

                        bool check_user = user_adapter.Check_UserName(id, UserNameField_AccountsMang.Text);
                        if (check_user == false)
                        {
                            // MessageBox For Yes No Updated
                            System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الحقول بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                            if (((int)dialog) == 6)
                            {
                                // Call Update_User()
                                bool result = user_adapter.Update_User(id, UserNameField_AccountsMang.Text, PasswordField_AccountsMang.Text, TypesRoles.Text, EmailField_AccountsMang.Text);
                                if (result)
                                {
                                    MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                    LoadUsersGridViw();
                                    Clear_User();
                                }
                                else
                                {
                                    MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                }
                            }
                            else
                                MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                        else
                        {
                            MessageBox.Show("اسم المستخدم المدخل موجود مسبقاً ! أدخل اسم اّخر", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            UserNameField_AccountsMang.Focus();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }
        
        //Clear Button
        private void ClearButton_AccountsMang_Click(object sender, RoutedEventArgs e)
        {
            // Clear Fields in AccountsMng View
            Clear_User();
        }

        //Function For Verify From AccountsMangement View  Fields
        private bool Accounts_Isvalid()
        {
            if (UserNameField_AccountsMang.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                UserNameField_AccountsMang.Focus();
                return false;
            }
            if (PasswordField_AccountsMang.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PasswordField_AccountsMang.Focus();
                return false;
            }
            if (EmailField_AccountsMang.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                EmailField_AccountsMang.Focus();
                return false;
            }
            if (EmailField_AccountsMang.Text != string.Empty)
            {
                if (!EmailField_AccountsMang.Text.Contains("@") || !EmailField_AccountsMang.Text.Contains(".com"))
                {
                    MessageBox.Show("فضلاً أدخل البريد الإلكتروني بالشكل الصحيح", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    EmailField_AccountsMang.Focus();
                    return false;
                }
                if (!EmailField_AccountsMang.Text.Contains("gmail"))
                {
                    MessageBox.Show("فضلاً أدخل بريد الكتروني gmail ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    EmailField_AccountsMang.Focus();
                    return false;
                }
            }
            if (TypesRoles.SelectedItem == null)
            {
                MessageBox.Show(" حقل نوع المستخدم اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                TypesRoles.Focus();
                return false;
            }
            return true;
        }

        // Clear Values in AccountsMangement View  Fields
        private void Clear_User()
        {
            UserNameField_AccountsMang.Clear();
            PasswordField_AccountsMang.Clear();
            EmailField_AccountsMang.Clear();
            TypesRoles.SelectedItem = null;
        }

    }
}
