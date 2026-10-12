using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class PatientAdapter
    {
        public PatientAdapter()
        { }

        // Insert New Record Of Patient File
        public bool Insert_Patient(string p_name, string file_date, string national_num, string contact_num, string age, string gender, string marital_status, string address, string job, string job_address, string email, string pnameforemergency, string contact_numemergency)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into Patient Values (@PatientName ,@FileDate ,@NationalNum ,@ContactNum ,@Age ,@Gender ,@MaritalStatus ,@Address ,@Job ,@JobAddress ,@Email ,@PnameForEmergency ,@ContactNumForEmergency)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@PatientName", p_name));
            cmd.Parameters.Add(new SqlParameter("@FileDate", System.Data.SqlDbType.Date)).Value = file_date;
            cmd.Parameters.Add(new SqlParameter("@ContactNum", contact_num));
            if (national_num != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@NationalNum", national_num));
            else
                cmd.Parameters.Add(new SqlParameter("@NationalNum", DBNull.Value));

            if (age != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Age", System.Data.SqlDbType.Int)).Value = age;
            else
                cmd.Parameters.Add(new SqlParameter("@Age", DBNull.Value));

            if (gender != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Gender", gender));
            else
                cmd.Parameters.Add(new SqlParameter("@Gender", DBNull.Value));

            if (marital_status != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@MaritalStatus", marital_status));
            else
                cmd.Parameters.Add(new SqlParameter("@MaritalStatus", DBNull.Value));

            if (address != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Address", address));
            else
                cmd.Parameters.Add(new SqlParameter("@Address", DBNull.Value));

            if (job != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Job", job));
            else
                cmd.Parameters.Add(new SqlParameter("@Job", DBNull.Value));

            if (job_address != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@JobAddress", job_address));
            else
                cmd.Parameters.Add(new SqlParameter("@JobAddress", DBNull.Value));

            if (email != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Email", email));
            else
                cmd.Parameters.Add(new SqlParameter("@Email", DBNull.Value));

            if (pnameforemergency != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PnameForEmergency", pnameforemergency));
            else
                cmd.Parameters.Add(new SqlParameter("@PnameForEmergency", DBNull.Value));

            if (contact_numemergency != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ContactNumForEmergency", contact_numemergency));
            else
                cmd.Parameters.Add(new SqlParameter("@ContactNumForEmergency", DBNull.Value));


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

        // Delete Patient_File
        public bool Delete_Patient(string fille_num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From Patient Where FileNum = @FileNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@FileNum", System.Data.SqlDbType.Int)).Value = fille_num;

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

        // Update Record Of Patient File
        public bool Update_Patient(string file_num,  string p_name, string file_date, string national_num, string contact_num, string age, string gender, string marital_status, string address, string job, string job_address, string email, string pnameforemergency, string contact_numemergency)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Patient Set PatientName = @PatientName , FileDate = @FileDate , ContactNum = @ContactNum , NationalNum = @NationalNum , Age = @Age , Gender = @Gender, MaritalStatus = @MaritalStatus ,Address = @Address, Job = @Job , JobAddress = @JobAddress , Email = @Email ,PnameForEmergency = @PnameForEmergency ,ContactNumForEmergency = @ContactNumForEmergency   Where FileNum = @FileNum", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@FileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@PatientName", p_name));
            cmd.Parameters.Add(new SqlParameter("@FileDate", System.Data.SqlDbType.Date)).Value = file_date;
            cmd.Parameters.Add(new SqlParameter("@ContactNum", contact_num));

            if (national_num != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@NationalNum", national_num));
            else
                cmd.Parameters.Add(new SqlParameter("@NationalNum", DBNull.Value));

            if (age != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Age", System.Data.SqlDbType.Int)).Value = age;
            else
                cmd.Parameters.Add(new SqlParameter("@Age", DBNull.Value));

            if (gender != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Gender", gender));
            else
                cmd.Parameters.Add(new SqlParameter("@Gender", DBNull.Value));

            if (marital_status != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@MaritalStatus", marital_status));
            else
                cmd.Parameters.Add(new SqlParameter("@MaritalStatus", DBNull.Value));

            if (address != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Address", address));
            else
                cmd.Parameters.Add(new SqlParameter("@Address", DBNull.Value));

            if (job != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Job", job));
            else
                cmd.Parameters.Add(new SqlParameter("@Job", DBNull.Value));

            if (job_address != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@JobAddress", job_address));
            else
                cmd.Parameters.Add(new SqlParameter("@JobAddress", DBNull.Value));

            if (email != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Email", email));
            else
                cmd.Parameters.Add(new SqlParameter("@Email", DBNull.Value));

            if (pnameforemergency != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PnameForEmergency", pnameforemergency));
            else
                cmd.Parameters.Add(new SqlParameter("@PnameForEmergency", DBNull.Value));

            if (contact_numemergency != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ContactNumForEmergency", contact_numemergency));
            else
                cmd.Parameters.Add(new SqlParameter("@ContactNumForEmergency", DBNull.Value));

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

        // Get specefic Patient based on Filenum
        public SqlDataReader Get_Patient_Filenum(string file_num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Patient where FileNum = @FileNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@FileNum", System.Data.SqlDbType.Int)).Value = file_num;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get specefic Patient based on contact
        public SqlDataReader Get_Patient_Contactnum(string contact_num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Patient where ContactNum = @ContactNum", ConnectionDB.con);
            //cmd.Parameters.Add(new SqlParameter("@ContactNum", System.Data.SqlDbType.Int)).Value = contact_num;
            cmd.Parameters.Add(new SqlParameter("@ContactNum", contact_num));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //  Get specefic Patients Records based on name
        public SqlDataReader Get_Patients_Name(string patient_name)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Patient where PatientName = @PatientName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientName", patient_name));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //  Get specefic Patients Records based on national num
        public SqlDataReader Get_Patients_NationalNum(string national_num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Patient where NationalNum = @NationalNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@NationalNum", national_num));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Patients
        public SqlDataReader Get_Patients()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Patient ORDER BY FileNum DESC", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //Check If this file already Exist or not
        public bool Check_ExistFile(string patient_name)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Patient where PatientName = @PatientName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientName", patient_name));
            SqlDataReader sdr = cmd.ExecuteReader();
            if (sdr.HasRows)
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

        // Query For Adjus Identity After Delete Record
        public void Edit_Identity()
        {
            SqlDataReader sqlData = Get_Patients();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Patient, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(FileNum)  From Patient", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Patient, RESEED, @FileNum)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@FileNum", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

        //Query for count of all patients using in HomeContentView
        public int Count_Of_patients()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Count(*) From Patient", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            sdr.Close();
            int count_patients = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return count_patients;
        }

    }
}
