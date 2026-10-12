using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Data;
using System.Xml;
using Newtonsoft.Json;
using System.Windows;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class SmsAdapter
    {
        // Function for Send SMS to any Number within Taqnyat.sa after print patient fill
        public void SendSms_Process(string Number, string Patient_Name, string Patient_file)
        {
            Taqnyat.Taqnyat taqnyt = new Taqnyat.Taqnyat();
            string bearer = "dc2b4f26021bb5ea3cb447ff92043734";
            string body =  "تعليمات مابعد الحجامة ......"+ "نشكرك لزيارة لمسة شفاء" + Patient_file + "برقم الملف" + Patient_Name;
            string recipients = Number;
            string sender = "Taqnyat.sa";

            int check_current_balance = Check_Balance();
            if(check_current_balance == 0)
                MessageBox.Show("يرجى إعادة شحن الرصيد ... الرصيد الحالي ريال فقط", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK,
                MessageBoxOptions.RtlReading);
            else
            {
                string message = taqnyt.SendMessage(bearer, recipients, sender, body);
                //check result from status code ???
            }      
        }

        // Function for Send SMS to any Number that callback within Boadcast_Sms function
        public void SendSms_Process(string Number, string Message)
        {
            Taqnyat.Taqnyat taqnyt = new Taqnyat.Taqnyat();
            string bearer = "dc2b4f26021bb5ea3cb447ff92043734";
            string body = Message;
            string recipients = Number;
            string sender = "Taqnyat.sa";

            int check_current_balance = Check_Balance();
            if (check_current_balance == 0)
                MessageBox.Show("يرجى إعادة شحن الرصيد ... الرصيد الحالي ريال فقط", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK,
                MessageBoxOptions.RtlReading);
            else
            {
                string message = taqnyt.SendMessage(bearer, recipients, sender, body);
                //check result from status code
            }
        }

        // Function for Get account balance
        public string Get_Balance()
        {
            Taqnyat.Taqnyat taqnyt = new Taqnyat.Taqnyat();
            string bearer = "dc2b4f26021bb5ea3cb447ff92043734";
            string balance = taqnyt.GetBalance(bearer);

            // MessageBox.Show(balance, "اختبار", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            //check result from status code
            return balance;
        }

        // Function for Check account balance
        public int Check_Balance()
        {
            string current_balance = Get_Balance();
            if (current_balance.Equals("1"))
                // end of balance
                return 0;
            else
            {
                // still there is balance
                return 1;
            }
            
        }

        // Function for Send SMS to all Patients in db at specefic days (Sunnah days & National day & others)
        public void Boadcast_Sms(string body)
        {
            SqlDataReader sdr = Patients_Phone_Nums();
            DataTable dt = new DataTable();

            if(sdr.HasRows)
            {
                dt.Load(sdr);
                ConnectionDB.CloseConnection();
                foreach (DataRow row in dt.Rows)
                {
                    string num = row.Field<string>("ContactNum");
                    SendSms_Process(num, body);
                }
            }
            else
            {
                ConnectionDB.CloseConnection();
                MessageBox.Show("يرجى إضافة ملف مريض على الأقل لايوجد ملفات بعد ... لم تتم عملية الإرسال", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK,
              MessageBoxOptions.RtlReading);
            }
        }

        // Function for get all patients phone numbers from DB
        public SqlDataReader Patients_Phone_Nums()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select ContactNum from Patient", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Function For Check Connection ......   not used
        public bool Check_Connection()
        {
            WebClient client = new WebClient();
            try
            {
                using (client.OpenRead("http://www.google.com"))
                { 
                }
                return true;

            }
            catch (WebException)
            {
                return false;
            }
        }

       
    }
}
