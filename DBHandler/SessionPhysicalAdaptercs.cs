using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class SessionPhysicalAdaptercs
    {
        public SessionPhysicalAdaptercs()
        { }

        // Get SessionNum for specefic Filenum
        public int Get_SessionNum(string file_num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd1 = new SqlCommand("Select * From Session where PatientFileNum = @PatientFileNum", ConnectionDB.con);
            cmd1.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            SqlDataReader sdr = cmd1.ExecuteReader();
            if(sdr.HasRows == false)
            {
                int num = 1;
                sdr.Close();
                ConnectionDB.CloseConnection();
                return num;
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(SessionNum) From Session where PatientFileNum = @PatientFileNum", ConnectionDB.con);
                cmd2.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
                sdr.Close();
                int num = Convert.ToInt32(cmd2.ExecuteScalar());
                int r_num = num + 1;
                ConnectionDB.CloseConnection();
                return r_num;
            }          
        }

        // Inser Session 
        public bool Insert_Session(string s_num, string s_date, string complaints, string complications, string s_duration, string file_num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into Session Values (@SessionNum ,@SessionDate ,@CurrentCopmlaints ,@Complications ,@SessionDuration ,@PatientFileNum)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@SessionNum", System.Data.SqlDbType.Int)).Value = s_num;
            cmd.Parameters.Add(new SqlParameter("@SessionDate", System.Data.SqlDbType.Date)).Value = s_date;
            if (complaints != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@CurrentCopmlaints", complaints));
            else
                cmd.Parameters.Add(new SqlParameter("@CurrentCopmlaints", DBNull.Value));

            if (complications != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Complications", complications));
            else
                cmd.Parameters.Add(new SqlParameter("@Complications", DBNull.Value));

            if (s_duration != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@SessionDuration", s_duration));
            else
                cmd.Parameters.Add(new SqlParameter("@SessionDuration", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;

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

        // Inser PhysicalExamination Only If Session Inserted
        public bool Insert_PhysicalExamination(string temp, string hr, string rr, string bp, string HeadAndNeck, string ChestAndBack, string Abdomen, string Pelvis, string UpperAndLowerLimbs, string Others, string PatientFileNum, string SessionId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into PhysicalExamination Values (@Temp ,@Hr ,@Rr ,@Bp ,@HeadAndNeck ,@ChestAndBack ,@Abdomen ,@Pelvis ,@UpperAndLowerLimbs ,@Others ,@PatientFileNum ,@SessionId)", ConnectionDB.con);

            if (temp != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Temp", System.Data.SqlDbType.Float)).Value = temp;
            else
                cmd.Parameters.Add(new SqlParameter("@Temp", DBNull.Value));

            if (hr != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Hr", System.Data.SqlDbType.Int)).Value = hr;
            else
                cmd.Parameters.Add(new SqlParameter("@Hr", DBNull.Value));

            if (rr != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Rr", System.Data.SqlDbType.Int)).Value = rr;
            else
                cmd.Parameters.Add(new SqlParameter("@Rr", DBNull.Value));

            if (bp != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Bp", bp));
            else
                cmd.Parameters.Add(new SqlParameter("@Bp", DBNull.Value));

            if (HeadAndNeck != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@HeadAndNeck", HeadAndNeck));
            else
                cmd.Parameters.Add(new SqlParameter("@HeadAndNeck", DBNull.Value));

            if (ChestAndBack != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ChestAndBack", ChestAndBack));
            else
                cmd.Parameters.Add(new SqlParameter("@ChestAndBack", DBNull.Value));

            if (Abdomen != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Abdomen", Abdomen));
            else
                cmd.Parameters.Add(new SqlParameter("@Abdomen", DBNull.Value));

            if (Pelvis != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Pelvis", Pelvis));
            else
                cmd.Parameters.Add(new SqlParameter("@Pelvis", DBNull.Value));

            if (UpperAndLowerLimbs != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@UpperAndLowerLimbs", UpperAndLowerLimbs));
            else
                cmd.Parameters.Add(new SqlParameter("@UpperAndLowerLimbs", DBNull.Value));

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

        //Delete Specefic Session Based On Seesion Id and then delete all associated Records with this session from other Tables
        public bool Delete_Session(string seesion_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From Session Where SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = seesion_id;

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

        // Update Session
        public bool Update_Session(string s_id, string s_date, string complaints, string complications, string s_duration)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Session Set SessionDate = @SessionDate , CurrentCopmlaints = @CurrentCopmlaints , Complications = @Complications , SessionDuration = @SessionDuration  Where SessionId = @SessionId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = s_id;
            cmd.Parameters.Add(new SqlParameter("@SessionDate", System.Data.SqlDbType.Date)).Value = s_date;
            if (complaints != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@CurrentCopmlaints", complaints));
            else
                cmd.Parameters.Add(new SqlParameter("@CurrentCopmlaints", DBNull.Value));

            if (complications != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Complications", complications));
            else
                cmd.Parameters.Add(new SqlParameter("@Complications", DBNull.Value));

            if (s_duration != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@SessionDuration", s_duration));
            else
                cmd.Parameters.Add(new SqlParameter("@SessionDuration", DBNull.Value));

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

        // Update PhysicalExamination
        public bool Update_PhysicalExamination(string s_id, string temp, string hr, string rr, string bp, string HeadAndNeck, string ChestAndBack, string Abdomen, string Pelvis, string UpperAndLowerLimbs, string Others)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update PhysicalExamination Set Temp = @Temp , Hr = @Hr , Rr = @Rr , Bp = @Bp , HeadAndNeck = @HeadAndNeck, ChestAndBack = @ChestAndBack, Abdomen = @Abdomen, Pelvis = @Pelvis, UpperAndLowerLimbs = @UpperAndLowerLimbs, Others = @Others  Where SessionId = @SessionId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = s_id;

            if (temp != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Temp", System.Data.SqlDbType.Float)).Value = temp;
            else
                cmd.Parameters.Add(new SqlParameter("@Temp", DBNull.Value));

            if (hr != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Hr", System.Data.SqlDbType.Int)).Value = hr;
            else
                cmd.Parameters.Add(new SqlParameter("@Hr", DBNull.Value));

            if (rr != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Rr", System.Data.SqlDbType.Int)).Value = rr;
            else
                cmd.Parameters.Add(new SqlParameter("@Rr", DBNull.Value));

            if (bp != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Bp", bp));
            else
                cmd.Parameters.Add(new SqlParameter("@Bp", DBNull.Value));

            if (HeadAndNeck != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@HeadAndNeck", HeadAndNeck));
            else
                cmd.Parameters.Add(new SqlParameter("@HeadAndNeck", DBNull.Value));

            if (ChestAndBack != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ChestAndBack", ChestAndBack));
            else
                cmd.Parameters.Add(new SqlParameter("@ChestAndBack", DBNull.Value));

            if (Abdomen != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Abdomen", Abdomen));
            else
                cmd.Parameters.Add(new SqlParameter("@Abdomen", DBNull.Value));

            if (Pelvis != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Pelvis", Pelvis));
            else
                cmd.Parameters.Add(new SqlParameter("@Pelvis", DBNull.Value));

            if (UpperAndLowerLimbs != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@UpperAndLowerLimbs", UpperAndLowerLimbs));
            else
                cmd.Parameters.Add(new SqlParameter("@UpperAndLowerLimbs", DBNull.Value));

            if (Others != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Others", Others));
            else
                cmd.Parameters.Add(new SqlParameter("@Others", DBNull.Value));

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

        // Get All Sessions For Specefic fileNum
        public SqlDataReader Get_Sessions(string file_num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionId,SessionNum,SessionDate,CurrentCopmlaints,Complications,SessionDuration  From Session where PatientFileNum = @PatientFileNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Sessions For Specefic fileNum
        public SqlDataReader Get_Sessions_OtherTables(string file_num, string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionId,SessionDate,SessionNum  From Session where PatientFileNum = @PatientFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Sessions 
        public SqlDataReader Get_Sessions()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Session ", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get Last Session Record
        public int Get_LastSession()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Max(SessionId) From Session ", ConnectionDB.con);
            int s_id = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return s_id;
        }

        // Get Specefic SessionNum Field For Specefic Bills Records according to filenum
        public SqlDataReader Get_SessionNum_BillMg(string filenum , string session_id_bill)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionNum From Session  where PatientFileNum = @PatientFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = filenum;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id_bill;
            SqlDataReader sdr = cmd.ExecuteReader();         
            return sdr;
        }
       
        // Get All PHysicalExamination 
        public SqlDataReader Get_PhysicalExamination()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From PhysicalExamination", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All PHysicalExamination For Specefic fileNum
        public SqlDataReader Get_PhysicalExamination(string file_num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionId,Temp,Hr,Rr,Bp,HeadAndNeck,ChestAndBack,Abdomen,Pelvis,UpperAndLowerLimbs  From PhysicalExamination where PatientFileNum = @PatientFileNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record from Session table
        public void Edit_Identity_Sessions()
        {
            SqlDataReader sqlData = Get_Sessions();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Session, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(SessionId)  From Session", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Session, RESEED, @SessionId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@SessionId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

        // Query For Adjus Identity After Delete Record from PhysicalExamination table
        public void Edit_Identity_PhysicalExamination()
        {
            SqlDataReader sqlData = Get_Sessions();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (PhysicalExamination, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(PhysicalExaminationId)  From PhysicalExamination", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (PhysicalExamination, RESEED, @PhysicalExaminationId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@PhysicalExaminationId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }


    }
}
