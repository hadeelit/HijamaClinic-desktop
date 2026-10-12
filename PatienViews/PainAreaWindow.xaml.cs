using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Shapes;
using System.IO;
using System.Data.SqlClient;
using System.Data;

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for PainAreaWindow.xaml
    /// </summary>
    public partial class PainAreaWindow : Window
    {
        // create value for check status of window
        public static bool open = false;
        // Create Value for Check if painarea image edited or not 
        private int check_editimg;
        // reference from PainAreasAdapter Class
        DBHandler.PainAreasAdapter painarea_adapter = new DBHandler.PainAreasAdapter();
        // Get SessionId Variable
        string session_id;
        // Get Session_Num
        string session_num;
        // Get PainAreaimg Path
        string imgpath;
        // Image For Background  When Delete Currently Img
        string img = @"C:\Users\PcMax\source\repos\HijamaClinicManagement\HijamaClinicManagement\bin\Debug\ImagesTreatmentPlan\Body.jpg";
        string path;
        public PainAreaWindow()
        {
            InitializeComponent();
            session_id = PatientsSessionView.s_id;
            session_num = PatientsSessionView.s_num;
        }

        //Dragable Window
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
        //Get Current Date and Time
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Check_PainArea_Info();
            check_editimg = 0;
        }

        // Exit From This  Pain Area Windpw 
        private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            open = true;
        }

        private void Check_PainArea_Info()
        {
       
            PatientFileNumField.Text = PatientsSessionView.FileNum;
            SessionNumField.Text = session_num;
            // Get Data To this window
            Get_PainAreaValues();
        }

        // Functions For Pain Area View
        private void AddButton_PainArea_Click(object sender, RoutedEventArgs e)
        {
           if(PainArea_IsValid())
            {
                if (check_editimg > 0)
                {
                   MessageBoxResult message = MessageBox.Show("هل تريد حفظ الصورة بالتأكيد لايمكن التعديل عليها لاحقا ؟ ", "تنبيه", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if(message == MessageBoxResult.Yes)
                    {
                           imgpath = Save_PainAreaImage(PatientFileNumField.Text, session_id);
                            bool result = painarea_adapter.Insert_PainArea(imgpath, ShowSliderValue.Content.ToString(), Diagnosis.Text, PatientFileNumField.Text, session_id);
                            if (result)
                            {
                                MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                                AddButton_PainArea.IsEnabled = false;
                                // Get PainAreaId
                                int PainArea = painarea_adapter.Get_MaxPainAreasId();
                                if (PainArea > 0)
                                {
                                    PainAreaNumField.Text = PainArea.ToString();
                                }
                                // For prevent editing on image
                                check_editimg = 0;
                                open = true;
                                Close();

                                // path = Get_ImgPath(session_id);

                            }
                            else
                            {
                                MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

                            }
                    }
                    else
                    {
                        MessageBox.Show("تم الغاء الحفظ ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }

                }
                else
                {
                    imgpath = "";
                    bool result = painarea_adapter.Insert_PainArea(imgpath, ShowSliderValue.Content.ToString(), Diagnosis.Text, PatientFileNumField.Text, session_id);
                    if (result)
                    {
                        MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        AddButton_PainArea.IsEnabled = false;
                        // Get PainAreaId
                        int PainArea = painarea_adapter.Get_MaxPainAreasId();
                        if (PainArea > 0)
                        {
                            PainAreaNumField.Text = PainArea.ToString();
                        }
                        open = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("فشلت عملية الاضافة ! أعد الادخال ", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

                    }
                }
            }
           
        }

        private void DeleteButton_PainArea_Click(object sender, RoutedEventArgs e)
        {
            if(PainAreaNumField.Text != string.Empty)
            {

                System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد حذف السجل التابع لملف المريض رقم  " + " " + ":" + PatientFileNumField.Text + "" + "والجلسة رقم :" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                if (((int)dialog) == 6)
                {
                    bool result = painarea_adapter.Delete_PainArea(PainAreaNumField.Text);
                    if (result)
                    {
                        MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        Clear_PainArea();
                        painarea_adapter.Edit_Identity_PainAreas();
                        open = true;
                       
                        // Delete Image from ImagesPainAreas Folder
                        if (imgpath != null)
                        {
                            // Adjust Background For Can Delete Currently Img
                            //Cnv.Background = new ImageBrush(new BitmapImage(new Uri(img)));
                            Delete_PainAreaImg(imgpath);
                        }

                        Close();
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
            else
            {
                MessageBox.Show("لايمكن الحذف  رقم السجل غير موجود !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void EdieButton_PainArea_Click(object sender, RoutedEventArgs e)
        {
            if (PainAreaNumField.Text != string.Empty)
            {
                if (imgpath != null)
                {
                    if(check_editimg > 0)
                    {
                        MessageBox.Show("لايمكن التعديل على الصورة تم حفظها مسبقا سوف يتم اهمال التعديلات", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث السجل التابع لملف المريض رقم  " + " " + ":" + PatientFileNumField.Text + "" + "والجلسة رقم :" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = painarea_adapter.Update_PainAreas(PainAreaNumField.Text, imgpath, ShowSliderValue.Content.ToString(), Diagnosis.Text);
                        if (result)
                        {
                            MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

                        }
                        else
                        {
                            MessageBox.Show("فشلت عملية التحديث", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }
                    else
                    {
                        MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }

                }
                else
                {
                    if (check_editimg > 0)
                        imgpath = Save_PainAreaImage(PatientFileNumField.Text, session_id);
                    else
                        imgpath = "";

                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث السجل التابع لملف المريض رقم  " + " " + ":" + PatientFileNumField.Text + "" + "والجلسة رقم :" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = painarea_adapter.Update_PainAreas(PainAreaNumField.Text, imgpath, ShowSliderValue.Content.ToString(), Diagnosis.Text);
                        if (result)
                        {
                            MessageBox.Show("تم التحديث بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                        else
                        {
                            MessageBox.Show("فشلت عملية التحديث", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }
                    else
                    {
                        imgpath = null;
                        MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                }

            }
            else
            {
                MessageBox.Show("لايمكن التحديث  رقم السجل غير موجود !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }

        }

        // Function For Create Painareas Image from Canva and save it to PainArea Folder in Debug Foldder in Project
        private string Save_PainAreaImage(string PatientFileNum, string Session_Id)
        {
            // Get the size of the Visual and its descendants step 1
            Rect rect = VisualTreeHelper.GetDescendantBounds(Cnv);
            // Make a DrawingVisual to make a screen step 2 [all pixels on Canva Drawing on rect] 
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(Cnv);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }
            // Make a bitmap and draw on it For getting final image from Canva step 3
            // Convert Visual imagin to Bitmap
            RenderTargetBitmap render = new RenderTargetBitmap((int)Cnv.Width, (int)Cnv.Height, 96, 96, PixelFormats.Default);
            render.Render(dv);
            // Make a PNG encoder Step 4 Convert bitmap ti PNG image
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(render));

            // Save PainArea Image to ImagesPainArea Folder in Debug Foldder in Project
            /* if (!Directory.Exists(@"C:\Program Files (x86)\Eng.HadeelKhalili\HijamaAppSetup\ImagesPainArea"))
                 Directory.CreateDirectory(@"C:\Program Files (x86)\Eng.HadeelKhalili\HijamaAppSetup\ImagesPainArea");
             // Get Full Path for Directory
             string dirpath = Directory.GetCurrentDirectory();
            */
            // imgpath = dirpath + @"\ImagesPainArea\" + PatientFileNumField.Text + Session_Id + ".png";

            // imgpath = @"C:\HijamaAppImages\ImagesPainArea\" + PatientFileNumField.Text + Session_Id + ".png";
            //new edit move from C folder to F(ssd) folder
            imgpath = @"F:\HijamaAppImages\ImagesPainArea\" + PatientFileNumField.Text + Session_Id + ".png";
            if (File.Exists(imgpath))
            {
                File.Delete(imgpath);
                FileStream fs = File.Create(imgpath);
                encoder.Save(fs);
                fs.Close();
            }
            else
            {
                FileStream fs = File.Create(imgpath);
                encoder.Save(fs);
                fs.Close();
            }
          
            return imgpath;
        }
        //  Function For Drawing On Pain Areas Image 
        private void ImgPainAreas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Remove Ellipse when Click on it
            if (e.OriginalSource is Ellipse)
            {
                Ellipse actellipse = (Ellipse)e.OriginalSource;
                Cnv.Children.Remove(actellipse);
                check_editimg--;
            }

            // Add Ellipse on Specific Cordinates in Image
            else
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 9;
                ellipse.Height = 9;
                ellipse.Fill = new SolidColorBrush(Colors.Red);
                ellipse.StrokeThickness = 2;
                _ = Cnv.Children.Add(ellipse);

                Canvas.SetLeft(ellipse, e.GetPosition(Cnv).X);
                Canvas.SetTop(ellipse, e.GetPosition(Cnv).Y);
                ellipse.Margin = new Thickness(0, 0, 20, 0);
                check_editimg++;
            }

        }

        //Get PainArea Values From PainArea Table for specefic seesion and FileNum
        private void Get_PainAreaValues()
        {
            SqlDataReader sdr = painarea_adapter.Get_PainArea(PatientFileNumField.Text, session_id);
            if (sdr.HasRows == false)
            {
                DBHandler.ConnectionDB.CloseConnection();
                AddButton_PainArea.IsEnabled = true;
                MessageBox.Show("لا يوجد سجل أماكن الألم بعد لهذه الجلسة !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
                AddButton_PainArea.IsEnabled = false;
                DataTable dt = new DataTable();
                dt.Load(sdr);
                // Column 1
                string painarea_id = dt.Rows[0].Field<int>(0).ToString();
                PainAreaNumField.Text = painarea_id;
                // Column 2
                /*imgpath = dt.Rows[0].Field<string>(1);
                if(imgpath != null)
                {
                    Cnv.Background = new ImageBrush(new BitmapImage(new Uri(imgpath)));
                }*/
                //(new edit)   Move from c folder to f folder
                imgpath = @"F:\HijamaAppImages\ImagesPainArea\" + PatientFileNumField.Text + session_id + ".png";
                Cnv.Background = new ImageBrush(new BitmapImage(new Uri(imgpath)));
                // Column 3
                string painseverity = dt.Rows[0].Field<int>(2).ToString();
                PainSeverityValue.Value = int.Parse(painseverity);
                // Column 4
                string diagnosis = dt.Rows[0].Field<string>(3);
                if(diagnosis != string.Empty)
                Diagnosis.Text = diagnosis;
                DBHandler.ConnectionDB.CloseConnection();

            }
        }

        // Delete PainAreaImg from PainAreaImages Folder when Delete  Specefic painArea Record
        public static void Delete_PainAreaImg(string imgpath)
        {
            try
            {
                File.Delete(imgpath);
            }
            catch(IOException)
            {
                //MessageBox.Show("" + e);
                MessageBox.Show("فضلا أغلق البرنامج حتى تكتمل عملية الحذف");
            }
        }

        // Get Only Path Or NULL For Img (Not Used)
        private string Get_ImgPath(string s_id)
        {
            SqlDataReader sdr = painarea_adapter.Get_ImgPath(s_id);
            DataTable dt = new DataTable();
            dt.Load(sdr);
            path = dt.Rows[0].Field<string>(1);
            if(path != null)
            {
                path = "FoundPath";
            }
            else
            {
                path = "EmpityPath";
            }
            DBHandler.ConnectionDB.CloseConnection();
            return path;
        }

        private void PrintButton_PainArea_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فضلاَ قم بعملية الطباعة لهذه الصفحة بشكل أفقي ");
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.PrintVisual(PanelPainAreas, "طباعة نقاط الألم");
            }
            else
            {
                MessageBox.Show("تم الغاء عملية الطباعة  ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        
        // Validation Function
        private bool PainArea_IsValid()
        {
            if (Diagnosis.Text == string.Empty)
            {
                MessageBox.Show(" فضلا املأ حقل التشخيص", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return false;
            }
            if (check_editimg == 0)
            {
                MessageBox.Show("فضلا ضع نقاط حول أماكن الألم ", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return false;
            }
            return true;
        }
        // Clear Function 
        public void Clear_PainArea()
        {
           // Cnv.Background = new ImageBrush(new BitmapImage(new Uri(@"component/Images/جسم.jpg", UriKind.RelativeOrAbsolute)));
            PainSeverityValue.Value = 5;
            Diagnosis.Clear();
        }

       
    }
}
