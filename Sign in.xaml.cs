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
using System.Windows.Shapes;

namespace HijamaClinicManagement
{
    /// <summary>
    /// Interaction logic for Sign_in.xaml
    /// </summary>
    public partial class Sign_in : Window
    {
        public bool result;
        DBHandler.LoginAdapter adapter = new DBHandler.LoginAdapter();
        //Create Login User
        public static string User;

        public Sign_in()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }

        //Exit from App
        private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        //Show and hide checkbox during passswordbox
        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordValue.Password.Length > 0)
            {
                ShowHidPass.Visibility = Visibility.Visible;
            }
            else
            {
                ShowHidPass.Visibility = Visibility.Hidden;
            }
        }

        //Show and hide checkbox during textbox
        private void ShowPasswordUnmask_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ShowPasswordUnmask.Text.Length > 0)
            {
                ShowHidPass.Visibility = Visibility.Visible;
            }
            else
                ShowHidPass.Visibility = Visibility.Hidden;
        }

        //Unmask password inside the passwordBox
        private void ShowHidPass_Checked(object sender, RoutedEventArgs e)
        {
            ShowPasswordUnmask.Visibility = Visibility.Visible;
            sp2.Visibility = Visibility.Visible;
            ShowPasswordUnmask.Text = PasswordValue.Password;
            PasswordValue.Visibility = Visibility.Visible;

        }

        //mask password inside the passwordBox
        private void ShowHidPass_Unchecked(object sender, RoutedEventArgs e)
        {

            PasswordValue.Visibility = Visibility.Visible;
            PasswordValue.Password = ShowPasswordUnmask.Text;
            ShowPasswordUnmask.Visibility = Visibility.Hidden;
            sp2.Visibility = Visibility.Hidden;

        }

        private void ForgetPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
           /* ForgetPassword fp = new ForgetPassword();
            fp.Show();
            Close();*/
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            //Get Login User For Current Session
            User = UserName.Text;

            //Code With DB
            if (IsValid())
            {
                if (ShowHidPass.IsChecked == true)
                {
                    // Call CheckUser Function In LoginAdapter For Execute Query
                    result = adapter.CheckUser(UserName.Text, ShowPasswordUnmask.Text);
                }
                else
                {
                    result = adapter.CheckUser(UserName.Text, PasswordValue.Password);
                }
                if (result)
                {
                    //Open HomeWindow and close this login window
                    HomeWindow home = new HomeWindow();
                    home.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("فضلاً أدخل معلومات صحيحة!", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
        }

        // Finction For Validation Login View Fields
        public bool IsValid()
        {
            if (UserName.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                UserName.Focus();
                return false;
            }
            if (PasswordValue.Password == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                PasswordValue.Focus();
                return false;
            }
            return true;
        }

        // Call CheckUser Function In LoginAdapter For Execute Query
        private void UserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            // Call CheckUser Function In LoginAdapter For Execute Query
            result = adapter.CheckOnlyUser(UserName.Text);
            if(result)
            {
                PasswordValue.Focus();
            }
        }

        //Check Input Language
        private void UserName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //Check Input Language
            string[] alpha = { "A","B","C","D","E","F","G","H","I","J","K","L","M","N","O","P","Q","R","S","T","U","V","W","X","Y","Z"};
            //string input = UserName.Text;
            string ch = e.Text[0].ToString();
            for (int i = 0; i < alpha.Length; i++)
            {
                if (string.Equals(ch, alpha[i], StringComparison.OrdinalIgnoreCase))
                {
                    UserName.TextAlignment = TextAlignment.Right;
                    return;
                }
            }
            UserName.TextAlignment = TextAlignment.Left;
        }

    }
}
