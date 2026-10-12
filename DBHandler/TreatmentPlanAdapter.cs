using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class TreatmentPlanAdapter
    {
        public TreatmentPlanAdapter()
        { }

        //Insert TreatmentPlan New Record 
        public bool Insert_TreatmentPlan(string Date, string NumberOfSuggestedVisits, string DryCupping, string BloodyCupping, string SlidingCupping, string FlashingCupping, 
            string DurationBetweenCupping, string DateOfNextVisit, string CuppingPointsInSession, string PatientFileNum, string SessionId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into TreatmentPlan Values (@Date ,@NumberOfSuggestedVisits ,@DryCupping ,@BloodyCupping ,@SlidingCupping,@FlashingCupping,@DurationBetweenCupping,@DateOfNextVisit,@CuppingPointsInSession,@PatientFileNum,@SessionId)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@Date",System.Data.SqlDbType.Date)).Value = Date;

            if (NumberOfSuggestedVisits != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@NumberOfSuggestedVisits", System.Data.SqlDbType.Int)).Value  = NumberOfSuggestedVisits;
            else
                cmd.Parameters.Add(new SqlParameter("@NumberOfSuggestedVisits", DBNull.Value));

            if (DryCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@DryCupping", System.Data.SqlDbType.Int)).Value = DryCupping;
            else
                cmd.Parameters.Add(new SqlParameter("@DryCupping", DBNull.Value));

            if (BloodyCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@BloodyCupping", System.Data.SqlDbType.Int)).Value = BloodyCupping;
            else
                cmd.Parameters.Add(new SqlParameter("@BloodyCupping", DBNull.Value));

            if (SlidingCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@SlidingCupping", System.Data.SqlDbType.Int)).Value = SlidingCupping;
            else
                cmd.Parameters.Add(new SqlParameter("@SlidingCupping", DBNull.Value));

            if (FlashingCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@FlashingCupping", System.Data.SqlDbType.Int)).Value = FlashingCupping;
            else
                cmd.Parameters.Add(new SqlParameter("@FlashingCupping", DBNull.Value));

            if (DurationBetweenCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@DurationBetweenCupping", DurationBetweenCupping));
            else
                cmd.Parameters.Add(new SqlParameter("@DurationBetweenCupping", DBNull.Value));

            if (DateOfNextVisit != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@DateOfNextVisit", System.Data.SqlDbType.Date)).Value = DateOfNextVisit;
            else
                cmd.Parameters.Add(new SqlParameter("@DateOfNextVisit", DBNull.Value));

            if (CuppingPointsInSession != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@CuppingPointsInSession", CuppingPointsInSession));
            else
                cmd.Parameters.Add(new SqlParameter("@CuppingPointsInSession", DBNull.Value));

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

        // Delete Specefic Record from TreatmentPlan without Delete Session 
        public bool Delete_TreatmentPlan(string TreatmentPlanId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From TreatmentPlan Where TreatmentPlanId = @TreatmentPlanId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@TreatmentPlanId", System.Data.SqlDbType.Int)).Value = TreatmentPlanId;

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

        // Update Record From TreatmentPlan
        public bool Update_TreatmentPlan(string TreatmentPlanId, string Date, string NumberOfSuggestedVisits, string DryCupping, string BloodyCupping, string SlidingCupping, 
            string FlashingCupping, string DurationBetweenCupping, string DateOfNextVisit, string CuppingPointsInSession)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update TreatmentPlan Set Date = @Date , NumberOfSuggestedVisits = @NumberOfSuggestedVisits , DryCupping = @DryCupping , BloodyCupping = @BloodyCupping , SlidingCupping = @SlidingCupping, FlashingCupping = @FlashingCupping, DurationBetweenCupping = @DurationBetweenCupping, DateOfNextVisit = @DateOfNextVisit ,CuppingPointsInSession = @CuppingPointsInSession Where TreatmentPlanId = @TreatmentPlanId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@TreatmentPlanId", System.Data.SqlDbType.Int)).Value = TreatmentPlanId;

            cmd.Parameters.Add(new SqlParameter("@Date", System.Data.SqlDbType.Date)).Value = Date;

            if (NumberOfSuggestedVisits != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@NumberOfSuggestedVisits", System.Data.SqlDbType.Int)).Value = NumberOfSuggestedVisits;
            else
                cmd.Parameters.Add(new SqlParameter("@NumberOfSuggestedVisits", DBNull.Value));

            if (DryCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@DryCupping", System.Data.SqlDbType.Int)).Value = DryCupping;
            else
                cmd.Parameters.Add(new SqlParameter("@DryCupping", DBNull.Value));

            if (BloodyCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@BloodyCupping", System.Data.SqlDbType.Int)).Value = BloodyCupping;
            else
                cmd.Parameters.Add(new SqlParameter("@BloodyCupping", DBNull.Value));

            if (SlidingCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@SlidingCupping", System.Data.SqlDbType.Int)).Value = SlidingCupping;
            else
                cmd.Parameters.Add(new SqlParameter("@SlidingCupping", DBNull.Value));

            if (FlashingCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@FlashingCupping", System.Data.SqlDbType.Int)).Value = FlashingCupping;
            else
                cmd.Parameters.Add(new SqlParameter("@FlashingCupping", DBNull.Value));

            if (DurationBetweenCupping != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@DurationBetweenCupping", DurationBetweenCupping));
            else
                cmd.Parameters.Add(new SqlParameter("@DurationBetweenCupping", DBNull.Value));

            if (DateOfNextVisit != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@DateOfNextVisit", System.Data.SqlDbType.Date)).Value = DateOfNextVisit;
            else
                cmd.Parameters.Add(new SqlParameter("@DateOfNextVisit", DBNull.Value));

            if (CuppingPointsInSession != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@CuppingPointsInSession", CuppingPointsInSession));
            else
                cmd.Parameters.Add(new SqlParameter("@CuppingPointsInSession", DBNull.Value));

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

        //Get All TreatmentPlans Rcords 
        public SqlDataReader Get_AllTreatmentPlan()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From TreatmentPlan", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get  Record From TreatmentPlan for specefic session 
        public SqlDataReader Get_TreatmentPlan(string file_num, string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select TreatmentPlanId,Date,NumberOfSuggestedVisits,DryCupping,BloodyCupping,SlidingCupping,FlashingCupping,DurationBetweenCupping,DateOfNextVisit,CuppingPointsInSession  From TreatmentPlan where PatientFileNum = @PatientFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get Last Record Added To TreatmentPlan Table
        public int Get_MaxTreatmentPlanId()
        {
            SqlDataReader sqlData = Get_AllTreatmentPlan();
            if (sqlData.HasRows)
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(TreatmentPlanId)  From TreatmentPlan", ConnectionDB.con);
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

        // Get CuppingPointsInSession Image path from TreatmentPlan Table 
        public SqlDataReader Get_ImgPath(string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select TreatmentPlanId,CuppingPointsInSession  From TreatmentPlan where SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //get Id_Bill from specefic filenum & session id
        public int Get_Bill_Id(string file_num, string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select BillId From Bill where PatientFileNum = @PatientFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;
            SqlDataReader sdr = cmd.ExecuteReader();
            //No Bill for this sessionid yet
            if (sdr.HasRows == false)
            {
                ConnectionDB.CloseConnection();
                return 0;
            }
            else
            {
                sdr.Close();
                int bill_id = Convert.ToInt32(cmd.ExecuteScalar());
                ConnectionDB.CloseConnection();
                return bill_id;
            }
        }

        // Query For Adjus Identity After Delete Record from TreatmentPlan table
        public void Edit_Identity_TreatmentPlan()
        {
            SqlDataReader sqlData = Get_AllTreatmentPlan();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (TreatmentPlan, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(TreatmentPlanId)  From TreatmentPlan", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (TreatmentPlan, RESEED, @TreatmentPlanId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@TreatmentPlanId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }


    }
}
