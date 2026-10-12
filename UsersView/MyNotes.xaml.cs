using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace HijamaClinicManagement.UsersView
{
    /// <summary>
    /// Interaction logic for MyNotes.xaml
    /// </summary>
    public partial class MyNotes : UserControl
    {
        DBHandler.MyNotesAdapter note_adapter = new DBHandler.MyNotesAdapter();

        public MyNotes()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_Notes();
            DefaultData();
            LoadNotesGridView();
        }

        // Default Data For NoteDate in MyNotes View
        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            MyNoteDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);
        }

        //Add Delete Edie Buttons For MyNote Views
        private void AddButton_MyNote_Click(object sender, RoutedEventArgs e)
        {
            if (MyNotes_Isvalid())
            {
                string u_name = Sign_in.User;
                bool result = note_adapter.Insert_NewNote(MyNoteTitleField.Text, MyNoteContentField.Text, MyNoteDateField.Text, u_name, MyNoteDateReminderField.Text);
                if (result)
                {
                    MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    Clear_Notes();
                    DefaultData();
                    LoadNotesGridView();
                }
                else
                {
                    MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    Clear_Notes();
                    DefaultData();
                }
            }

        }

        private void DeleteButton_MyNote_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromNotesTable.SelectedItem != null)
            {
                if (MyNoteTitleField.Text == string.Empty || MyNoteContentField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد الحذف بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        var cell0 = DataViewFromNotesTable.SelectedCells[0];
                        string note_id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                        bool result = note_adapter.Delete_Note(note_id);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadNotesGridView();
                            Clear_Notes();
                            DefaultData();
                            note_adapter.Edit_Identity();
                        }
                        else
                        {
                            MessageBox.Show("فشلت عملية الحذف", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }

                    }
                    else
                    {
                        MessageBox.Show("تم الغاء الحذف ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
            }
            else
            {
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void EdieButton_MyNote_Click(object sender, RoutedEventArgs e)
        {

            // Detect if one of rows in GridView Selection Or not
            if (DataViewFromNotesTable.SelectedItem != null)
            {
                // If Not Detect Row Correct
                if (MyNoteTitleField.Text == string.Empty || MyNoteContentField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    MessageBox.Show("يجب ادخال  حقلي عنوان الملاحظة ومحتواها ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    if (MyNotes_Isvalid())
                    {
                        // MessageBox For Yes No Updated
                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الحقول بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            // Step 1 : Fetch Info For Specefic cell
                            var cell0 = DataViewFromNotesTable.SelectedCells[0];
                            // Step 2: get the content for this info Cell (MyNote_id Cell)
                            string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;

                            bool result = note_adapter.Update_Note(id, MyNoteTitleField.Text, MyNoteContentField.Text, MyNoteDateField.Text, MyNoteDateReminderField.Text);
                            if (result)
                            {
                                MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                LoadNotesGridView();
                                Clear_Notes();
                                DefaultData();
                            }
                            else
                            {
                                MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            }

                        }
                        else
                        {
                            MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }

                }
            }
            else
            {
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }

        }

        private void ClearButton_MyNote_Click(object sender, RoutedEventArgs e)
        {
            // Clear Fields in MyNotes View
            Clear_Notes();
        }

        private void ReloadGridButton_MyNote_Click(object sender, RoutedEventArgs e)
        {
            // Reload Grid After Search  in MyNotes View
            LoadNotesGridView();
        }

        private void LoadNotesGridView()
        {
            string u_name = Sign_in.User;
            SqlDataReader sdr = note_adapter.Get_AllNotes(u_name);
            DataTable dt = new DataTable("MyNotes");
            dt.Load(sdr);
            DataViewFromNotesTable.ItemsSource = dt.DefaultView;
            DBHandler.ConnectionDB.CloseConnection();
        }

        private void DataViewFromNotesTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cell1 = DataViewFromNotesTable.SelectedCells[1];
            var cell2 = DataViewFromNotesTable.SelectedCells[2];
            var cell3 = DataViewFromNotesTable.SelectedCells[3];
            var cell4 = DataViewFromNotesTable.SelectedCells[4];

            string n_title = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
            string n_content = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
            string n_date = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
            string n_remindme = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;

            Clear_Notes();
            MyNoteContentField.Text = n_content;
            MyNoteTitleField.Text = n_title;
            MyNoteDateField.SelectedDate = DateTime.Parse(n_date);
            if (n_remindme != string.Empty)
                MyNoteDateReminderField.SelectedDate = DateTime.Parse(n_remindme);

        }

        //Validition For Search Input
        private void SearchInput_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchInput.Text = "أدخل عنوان الملاحظة فقط";

        }
        //SearchMyNote Function
        private void SearchMyNote_Click(object sender, RoutedEventArgs e)
        {
            string u_name = Sign_in.User;
            string S_Input = SearchInput.Text;
            SqlDataReader sdr = note_adapter.Search_Note(S_Input, u_name);
            if (sdr.HasRows)
            {
                DataTable dt = new DataTable();
                dt.Load(sdr);
                //string title = sdr["MyNoteName"].ToString();
                DataViewFromNotesTable.ItemsSource = dt.DefaultView;
                SearchInput.Clear();
                DBHandler.ConnectionDB.CloseConnection();

            }
            else
            {
                DBHandler.ConnectionDB.CloseConnection();
                MessageBox.Show("لايوجد عنوان ملاحظة مطابقة !", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        //Function For Verify From MyNotes View  Fields
        private bool MyNotes_Isvalid()
        {
            if (MyNoteTitleField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                MyNoteTitleField.Focus();
                return false;
            }
            if (MyNoteContentField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                MyNoteContentField.Focus();
                return false;
            }
            if (MyNoteDateField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                MyNoteDateField.Focus();
                return false;
            }
            return true;
        }

        // Clear Values in MyNotes View  Fields
        private void Clear_Notes()
        {
            MyNoteTitleField.Clear();
            MyNoteContentField.Clear();
            MyNoteDateField.SelectedDate = null;
            MyNoteDateReminderField.SelectedDate = null;
        }

       
    }
}
