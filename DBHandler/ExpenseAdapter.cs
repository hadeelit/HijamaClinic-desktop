using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HijamaClinicManagement.DBHandler
{
    class ExpenseAdapter
    {
        public ExpenseAdapter()
        { }

        // Insert New Expense
        public bool Insert_Expense(string expense_billnum, string expense_name, string expense_date, string expense_amount, string expense_tax, string expense_totallpaid, string enty_name)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into Expenses Values (@ExpenseBillNum ,@ExpenseName ,@ExpenseDate ,@Amount ,@Tax ,@TotalPricePaid,@EntryName)", ConnectionDB.con);

            if (expense_billnum != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ExpenseBillNum", expense_billnum));
            else
                cmd.Parameters.Add(new SqlParameter("@ExpenseBillNum", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@ExpenseName", expense_name));
            cmd.Parameters.Add(new SqlParameter("@ExpenseDate", System.Data.SqlDbType.Date)).Value = expense_date;

            if (expense_amount != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Amount", System.Data.SqlDbType.Float)).Value = expense_amount;
            else
                cmd.Parameters.Add(new SqlParameter("@Amount", DBNull.Value));

            if (expense_tax != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Tax", System.Data.SqlDbType.Float)).Value = expense_tax;
            else
                cmd.Parameters.Add(new SqlParameter("@Tax", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@TotalPricePaid", System.Data.SqlDbType.Float)).Value = expense_totallpaid;

            cmd.Parameters.Add(new SqlParameter("@EntryName", enty_name));

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

        // Delete Specefic expense
        public bool Delete_Expense(string id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From Expenses Where ExpenseId = @ExpenseId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@ExpenseId", System.Data.SqlDbType.Int)).Value = id;

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

        // Update Expenes
        public bool Update_Expense(string id, string expense_billnum, string expense_name, string expense_date, string expense_amount, string expense_tax, string expense_totallpaid, string enty_name)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Expenses Set ExpenseBillNum = @ExpenseBillNum , ExpenseName = @ExpenseName , ExpenseDate = @ExpenseDate , Amount = @Amount ,Tax = @Tax ,TotalPricePaid = @TotalPricePaid,EntryName = @EntryName Where  ExpenseId = @ExpenseId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@ExpenseId", System.Data.SqlDbType.Int)).Value = id;

            if (expense_billnum != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@ExpenseBillNum", expense_billnum));
            else
                cmd.Parameters.Add(new SqlParameter("@ExpenseBillNum", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@ExpenseName", expense_name));
            cmd.Parameters.Add(new SqlParameter("@ExpenseDate", System.Data.SqlDbType.Date)).Value = expense_date;

            if (expense_amount != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Amount", System.Data.SqlDbType.Float)).Value = expense_amount;
            else
                cmd.Parameters.Add(new SqlParameter("@Amount", DBNull.Value));

            if (expense_tax != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@Tax", System.Data.SqlDbType.Float)).Value = expense_tax;
            else
                cmd.Parameters.Add(new SqlParameter("@Tax", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@TotalPricePaid", System.Data.SqlDbType.Float)).Value = expense_totallpaid;

            cmd.Parameters.Add(new SqlParameter("@EntryName", enty_name));

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

        // Get All Expenes records
        public SqlDataReader Get_Expenses()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Expenses", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
           
        }

        // Search based on ExpenseName
        public SqlDataReader Search_ExpenseName(string name)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Expenses Where ExpenseName = @ExpenseName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@ExpenseName", name));

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Search based on ExpenseBillNum
        public SqlDataReader Search_ExpenseBillNum(string billnum)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Expenses Where ExpenseBillNum = @ExpenseBillNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@ExpenseBillNum",billnum));

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Search based on ExpenseBillNum and ExpenseName both
        public SqlDataReader Search_ExpenseBoth(string billnum, string name)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Expenses Where ExpenseBillNum = @ExpenseBillNum and ExpenseName = @ExpenseName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@ExpenseBillNum",billnum));
            cmd.Parameters.Add(new SqlParameter("@ExpenseName", name));

            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record
        public void Edit_Identity()
        {
            SqlDataReader sqlData = Get_Expenses();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Expenses, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(ExpenseId)  From Expenses", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Expenses, RESEED, @ExpenseId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@ExpenseId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }
    }
}
