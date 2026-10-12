using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Data;

namespace HijamaClinicManagement.DBHandler
{
    class UsersAdapter
    {
        public UsersAdapter()
        { 
        }

        // ....... Functions For MyAccountManagement View
        // Function For Edit MyAccount
        public bool Update_Myaccount(string id, string user_name, string pass, string personal_img,string email)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Users Set UserName = @UserName , Password = @Password , PersonalImg = @PersonalImg , Email = @Email Where UserId = @UserId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@UserId", System.Data.SqlDbType.Int)).Value = id;
            cmd.Parameters.Add(new SqlParameter("@UserName", user_name));
            cmd.Parameters.Add(new SqlParameter("@Password", pass));

            if(personal_img != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PersonalImg", personal_img));
            else
                cmd.Parameters.Add(new SqlParameter("@PersonalImg", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@Email",email));

            int affect_r = cmd.ExecuteNonQuery();
            if (affect_r > 0)
            {
                // If NewRecord updated in DB
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                // If NewRecord  Not updated in DB
                ConnectionDB.CloseConnection();
                return false;
            }
        }

        // Function For get Info for specefic username
        public SqlDataReader Get_PersonalInfo(string username)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Users Where UserName=@UserName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@UserName", username));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Function For Delete My PersonalImg
        public bool Delete_PersonalImg(string id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Users Set PersonalImg = @PersonalImg Where UserId = @UserId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@UserId", System.Data.SqlDbType.Int)).Value = id;
            cmd.Parameters.Add(new SqlParameter("@PersonalImg", DBNull.Value));

            int affect_r = cmd.ExecuteNonQuery();
            if (affect_r > 0)
            {
                // If updated in DB
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                // If Not updated in DB
                ConnectionDB.CloseConnection();
                return false;
            }

        }


        // ....... Functions For AccountsManagement View
        // Check if Username is Founded previously in User Table before added
        public bool Check_UserName(string username)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select UserName From Users Where UserName=@UserName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@UserName", username));
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                ConnectionDB.CloseConnection();
                return false;
            }
        }

        // Check if Username is Founded previously in User Table before added
        public bool Check_UserName(string id, string username)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select UserName From Users Where UserName=@UserName and UserId !=@UserId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@UserName", username));
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)).Value = id;
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.Read())
            {
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                ConnectionDB.CloseConnection();
                return false;
            }
        }


        //Insert New User
        public bool Insert_User(string username, string password, string role, string email)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into Users Values (@UserName ,@Password ,@Role ,@PersonalImg ,@Email)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@UserName", username));
            cmd.Parameters.Add(new SqlParameter("@Password", password));
            cmd.Parameters.Add(new SqlParameter("@Role", role));
            cmd.Parameters.Add(new SqlParameter("@PersonalImg", DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@Email", email));

            int affect_r = cmd.ExecuteNonQuery();
            if (affect_r > 0)
            {
                // If NewRecord Inserted in DB
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                // If NewRecord  Not Inserted in DB
                ConnectionDB.CloseConnection();
                return false;
            }
        }

        // Get All UsersInfo
        public SqlDataReader Get_Users()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Users where UserName != @UserName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@UserName", "Admin"));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Delete Specefic User From Users Table
        public bool Delete_user(string id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From Users Where UserId = @UserId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)).Value = id;

            int affected_r = cmd.ExecuteNonQuery();
            if (affected_r > 0)
            {

                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {

                ConnectionDB.CloseConnection();
                return false;
            }
        }

        //Update UserInfo
        public bool Update_User(string id, string username, string pass, string role, string email)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Users Set UserName = @UserName , Password = @Password , Role = @Role , Email = @Email Where UserId = @UserId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.Int)).Value = id;
            cmd.Parameters.Add(new SqlParameter("@UserName", username));
            cmd.Parameters.Add(new SqlParameter("@Password", pass));
            cmd.Parameters.Add(new SqlParameter("@Role", role));
            cmd.Parameters.Add(new SqlParameter("@Email", email));

            int affect_r = cmd.ExecuteNonQuery();
            if (affect_r > 0)
            {
                // If NewRecord updated in DB
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                // If NewRecord  Not updated in DB
                ConnectionDB.CloseConnection();
                return false;
            }
        }

        // Query For Adjus Identity After Delete Record
        public void Edit_Identity()
        {
            SqlDataReader sqlData = Get_Users();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Usres, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(UserId)  From Users", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Users, RESEED, @UserId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@UserId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

        //Query for count users
        public int Get_Count_Users()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Count(*) From Users", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            sdr.Close();
            int count_users = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return count_users;
        }
    }
}
