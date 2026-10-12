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
using System.Windows.Media.Animation;

namespace HijamaClinicManagement
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            LoadingProgressBar();
        }


        private async void LoadingProgressBar()
        {
            //create duration for loading animation on ProgressBar
            Duration dur = new Duration(TimeSpan.FromSeconds(10));
            //create Animation during ProgressBar
            DoubleAnimation doubleAnimation = new DoubleAnimation(200.0, dur);
            // connect animation with ProgressBar on window
            loadingprogressbar.BeginAnimation(ProgressBar.ValueProperty, doubleAnimation);
            //Delay for 5000ms After Loading ProgressBar
            await Task.Delay(5000);
            OpenLoginWidow();

        }
        // close splash Screen and open login screen
        private void OpenLoginWidow()
        {
            Sign_in sign_In = new Sign_in();
            sign_In.Show();
            Close();
        }

    }
}
