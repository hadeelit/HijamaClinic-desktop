using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace HijamaClinicManagement.DBHandler
{
    class DashBoardAdapter
    {
        public DashBoardAdapter()
        { }

        //Query For the  Files Create Recently 
        public int Get_NewFilesCount(string file_date)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Count(*) From Patient Where FileDate = @FileDate", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@FileDate", System.Data.SqlDbType.Date)).Value = file_date;
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return count;
        }

        //Query For the  Daily Appointments 
        public int Get_DailyAppointCount(string appoint_date)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Count(*) From Appointments Where AppointmentDate = @AppointmentDate", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@AppointmentDate", System.Data.SqlDbType.Date)).Value = appoint_date;
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return count;

        }

        //Query For Getting  Daily Deleted Bills
        public int Get_DailyDeletedBills(string dbill_date)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Count(*) From DeletedBillsRecords Where DeleteBillDate = @DeleteBillDate", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@DeleteBillDate", System.Data.SqlDbType.Date)).Value = dbill_date;
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return count;

        }

        //Query For Getting Daily Bills Count
        public int Get_DailyBills(string bill_date)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Count(*) From Bill Where BillDate = @BillDate", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@BillDate", System.Data.SqlDbType.Date)).Value = bill_date;
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return count;

        }

    }
}
