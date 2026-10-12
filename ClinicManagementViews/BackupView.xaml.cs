using ClosedXML.Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Mail;
using System.Net;
using Microsoft.Win32;

namespace HijamaClinicManagement.ClinicManagementViews
{
    /// <summary>
    /// Interaction logic for BackupView.xaml
    /// </summary>
    public partial class BackupView : UserControl
    {
        DBHandler.BackupDbAdapter backupDb = new DBHandler.BackupDbAdapter();

        public BackupView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_FileuploadBox();
        }

        //Functions For Backup View ...
        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FileLocation.Text = folderBrowser.SelectedPath;
                Backup.IsEnabled = true;
            }
        }

        // Backup  DB View
        private void Backup_Click(object sender, RoutedEventArgs e)
        {
            if (FileLocation.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                FileLocation.Focus();
                return;
            }
            else
            {
                //CloseIcon.IsEnabled = false;
                //RightSideMenu.Visibility = Visibility.Hidden;
                try
                {
                    MessageBox.Show(" فضلاً لا تغادر النافذة حتى تنتهي النسخة الاحتياطية", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    backupDb.Create_BackupFile(FileLocation.Text.Trim());
                   // CloseIcon.IsEnabled = true;
                   // RightSideMenu.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                   // MessageBox.Show(" لم يتم أخد النسخة الاحتياطية بنجاح ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
        }

        private void Clear_FileuploadBox()
        {
            FileLocation.Text = string.Empty;
        }

    }
}
