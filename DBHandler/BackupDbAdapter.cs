using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows;

namespace HijamaClinicManagement.DBHandler
{
    class BackupDbAdapter
    {
        public BackupDbAdapter()
        { }

        // Create Backup File For DataBase
        public void Create_BackupFile(string filelocation)
        {
            ConnectionDB.CheckConnection();
            string database = ConnectionDB.con.Database.ToString();
            MessageBox.Show("فضلا لاتخرج من النافذة حتى يتم اشعارك  بانتهاء عملية النسخة الاحتياطية ", "تحذير", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            SqlCommand cmd = new SqlCommand("Backup Database " + database + " To Disk = '" + filelocation + "\\" + "HijamaClinicDataBase" + "-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".bak'", ConnectionDB.con);
            int count = cmd.ExecuteNonQuery();
            if(count == -1)
            {
                MessageBox.Show("تم أخد نسخة احتياطية بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                ConnectionDB.CloseConnection();
            }
            else
            {
                MessageBox.Show("لم يتم أخد النسخة الاحتياطية بنجاح ", "فشل", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                ConnectionDB.CloseConnection();
            }
            
        }
    }
}
