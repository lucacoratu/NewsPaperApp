using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NewsPaperApp
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Window
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void elipse_profile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Go to the profile window
            ProfileWindow pWindow = new ProfileWindow();
            this.Close();
            pWindow.Show();
        }
    }
}
