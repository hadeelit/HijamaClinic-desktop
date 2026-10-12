using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Data;

namespace HijamaClinicManagement.DBHandler
{
    class BillAdapter
    {
        public BillAdapter()
        { }

        // Insert New Record Bill
        public bool Insert_Bill(string BillDate, int PaymentChipType, string AdditionalServices, string PriceOfAdditionalServices, string Nationality, string Tax,string TaxPercentage, string Discount, string PaymentType, string SessionPrice, string TotalPricePaid,string PaidCash,string PaidNetwork,string FinallyPaidAmount,string PaidValue, string AmountRemainingUnpaid, string EntryName, string PatientFileNum, string SessionId)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into Bill Values (@date ,@pchip_type ,@additional_service ,@price_additional_service ,@nationality ,@tax,@TaxPercentage, @discount, @payment_type,@SessionPrice, @total_price,@PaidCash ,@PaidNetwork ,@FinallyPaidAmount ,@PaidValue ,@amount_remaining, @entry_name, @patient_filenum, @session_id)", ConnectionDB.con);

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

            //S.R tax Value
            if (Tax != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@tax", System.Data.SqlDbType.Float)).Value = Tax;
            else
                cmd.Parameters.Add(new SqlParameter("@tax", DBNull.Value));

            // tax percentage Value
            if (TaxPercentage != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@TaxPercentage", TaxPercentage));
            else
                cmd.Parameters.Add(new SqlParameter("@TaxPercentage", DBNull.Value));

            if (Discount != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@discount", System.Data.SqlDbType.Float)).Value = Discount;
            else
                cmd.Parameters.Add(new SqlParameter("@discount", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@payment_type", PaymentType ));
            cmd.Parameters.Add(new SqlParameter("@SessionPrice", System.Data.SqlDbType.Float)).Value = SessionPrice;
            cmd.Parameters.Add(new SqlParameter("@total_price", System.Data.SqlDbType.Float)).Value = TotalPricePaid;

            // Paid Cash
            if (PaidCash != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PaidCash ", System.Data.SqlDbType.Float)).Value = PaidCash;
            else
                cmd.Parameters.Add(new SqlParameter("@PaidCash ", DBNull.Value));

            //paid network
            if (PaidNetwork != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PaidNetwork", System.Data.SqlDbType.Float)).Value = PaidNetwork;
            else
                cmd.Parameters.Add(new SqlParameter("@PaidNetwork", DBNull.Value));
            //Finally Paid Amount
            cmd.Parameters.Add(new SqlParameter("@FinallyPaidAmount", System.Data.SqlDbType.Float)).Value = FinallyPaidAmount;
            cmd.Parameters.Add(new SqlParameter("@PaidValue", System.Data.SqlDbType.Float)).Value = PaidValue;
            cmd.Parameters.Add(new SqlParameter("@amount_remaining", System.Data.SqlDbType.Float)).Value = AmountRemainingUnpaid;
            cmd.Parameters.Add(new SqlParameter("@entry_name", EntryName));
            cmd.Parameters.Add(new SqlParameter("@patient_filenum", System.Data.SqlDbType.Int)).Value = PatientFileNum;
            cmd.Parameters.Add(new SqlParameter("@session_id", System.Data.SqlDbType.Int)).Value = SessionId;

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

        //Get specefic Record according to session id  using in BillExtractWindow
        //editted Function 
        public SqlDataReader Get_BillExtract(string s_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select BillId,BillDate,PaymentChipType,AdditionalServices,PriceOfAdditionalServices,Nationality,Tax,TaxPercentage,Discount,PaymentType,SessionPrice,TotalPricePaid,PaidCash,PaidNetwork,FinallyPaidAmount,PaidValue,AmountRemainingUnpaid  From Bill where SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = s_id;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }


        //Get specefic Bill_Id according to session id for delete bill when delete session 
        public int Get_BillExtract_id(string s_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select BillId  From Bill where SessionId = @SessionId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@SessionId", System.Data.SqlDbType.Int)).Value = s_id;
            SqlDataReader sdr = cmd.ExecuteReader();
            //No Bill for this sessionid yet
            if(sdr.HasRows == false)
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

        //Get specefic Session_id according to Bill_ id for Get Session_id and session_Num For Bill_Mg
        public int Get_BillExtract_Sid(string bill_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select SessionId  From Bill where BillId = @BillId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@BillId", System.Data.SqlDbType.Int)).Value = bill_id;
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
                int session_id = Convert.ToInt32(cmd.ExecuteScalar());
                ConnectionDB.CloseConnection();
                return session_id;
            }
        }

        //Get All Records from Bill Tables
        public SqlDataReader Get_BillExtract()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Bill ", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //Get specefic Record  according to FileNum  using in BillMgView
        public SqlDataReader Get_BillsExtract(string filenum)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select BillId,BillDate,PaymentChipType,AdditionalServices,PriceOfAdditionalServices,Nationality,TaxPercentage,SessionPrice,Tax,TotalPricePaid,Discount,PaymentType,PaidCash,PaidNetwork,FinallyPaidAmount,PaidValue,AmountRemainingUnpaid,SessionId  From Bill where PatientFileNum = @PatientFileNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = filenum;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //Get data for deleted record in patient file window
        public SqlDataReader Get_BillsPatientFile(string filenum)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select BillId,BillDate,PaymentChipType,AdditionalServices,PriceOfAdditionalServices,Nationality,Tax,Discount,PaymentType,TotalPricePaid,AmountRemainingUnpaid,EntryName  From Bill where PatientFileNum = @PatientFileNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@PatientFileNum", System.Data.SqlDbType.Int)).Value = filenum;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get specefic bill record according to bill id for delete session in PatientSessionWindow
        public SqlDataReader Get_Bill(string bill_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select BillId,BillDate,PaymentChipType,AdditionalServices,PriceOfAdditionalServices,Nationality,Tax,Discount,PaymentType,TotalPricePaid,AmountRemainingUnpaid,EntryName  From Bill where BillId = @BillId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@BillId", System.Data.SqlDbType.Int)).Value = bill_id;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //Get Bill_ num for Last Record added For add bill_num to TextBox After Add 
        public int Get_LastRecord()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Max(BillId)  From Bill", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            sdr.Close();
            int bill_id = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return bill_id;
        }


        //Delete Specefic record 
        public bool Delete_BillExtract(string bill_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From Bill Where BillId = @BillId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@BillId", System.Data.SqlDbType.Int)).Value = bill_id;

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

        // Edit Specefic record using in (BillMgView)
        public bool Update_BillExtract(string bill_id, int PaymentChipType, string AdditionalServices, string PriceOfAdditionalServices, string Nationality, string Tax,string TaxPercentage, string Discount, string PaymentType, string SessionPrice, string TotalPricePaid, string PaidCash, string PaidNetwork,string FinallyPaidAmount,string PaidValue,string AmountRemainingUnpaid)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Bill set PaymentChipType = @pchip_type ,AdditionalServices = @additional_service, PriceOfAdditionalServices = @price_additional_service, Nationality = @nationality ,Tax = @tax, TaxPercentage = @TaxPercentage ,Discount = @discount, PaymentType = @payment_type , SessionPrice = @SessionPrice ,TotalPricePaid = @total_price, PaidCash = @PaidCash ,PaidNetwork = @PaidNetwork, FinallyPaidAmount = @FinallyPaidAmount , PaidValue = @PaidValue , AmountRemainingUnpaid = @amount_remaining   Where BillId = @BillId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@BillId", System.Data.SqlDbType.Int)).Value = bill_id;

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

            //S.R tax Value
            if (Tax != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@tax", System.Data.SqlDbType.Float)).Value = Tax;
            else
                cmd.Parameters.Add(new SqlParameter("@tax", DBNull.Value));

            // tax percentage Value
            if (TaxPercentage != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@TaxPercentage", TaxPercentage));
            else
                cmd.Parameters.Add(new SqlParameter("@TaxPercentage", DBNull.Value));

            if (Discount != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@discount", System.Data.SqlDbType.Float)).Value = Discount;
            else
                cmd.Parameters.Add(new SqlParameter("@discount", DBNull.Value));


            cmd.Parameters.Add(new SqlParameter("@payment_type", PaymentType));
            cmd.Parameters.Add(new SqlParameter("@SessionPrice", System.Data.SqlDbType.Float)).Value = SessionPrice;
            cmd.Parameters.Add(new SqlParameter("@total_price", System.Data.SqlDbType.Float)).Value = TotalPricePaid;

            // Paid Cash
            if (PaidCash != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PaidCash ", System.Data.SqlDbType.Float)).Value = PaidCash;
            else
                cmd.Parameters.Add(new SqlParameter("@PaidCash ", DBNull.Value));

            //paid network
            if (PaidNetwork != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@PaidNetwork", System.Data.SqlDbType.Float)).Value = PaidNetwork;
            else
                cmd.Parameters.Add(new SqlParameter("@PaidNetwork", DBNull.Value));

            //Finally Paid Amount
            cmd.Parameters.Add(new SqlParameter("@FinallyPaidAmount", System.Data.SqlDbType.Float)).Value = FinallyPaidAmount;
            cmd.Parameters.Add(new SqlParameter("@PaidValue", System.Data.SqlDbType.Float)).Value = PaidValue;
            cmd.Parameters.Add(new SqlParameter("@amount_remaining", System.Data.SqlDbType.Float)).Value = AmountRemainingUnpaid;

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

        // Query For Adjus Identity After Delete Record from Bill table
        public void Edit_Identity_Bill()
        {
            SqlDataReader sqlData = Get_BillExtract();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Bill, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(BillId)  From Bill", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Bill, RESEED, @BillId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@BillId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

        //Query For getting SaudiColumn values 
        public SqlDataReader Get_SaudiValues_Bill(string fdte, string todate)
        {
            ConnectionDB.CheckConnection();
           /* SqlCommand cmd = new SqlCommand("Select TotalPricePaid  From Bill where Nationality = 'سعودي' and ( BillDate >= @fdate and BillDate <= @todate ) ", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@fdate", fdte));
            cmd.Parameters.Add(new SqlParameter("@todate", todate));
            decimal totalsaudi = Convert.ToDecimal(cmd.ExecuteScalar());
           */
            SqlCommand cmd = new SqlCommand("Select BillDate , TotalPricePaid  From Bill where Nationality = 'سعودي' and ( BillDate >= @fdate and BillDate <= @todate ) ", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@fdate", fdte));
            cmd.Parameters.Add(new SqlParameter("@todate", todate));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //Query For getting  NonSaudiColumn values 
        public SqlDataReader Get_NonSaudiValues_Bill(string fdte, string todate)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select TotalPricePaid  From Bill where Nationality = 'غير سعودي' and ( BillDate >= @fdate and BillDate <= @todate ) ", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@fdate", fdte));
            cmd.Parameters.Add(new SqlParameter("@todate", todate));
            SqlDataReader totalnonsaudi = cmd.ExecuteReader();
           // ConnectionDB.CloseConnection();
            return totalnonsaudi;
        }

        // Query For getting  All values saudies + nonsudies For Extracting pdf
        public SqlDataReader Get_AllValues_Bill(string fdte, [Optional] string todate)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd;
            if (todate != null)
            {
                cmd = new SqlCommand("Select  FORMAT ( BillDate,'yyyy-MM-dd') as 'تاريخ الفاتورة' , Nationality as 'الجنسية' ,Sum(TotalPricePaid)  as 'القيمة المالية'  From Bill  where  ( BillDate >= @fdate and BillDate <= @todate ) GROUP BY Nationality,BillDate", ConnectionDB.con);
                cmd.Parameters.Add(new SqlParameter("@fdate", System.Data.SqlDbType.Date)).Value = fdte;
                cmd.Parameters.Add(new SqlParameter("@todate", System.Data.SqlDbType.Date)).Value = todate;
            }
            else
            {
                cmd = new SqlCommand("Select FORMAT ( BillDate,'yyyy-MM-dd') as 'تاريخ الفاتورة' , Nationality as 'الجنسية' ,Sum(TotalPricePaid)  as 'القيمة المالية'  From Bill  where BillDate = @fdate  GROUP BY Nationality,BillDate", ConnectionDB.con);
                cmd.Parameters.Add(new SqlParameter("@fdate", System.Data.SqlDbType.Date)).Value = fdte;
            }
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For getting  All values saudies + nonsudies For nonExtracting pdf
        public SqlDataReader Get_AllValues_Bill_(string fdte, [Optional] string todate)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd;
            if (todate != null)
            {
                cmd = new SqlCommand("Select  Nationality as 'الجنسية' ,Sum(TotalPricePaid)  as 'القيمة المالية'  From Bill  where  ( BillDate >= @fdate and BillDate <= @todate ) GROUP BY Nationality", ConnectionDB.con);
                cmd.Parameters.Add(new SqlParameter("@fdate", System.Data.SqlDbType.Date)).Value = fdte;
                cmd.Parameters.Add(new SqlParameter("@todate", System.Data.SqlDbType.Date)).Value = todate;
            }
            else
            {
                cmd = new SqlCommand("Select  Nationality as 'الجنسية' ,Sum(TotalPricePaid)  as 'القيمة المالية'  From Bill  where BillDate = @fdate  GROUP BY Nationality", ConnectionDB.con);
                cmd.Parameters.Add(new SqlParameter("@fdate", System.Data.SqlDbType.Date)).Value = fdte;
            }
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }


        // Query For getting  All values cash and networking in Bill Table for specefic day 
        public SqlDataReader DailyBill_Content(string datenow)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd;
            //  cmd = new SqlCommand("Select  PaymentType as 'نوع الدفع', Sum(TotalPricePaid) as 'القيمة المالية' From Bill  where BillDate = @datenow  GROUP BY PaymentType", ConnectionDB.con);
            cmd = new SqlCommand("Select  PaymentType as 'نوع الدفع', Sum(FinallyPaidAmount) as 'القيمة المالية' From Bill  where BillDate = @datenow  GROUP BY PaymentType", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@datenow", System.Data.SqlDbType.Date)).Value = datenow;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For getting  All values cash paid (payment type ="نقدي وشبكة")  in Bill Table for specefic day using for Daily income
        public double DailyBill_PaidCash(string datenow)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd;
            //  cmd = new SqlCommand("Select  PaymentType as 'نوع الدفع', Sum(TotalPricePaid) as 'القيمة المالية' From Bill  where BillDate = @datenow  GROUP BY PaymentType", ConnectionDB.con);
            cmd = new SqlCommand("Select Sum(PaidCash) as PaidCash From Bill  where BillDate = @datenow and PaymentType = 'نقدي وشبكة '", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@datenow", System.Data.SqlDbType.Date)).Value = datenow;
            SqlDataReader sdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            if (!dt.Rows[0].IsNull("PaidCash"))
            {
                sdr.Close();
                double paid_cash = Convert.ToDouble(cmd.ExecuteScalar());
                ConnectionDB.CloseConnection();
                return paid_cash;
            }
            else
            {
                sdr.Close();
                ConnectionDB.CloseConnection();
                return 0;
            }
            
        }

        // Query For getting  All values PaidNetwork (payment type ="نقدي وشبكة")  in Bill Table for specefic day using for Daily income
        public double DailyBill_PaidNetwork(string datenow)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd;
            //  cmd = new SqlCommand("Select  PaymentType as 'نوع الدفع', Sum(TotalPricePaid) as 'القيمة المالية' From Bill  where BillDate = @datenow  GROUP BY PaymentType", ConnectionDB.con);
            cmd = new SqlCommand("Select Sum(PaidNetwork) as PaidNetwork From Bill  where BillDate = @datenow and PaymentType = 'نقدي وشبكة '", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@datenow", System.Data.SqlDbType.Date)).Value = datenow;
            SqlDataReader sdr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            if (!dt.Rows[0].IsNull("PaidNetwork"))
            {
                sdr.Close();
                double paid_network = Convert.ToDouble(cmd.ExecuteScalar());
                ConnectionDB.CloseConnection();
                return paid_network;
            }
            else
            {
                sdr.Close();
                ConnectionDB.CloseConnection();
                return 0;
            }
          
        }

    }
}
