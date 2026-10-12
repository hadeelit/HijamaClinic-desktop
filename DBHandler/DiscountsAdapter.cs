using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;

namespace HijamaClinicManagement.DBHandler
{
    class DiscountsAdapter
    {
        public DiscountsAdapter()
        { 
        }

        // Function For Insert New Discount Record To DB
        public bool Discount_Insert(string date, string Discount_r, string Stime, string Etime, string TotatP, string NumSession, int SPerson)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into Discounts Values (@DiscountDate ,@DiscountRate ,@StartTime ,@EndTime ,@TotalPricePaid ,@NumOfSessions ,@SpeceficPerson)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@DiscountDate", System.Data.SqlDbType.Date)).Value = date;
            cmd.Parameters.Add(new SqlParameter("@DiscountRate", System.Data.SqlDbType.Float)).Value = Discount_r;

            if (Stime == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@StartTime", DBNull.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@StartTime", System.Data.SqlDbType.DateTime)).Value = Stime;

            if (Etime == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@EndTime", DBNull.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@EndTime", System.Data.SqlDbType.DateTime)).Value = Etime;

            if (TotatP == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@TotalPricePaid", DBNull.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@TotalPricePaid", System.Data.SqlDbType.Float)).Value = TotatP;

            if (NumSession == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@NumOfSessions", DBNull.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@NumOfSessions", System.Data.SqlDbType.Int)).Value = NumSession;

            cmd.Parameters.Add(new SqlParameter("@SpeceficPerson", SPerson));

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

        // Function For Select All Discounts from Table
        public SqlDataReader Discounts_Get()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Discounts", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // check if current date is found in Discount table or not
        public SqlDataReader Get_Dis_Date(string date)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Discounts where DiscountDate = @DiscountDate", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@DiscountDate", SqlDbType.Date)).Value = date;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Delete Record From Discount Table
        public bool Delete_Discount(string id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From Discounts Where Discount_Id = @Discount_Id", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@Discount_Id", SqlDbType.Int)).Value = id;

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

        // Query For Adjus Identity After Delete Record
        public void Edit_Identity()
        {
            
            SqlDataReader sqlData = Discounts_Get();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Discounts, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(Discount_Id)  From Discounts", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Discounts, RESEED, @Discount_Id)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@Discount_Id", id));
                    cmd1.ExecuteNonQuery();
                  
                    ConnectionDB.CloseConnection();
                }
            }
        }

        // Function For Update Discounts Fields 
        public bool Update_Discount(string id, string date, string Discount_r, string Stime, string Etime, string TotatP, string NumSession, int SPerson)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Discounts Set DiscountDate = @DiscountDate ,DiscountRate = @DiscountRate ,StartTime = @StartTime ,EndTime = @EndTime ,TotalPricePaid = @TotalPricePaid ,NumOfSessions = @NumOfSessions,SpeceficPerson = @SpeceficPerson Where Discount_Id = @Discount_Id", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@Discount_Id", System.Data.SqlDbType.Int)).Value = id;
            cmd.Parameters.Add(new SqlParameter("@DiscountDate", System.Data.SqlDbType.Date)).Value = date;
            cmd.Parameters.Add(new SqlParameter("@DiscountRate", System.Data.SqlDbType.Float)).Value = Discount_r;

            if (Stime == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@StartTime", DBNull.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@StartTime", System.Data.SqlDbType.DateTime)).Value = Stime;

            if (Etime == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@EndTime", DBNull.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@EndTime", System.Data.SqlDbType.DateTime)).Value = Etime;

            if (TotatP == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@TotalPricePaid", DBNull.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@TotalPricePaid", System.Data.SqlDbType.Float)).Value = TotatP;

            if (NumSession == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@NumOfSessions", DBNull.Value));
            else
                cmd.Parameters.Add(new SqlParameter("@NumOfSessions", System.Data.SqlDbType.Int)).Value = NumSession;

            cmd.Parameters.Add(new SqlParameter("@SpeceficPerson", SPerson));

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

    }
}
