using ClosedXML.Excel;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Net.Mail;
using System.Net;
using Microsoft.Win32;

namespace HijamaClinicManagement.ClinicManagementViews
{
    /// <summary>
    /// Interaction logic for ExpensesView.xaml
    /// </summary>
    public partial class ExpensesView : UserControl
    {
        DBHandler.ExpenseAdapter expense_adapter = new DBHandler.ExpenseAdapter();

        public ExpensesView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_Expense();
            DefaultData();
            LoadDataGridViw();
        }

        //Handler Functions For Add + Delete + Edit + Print + Extract Excel in ExpenseMg View
        private void EXpenseSearchBillNum_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchExpenseBillNum.Text = "أدخل رقم فاتورة النفقة فقط";
        }

        private void EXpenseSearchName_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchExpenseName.Text = "أدخل اسم النفقة فقط";
        }

        //Restrict Input Data in TextBoxes (Discount - Tax - TotalPaidPrice - Price - RemainingUnPaid ... etc) 
        private void Decimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            //Recognize Digites + dot character and  Comma character from others Characters (Decimal Input)
            Char ch = e.Text[0];
            String TextBoxValue = ((System.Windows.Controls.TextBox)e.Source).Text.ToString();
            if (Char.IsDigit(ch) || ch == '.' || ch == ',')
            {
                if ((ch == '.' && TextBoxValue.Contains('.')))
                {
                    System.Windows.MessageBox.Show("ادخال غير صحيح", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, System.Windows.MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
                if (ch == ',')
                {
                    System.Windows.MessageBox.Show("لايسمح بالفاصلة ", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, System.Windows.MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
            }
            else
            {

                System.Windows.MessageBox.Show("يسمح بادخال الأرقام", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, System.Windows.MessageBoxOptions.RtlReading);
                e.Handled = true;
            }
        }

        //Load Data from Table to DATA Grid View Expenses
        private void LoadDataGridViw()
        {
            SqlDataReader sdr = expense_adapter.Get_Expenses();
            DataTable dt = new DataTable();
            dt.Load(sdr);
            DataViewFromExpenseTable.ItemsSource = dt.DefaultView;
            DBHandler.ConnectionDB.CloseConnection();
        }

        private void DataViewFromExpenseTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var cell1 = DataViewFromExpenseTable.SelectedCells[1];
            var cell2 = DataViewFromExpenseTable.SelectedCells[2];
            var cell3 = DataViewFromExpenseTable.SelectedCells[3];
            var cell4 = DataViewFromExpenseTable.SelectedCells[4];
            var cell5 = DataViewFromExpenseTable.SelectedCells[5];
            var cell6 = DataViewFromExpenseTable.SelectedCells[6];

            string exp_billnum = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
            string exp_name = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
            string exp_date = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;
            string exp_amount = (cell4.Column.GetCellContent(cell4.Item) as TextBlock).Text;
            string exp_tax = (cell5.Column.GetCellContent(cell5.Item) as TextBlock).Text;
            string exp_totalpricepaid = (cell6.Column.GetCellContent(cell6.Item) as TextBlock).Text;

            Clear_Expense();

            if (exp_billnum != string.Empty)
                ExpenseBillNumField.Text = exp_billnum;
            ExpenseNameField.Text = exp_name;
            ExpenseDateField.SelectedDate = DateTime.Parse(exp_date);
            if (exp_amount != string.Empty)
                ExpensequantityField.Text = exp_amount;
            if (exp_tax != string.Empty)
                ExpenseTaxField.Text = exp_tax;
            TotalPaidPriceField.Text = exp_totalpricepaid;
        }

        private void AddButton_ExpenseMg_Click(object sender, RoutedEventArgs e)
        {
            string entry_name = Sign_in.User;
            if (Expense_Isvalid())
            {
                bool result = expense_adapter.Insert_Expense(ExpenseBillNumField.Text, ExpenseNameField.Text, ExpenseDateField.Text, ExpensequantityField.Text, ExpenseTaxField.Text, TotalPaidPriceField.Text, entry_name);
                if (result)
                {
                    MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    Clear_Expense();
                    DefaultData();
                    LoadDataGridViw();
                }
                else
                {
                    MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    Clear_Expense();
                    DefaultData();
                }
            }
        }

        private void DeleteButton_ExpenseMg_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromExpenseTable.SelectedItem != null)
            {
                if (ExpenseNameField.Text == string.Empty || ExpenseDateField.Text == string.Empty || TotalPaidPriceField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد الحذف بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        var cell0 = DataViewFromExpenseTable.SelectedCells[0];
                        string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;

                        bool result = expense_adapter.Delete_Expense(id);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            LoadDataGridViw();
                            Clear_Expense();
                            DefaultData();
                            expense_adapter.Edit_Identity();
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

        private void EdieButton_ExpenseMg_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromExpenseTable.SelectedItem != null)
            {
                if (ExpenseNameField.Text == string.Empty || ExpenseDateField.Text == string.Empty || TotalPaidPriceField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    MessageBox.Show("حقل الاسم والتاريخ والسعر لايمكن أن تكون فارغة ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    if (Expense_Isvalid())
                    {
                        // MessageBox For Yes No Updated
                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الحقول بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            // Step 1 : Fetch Info For Specefic cell
                            var cell0 = DataViewFromExpenseTable.SelectedCells[0];
                            // Step 2: get the content for this info Cell (MyNote_id Cell)
                            string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                            // Get Current SessionUser
                            string entry_name = Sign_in.User;

                            bool result = expense_adapter.Update_Expense(id, ExpenseBillNumField.Text, ExpenseNameField.Text, ExpenseDateField.Text, ExpensequantityField.Text, ExpenseTaxField.Text, TotalPaidPriceField.Text, entry_name);
                            if (result)
                            {
                                MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                LoadDataGridViw();
                                Clear_Expense();
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

        private void ClearButton_ExpenseMg_Click(object sender, RoutedEventArgs e)
        {
            Clear_Expense();
            DefaultData();
        }

        // Extract Data from Gridview to Excel file
        private void ExcelExtractButton_ExpenseMg_Click(object sender, RoutedEventArgs e)
        {
            DateTime now = DateTime.Now;
            System.Windows.Forms.FolderBrowserDialog folderBrowser;

            if (DataViewFromExpenseTable.ItemsSource == null)
            {
                MessageBox.Show("لايوجد أي بيانات لاكمال الاستخراج إلى  Excel", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return;
            }
            else
            {
                var wb = new XLWorkbook();
                DataTable dt = Get_ExpenseGV();
                //DBHandler.ConnectionDB.CloseConnection();
                string extract_date = now.Day + "-" + now.Month + "-" + now.Year;
                string file_name = "المصروفات" + "" + extract_date;
                var style_worksheet = wb.Worksheets.Add(dt, file_name);

                style_worksheet.Columns().AdjustToContents();  // Adjust column width
                style_worksheet.Rows().AdjustToContents();     // Adjust row heights

                //Rename Columns Names in Excel File
                style_worksheet.Cell(1, 1).SetValue("رقم النفقة");
                style_worksheet.Cell(1, 2).SetValue("رقم فاتورة النفقة");
                style_worksheet.Cell(1, 3).SetValue("اسم النفقة");
                style_worksheet.Cell(1, 4).SetValue("تاريخ النفقة");
                style_worksheet.Cell(1, 5).SetValue("الكمية");
                style_worksheet.Cell(1, 6).SetValue("الضريبة");
                style_worksheet.Cell(1, 7).SetValue("السعر الإجمالي المدفوع");
                style_worksheet.Cell(1, 8).SetValue("اسم المدخل");

                var style_cell = style_worksheet.Style;
                style_cell.Font.Bold = true;
                style_cell.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                style_worksheet.Cell(1, 6).Style.Font.FontSize = 10;
                style_worksheet.Cell(1, 7).Style.Font.FontSize = 10;
                style_worksheet.Column(6).AdjustToContents();
                style_worksheet.Column(7).AdjustToContents();
                style_worksheet.Columns(1, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;


                folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string location = folderBrowser.SelectedPath;
                    wb.SaveAs(location + @"\" + file_name + ".xlsx");
                    MessageBox.Show("تم الاستخراج بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    MessageBox.Show("يجب تحديد مكان لحفظ الملف", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                }
            }
        }

        // Search About expensename or expensebillnum or both (not used)
        private void SearchEXpense_Click(object sender, RoutedEventArgs e)
        {
            if (SearchExpenseBillNum.Text != string.Empty || SearchExpenseName.Text != string.Empty)
            {
                if (SearchExpenseBillNum.Text != string.Empty && SearchExpenseName.Text == string.Empty)
                {
                    SqlDataReader sdr = expense_adapter.Search_ExpenseBillNum(SearchExpenseBillNum.Text);
                    if (sdr.HasRows)
                    {
                        Load_Search(sdr);
                        SearchExpenseBillNum.Clear();
                        DBHandler.ConnectionDB.CloseConnection();

                    }
                    else
                    {
                        MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلةرقم فاتورة النفقة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        SearchExpenseBillNum.Clear();
                        DBHandler.ConnectionDB.CloseConnection();
                        return;
                    }
                }
                if (SearchExpenseBillNum.Text == string.Empty && SearchExpenseName.Text != string.Empty)
                {
                    SqlDataReader sdr = expense_adapter.Search_ExpenseName(SearchExpenseName.Text);
                    if (sdr.HasRows)
                    {
                        Load_Search(sdr);
                        SearchExpenseName.Clear();
                        DBHandler.ConnectionDB.CloseConnection();
                    }
                    else
                    {
                        MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلة اسم النفقة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        SearchExpenseName.Clear();
                        DBHandler.ConnectionDB.CloseConnection();
                        return;
                    }
                }
                if (SearchExpenseBillNum.Text != string.Empty && SearchExpenseName.Text != string.Empty)
                {
                    MessageBox.Show("أدخل أحد الحقلين  فقط لتتم عملية البحث", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
            }
            else
            {
                MessageBox.Show("حقلي البحث فارغان أدخل أحدهما", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        // Search About   expensebillnum
        private void SearchEXpenseBillNum_Click(object sender, RoutedEventArgs e)
        {
            if (SearchExpenseBillNum.Text != string.Empty && SearchExpenseBillNum.Text != "أدخل رقم فاتورة النفقة فقط")
            {
                SqlDataReader sdr = expense_adapter.Search_ExpenseBillNum(SearchExpenseBillNum.Text);
                if (sdr.HasRows)
                {
                    Load_Search(sdr);
                    SearchExpenseBillNum.Clear();
                    DBHandler.ConnectionDB.CloseConnection();

                }
                else
                {
                    MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلةرقم فاتورة النفقة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SearchExpenseBillNum.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    return;
                }
            }
            else
            {
                MessageBox.Show("حقل رقم فاتورة النفقة فارغ أدخل قيمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }
        // Search About expensename
        private void SearchEXpenseName_Click(object sender, RoutedEventArgs e)
        {
            if (SearchExpenseName.Text != string.Empty)
            {
                SqlDataReader sdr = expense_adapter.Search_ExpenseName(SearchExpenseName.Text);
                if (sdr.HasRows)
                {
                    Load_Search(sdr);
                    SearchExpenseName.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                }
                else
                {
                    MessageBox.Show("لايوجد نتائج بحث مطابقة للقيمة المدخلة اسم النفقة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SearchExpenseName.Clear();
                    DBHandler.ConnectionDB.CloseConnection();
                    return;
                }
            }
            else
            {
                MessageBox.Show("حقل اسم النفقة فارغ أدخل قيمة", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }

        }

        // LoadData For Search
        private void Load_Search(SqlDataReader sdr)
        {
            DataTable dt = new DataTable();
            dt.Load(sdr);
            DataViewFromExpenseTable.ItemsSource = dt.DefaultView;
        }

        // Get Data From Expense GridView To DataTable
        private DataTable Get_ExpenseGV()
        {
            DataTable dt = new DataTable();
            dt = ((DataView)DataViewFromExpenseTable.ItemsSource).ToTable();
            return dt;
        }

        //loade all data to my account when loaded 
        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            ExpenseDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);
        }

        // Validation Functions
        private bool Expense_Isvalid()
        {
            if (ExpenseNameField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                ExpenseNameField.Focus();
                return false;
            }
            if (ExpenseDateField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                ExpenseDateField.Focus();
                return false;
            }
            if (TotalPaidPriceField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                TotalPaidPriceField.Focus();
                return false;
            }
            return true;
        }

        // Clear Function
        private void Clear_Expense()
        {
            ExpenseBillNumField.Clear();
            ExpenseNameField.Clear();
            ExpenseDateField.SelectedDate = null;
            ExpensequantityField.Clear();
            ExpenseTaxField.Clear();
            TotalPaidPriceField.Clear();
        }

    }
}
