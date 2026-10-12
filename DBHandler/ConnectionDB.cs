using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HijamaClinicManagement.DBHandler
{
    class ConnectionDB
    {
        public static SqlConnection con;

        public static void CheckConnection()
        {
            if (con == null)
            {
                
                // new connection after formatting computer lamsat shifaa
                //con = new SqlConnection("Data Source=DESKTOP-FC0UDIL\\SQLEXPRESS;Initial Catalog=HijamaCenterDb;Integrated Security=True");
                
                //connection sql labtop
                // con = new SqlConnection("Data Source=HADEEL;Initial Catalog=HijamaCenterDb;Integrated Security=True");
                //connection sql desktop
                con = new SqlConnection("Data Source=DESKTOP-IA1BPMR;Initial Catalog=HijamaCenterDb;Integrated Security=True");
                con.Open();
            }
            else
            {
                con.Open();
            }
        }

        // Close Connection With DB
        public static void CloseConnection()
        {
            con.Close();
        }
    }
}
