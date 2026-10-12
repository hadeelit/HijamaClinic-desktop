using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class AppointmentsAdapter
    {
        public AppointmentsAdapter()
        { 
        }

        // Inser New Appointements 
        public bool Insert_Appointment(string Contact_Num, string Patient_name, string Appointment_Date, string Appointment_Time, string Room_Service)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into Appointments Values (@ContactNum ,@PatientName ,@AppointmentDate ,@AppointmentTime ,@RoomType ,@OtherNote)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@ContactNum", Contact_Num));
            cmd.Parameters.Add(new SqlParameter("@PatientName", Patient_name));
            cmd.Parameters.Add(new SqlParameter("@AppointmentDate", System.Data.SqlDbType.Date)).Value = Appointment_Date;
            cmd.Parameters.Add(new SqlParameter("@AppointmentTime", System.Data.SqlDbType.DateTime)).Value = Appointment_Time;

            if (Room_Service != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@RoomType", Room_Service));
            else
                cmd.Parameters.Add(new SqlParameter("@RoomType", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@OtherNote", DBNull.Value));

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

        // Delete specefic Appointment
        public bool Delete_Appointment(string id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From Appointments Where AppointmentId = @AppointmentId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@AppointmentId", System.Data.SqlDbType.Int)).Value = id;

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

        // Update Spesefic Appointment
        public bool Update_Appointment(string id, string Contact_Num, string Patient_name, string Appointment_Date, string Appointment_Time, string Room_Service)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update Appointments Set ContactNum = @ContactNum , PatientName = @PatientName , AppointmentDate = @AppointmentDate , AppointmentTime = @AppointmentTime ,RoomType = @RoomType Where AppointmentId = @AppointmentId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@ContactNum", Contact_Num));
            cmd.Parameters.Add(new SqlParameter("@PatientName", Patient_name));
            cmd.Parameters.Add(new SqlParameter("@AppointmentDate", System.Data.SqlDbType.Date)).Value = Appointment_Date;
            cmd.Parameters.Add(new SqlParameter("@AppointmentTime", System.Data.SqlDbType.DateTime)).Value = Appointment_Time;

            if (Room_Service != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@RoomType", Room_Service));
            else
                cmd.Parameters.Add(new SqlParameter("@RoomType", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@AppointmentId", System.Data.SqlDbType.Int)).Value = id;

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

        // Get All Appointments for Edit_Identity()
        public SqlDataReader Get_AllAppointments()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Appointments", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        //Get New Appointment Record for DataGridView
        public SqlDataReader Get_NewAppointment()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Appointments where AppointmentId = (Select Max(AppointmentId) From Appointments)", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Daily Appointments Only
        public SqlDataReader Get_AllAppointments(string Appointment_Date)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Appointments Where AppointmentDate = @AppointmentDate", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@AppointmentDate", System.Data.SqlDbType.Date)).Value = Appointment_Date;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Check Specefic Time 
        public bool Check_Time(string Appointment_date, string Appointment_time)
        {
            ConnectionDB.CheckConnection();

            SqlCommand cmd1 = new SqlCommand("Select AppointmentTime From Appointments Where AppointmentDate =@AppointmentDate", ConnectionDB.con);
            cmd1.Parameters.Add(new SqlParameter("@AppointmentDate", System.Data.SqlDbType.Date)).Value = Appointment_date;
            // Time Field In View
            DateTime input_time = DateTime.Parse(Appointment_time);
            SqlDataReader sdr = cmd1.ExecuteReader();
            if (sdr.HasRows)
            {
                // Compare All Values in Table With Input Value 
                while (sdr.Read())
                {
                    // Get Valu From First Column 
                    DateTime stored_time = DateTime.Parse(sdr.GetValue(0).ToString());
                    if (input_time.CompareTo(stored_time) == 0)
                    {
                        ConnectionDB.CloseConnection();
                        return true;
                    }
                }
               
                ConnectionDB.CloseConnection();
                return false;
            }
            else
            {
                ConnectionDB.CloseConnection();
                return false;
            }
        }

        // Get All Daily Appointments Only
        public SqlDataReader Get_AllAppointments_Times(string Appointment_Date)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select AppointmentTime From Appointments Where AppointmentDate = @AppointmentDate", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@AppointmentDate", System.Data.SqlDbType.Date)).Value = Appointment_Date;
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Get All Appointments based On ContactNum
        public SqlDataReader Get_AllAppointments_ContactNum(string Contact_Num)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From Appointments  Where ContactNum = @ContactNum", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@ContactNum", Contact_Num));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }

        // Query For Adjus Identity After Delete Record
        public void Edit_Identity()
        {
            SqlDataReader sqlData = Get_AllAppointments();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Appointments, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(AppointmentId)  From Appointments", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (Appointments, RESEED, @AppointmentId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@AppointmentId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

        //Query For Count of Appointments
        public int Count_Appointments(string Appointment_Date)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select Count(*) From Appointments Where AppointmentDate = @AppointmentDate", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@AppointmentDate", System.Data.SqlDbType.Date)).Value = Appointment_Date;
            SqlDataReader sdr = cmd.ExecuteReader();
            sdr.Close();
            int count_Appointments = Convert.ToInt32(cmd.ExecuteScalar());
            ConnectionDB.CloseConnection();
            return count_Appointments;
        }
    }
}
