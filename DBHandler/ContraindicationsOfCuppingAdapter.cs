using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class ContraindicationsOfCuppingAdapter
    {
        public ContraindicationsOfCuppingAdapter()
        { 
        }

        // Insert New record of ContraindicationsOfCupping
        public bool Insert_ContraindicationsOfCupping(string Diabetes, string RenalFailure, string BleedingDisordersAndAnticoagulants, string Aids, string LiverFailureOrHepAtitis , string Pregnancy, string ChildrenBelow12Years, string ChemoRadiotherapyOrCancer, string Pacemaker, string VaricoseVeins, string PatientFileNum, string SessionId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into ContraindicationsOfCupping Values (@Diabetes ,@RenalFailure ,@BleedingDisordersAndAnticoagulants ,@Aids ,@LiverFailureOrHepAtitis ,@Pregnancy ,@ChildrenBelow12Years ,@ChemoRadiotherapyOrCancer ,@Pacemaker ,@VaricoseVeins ,@PatientFileNum ,@SessionId)", ConnectionDB.con);

            if (Diabetes != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Diabetes", Diabetes));
            else
                cmd.Parameters.Add(new SqlParameter("@Diabetes", DBNull.Value));

            if (RenalFailure != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@RenalFailure", RenalFailure));
            else
                cmd.Parameters.Add(new SqlParameter("@RenalFailure", DBNull.Value));

            if (BleedingDisordersAndAnticoagulants != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@BleedingDisordersAndAnticoagulants", BleedingDisordersAndAnticoagulants));
            else
                cmd.Parameters.Add(new SqlParameter("@BleedingDisordersAndAnticoagulants", DBNull.Value));

            if (Aids != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Aids", Aids));
            else
                cmd.Parameters.Add(new SqlParameter("@Aids", DBNull.Value));

            if (LiverFailureOrHepAtitis != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@LiverFailureOrHepAtitis", LiverFailureOrHepAtitis));
            else
                cmd.Parameters.Add(new SqlParameter("@LiverFailureOrHepAtitis", DBNull.Value));


            if (Pregnancy != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Pregnancy", Pregnancy));
            else
                cmd.Parameters.Add(new SqlParameter("@Pregnancy", DBNull.Value));


            if (ChildrenBelow12Years != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ChildrenBelow12Years", ChildrenBelow12Years));
            else
                cmd.Parameters.Add(new SqlParameter("@ChildrenBelow12Years", DBNull.Value));


            if (ChemoRadiotherapyOrCancer != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ChemoRadiotherapyOrCancer", ChemoRadiotherapyOrCancer));
            else
                cmd.Parameters.Add(new SqlParameter("@ChemoRadiotherapyOrCancer", DBNull.Value));


            if (Pacemaker != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Pacemaker", Pacemaker));
            else
                cmd.Parameters.Add(new SqlParameter("@Pacemaker", DBNull.Value));


            if (VaricoseVeins != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@VaricoseVeins", VaricoseVeins));
            else
                cmd.Parameters.Add(new SqlParameter("@VaricoseVeins", DBNull.Value));


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

        // Update from ContraindicationsOfCupping
        public bool Update_ContraindicationsOfCupping(string Con_id, string Diabetes, string RenalFailure, string BleedingDisordersAndAnticoagulants, string Aids, string LiverFailureOrHepAtitis, string Pregnancy, string ChildrenBelow12Years, string ChemoRadiotherapyOrCancer, string Pacemaker, string VaricoseVeins)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update ContraindicationsOfCupping Set Diabetes = @Diabetes , RenalFailure = @RenalFailure , BleedingDisordersAndAnticoagulants = @BleedingDisordersAndAnticoagulants , Aids = @Aids , LiverFailureOrHepAtitis = @LiverFailureOrHepAtitis, Pregnancy = @Pregnancy, ChildrenBelow12Years = @ChildrenBelow12Years, ChemoOrCancer = @ChemoRadiotherapyOrCancer, Pacemaker = @Pacemaker, VaricoseVeins = @VaricoseVeins  Where ContraindicationId = @ContraindicationId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@ContraindicationId", System.Data.SqlDbType.Int)).Value = Con_id;

            if (Diabetes != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Diabetes", Diabetes));
            else
                cmd.Parameters.Add(new SqlParameter("@Diabetes", DBNull.Value));

            if (RenalFailure != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@RenalFailure", RenalFailure));
            else
                cmd.Parameters.Add(new SqlParameter("@RenalFailure", DBNull.Value));

            if (BleedingDisordersAndAnticoagulants != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@BleedingDisordersAndAnticoagulants", BleedingDisordersAndAnticoagulants));
            else
                cmd.Parameters.Add(new SqlParameter("@BleedingDisordersAndAnticoagulants", DBNull.Value));

            if (Aids != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Aids", Aids));
            else
                cmd.Parameters.Add(new SqlParameter("@Aids", DBNull.Value));


            if (LiverFailureOrHepAtitis != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@LiverFailureOrHepAtitis", LiverFailureOrHepAtitis));
            else
                cmd.Parameters.Add(new SqlParameter("@LiverFailureOrHepAtitis", DBNull.Value));


            if (Pregnancy != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Pregnancy", Pregnancy));
            else
                cmd.Parameters.Add(new SqlParameter("@Pregnancy", DBNull.Value));


            if (ChildrenBelow12Years != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ChildrenBelow12Years", ChildrenBelow12Years));
            else
                cmd.Parameters.Add(new SqlParameter("@ChildrenBelow12Years", DBNull.Value));


            if (ChemoRadiotherapyOrCancer != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ChemoRadiotherapyOrCancer", ChemoRadiotherapyOrCancer));
            else
                cmd.Parameters.Add(new SqlParameter("@ChemoRadiotherapyOrCancer", DBNull.Value));


            if (Pacemaker != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Pacemaker", Pacemaker));
            else
                cmd.Parameters.Add(new SqlParameter("@Pacemaker", DBNull.Value));


            if (VaricoseVeins != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@VaricoseVeins", VaricoseVeins));
            else
                cmd.Parameters.Add(new SqlParameter("@VaricoseVeins", DBNull.Value));



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

        // Delete Only Record of Contraindications of Cupping Without Totaly session
        public bool Delete_ContraindicationsOfCupping(string contraindication_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From ContraindicationsOfCupping Where ContraindicationId = @ContraindicationId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@ContraindicationId", System.Data.SqlDbType.Int)).Value = contraindication_id;

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

        // Get All Records From ContraindicationsOfCupping
        public SqlDataReader Get_ContraindicationsOfCupping()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From ContraindicationsOfCupping", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Records From ContraindicationsOfCupping for specefic session 
        public SqlDataReader Get_ContraindicationsOfCupping(string file_num ,string session_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionId,ContraindicationId,Diabetes,RenalFailure,Pregnancy,Pacemaker,VaricoseVeins,BleedingDisordersAndAnticoagulants,Aids,LiverFailureOrHepAtitis,ChildrenBelow12Years,ChemoOrCancer  From ContraindicationsOfCupping where PatientFileNum = @PatientFileNum and SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = file_num;
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = session_id;

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record from ContraindicationsOfCupping table
        public void Edit_Identity_ContraindicationsOfCupping()
        {
            SqlDataReader sqlData = Get_ContraindicationsOfCupping();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (ContraindicationsOfCupping, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(ContraindicationId)  From ContraindicationsOfCupping", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (ContraindicationsOfCupping, RESEED, @ContraindicationId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@ContraindicationId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

    }
}
