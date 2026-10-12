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
    /// Interaction logic for SendEmailView.xaml
    /// </summary>
    public partial class SendEmailView : UserControl
    {
        public SendEmailView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_Send_Email();
        }

        //Functions for Send Email View ......
        private void UploadFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFile = new Microsoft.Win32.OpenFileDialog();
            openFile.Title = "ارفق الملف المطلوب";
            openFile.Filter = "All files(*.*) | *.*";
            if (openFile.ShowDialog() == true)
            {
                FileName.Text = openFile.FileName;
            }
        }

        // Send Email
        private void Send_Email_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Send_Email_IsValid())
                {
                    Mail();
                }
            }
            catch (System.Net.Mail.SmtpException)
            {
                MessageBox.Show("لم تتم عملية الارسال يبدو أن  بريد المرسل   بحاجة إلى تفعيل خاصية    Less secure app access  في حساب البريد الإلكتروني ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                MessageBox.Show("إن كانت الخاصية مفعلة فضلا تأكد من البريد المرسل أو كلمة المرور", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }


        }

        private void Mail()
        {
            MailMessage message = new MailMessage();
            // Create Protocol To send This Message to Reciver
            SmtpClient smtp = new SmtpClient();
            // Create properties for this message
            message.To.Add(new MailAddress(ReceiverMail.Text));
            message.From = new MailAddress(SenderMail.Text);
            message.Subject = MessageSubject.Text;
            message.Body = MessageContent.Text + "\n";
            if (FileName.Text != string.Empty)
            {
                message.Attachments.Add(new Attachment(FileName.Text));
            }

            smtp.Port = 587;
            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(SenderMail.Text, SenderPass.Text);
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            // Send This Message Using Smtp
            smtp.Send(message);
            MessageBox.Show("تم ارسال البريد الإلكتروني بنجاح", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

        }

        private void Clear_Fields_Click(object sender, RoutedEventArgs e)
        {
            Clear_Send_Email();
        }

        //Email validation
        private bool Send_Email_IsValid()
        {
            if (ReceiverMail.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                ReceiverMail.Focus();
                return false;
            }
            if (!ReceiverMail.Text.Contains("gmail"))
            {
                MessageBox.Show("فضلا أدخل ايميل   Gmail  فقط", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                ReceiverMail.Focus();
                return false;
            }
            if (!ReceiverMail.Text.Contains("@") || !ReceiverMail.Text.Contains(".com"))
            {
                MessageBox.Show("فضلا أدخل الايميل بالشكل الصحيح", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                ReceiverMail.Focus();
                return false;
            }
            if (!SenderMail.Text.Contains("gmail"))
            {
                MessageBox.Show("فضلا أدخل ايميل   Gmail  فقط", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                SenderMail.Focus();
                return false;
            }
            if (!SenderMail.Text.Contains("@") || !ReceiverMail.Text.Contains(".com"))
            {
                MessageBox.Show("فضلا أدخل الايميل بالشكل الصحيح", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                SenderMail.Focus();
                return false;
            }
            if (SenderMail.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                SenderMail.Focus();
                return false;
            }
            if (SenderPass.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                SenderPass.Focus();
                return false;
            }
            if (MessageSubject.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                MessageSubject.Focus();
                return false;
            }
            if (MessageContent.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                MessageContent.Focus();
                return false;
            }

            return true;
        }

        //Clear Function
        private void Clear_Send_Email()
        {
            ReceiverMail.Clear();
            SenderMail.Clear();
            SenderPass.Clear();
            MessageContent.Clear();
            MessageSubject.Clear();
            FileName.Text = "";
        }
    }
}
