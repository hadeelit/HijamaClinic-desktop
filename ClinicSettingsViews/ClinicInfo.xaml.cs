using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HijamaClinicManagement.ClinicSettingsViews
{
    /// <summary>
    /// Interaction logic for ClinicInfo.xaml
    /// </summary>
    public partial class ClinicInfo : UserControl
    {
        public ClinicInfo()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void NumberField_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void AddButton_ClinicInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_ClinicInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EdieButton_ClinicInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClearButton_ClinicInfo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddLogo_Clinic_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddHeader_Clinic_Click(object sender, RoutedEventArgs e)
        {

        }

      
    }
}
