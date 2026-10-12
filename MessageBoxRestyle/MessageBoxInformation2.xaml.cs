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

namespace HijamaClinicManagement.MessageBoxRestyle
{
    /// <summary>
    /// Interaction logic for MessageBoxInformation2.xaml
    /// </summary>
    public partial class MessageBoxInformation2 : Window
    {
        public MessageBoxInformation2()
        {
            InitializeComponent();
        }

        private void CloseIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }
}
