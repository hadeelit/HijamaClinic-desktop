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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace HijamaClinicManagement.PatienViews
{
    /// <summary>
    /// Interaction logic for TreatmentPlanWindow.xaml
    /// </summary>
    public partial class TreatmentPlanWindow : Window
    {
        private BillExtractWindow billExtract;
        public static bool open = false;
        // Create Value for Check if CuppingPointsInSession image edited or not 
        private int check_editimg;
        // reference from TreatmentPlanAdapter Class
        DBHandler.TreatmentPlanAdapter treatmentplan_adapter = new DBHandler.TreatmentPlanAdapter();
        // Get SessionId Variable
        string session_id;
        // Get Session_Num
        string session_num;
        // Get CuppingPointsInSession Path
        string cupping_point_path;
        // Image For Background  When Delete Currently Img
        string img = @"C:\Users\PcMax\source\repos\HijamaClinicManagement\HijamaClinicManagement\bin\Debug\ImagesTreatmentPlan\Body.jpg";
        // Variables for RadioButtons Is Checked or not
        private bool dry = false, bloody = false, sliding = false, flashing = false;
        

        public TreatmentPlanWindow()
        {
            InitializeComponent();
            session_id = PatientsSessionView.s_id;
            session_num = PatientsSessionView.s_num;
            // configuration Bill Extract window variable for moving between windows
            billExtract = null;
            BillExtractWindow.open = false;

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
            Check_TreatmentPlan_Info();
            check_editimg = 0;
            DefaultData();
        }

        // Exit From This  TreatmentPlan Window 
        private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
            open = true;
        }


        //Load Session_Date to Treatment_Date
        private void DefaultData()
        {
            TreatmentDate.SelectedDate = DateTime.Parse(PatientsSessionView.session_date);
        }


        // Functions For Tratment Plan View
        // Number Of Visits Fields (Allow onlu number input)
        private void TSessionsNumField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void Check_TreatmentPlan_Info()
        {
            PatientFileNumField.Text = PatientsSessionView.FileNum;
            SessionNumField.Text = session_num;
            // Get Data To this window
            Get_TreatmentPlanValues();
        }

        private void AddButton_TreatmentP_Click(object sender, RoutedEventArgs e)
        {
            if(TreatmentPlan_IsValid())
            {
                string dry_c = "", bloody_c = "", sliding_c = "", flashing_c = "";
                bool result;
                // Check What RadioButton IsChecked
                if (dry)
                {
                    dry_c = "1";
                }
                if (bloody)
                {
                    bloody_c = "1";
                }
                if (sliding)
                {
                    sliding_c = "1";
                }
                if (flashing)
                {
                    flashing_c = "1";
                }

                if (check_editimg > 0)
                {
                    MessageBoxResult message = MessageBox.Show("هل تريد حفظ الصورة بالتأكيد لايمكن التعديل عليها لاحقا ؟ ", "تنبيه", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (message == MessageBoxResult.Yes)
                    {
                        cupping_point_path = Save_TreatmentPointsImg(PatientFileNumField.Text, session_id);
                        result = treatmentplan_adapter.Insert_TreatmentPlan(TreatmentDate.Text, TSessionsNumField.Text, dry_c, bloody_c, sliding_c, flashing_c, DurationBCuppingField.Text, DateOfNextVisit.Text,cupping_point_path, PatientFileNumField.Text, session_id);
                        if(result)
                        {
                            MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            AddButton_TreatmentP.IsEnabled = false;
                            // Get TreatmentPlanId
                            int treatmentplanid = treatmentplan_adapter.Get_MaxTreatmentPlanId();
                            if (treatmentplanid > 0)
                            {
                                TreatmentPlanNumField.Text = treatmentplanid.ToString();
                            }
                            // For prevent editing on image
                            check_editimg = 0;
                            open = true;
                            Close();
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
                    cupping_point_path = "";
                    result = treatmentplan_adapter.Insert_TreatmentPlan(TreatmentDate.Text, TSessionsNumField.Text, dry_c, bloody_c, sliding_c, flashing_c, DurationBCuppingField.Text, DateOfNextVisit.Text, cupping_point_path, PatientFileNumField.Text, session_id);
                    if (result)
                    {
                        MessageBox.Show("تمت الاضافة بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        AddButton_TreatmentP.IsEnabled = false;
                        // Get TreatmentPlanId
                        int treatmentplanid = treatmentplan_adapter.Get_MaxTreatmentPlanId();
                        if (treatmentplanid > 0)
                        {
                            TreatmentPlanNumField.Text = treatmentplanid.ToString();
                        }
                        // For prevent editing on image
                        check_editimg = 0;
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

        //  Function For Drawing On Treatment Points Image 
        private void ImgTreatmentP_MouseDown(object sender, MouseButtonEventArgs e)
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
                if (Dry_Cupping.IsChecked == true)
                {
                    // Check id Dry_cupping checked or not 
                    dry = true;
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 9;
                    ellipse.Height = 9;
                    ellipse.Fill = new SolidColorBrush(Colors.DarkBlue);
                    ellipse.StrokeThickness = 2;
                    _ = Cnv.Children.Add(ellipse);

                    Canvas.SetLeft(ellipse, e.GetPosition(Cnv).X);
                    Canvas.SetTop(ellipse, e.GetPosition(Cnv).Y);
                    ellipse.Margin = new Thickness(0, 0, 20, 0);
                    check_editimg++;
                }

                if (Bloody_Cupping.IsChecked == true)
                {
                    // Check id bloody_cupping checked or not 
                    bloody = true;
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

                if (Sliding_Cupping.IsChecked == true)
                {
                    // Check id Sliding_cupping checked or not 
                    sliding = true;
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 9;
                    ellipse.Height = 9;
                    ellipse.Fill = new SolidColorBrush(Colors.Green);
                    ellipse.StrokeThickness = 2;
                    _ = Cnv.Children.Add(ellipse);

                    Canvas.SetLeft(ellipse, e.GetPosition(Cnv).X);
                    Canvas.SetTop(ellipse, e.GetPosition(Cnv).Y);
                    ellipse.Margin = new Thickness(0, 0, 20, 0);
                    check_editimg++;
                }

                if (Flashing_Cupping.IsChecked == true)
                {
                    // Check id Flashing_cupping checked or not 
                    flashing = true;
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 9;
                    ellipse.Height = 9;
                    ellipse.Fill = new SolidColorBrush(Colors.Yellow);
                    ellipse.StrokeThickness = 2;
                    _ = Cnv.Children.Add(ellipse);

                    Canvas.SetLeft(ellipse, e.GetPosition(Cnv).X);
                    Canvas.SetTop(ellipse, e.GetPosition(Cnv).Y);
                    ellipse.Margin = new Thickness(0, 0, 20, 0);
                    check_editimg++;
                }
                
            }

        }

        //  Function For saving  Treatment Points Image 
        private string Save_TreatmentPointsImg(string file_num, string session_id)
        {
            // Step 1
            Rect rect = VisualTreeHelper.GetDescendantBounds(Cnv);
            // Step 2
            DrawingVisual dv = new DrawingVisual();
            using(DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(Cnv);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }
            // Step 3
            RenderTargetBitmap render = new RenderTargetBitmap((int)Cnv.Width, (int)Cnv.Height, 96, 96, PixelFormats.Default);
            render.Render(dv);
            // Step 4
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(render));

            // Step 5  Save TreatmentPoints Image to ImagesTreatmentPlan Folder in Debug Foldder in Project
            /*if (!Directory.Exists(@"C:\Program Files (x86)\Eng.HadeelKhalili\HijamaAppSetup\ImagesTreatmentPlan"))
                Directory.CreateDirectory(@"C:\Program Files (x86)\Eng.HadeelKhalili\HijamaAppSetup\ImagesTreatmentPlan");
            // Get Full Path for Directory
            string dirpath = Directory.GetCurrentDirectory();
            */
            //cupping_point_path = dirpath + @"\ImagesTreatmentPlan\" + PatientFileNumField.Text + session_id + ".png";
            //cupping_point_path = @"C:\HijamaAppImages\ImagesTreatmentPlan\" + PatientFileNumField.Text + session_id + ".png";
            //new edit move from C folder to F(ssd) folder
            cupping_point_path = @"C:\HijamaAppImages\ImagesTreatmentPlan\" + PatientFileNumField.Text + session_id + ".png";
            if (File.Exists(cupping_point_path))
            {
                File.Delete(cupping_point_path);
                FileStream fs = File.Create(cupping_point_path);
                encoder.Save(fs);
                fs.Close();
            }
            else
            {
                FileStream fs = File.Create(cupping_point_path);
                encoder.Save(fs);
                fs.Close();
            }
           
            return cupping_point_path;
        }

        private void DeleteButton_TreatmentP_Click(object sender, RoutedEventArgs e)
        {
            if(TreatmentPlanNumField.Text != string.Empty)
            {
                //query for get Id_Bill from specefic filenum & session id
                int bill_id = treatmentplan_adapter.Get_Bill_Id(PatientFileNumField.Text, session_id);
                if(bill_id > 0)
                {
                    //تم تصدير فاتورة للخطة ويمنع الحذف
                    MessageBox.Show("لا يمكن حذف الخطة , تم تصدير الفاتورة الخاصة بها", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                }
                else
                {
                    System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد حذف السجل التابع لملف المريض رقم  " + " " + ":" +
                 PatientFileNumField.Text + "" + "والجلسة رقم :" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                    if (((int)dialog) == 6)
                    {
                        bool result = treatmentplan_adapter.Delete_TreatmentPlan(TreatmentPlanNumField.Text);
                        if (result)
                        {
                            MessageBox.Show("تم الحذف بنجاح ", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                            treatmentplan_adapter.Edit_Identity_TreatmentPlan();
                            open = true;

                            // Delete Image from ImagesTreatmentPlan Folder
                            if (cupping_point_path != null)
                            {
                                //Adjust Background For Can Delete Currently Img
                                //Cnv.Background = new ImageBrush(new BitmapImage(new Uri(img)));
                                Delete_TreatmentPlanImg(cupping_point_path);
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
            }
            else
            {
                MessageBox.Show("لايمكن الحذف  رقم السجل غير موجود !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        public static void Delete_TreatmentPlanImg(string imgpath)
        {           
            try
            {
                File.Delete(imgpath);
            }
            catch(IOException)
            {
                //MessageBox.Show("" + e);
                MessageBox.Show("فضلاً أغلق البرنامج لتكتمل عملية الحذف");
            }
        }

        private void EdieButton_TreatmentP_Click(object sender, RoutedEventArgs e)
        {
            if(TreatmentPlanNumField.Text != string.Empty)
            {
                string dry_c = "", bloody_c = "", sliding_c = "", flashing_c = "";
                bool result;
                // Check What RadioButton IsChecked
                if (dry)
                {
                    dry_c = "1";
                }
                if (bloody)
                {
                    bloody_c = "1";
                }
                if (sliding)
                {
                    sliding_c = "1";
                }
                if (flashing)
                {
                    flashing_c = "1";
                }

                if (TreatmentDate.Text != string.Empty)
                {
                    if (cupping_point_path != null)
                    {
                        if (check_editimg > 0)
                        {
                            MessageBox.Show("لايمكن التعديل على الصورة تم حفظها مسبقا سوف يتم اهمال التعديلات", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }

                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث السجل التابع لملف المريض رقم  " + " " + ":" + PatientFileNumField.Text + "" + "والجلسة رقم :" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            result = treatmentplan_adapter.Update_TreatmentPlan(TreatmentPlanNumField.Text, TreatmentDate.Text, TSessionsNumField.Text, dry_c, bloody_c, sliding_c, flashing_c, DurationBCuppingField.Text, DateOfNextVisit.Text,cupping_point_path);
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
                        // If cupping_point_path empity allow edit or not
                        if (check_editimg > 0)
                            cupping_point_path = Save_TreatmentPointsImg(PatientFileNumField.Text, session_id);
                        else
                         cupping_point_path = "";

                        System.Windows.Forms.DialogResult dialog = (System.Windows.Forms.DialogResult)MessageBox.Show("هل تريد تحديث السجل التابع لملف المريض رقم  " + " " + ":" + PatientFileNumField.Text + "" + "والجلسة رقم :" + session_num, "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
                        if (((int)dialog) == 6)
                        {
                            result = treatmentplan_adapter.Update_TreatmentPlan(TreatmentPlanNumField.Text, TreatmentDate.Text, TSessionsNumField.Text, dry_c, bloody_c, sliding_c, flashing_c, DurationBCuppingField.Text, DateOfNextVisit.Text,cupping_point_path);
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
                            cupping_point_path = null;
                            MessageBox.Show("تم الغاء التحديث ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("حقل  تاريخ خطة العلاج لايمكن أن يكون فارغ ", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

                }
            }
            else
            {
                MessageBox.Show("لايمكن التحديث  رقم السجل غير موجود !", "فشل", MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        // Get Data To this window
        private void Get_TreatmentPlanValues()
        {
            SqlDataReader sdr = treatmentplan_adapter.Get_TreatmentPlan(PatientFileNumField.Text, session_id);
            if (sdr.HasRows == false)
            {
                DBHandler.ConnectionDB.CloseConnection();
                AddButton_TreatmentP.IsEnabled = true;
                MessageBox.Show("لا يوجد سجل خطة علاج بعد لهذه الجلسة !", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
            else
            {
               
                AddButton_TreatmentP.IsEnabled = false;
                DataTable dt = new DataTable();
                dt.Load(sdr);
                // Column 1
                string treatmentp_id = dt.Rows[0].Field<int>(0).ToString();
                TreatmentPlanNumField.Text = treatmentp_id;
                // Column 2
                DateTime treatmentp_date = dt.Rows[0].Field<DateTime>(1);
                TreatmentDate.SelectedDate = treatmentp_date;
                // Column 3
                
                if (!dt.Rows[0].IsNull("NumberOfSuggestedVisits"))
                {
                    string numof_visits = dt.Rows[0].Field<int>(2).ToString();
                    TSessionsNumField.Text = numof_visits;
                }
              
                // Column 4
                /* Not Used For RadioButtons
                int dry_cupping = dt.Rows[0].Field<int>(3);
                if(dry_cupping.Equals("1"))
                {
                    Dry_Cupping.IsChecked = true;
                }
                // Column 5
                int bloody_cupping = dt.Rows[0].Field<int>(4);
                if (bloody_cupping.Equals("1"))
                {
                    Bloody_Cupping.IsChecked = true;
                }
                // Column 6
                int sliding_cupping = dt.Rows[0].Field<int>(5);
                if (sliding_cupping.Equals("1"))
                {
                    Sliding_Cupping.IsChecked = true;
                }
                // Column 7
                int flashing_cupping = dt.Rows[0].Field<int>(6);
                if (flashing_cupping.Equals("1"))
                {
                    Flashing_Cupping.IsChecked = true;
                }
                */
                // Column 8
                string duration_cupping = dt.Rows[0].Field<string>(7);
                if (duration_cupping != string.Empty)
                {
                    DurationBCuppingField.Text = duration_cupping;
                }
                // Column 9
                if (!dt.Rows[0].IsNull("DateOfNextVisit"))
                {
                    DateOfNextVisit.SelectedDate = dt.Rows[0].Field<DateTime>(8);
                }
                /*Column 10
                  cupping_point_path = dt.Rows[0].Field<string>(9);
                 if (cupping_point_path != null)
                           {
               Cnv.Background = new ImageBrush(new BitmapImage(new Uri(cupping_point_path)));
                 }*/
                //(new edit)   Move from c folder to f folder
                cupping_point_path = @"C:\HijamaAppImages\ImagesTreatmentPlan\" + PatientFileNumField.Text + session_id + ".png";
                Cnv.Background = new ImageBrush(new BitmapImage(new Uri(cupping_point_path)));


                DBHandler.ConnectionDB.CloseConnection();

            }
        }

        private void PrintButton_TreatmentP_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("فضلاَ قم بعملية الطباعة لهذه الصفحة بشكل أفقي ");
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.PrintVisual(PanelTreatmentPoint, "طباعة خطة العلاج");
            }
            else
            {
                MessageBox.Show("تم الغاء عملية الطباعة  ", "الغاء", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
            }
        }

        private void BillExtractButton_TreatmentP_Click(object sender, RoutedEventArgs e)
        {
            if (TreatmentPlanNumField.Text != string.Empty)
            {
                if (cupping_point_path == null)
                {
                    // Check If Type of Cupping Checked or not For Activate Bill
                    if (Dry_Cupping.IsChecked == false && Bloody_Cupping.IsChecked == false && Sliding_Cupping.IsChecked == false && Flashing_Cupping.IsChecked == false)
                    {
                        MessageBox.Show("فضلا حدد أحد أنواع الحجامة حتى تتمكن من اصدار الفاتورة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                    }
                    if (Dry_Cupping.IsChecked == true || Bloody_Cupping.IsChecked == true || Sliding_Cupping.IsChecked == true || Flashing_Cupping.IsChecked == true)
                    {
                        if (billExtract == null && BillExtractWindow.open == false)
                        {
                            billExtract = new BillExtractWindow();
                            billExtract.Show();
                        }
                        if (billExtract != null && BillExtractWindow.open == true)
                        {
                            billExtract = new BillExtractWindow();
                            billExtract.Show();
                            BillExtractWindow.open = false;
                        }
                        if (billExtract != null && BillExtractWindow.open == false)
                        {
                            _ = billExtract.Activate();
                            _ = billExtract.Focus();
                        }
                    }
                }
                else
                {
                    if (billExtract == null && BillExtractWindow.open == false)
                    {
                        billExtract = new BillExtractWindow();
                        billExtract.Show();
                    }
                    if (billExtract != null && BillExtractWindow.open == true)
                    {
                        billExtract = new BillExtractWindow();
                        billExtract.Show();
                        BillExtractWindow.open = false;
                    }
                    if (billExtract != null && BillExtractWindow.open == false)
                    {
                        _ = billExtract.Activate();
                        _ = billExtract.Focus();
                    }
                }

            }
            else
            {
                MessageBox.Show("لايمكن اصدار فاتورة فضلاً أضف خطة العلاج لهذه الجلسة ثم اصدر الفاتورة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);

            }
        }

        // Validation TreatmentPlan Fields
        private bool TreatmentPlan_IsValid()
        {
            if (TreatmentDate.Text == string.Empty)
            {
                MessageBox.Show("  حقل التاريخ اجباري", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                TreatmentDate.Focus();
                return false;
            }
          /*  if(dry == false && bloody == false && sliding == false && flashing == false)
            {
                MessageBox.Show("فضلا حدد نوع الحجامة لايمكن أن تكون فارغة", "فشل", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return false;
            }*/
            return true;
        }

       
    }
}
