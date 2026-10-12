using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class TaxAdapter
    {
        public TaxAdapter()
        { 
        }

        // Function For Inser Tax To DB
        public bool Insert_Tax(string Tax_date , string Tax_NonSaudis , string Tax_Saudis)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into Taxes Values (@Date ,@TaxForNonSaudis ,@TaxForSaudis)",ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@Date", System.Data.SqlDbType.Date)).Value = Tax_date;

            if (Tax_NonSaudis == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@TaxForNonSaudis", DBNull.Value));
            else
                // cmd.Parameters.Add(new SqlParameter("@TaxForNonSaudis", System.Data.SqlDbType.Float)).Value = Tax_NonSaudis;
                cmd.Parameters.Add(new SqlParameter("@TaxForNonSaudis", Tax_NonSaudis));


            if (Tax_Saudis == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@TaxForSaudis", DBNull.Value));
            else
                // cmd.Parameters.Add(new SqlParameter("@TaxForSaudis", System.Data.SqlDbType.Float)).Value = Tax_Saudis;
                cmd.Parameters.Add(new SqlParameter("@TaxForSaudis", Tax_Saudis));

            int affect_r = cmd.ExecuteNonQuery();
            if (affect_r > 0)
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

        // Function For Select All Taxes from Table
        public SqlDataReader Taxes_Get()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Taxes", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Function For Select TaxForNonSaudis from Table (using in Bill Extract)
        public SqlDataReader Get_TaxForNonSaudis()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select TaxForNonSaudis From Taxes", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Function For Select TaxForSaudis from Table (using in Bill Extract)
        public SqlDataReader Get_TaxForSaudis()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select TaxForSaudis From Taxes", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Function For Delete Record from Taxes Table
        public bool Delete_Tax(string id)
        {
            ConnectionDB.CheckConnection();

            SqlCommand cmd = new SqlCommand("Delete From Taxes Where TaxId =@TaxId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@TaxId", System.Data.SqlDbType.Int)).Value = id;
            int affect_r = cmd.ExecuteNonQuery();

            if (affect_r > 0)
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

            SqlDataReader sqlData = Taxes_Get();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Taxes, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(TaxId)  From Taxes", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Taxes, RESEED, @TaxId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@TaxId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

        // Function For Update Taxe Fields 
        public bool Update_Tax(string id, string date, string non_saudis_r, string saudis_r)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Taxes Set Date = @Date ,TaxForNonSaudis = @TaxForNonSaudis ,TaxForSaudis = @TaxForSaudis Where TaxId =@TaxId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@TaxId", System.Data.SqlDbType.Int)).Value = id;
            cmd.Parameters.Add(new SqlParameter("@Date", System.Data.SqlDbType.Date)).Value = date;

            if (non_saudis_r == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@TaxForNonSaudis", DBNull.Value));
            else
                // cmd.Parameters.Add(new SqlParameter("@TaxForNonSaudis", System.Data.SqlDbType.Float)).Value = non_saudis_r;
                cmd.Parameters.Add(new SqlParameter("@TaxForNonSaudis", non_saudis_r));


            if (saudis_r == string.Empty)
                cmd.Parameters.Add(new SqlParameter("@TaxForSaudis", DBNull.Value));
            else
                //  cmd.Parameters.Add(new SqlParameter("@TaxForSaudis", System.Data.SqlDbType.Float)).Value = saudis_r;
                cmd.Parameters.Add(new SqlParameter("@TaxForSaudis", saudis_r));


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
