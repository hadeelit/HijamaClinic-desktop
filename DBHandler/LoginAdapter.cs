using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace HijamaClinicManagement.DBHandler
{
    class LoginAdapter
    {
      

        public LoginAdapter()
        { 
        }

        // Function For Check If this User in db or not
        public bool CheckUser(string UserN,string UserPass)
        {
            // Check Status of Connection With DB
            ConnectionDB.CheckConnection();
            // Execute Query on User Table
            SqlCommand cmd = new SqlCommand("Select * From Users Where UserName = @UserName and Password = @Password COLLATE SQL_Latin1_General_CP1_CS_AS", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@UserName", UserN));
            cmd.Parameters.Add(new SqlParameter("@Password", UserPass));
            // Execute this Query
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                // If the user is Found in DB
                ConnectionDB.CloseConnection();
                return true;              
            }
            else
            {
                // If the user is Not Found in DB
                ConnectionDB.CloseConnection();
                return false;
            }
               
        }

        // Function For Check If this User in db or not (only User without password)
        public bool CheckOnlyUser(string UserN)
        {
            // Check Status of Connection With DB
            ConnectionDB.CheckConnection();
            // Execute Query on User Table
            SqlCommand cmd = new SqlCommand("Select * From Users Where UserName = @UserName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@UserName", UserN));
            // Execute this Query
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                // If the user is Found in DB
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                // If the user is Not Found in DB
                ConnectionDB.CloseConnection();
                return false;
            }

        }

    }
}
