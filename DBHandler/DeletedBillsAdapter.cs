using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class DeletedBillsAdapter
    {
        public DeletedBillsAdapter()
        { 
        }

        //Insert New Record To DeletedBillsRecords Table
        public bool Insert_DeletedBillsRecords(string PatientFileNum, string BillId, string BillDate, int PaymentChipType, string AdditionalServices, string PriceOfAdditionalServices, string Nationality, string Tax, string Discount, string PaymentType, string TotalPricePaid, string AmountRemainingUnpaid, string EntryName, string DeleteBillDate)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into DeletedBillsRecords Values (@pnum ,@billid,@date ,@pchip_type ,@additional_service ,@price_additional_service ,@nationality ,@tax, @discount, @payment_type, @total_price, @amount_remaining, @entry_name, @deleted_date)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@pnum", System.Data.SqlDbType.Int)).Value = PatientFileNum;
            cmd.Parameters.Add(new SqlParameter("@billid", System.Data.SqlDbType.Int)).Value = BillId;
            cmd.Parameters.Add(new SqlParameter("@date", System.Data.SqlDbType.Date)).Value = BillDate;
            // cmd.Parameters.Add(new SqlParameter("@pchip_type", System.Data.SqlDbType.Int)).Value = PaymentChipType;
            cmd.Parameters.Add(new SqlParameter("@pchip_type", PaymentChipType)); // true

            if (AdditionalServices != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@additional_service", AdditionalServices));
            else
                cmd.Parameters.Add(new SqlParameter("@additional_service", DBNull.Value));

            if (PriceOfAdditionalServices != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@price_additional_service", System.Data.SqlDbType.Float)).Value = PriceOfAdditionalServices;
            else
                cmd.Parameters.Add(new SqlParameter("@price_additional_service", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@nationality", Nationality));

            if (Tax != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@tax", Tax));
            else
                cmd.Parameters.Add(new SqlParameter("@tax", DBNull.Value));

            if (Discount != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@discount", System.Data.SqlDbType.Float)).Value = Discount;
            else
                cmd.Parameters.Add(new SqlParameter("@discount", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@payment_type", PaymentType));
            cmd.Parameters.Add(new SqlParameter("@total_price", System.Data.SqlDbType.Float)).Value = TotalPricePaid;
            cmd.Parameters.Add(new SqlParameter("@amount_remaining", System.Data.SqlDbType.Float)).Value = AmountRemainingUnpaid;
            cmd.Parameters.Add(new SqlParameter("@entry_name", EntryName));
            cmd.Parameters.Add(new SqlParameter("@deleted_date", System.Data.SqlDbType.Date)).Value = DeleteBillDate;

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

        // Delete All Records From DeletedBillsRecords Table
        public bool Delete_DeletedBillsRecords()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete  From DeletedBillsRecords", ConnectionDB.con);

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

        // Get All Records From DeletedBillsRecords
        public SqlDataReader Get_DeletedBillsRecords()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From DeletedBillsRecords ", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record from DeletedBillsRecords table
        public void Edit_Identity_DeletedBillsRecords()
        {
            SqlDataReader sqlData = Get_DeletedBillsRecords();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (DeletedBillsRecords, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(DeletedBillRecId)  From DeletedBillsRecords", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (DeletedBillsRecords, RESEED, @DeletedBillRecId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@DeletedBillRecId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }


    }
}
