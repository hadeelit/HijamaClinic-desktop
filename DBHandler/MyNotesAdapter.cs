using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HijamaClinicManagement.DBHandler
{
    class MyNotesAdapter
    {
        public MyNotesAdapter()
        { 
        }

        // Insert New Note To MyNotes Table
        public bool Insert_NewNote(string N_Title, string N_Content, string N_Date, string Entry_Name, string N_Remindme)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Insert Into MyNotes Values (@MyNoteName ,@MyNoteContent ,@MyNoteDate ,@EntryName ,@RemindMe)", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@MyNoteName", N_Title));
            cmd.Parameters.Add(new SqlParameter("@MyNoteContent", N_Content));
            cmd.Parameters.Add(new SqlParameter("@MyNoteDate", System.Data.SqlDbType.Date)).Value = N_Date;
            cmd.Parameters.Add(new SqlParameter("@EntryName", Entry_Name));

            if(N_Remindme != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@RemindMe", System.Data.SqlDbType.Date)).Value = N_Remindme;
            else
                cmd.Parameters.Add(new SqlParameter("@RemindMe", DBNull.Value));

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

        // Get All Note For Specefic User
        public SqlDataReader Get_AllNotes(string username)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From MyNotes Where EntryName = @EntryName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@EntryName", username));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;

        }

        // Get All Note For all users
        public SqlDataReader Get_AllNotes()
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From MyNotes", ConnectionDB.con);
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;

        }

        //Get date For check daily note
        public SqlDataReader Get_date(string date,string user)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From MyNotes where RemindMe = @RemindMe and EntryName = @EntryName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@RemindMe", System.Data.SqlDbType.Date)).Value = date;
            cmd.Parameters.Add(new SqlParameter("@EntryName", user));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;
        }


        // Delete Specefic Note
        public bool Delete_Note(string note_id)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Delete From MyNotes Where MyNoteId = @MyNoteId", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@MyNoteId", System.Data.SqlDbType.Int)).Value = note_id;

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

        // Update Note
        public bool Update_Note(string id, string N_Title, string N_Content, string N_Date, string N_Remindme)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Update MyNotes Set MyNoteName = @MyNoteName , MyNoteContent = @MyNoteContent , MyNoteDate = @MyNoteDate , RemindMe = @RemindMe Where MyNoteId = @MyNoteId", ConnectionDB.con);

            cmd.Parameters.Add(new SqlParameter("@MyNoteName", N_Title));
            cmd.Parameters.Add(new SqlParameter("@MyNoteContent", N_Content));
            cmd.Parameters.Add(new SqlParameter("@MyNoteDate", System.Data.SqlDbType.Date)).Value = N_Date;

            if (N_Remindme != string.Empty)
                cmd.Parameters.Add(new SqlParameter("@RemindMe", System.Data.SqlDbType.Date)).Value = N_Remindme;
            else
                cmd.Parameters.Add(new SqlParameter("@RemindMe", DBNull.Value));

            cmd.Parameters.Add(new SqlParameter("@MyNoteId", id));

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

        // Query For Adjus Identity After Delete Record
        public void Edit_Identity()
        {
            SqlDataReader sqlData = Get_AllNotes();
            if (sqlData.HasRows == false)
            {
                SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (MyNotes, RESEED, 0)", ConnectionDB.con);
                sqlData.Close();
                cmd1.ExecuteNonQuery();
                ConnectionDB.CloseConnection();
            }
            else
            {
                SqlCommand cmd2 = new SqlCommand("Select Max(MyNoteId)  From MyNotes", ConnectionDB.con);
                sqlData.Close();
                int id = Convert.ToInt32(cmd2.ExecuteScalar());
                if (id > 0)
                {
                    SqlCommand cmd1 = new SqlCommand("DBCC CHECKIDENT (MyNotes, RESEED, @MyNoteId)", ConnectionDB.con);
                    cmd1.Parameters.Add(new SqlParameter("@MyNoteId", id));
                    cmd1.ExecuteNonQuery();
                    ConnectionDB.CloseConnection();
                }
            }
        }

        // Search About MyNoteTitle
        public SqlDataReader Search_Note(string Note_Title, string U_Name)
        {
            ConnectionDB.CheckConnection();
            SqlCommand cmd = new SqlCommand("Select * From MyNotes Where MyNoteName =@MyNoteName and EntryName = @EntryName", ConnectionDB.con);
            cmd.Parameters.Add(new SqlParameter("@MyNoteName", Note_Title));
            cmd.Parameters.Add(new SqlParameter("@EntryName", U_Name));
            SqlDataReader sdr = cmd.ExecuteReader();
            return sdr;

        }
    }
}
