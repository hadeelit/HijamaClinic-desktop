using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class PastMedicalHistoryAdapter
    {
        public PastMedicalHistoryAdapter()
        { }

        // Inser New Record PastMedicalHistory table
        public bool Insert_PastMedicalHistory(string BleedingDisorders, string Hypertension, string CardiacDiseases, string Diabetes, string HepAtitis, string Aids, string Cva, string Tb, string Epilepsy, string KidneyDiseases, string PsychiatricDiseases, string Cancer, string Anemia, string Smoking, string Others, string PatientFileNum, string SessionId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into PastMedicalHistory Values (@BleedingDisorders ,@Hypertension ,@CardiacDiseases ,@Diabetes ,@HepAtitis ,@Aids ,@Cva ,@Tb ,@Epilepsy ,@KidneyDiseases ,@PsychiatricDiseases,@Cancer,@Anemia,@Smoking,@Others ,@PatientFileNum ,@SessionId)", ConnectionDB.con);

            if (BleedingDisorders != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@BleedingDisorders", BleedingDisorders));
            else
                cmd.Parameters.Add(new SqlParameter("@BleedingDisorders", DBNull.Value));

            if (Hypertension != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Hypertension", Hypertension));
            else
                cmd.Parameters.Add(new SqlParameter("@Hypertension", DBNull.Value));

            if (CardiacDiseases != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@CardiacDiseases", CardiacDiseases));
            else
                cmd.Parameters.Add(new SqlParameter("@CardiacDiseases", DBNull.Value));

            if (Diabetes != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Diabetes", Diabetes));
            else
                cmd.Parameters.Add(new SqlParameter("@Diabetes", DBNull.Value));

            if (HepAtitis != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@HepAtitis", HepAtitis));
            else
                cmd.Parameters.Add(new SqlParameter("@HepAtitis", DBNull.Value));


            if (Aids != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Aids", Aids));
            else
                cmd.Parameters.Add(new SqlParameter("@Aids", DBNull.Value));


            if (Cva != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Cva", Cva));
            else
                cmd.Parameters.Add(new SqlParameter("@Cva", DBNull.Value));


            if (Tb != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Tb", Tb));
            else
                cmd.Parameters.Add(new SqlParameter("@Tb", DBNull.Value));


            if (Epilepsy != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Epilepsy", Epilepsy));
            else
                cmd.Parameters.Add(new SqlParameter("@Epilepsy", DBNull.Value));


            if (KidneyDiseases != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@KidneyDiseases", KidneyDiseases));
            else
                cmd.Parameters.Add(new SqlParameter("@KidneyDiseases", DBNull.Value));


            if (PsychiatricDiseases != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PsychiatricDiseases", PsychiatricDiseases));
            else
                cmd.Parameters.Add(new SqlParameter("@PsychiatricDiseases", DBNull.Value));


            if (Cancer != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Cancer", Cancer));
            else
                cmd.Parameters.Add(new SqlParameter("@Cancer", DBNull.Value));


            if (Anemia != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Anemia", Anemia));
            else
                cmd.Parameters.Add(new SqlParameter("@Anemia", DBNull.Value));


            if (Smoking != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Smoking", Smoking));
            else
                cmd.Parameters.Add(new SqlParameter("@Smoking", DBNull.Value));


            if (Others != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Others", Others));
            else
                cmd.Parameters.Add(new SqlParameter("@Others", DBNull.Value));


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

        // Delete record from Only PastMedicalHistory table
        public bool Delete_PastMedicalHistory(string PastMedicalHistoryId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From PastMedicalHistory Where PastMedicalHistoryId = @PastMedicalHistoryId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PastMedicalHistoryId", System.Data.SqlDbType.Int)).Value = PastMedicalHistoryId;

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

        //update record from PastMedicalHistory
        public bool Update_PastMedicalHistory(string PastMedicalHistoryId, string BleedingDisorders, string Hypertension, string CardiacDiseases, string Diabetes, string HepAtitis, string Aids, string Cva, string Tb, string Epilepsy, string KidneyDiseases, string PsychiatricDiseases, string Cancer, string Anemia, string Smoking, string Others)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update PastMedicalHistory Set BleedingDisorders = @BleedingDisorders , Hypertension = @Hypertension , CardiacDiseases = @CardiacDiseases , Diabetes = @Diabetes , HepAtitis = @HepAtitis, Aids = @Aids, Cva = @Cva, Tb = @Tb, Epilepsy = @Epilepsy, KidneyDiseases = @KidneyDiseases , PsychiatricDiseases =@PsychiatricDiseases, Cancer = @Cancer,Anemia = @Anemia, Smoking = @Smoking , Others = @Others  Where PastMedicalHistoryId = @PastMedicalHistoryId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@PastMedicalHistoryId", System.Data.SqlDbType.Int)).Value = PastMedicalHistoryId;

            if (BleedingDisorders != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@BleedingDisorders", BleedingDisorders));
            else
                cmd.Parameters.Add(new SqlParameter("@BleedingDisorders", DBNull.Value));

            if (Hypertension != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Hypertension", Hypertension));
            else
                cmd.Parameters.Add(new SqlParameter("@Hypertension", DBNull.Value));

            if (CardiacDiseases != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@CardiacDiseases", CardiacDiseases));
            else
                cmd.Parameters.Add(new SqlParameter("@CardiacDiseases", DBNull.Value));

            if (Diabetes != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Diabetes", Diabetes));
            else
                cmd.Parameters.Add(new SqlParameter("@Diabetes", DBNull.Value));

            if (HepAtitis != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@HepAtitis", HepAtitis));
            else
                cmd.Parameters.Add(new SqlParameter("@HepAtitis", DBNull.Value));


            if (Aids != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Aids", Aids));
            else
                cmd.Parameters.Add(new SqlParameter("@Aids", DBNull.Value));


            if (Cva != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Cva", Cva));
            else
                cmd.Parameters.Add(new SqlParameter("@Cva", DBNull.Value));


            if (Tb != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Tb", Tb));
            else
                cmd.Parameters.Add(new SqlParameter("@Tb", DBNull.Value));


            if (Epilepsy != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Epilepsy", Epilepsy));
            else
                cmd.Parameters.Add(new SqlParameter("@Epilepsy", DBNull.Value));


            if (KidneyDiseases != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@KidneyDiseases", KidneyDiseases));
            else
                cmd.Parameters.Add(new SqlParameter("@KidneyDiseases", DBNull.Value));


            if (PsychiatricDiseases != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PsychiatricDiseases", PsychiatricDiseases));
            else
                cmd.Parameters.Add(new SqlParameter("@PsychiatricDiseases", DBNull.Value));


            if (Cancer != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Cancer", Cancer));
            else
                cmd.Parameters.Add(new SqlParameter("@Cancer", DBNull.Value));


            if (Anemia != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Anemia", Anemia));
            else
                cmd.Parameters.Add(new SqlParameter("@Anemia", DBNull.Value));


            if (Smoking != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Smoking", Smoking));
            else
                cmd.Parameters.Add(new SqlParameter("@Smoking", DBNull.Value));


            if (Others != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Others", Others));
            else
                cmd.Parameters.Add(new SqlParameter("@Others", DBNull.Value));


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

        // Get All Records From PastMedicalHistory
        public SqlDataReader Get_PastMedicalHistories()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From PastMedicalHistory", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Records From PastMedicalHistory for specefic session 
        public SqlDataReader Get_PastMedicalHistory(string file_num, string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionId,PastMedicalHistoryId,BleedingDisorders,Hypertension,CardiacDiseases,Diabetes,HepAtitis,Aids,Cva,Tb,Epilepsy,KidneyDiseases,PsychiatricDiseases,Cancer,Anemia,Smoking,Others  From PastMedicalHistory where PatientFileNum = @PatientFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record from PastMedicalHistory table
        public void Edit_Identity_PastMedicalHistory()
        {
            SqlDataReader sqlData = Get_PastMedicalHistories();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (PastMedicalHistory, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(PastMedicalHistoryId)  From PastMedicalHistory", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (PastMedicalHistory, RESEED, @PastMedicalHistoryId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@PastMedicalHistoryId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }
    }
}
