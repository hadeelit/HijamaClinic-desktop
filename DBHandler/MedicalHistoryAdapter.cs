using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
namespace HijamaClinicManagement.DBHandler
{
    class MedicalHistoryAdapter
    {
        public MedicalHistoryAdapter()
        { }

        // Insert New record of MedicalHistory
        public bool Insert_MedicalHistory(string MainSymptoms, string OnsetOfPain, string LocationOfPain, string DurationOfPain, string CharacterOfPain, string AggravatingAndRelievingFactorsOnPain, string Timing, string Severity, string Allergies, string Medications, string PastSurgicalOrCuppingHistory,string AnotherComplaints,string ConsultationWithDoctor, string PatientFileNum, string SessionId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into MedicalHistory Values (@MainSymptoms ,@OnsetOfPain ,@LocationOfPain ,@DurationOfPain ,@CharacterOfPain ,@AggravatingAndRelievingFactorsOnPain ,@Timing ,@Severity ,@Allergies ,@Medications ,@PastSurgicalOrCuppingHistory,@AnotherComplaints,@ConsultationWithDoctor ,@PatienFileNum ,@SessionId)", ConnectionDB.con);

            if (MainSymptoms != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@MainSymptoms", MainSymptoms));
            else
                cmd.Parameters.Add(new SqlParameter("@MainSymptoms", DBNull.Value));

            if (OnsetOfPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@OnsetOfPain", OnsetOfPain));
            else
                cmd.Parameters.Add(new SqlParameter("@OnsetOfPain", DBNull.Value));

            if (LocationOfPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@LocationOfPain", LocationOfPain));
            else
                cmd.Parameters.Add(new SqlParameter("@LocationOfPain", DBNull.Value));

            if (DurationOfPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@DurationOfPain", DurationOfPain));
            else
                cmd.Parameters.Add(new SqlParameter("@DurationOfPain", DBNull.Value));

            if (CharacterOfPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@CharacterOfPain", CharacterOfPain));
            else
                cmd.Parameters.Add(new SqlParameter("@CharacterOfPain", DBNull.Value));


            if (AggravatingAndRelievingFactorsOnPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@AggravatingAndRelievingFactorsOnPain", AggravatingAndRelievingFactorsOnPain));
            else
                cmd.Parameters.Add(new SqlParameter("@AggravatingAndRelievingFactorsOnPain", DBNull.Value));


            if (Timing != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Timing", Timing));
            else
                cmd.Parameters.Add(new SqlParameter("@Timing", DBNull.Value));


            if (Severity != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Severity", Severity));
            else
                cmd.Parameters.Add(new SqlParameter("@Severity", DBNull.Value));


            if (Allergies != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Allergies", Allergies));
            else
                cmd.Parameters.Add(new SqlParameter("@Allergies", DBNull.Value));


            if (Medications != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Medications", Medications));
            else
                cmd.Parameters.Add(new SqlParameter("@Medications", DBNull.Value));


            if (PastSurgicalOrCuppingHistory != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PastSurgicalOrCuppingHistory", PastSurgicalOrCuppingHistory));
            else
                cmd.Parameters.Add(new SqlParameter("@PastSurgicalOrCuppingHistory", DBNull.Value));


            if (AnotherComplaints != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@AnotherComplaints", AnotherComplaints));
            else
                cmd.Parameters.Add(new SqlParameter("@AnotherComplaints", DBNull.Value));


            if (ConsultationWithDoctor != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ConsultationWithDoctor", ConsultationWithDoctor));
            else
                cmd.Parameters.Add(new SqlParameter("@ConsultationWithDoctor", DBNull.Value));


            cmd.Parameters.Add(new SqlParameter("@PatienFileNum", System.Data.SqlDbType.Int)).Value = PatientFileNum;
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

        // Delete Only Record of MedicalHistory  Without Totaly session
        public bool Delete_MedicalHistory(string MedicalHistoryId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From MedicalHistory Where MedicalHistoryId = @MedicalHistoryId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@MedicalHistoryId", System.Data.SqlDbType.Int)).Value = MedicalHistoryId;

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

        //Update Record of MedicalHistory table
        public bool Update_MedicalHistory(string MedicalHistoryId, string MainSymptoms, string OnsetOfPain, string LocationOfPain, string DurationOfPain, string CharacterOfPain, string AggravatingAndRelievingFactorsOnPain, string Timing, string Severity, string Allergies, string Medications, string PastSurgicalOrCuppingHistory, string AnotherComplaints, string ConsultationWithDoctor)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update MedicalHistory Set MainSymptoms = @MainSymptoms , OnsetOfPain = @OnsetOfPain , LocationOfPain = @LocationOfPain , DurationOfPain = @DurationOfPain , CharacterOfPain = @CharacterOfPain, AggravatingAndRelievingFactorsOnPain = @AggravatingAndRelievingFactorsOnPain, Timing = @Timing, Severity = @Severity, Allergies = @Allergies, Medications = @Medications , PastSurgicalOrCuppingHistory =@PastSurgicalOrCuppingHistory, AnotherComplaints = @AnotherComplaints,ConsultationWithDoctor=@ConsultationWithDoctor  Where MedicalHistoryId = @MedicalHistoryId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@MedicalHistoryId", System.Data.SqlDbType.Int)).Value = MedicalHistoryId;

            if (MainSymptoms != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@MainSymptoms", MainSymptoms));
            else
                cmd.Parameters.Add(new SqlParameter("@MainSymptoms", DBNull.Value));

            if (OnsetOfPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@OnsetOfPain", OnsetOfPain));
            else
                cmd.Parameters.Add(new SqlParameter("@OnsetOfPain", DBNull.Value));

            if (LocationOfPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@LocationOfPain", LocationOfPain));
            else
                cmd.Parameters.Add(new SqlParameter("@LocationOfPain", DBNull.Value));

            if (DurationOfPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@DurationOfPain", DurationOfPain));
            else
                cmd.Parameters.Add(new SqlParameter("@DurationOfPain", DBNull.Value));

            if (CharacterOfPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@CharacterOfPain", CharacterOfPain));
            else
                cmd.Parameters.Add(new SqlParameter("@CharacterOfPain", DBNull.Value));


            if (AggravatingAndRelievingFactorsOnPain != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@AggravatingAndRelievingFactorsOnPain", AggravatingAndRelievingFactorsOnPain));
            else
                cmd.Parameters.Add(new SqlParameter("@AggravatingAndRelievingFactorsOnPain", DBNull.Value));


            if (Timing != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Timing", Timing));
            else
                cmd.Parameters.Add(new SqlParameter("@Timing", DBNull.Value));


            if (Severity != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Severity", Severity));
            else
                cmd.Parameters.Add(new SqlParameter("@Severity", DBNull.Value));


            if (Allergies != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Allergies", Allergies));
            else
                cmd.Parameters.Add(new SqlParameter("@Allergies", DBNull.Value));


            if (Medications != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Medications", Medications));
            else
                cmd.Parameters.Add(new SqlParameter("@Medications", DBNull.Value));


            if (PastSurgicalOrCuppingHistory != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PastSurgicalOrCuppingHistory", PastSurgicalOrCuppingHistory));
            else
                cmd.Parameters.Add(new SqlParameter("@PastSurgicalOrCuppingHistory", DBNull.Value));


            if (AnotherComplaints != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@AnotherComplaints", AnotherComplaints));
            else
                cmd.Parameters.Add(new SqlParameter("@AnotherComplaints", DBNull.Value));


            if (ConsultationWithDoctor != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ConsultationWithDoctor", ConsultationWithDoctor));
            else
                cmd.Parameters.Add(new SqlParameter("@ConsultationWithDoctor", DBNull.Value));

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

        // Get All Records From MedicalHistory
        public SqlDataReader Get_MedicalHistories()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From MedicalHistory", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Records From MedicalHistory for specefic session 
        public SqlDataReader Get_MedicalHistory(string file_num, string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionId,MedicalHistoryId,MainSymptoms,OnsetOfPain,LocationOfPain,DurationOfPain,CharacterOfPain,AggravatingAndRelievingFactorsOnPain,Timing,Severity,Allergies,Medications,PastSurgicalOrCuppingHistory,AnotherComplaints,ConsultationWithDoctor  From MedicalHistory where PatienFileNum = @PatienFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatienFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record from MedicalHistory table
        public void Edit_Identity_MedicalHistory()
        {
            SqlDataReader sqlData = Get_MedicalHistories();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (MedicalHistory, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(MedicalHistoryId)  From MedicalHistory", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (MedicalHistory, RESEED, @MedicalHistoryId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@MedicalHistoryId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

    }
}
