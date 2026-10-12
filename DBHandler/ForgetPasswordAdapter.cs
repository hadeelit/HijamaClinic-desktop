using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HijamaClinicManagement.DBHandler
{
    class ForgetPasswordAdapter
    {
        public ForgetPasswordAdapter()
        { 
        }

        // Function for Check if User Email Is Founded In DB Or not
        public bool Check_UserEmail(string UserEmail)
        {
            // Check Status of Connection With DB
            ConnectionDB.CheckConnection();
            // Execute Query on User Table
            SqlCommand cmd = new SqlCommand("Select Email From Users Where Email = @Email", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("Email", UserEmail));
            // Execute this Query
            SqlDataReader sdr = cmd.ExecuteReader();

            if (sdr.Read())
            {
                // If UserEmail Is Found in DB
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                // If UserEmail Is Not Found in DB
                ConnectionDB.CloseConnection();
                return false;
            }
          
        }

        // Function For Update Password 
        public bool Update_NewPass(string NewPass ,string UserEmail)
        {
            ConnectionDB.CheckConnection();

            SqlCommand cmd = new SqlCommand("Update Users Set Password = @Password Where Email = @Email", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("Password", NewPass));
            cmd.Parameters.Add(new SqlParameter("Email",UserEmail));

           int affectedr = cmd.ExecuteNonQuery();
           
            if (affectedr>0)
            {

                // If NewPass updated in DB
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            { //If NewPass Not updated in DB
                ConnectionDB.CloseConnection();
                return false;
            }

        }
    }
}
