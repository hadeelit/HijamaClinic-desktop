using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace HijamaClinicManagement.DBHandler
{
    class PainAreasAdapter
    {
        public PainAreasAdapter()
        { 
        }

        // Insert New Record of PainAreas Table
        public bool Insert_PainArea(string imgpath,string PainSeverity, string Diagnosis, string PatientFileNum, string SessionId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into PainAreas Values (@imgpath ,@PainSeverity ,@Diagnosis ,@PatientFileNum ,@SessionId)", ConnectionDB.con);

            if (imgpath != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@imgpath", imgpath));
            else
                cmd.Parameters.Add(new SqlParameter("@imgpath", DBNull.Value));

            if (PainSeverity != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PainSeverity",System.Data.SqlDbType.Int)).Value = PainSeverity;
            else
                cmd.Parameters.Add(new SqlParameter("@PainSeverity", DBNull.Value));

            if (Diagnosis != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Diagnosis", Diagnosis));
            else
                cmd.Parameters.Add(new SqlParameter("@Diagnosis", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = PatientFileNum;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = SessionId;

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

        // Delete Record From PainAreas
        public bool Delete_PainArea(string PainAreaId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From PainAreas Where PainAreaId = @PainAreaId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PainAreaId", System.Data.SqlDbType.Int)).Value = PainAreaId;

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

        // Update Record From PainAreas
        public bool Update_PainAreas(string PainAreaId, string imgpath, string PainSeverity, string Diagnosis)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update PainAreas Set PainPoints = @imgpath , PainSeverity = @PainSeverity , Diagnosis = @Diagnosis  Where PainAreaId = @PainAreaId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@PainAreaId", System.Data.SqlDbType.Int)).Value = PainAreaId;
            if (imgpath != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@imgpath", imgpath));
            else
                cmd.Parameters.Add(new SqlParameter("@imgpath", DBNull.Value));

            if (PainSeverity != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PainSeverity", System.Data.SqlDbType.Int)).Value = PainSeverity;
            else
                cmd.Parameters.Add(new SqlParameter("@PainSeverity", DBNull.Value));

            if (Diagnosis != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Diagnosis", Diagnosis));
            else
                cmd.Parameters.Add(new SqlParameter("@Diagnosis", DBNull.Value));

            int affect_r = cmd.ExecuteNonQuery();
            if (affect_r > 0)
            {
                // If NewRecord Updated in DB
                ConnectionDB.CloseConnection();
                return true;
            }
            else
            {
                // If NewRecord  Not Updated in DB
                ConnectionDB.CloseConnection();
                return false;
            }
        }

        // Get All Records From PainAreas Table
        public SqlDataReader Get_PainAreas()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From PainAreas", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get  Record From PainAreas for specefic session 
        public SqlDataReader Get_PainArea(string file_num, string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select PainAreaId,PainPoints,PainSeverity,Diagnosis  From PainAreas where PatientFileNum = @PatientFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get Last Record Added To PainAreas Table
        public int Get_MaxPainAreasId()
        {
            SqlDataReader sqlData = Get_PainAreas();
            if (sqlData.HasRows)
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(PainAreaId)  From PainAreas", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                sqlData.Close();
                ConnectionDB.CloseConnection();
                return id;
            }
            else
            {
                ConnectionDB.CloseConnection();
                return 0;
            }
        }

        // Get PainAreaImage path from PainAreas Table 
        public SqlDataReader Get_ImgPath(string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select PainAreaId,PainPoints  From PainAreas where SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record from PainAreas table
        public void Edit_Identity_PainAreas()
        {
            SqlDataReader sqlData = Get_PainAreas();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (PainAreas, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(PainAreaId)  From PainAreas", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (PainAreas, RESEED, @PainAreaId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@PainAreaId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }
    }
}
