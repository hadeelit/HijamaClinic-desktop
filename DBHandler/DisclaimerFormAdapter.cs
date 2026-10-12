using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace HijamaClinicManagement.DBHandler
{
    class DisclaimerFormAdapter
    {
        public DisclaimerFormAdapter()
        { }
        // Insert DisclaimerForm
        public bool Insert_DisclaimerForm(string PrintImg)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into DisclaimerForm Values (@PrintImg)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@PrintImg", PrintImg));

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

        //Delete DisclaimerForm
        public bool Delete_DisclaimerForm(string DisclaimerFormId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From DisclaimerForm Where DisclaimerFormId = @DisclaimerFormId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@DisclaimerFormId", System.Data.SqlDbType.Int)).Value = DisclaimerFormId;

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

        // Get All Records From DisclaimerForm
        public SqlDataReader Get_DisclaimerForms()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From DisclaimerForm", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Records From DisclaimerForm for specefic session 
        public SqlDataReader Get_DisclaimerForm(string file_num, string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionId,DisclaimerFormId,PrintImg From DisclaimerForm where PatientFileNum = @PatientFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record from DisclaimerForm table
        public void Edit_Identity_DisclaimerForm()
        {
            SqlDataReader sqlData = Get_DisclaimerForms();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (DisclaimerForm, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(DisclaimerFormId)  From DisclaimerForm", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (DisclaimerForm, RESEED, @DisclaimerFormId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@DisclaimerFormId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }
    }
}
