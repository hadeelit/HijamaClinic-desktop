using Grpc.Core;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HijamaClinicManagement.ReportsViews
{
    /// <summary>
    /// Interaction logic for ZakatReport.xaml
    /// </summary>
    public partial class ZakatReport : UserControl
    {
        DBHandler.BillAdapter billAdapter = new DBHandler.BillAdapter();

        public ZakatReport()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Clear_zakat();
        }

        // Functionalties for Zakat panel Content
        private void Zakat_Export__Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PDF (*.pdf)|*.pdf";
            // Create Folder name 
            string newfolder = "تقارير الزكاة والدخل";
            // Create path for new folder 
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), newfolder);
            // create folder on Desktop
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (IOException ie)
                {
                    Console.WriteLine("IO Error: " + ie.Message);
                }
                catch (Exception ge)
                {
                    Console.WriteLine("General Error: " + ge.Message);
                }
            }
            // Count All files in current Direcyory
            // searches the current directory
            int fCount = Directory.GetFiles(path, "*", SearchOption.TopDirectoryOnly).Length;
            sfd.FileName = "Output" + (fCount + 1) + ".pdf";
            bool fileError = false;
            if (sfd.ShowDialog() == true)
            {
                if (File.Exists(sfd.FileName))
                {
                    try
                    {
                        File.Delete(sfd.FileName);
                    }
                    catch (IOException ex)
                    {
                        fileError = true;
                        MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                    }
                }
                try
                {
                    if (!fileError)
                    {
                        string celltext;
                        DataTable dt = new DataTable();
                        dt.Clear();
                        dt = ((DataView)Zakat_Data.ItemsSource).ToTable();
                        /*dt.Columns.Add(new DataColumn("تاريخ الفاتورة", typeof(DateTime)));
                        dt.Columns.Add(new DataColumn("الجنسية", typeof(string)));
                        dt.Columns.Add(new DataColumn("القيمة المالية", typeof(string)));*/

                        // Create instance of PdfPTable
                        iTextSharp.text.pdf.PdfPTable pdfTable = new PdfPTable(dt.Columns.Count)
                        {
                            RunDirection = PdfWriter.RUN_DIRECTION_RTL
                        };
                        //pdfTable.DefaultCell.Padding = 3;
                        pdfTable.WidthPercentage = 100;
                        pdfTable.HorizontalAlignment = Element.ALIGN_RIGHT;

                        //Full path to the Unicode Arial file
                        string ARIALUNI = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                        BaseFont bf = BaseFont.CreateFont(ARIALUNI, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                        iTextSharp.text.Font f = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.NORMAL);
                        //Header Table
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            celltext = dt.Columns[i].ColumnName;
                            iTextSharp.text.pdf.PdfPCell cell = new PdfPCell();
                            cell.Phrase = new Phrase(celltext, f);
                            cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#FFF97538"));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.PaddingBottom = 5;
                            cell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                            cell.Border = 0;
                            pdfTable.AddCell(cell);
                        }

                        // writing table Data
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                PdfPCell pdfcell = new PdfPCell();
                                pdfcell.Phrase = new Phrase(dt.Rows[i][j].ToString(), f);
                                pdfcell.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
                                pdfTable.AddCell(pdfcell);
                                //Align the cell in the center
                                pdfcell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                                pdfcell.VerticalAlignment = PdfPCell.ALIGN_CENTER;

                            }
                        }
                        using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                        {
                            // creating document object 
                            Document pdfDoc = new Document(PageSize.A4);
                            PdfWriter.GetInstance(pdfDoc, stream);
                            pdfDoc.Open();
                            pdfDoc.Add(pdfTable);
                            pdfDoc.Close();
                            stream.Close();
                        }

                        MessageBox.Show("تم استخراج الملف بنجاح", "Info");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error :" + ex.Message);
                }
            }
        }

        private void Clear_Date_Zakat_Button_Click(object sender, RoutedEventArgs e)
        {
            Clear_zakat();
        }

        private void Clear_zakat()
        {
            Zakat_DateFromField.SelectedDate = null;
            Zakat_DateToField.SelectedDate = null;
            Zakat_Data.ItemsSource = null;
            resultof_Zakat_Data.ItemsSource = null;
            Zakat_Export__Button.IsEnabled = false;
        }


        /*private void Zakat_Search__Button_Click(object sender, RoutedEventArgs e)
        {
            if (Zakat_DateFromField.SelectedDate != null)
            {
                decimal total_saudi = 0, total_nonsaudi = 0, total_Nationality=0 , SA=0 , FA=0;
                string date = "";
                int i = 0;
                DateTime fromdate = (DateTime)Zakat_DateFromField.SelectedDate;
                DateTime todate = (DateTime)Zakat_DateToField.SelectedDate;
                //decimal total_saudi = billAdapter.Get_SaudiValues_Bill(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"));
                SqlDataReader reader = billAdapter.Get_SaudiValues_Bill(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"));
               

                DataTable dt = new DataTable();
                dt.Clear();
                dt.Columns.Add("Date");
                dt.Columns.Add("SaudisAmount");
                dt.Columns.Add("ForignsAmount");
                dt.Columns.Add("TotalNationality");
                DataRow row = dt.NewRow();
                dt.Load(reader);
                while(i<dt.Rows.Count)
                {
                    date = dt.Rows[i].Field<string>(0);
                    total_saudi = Convert.ToDecimal(dt.Rows[i].Field<string>(1));
                    row["Date"] = date;
                    row["SaudisAmount"] = total_saudi;
                    dt.Rows.Add(row);
                    i++;
                }
                reader.Close();
                DBHandler.ConnectionDB.CloseConnection();
                SqlDataReader rdr = billAdapter.Get_NonSaudiValues_Bill(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"));
                while (rdr.HasRows && i < dt.Rows.Count)
                {
                    row["ForignsAmount"] = total_nonsaudi;
                    i++;
                }
                rdr.Close();
                DBHandler.ConnectionDB.CloseConnection();

                while(i < dt.Rows.Count)
                {
                    total_Nationality = total_saudi + total_nonsaudi;
                    row["TotalNationality"] = total_Nationality;
                    i++;
                }
                Zakat_Data.ItemsSource = dt.DefaultView;
                //result of All Zakat_Data Grid in one record 
                for(int j= 0;j <dt.Rows.Count;j++)
                {
                    SA = SA + (decimal)row["SaudisAmount"];
                    FA = FA + (decimal)row["ForignsAmount"];
                }
                decimal total = SA + FA;
                //Assignmen to Grid View
                var cell0 = resultof_Zakat_Data.SelectedCells[0];
                var cell1 = resultof_Zakat_Data.SelectedCells[1];
                var cell2 = resultof_Zakat_Data.SelectedCells[2];
                var cell3 = resultof_Zakat_Data.SelectedCells[3];

                (cell0.Column.GetCellContent(cell0.Item) as TextBlock).Text = Zakat_DateFromField.Text + "-" + Zakat_DateToField.Text;
                (cell1.Column.GetCellContent(cell1.Item) as TextBlock).Text = SA.ToString();
                (cell2.Column.GetCellContent(cell2.Item) as TextBlock).Text = FA.ToString();
                (cell3.Column.GetCellContent(cell3.Item) as TextBlock).Text = (SA + FA).ToString();
            }
            else
            {

            }

        }*/

        private void Zakat_Search__Button_Click(object sender, RoutedEventArgs e)
        {
            // Condetion for tow date 
            if (Zakat_DateFromField.SelectedDate == null && Zakat_DateToField.SelectedDate != null)
            {
                Zakat_Export__Button.IsEnabled = false;
                MessageBox.Show(" حقل من التاريخ يجب أن يكون ممتلئ فقط أو  كلا الحقلين", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                if (Zakat_DateFromField.SelectedDate != null || (Zakat_DateFromField.SelectedDate != null && Zakat_DateToField.SelectedDate != null))
                {

                    SqlDataReader reader1, reader2;
                    DateTime fromdate = ((DateTime)Zakat_DateFromField.SelectedDate).Date;
                    if (Zakat_DateToField.SelectedDate != null)
                    {
                        DateTime todate = ((DateTime)Zakat_DateToField.SelectedDate).Date;
                        //decimal total_saudi = billAdapter.Get_SaudiValues_Bill(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"));
                        reader1 = billAdapter.Get_AllValues_Bill(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        //decimal total_saudi = billAdapter.Get_SaudiValues_Bill(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"));
                        reader1 = billAdapter.Get_AllValues_Bill(fromdate.ToString("yyyy-MM-dd"));
                    }

                    DataTable dt1 = new DataTable();
                    dt1.Clear();
                    if (reader1.HasRows)
                    {
                        dt1.Load(reader1);
                        Zakat_Data.ItemsSource = dt1.DefaultView;
                        reader1.Close();
                        DBHandler.ConnectionDB.CloseConnection();
                        Zakat_Export__Button.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("قد يكون التاريخ المدخل غير موجود بعد", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Zakat_Export__Button.IsEnabled = false;
                        reader1.Close();
                        DBHandler.ConnectionDB.CloseConnection();
                    }


                    // Get Data for  resultof_Zakat_Data DataGrid
                    if (Zakat_DateToField.SelectedDate != null)
                    {
                        DateTime todate = ((DateTime)Zakat_DateToField.SelectedDate).Date;
                        //decimal total_saudi = billAdapter.Get_SaudiValues_Bill(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"))
                        reader2 = billAdapter.Get_AllValues_Bill_(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd"));

                    }
                    else
                    {
                        //decimal total_saudi = billAdapter.Get_SaudiValues_Bill(fromdate.ToString("yyyy-MM-dd"), todate.ToString("yyyy-MM-dd")
                        reader2 = billAdapter.Get_AllValues_Bill_(fromdate.ToString("yyyy-MM-dd"));
                    }

                    DataTable dt2 = new DataTable();
                    dt2.Clear();
                    if (reader2.HasRows)
                    {
                        dt2.Load(reader2);
                        resultof_Zakat_Data.ItemsSource = dt2.DefaultView;
                        reader2.Close();
                        DBHandler.ConnectionDB.CloseConnection();
                        Zakat_Export__Button.IsEnabled = true;
                    }
                    else
                    {
                        MessageBox.Show("قد يكون التاريخ المدخل غير موجود", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        reader2.Close();
                        DBHandler.ConnectionDB.CloseConnection();
                        Zakat_Export__Button.IsEnabled = false;
                    }
                }
                else
                {
                    Zakat_Export__Button.IsEnabled = false;
                    MessageBox.Show("أحد حقول التاريخ يجب أن تكون ممتلئة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }

            }

        }

    }
}
