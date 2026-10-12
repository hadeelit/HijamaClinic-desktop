using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HijamaClinicManagement.ClinicSettingsViews
{
    /// <summary>
    /// Interaction logic for TaxSettings.xaml
    /// </summary>
    public partial class TaxSettings : UserControl
    {
        DBHandler.TaxAdapter tax_adapter = new DBHandler.TaxAdapter();
        

        public TaxSettings()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ClearTaxes();
            DefaultData();
            LoadTaxesGridViw();
        }

        //Load Default Data in fields when Load Window
        private void DefaultData()
        {
            DateTime now = DateTime.Now;
            TaxDateField.SelectedDate = DateTime.Parse(now.Day + "-" + now.Month + "-" + now.Year);
        }

        //Finctions For Add + Delete + Edit  (TaxSettingsSettings)
        private void AddButton_TaxSettings_Click(object sender, RoutedEventArgs e)
        {
            if (TaxesIsValid())
            {
                bool result = tax_adapter.Insert_Tax(TaxDateField.Text, NonSaudianTaxRateField.Text, SaudianTaxRateField.Text);
                if (result)
                {
                    MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    ClearTaxes();
                    DefaultData();
                    LoadTaxesGridViw();
                    AddButton_TaxSettings.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    ClearTaxes();
                }
            }
        }

        private void DeleteButton_TaxSettings_Click(object sender, RoutedEventArgs e)
        {
            if (DataViewFromTaxTable.SelectedItem != null)
            {
                if (SaudianTaxRateField.Text == string.Empty && NonSaudianTaxRateField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

                }
                else
                {
                    // Step 1 : Fetch Info For  Specefic cell
                    var cell0 = DataViewFromTaxTable.SelectedCells[0];
                    // Step 2: get the content for this info Cell (Taxid Cell)
                    string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;
                    // MessageBox For Yes No Deleted
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد الحذف بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        // Call Delete_Discount Function For delete this record
                        bool result = tax_adapter.Delete_Tax(id);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            DataViewFromTaxTable.ItemsSource = null;
                            ClearTaxes();
                            DefaultData();
                            tax_adapter.Edit_Identity();
                            AddButton_TaxSettings.IsEnabled = true;
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

        private void EdieButton_TaxSettings_Click(object sender, RoutedEventArgs e)
        {

            // Detect if one of rows in GridView Selection Or not
            if (DataViewFromTaxTable.SelectedItem != null)
            {
                // If Not Detect Row Correct
                if (SaudianTaxRateField.Text == string.Empty && NonSaudianTaxRateField.Text == string.Empty)
                {
                    MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    MessageBox.Show("يجب ادخال أحد حقلي الضريبة ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    if (TaxesIsValid())
                    {
                        // Step 1 : Fetch Info For Specefic cell
                        var cell0 = DataViewFromTaxTable.SelectedCells[0];
                        // Step 2: get the content for this info Cell (Tax_id Cell)
                        string id = (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text;

                        // MessageBox For Yes No Updated
                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث الحقول بالتأكيد", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            // Call Update_Taxes()
                            bool result = tax_adapter.Update_Tax(id, TaxDateField.Text, NonSaudianTaxRateField.Text, SaudianTaxRateField.Text);
                            if (result)
                            {
                                MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                LoadTaxesGridViw();
                                ClearTaxes();
                                DefaultData();
                            }
                            else
                            {
                                MessageBox.Show("فشلت عملية التحديث !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            }
                        }
                        else
                            MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }
            }
            else
            {
                MessageBox.Show("حدد سطراً من الجدول بالنقر مرتين", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void ClearButton_TaxSettings_Click(object sender, RoutedEventArgs e)
        {
            ClearTaxes();
            DefaultData();
        }

        //Load DataGrid from Taxes Table In DB when Load View
        private void LoadTaxesGridViw()
        {
            //Fetch Data from Taxes Table
            SqlDataReader sdr = tax_adapter.Taxes_Get();
            if (sdr.HasRows == false)
            {
                AddButton_TaxSettings.IsEnabled = true;
                DBHandler.ConnectionDB.CloseConnection();
            }
            else
            {
                AddButton_TaxSettings.IsEnabled = false;
                DataTable dt = new DataTable("Taxes");
                dt.Load(sdr);
                DataViewFromTaxTable.ItemsSource = dt.DefaultView;
                DBHandler.ConnectionDB.CloseConnection();
            }
        }

        // Get Data From TaxesGridView To Fields When Click on Row              
        private void DataViewTaxes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // All Fields in Discounts Table
            var cell1 = DataViewFromTaxTable.SelectedCells[1];
            var cell2 = DataViewFromTaxTable.SelectedCells[2];
            var cell3 = DataViewFromTaxTable.SelectedCells[3];

            string date = (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text;
            string non_saudis_rate = (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text;
            string saudis_rate = (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text;


            // For Clear Previous Data in Fields When Clicked
            ClearTaxes();
            TaxDateField.SelectedDate = DateTime.Parse(date);
            if (non_saudis_rate != "")
            {
                NonSaudianTaxRateField.Text = non_saudis_rate;
            }
            if (saudis_rate != "")
            {
                SaudianTaxRateField.Text = saudis_rate;
            }
        }

        //Restrict Input Data in TextBoxes (Discount - Tax - TotalPaidPrice - Price - RemainingUnPaid) 
        private void Decimal_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            //Recognize Digites + dot character + Comma character nad % character from others Characters (Decimal Input)
            Char ch = e.Text[0];
            String TextBoxValue = (((TextBox)e.Source).Text).ToString();
            if (Char.IsDigit(ch) || ch == '.' || ch == ',' || ch == '%')
            {
                if ((ch == '.' && TextBoxValue.Contains('.')))
                {
                    MessageBox.Show("ادخال غير صحيح", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
                if ((ch == '%' && TextBoxValue.Contains('%')))
                {
                    MessageBox.Show("ادخال غير صحيح", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
                if (ch == ',')
                {
                    MessageBox.Show("لايسمح بالفاصلة ", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    // when previous condition is correct => prevent achive this character 
                    e.Handled = true;
                }
            }
            else
            {

                MessageBox.Show("يسمح بادخال الأرقام", "تحذير", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                e.Handled = true;
            }
        }

        // Verify Taxes Fields  TextBoxes
        private bool TaxesIsValid()
        {

            if (TaxDateField.Text == string.Empty)
            {
                MessageBox.Show("هذا الحقل اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                TaxDateField.Focus();
                return false;
            }
            if (SaudianTaxRateField.Text == string.Empty && NonSaudianTaxRateField.Text == string.Empty)
            {
                MessageBox.Show("يجب ادخال  حقلي الضريبة بالنسبة المئوية ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return false;
            }
            if (SaudianTaxRateField.Text == string.Empty || NonSaudianTaxRateField.Text == string.Empty)
            {
                MessageBox.Show("يجب ادخال  حقلي الضريبة بالنسبة المئوية ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return false;
            }
            if (SaudianTaxRateField.Text != string.Empty)
            {
                if (!SaudianTaxRateField.Text.Contains('%'))
                {
                    MessageBox.Show("يجب أن تكون القيمة المدخلة نسبة مئوية فقط ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    SaudianTaxRateField.Focus();
                    return false;
                }
            }
            if (NonSaudianTaxRateField.Text != string.Empty)
            {
                if (!NonSaudianTaxRateField.Text.Contains('%'))
                {
                    MessageBox.Show("يجب أن تكون القيمة المدخلة نسبة مئوية فقط ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    NonSaudianTaxRate.Focus();
                    return false;
                }
            }

            return true;
        }

        //Clear Data from Taxes View
        private void ClearTaxes()
        {
            TaxDateField.SelectedDate = null;
            SaudianTaxRateField.Clear();
            NonSaudianTaxRateField.Clear();

        }

    }
}
