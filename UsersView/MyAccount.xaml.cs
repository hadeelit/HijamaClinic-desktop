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
    /// Interaction logic for MyAccount.xaml
    /// </summary>
    public partial class MyAccount : UserControl
    {
        DBHandler.UsersAdapter user_adapter = new DBHandler.UsersAdapter();
        public static string imgfilename = "";
        private string User_id;

        public MyAccount()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPersonalInfo();
        }

        //EdieButton For MyAccountManagment Views
        private void EdieButton_MyAccount_Click(object sender, RoutedEventArgs e)
        {
            if (Myaccount_Isvalid())
            {
                // Test if the user already exists or not
                bool check_user = user_adapter.Check_UserName(User_id, UserNameField.Text);
                if (check_user == false)
                {
                    // MessageBox For Yes No Updated
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الحقول بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = user_adapter.Update_Myaccount(User_id, UserNameField.Text, PasswordField.Text, imgfilename, EmailField.Text);
                        if (result)
                        {
                            MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            if (imgfilename != string.Empty)
                            {
                                // Visible Delete Button If Pimage Not Empety in DB
                                DeletePersonalImg_MyAccount.Visibility = Visibility.Visible;
                            }

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
                    UserNameField.Focus();
                }

            }

        }

        // Add external image and save it
        private void AddPersonalImg_Click(object sender, RoutedEventArgs e)
        {
            // Get The Name of Button
            string NameOfButton = (((Button)e.Source).Name);

            //Create Object From OpenFileDialog
            OpenFileDialog openFile = new OpenFileDialog();

            //Add Properties to OpenFileDialog
            openFile.Title = "إختر ملف الصورة";
            openFile.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
        "Portable Network Graphic (*.png)|*.png";

            if (NameOfButton.Equals("AddPersonalImg_MyAccount"))
            {

                // Uploade img to Image Box
                if (openFile.ShowDialog() == true)
                {
                    imgfilename = openFile.FileName;
                    MyAccount_PersonalImg.Fill = new ImageBrush(new BitmapImage(new Uri(imgfilename)));
                    MessageBox.Show("تم اضافة الصورة احفظ التعديلات ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            /* else
             {

                 // Uploade img to Image Box
                 if (openFile.ShowDialog() == true)
                 {
                     AccountsMang_PersonalImg.Fill = new ImageBrush(new BitmapImage(new Uri(openFile.FileName)));
                     MessageBox.Show("تم اضافة الصورة احفظ التعديلات ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

                 }
             }*/
        }

        // Delete My Personalimage 
        private void DeletePersonalImg_Click(object sender, RoutedEventArgs e)
        {
            // MessageBox For Yes No Deleted
            System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريدحذف الصورة بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
            if (((int)dialog) == 6)
            {
                bool result = user_adapter.Delete_PersonalImg(User_id);
                if (result)
                {
                    MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    // Hidden Delete Button If Pimage Empety in DB
                    DeletePersonalImg_MyAccount.Visibility = Visibility.Hidden;
                    LoadPersonalInfo();
                }
                else
                {
                    MessageBox.Show("فشلت عملية الحذف !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }

            }
            else
                MessageBox.Show("تم الغاء الحذف ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

        }



        //load all data to my account when loaded MyAccountManagementView
        private void LoadPersonalInfo()
        {
            string user_name = Sign_in.User;
            SqlDataReader sdr = user_adapter.Get_PersonalInfo(user_name);
            if (sdr.Read())
            {
                User_id = sdr["UserId"].ToString();
                UserNameField.Text = sdr["UserName"].ToString();
                PasswordField.Text = sdr["Password"].ToString();
                RoleField.Text = sdr["Role"].ToString();
                string imgfile = sdr["PersonalImg"].ToString();
                if (imgfile != string.Empty)
                {
                    // Ckeck if current personalimg still at the same path in db or not 
                    try
                    {
                        MyAccount_PersonalImg.Fill = new ImageBrush(new BitmapImage(new Uri(imgfile)));
                    }
                    catch (System.IO.FileNotFoundException)
                    {
                        MessageBox.Show("فشل تحميل الصورة ربما تم نقل الصورة إلى مسار مختلف !", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
                else
                {
                    // Hidden Delete Button If Pimage Empety in DB
                    DeletePersonalImg_MyAccount.Visibility = Visibility.Hidden;
                    MyAccount_PersonalImg.Fill = new SolidColorBrush(System.Windows.Media.Colors.White);
                }

                EmailField.Text = sdr["Email"].ToString();
            }
            DBHandler.ConnectionDB.CloseConnection();
        }

        //Validation Data in Form 
        private bool Myaccount_Isvalid()
        {
            if (UserNameField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                UserNameField.Focus();
                return false;
            }
            if (PasswordField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PasswordField.Focus();
                return false;
            }
            if (EmailField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                EmailField.Focus();
                return false;
            }
            if (EmailField.Text != string.Empty)
            {
                if (!EmailField.Text.Contains("@") || !EmailField.Text.Contains(".com"))
                {
                    MessageBox.Show("فضلاً أدخل البريد الإلكتروني بالشكل الصحيح", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    EmailField.Focus();
                    return false;
                }
                if (!EmailField.Text.Contains("gmail"))
                {
                    MessageBox.Show("فضلاً أدخل بريد الكتروني gmail ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    EmailField.Focus();
                    return false;
                }
            }
            return true;
        }

    }
}
