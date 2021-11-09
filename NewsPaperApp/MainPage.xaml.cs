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
        List<Newspaper> newsPapers = new List<Newspaper>();
        List<NewsPaper> userControlNewsPapers = new List<NewsPaper>();

        void InitializeListNewsPapers()
        {
            this.newsPapers = DatabaseConnection.GetAvailableNewspapers();

            foreach(var newspaper in newsPapers)
            {
                this.userControlNewsPapers.Add(new NewsPaper());
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].lbl_newpaperName.Content = newspaper.GetName();
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].lbl_publishingHouse.Content = newspaper.GetPublishingHouse();
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].lbl_publishingDate.Content = newspaper.GetPublishingDate();
                this.userControlNewsPapers[userControlNewsPapers.Count - 1].grid_newspaper.MouseLeftButtonDown += this.usercontrol_clicked;

                this.listbox_newspapers.Items.Add(this.userControlNewsPapers[userControlNewsPapers.Count - 1]);
            }           
        }

        public MainPage()
        {
            InitializeComponent();

            InitializeListNewsPapers();
        }



        private void elipse_profile_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Go to the profile window
            ProfileWindow pWindow = new ProfileWindow();
            this.Hide();
            pWindow.Show();
        }

        private void usercontrol_clicked(object sender, MouseButtonEventArgs e)
        {
            //ClientData.SetCurrentNewspaper(this.listbox_newspapers.SelectedItem)
            ArticleViewer artviewer = new ArticleViewer();
            this.Hide();
            artviewer.Show();
        }


        private void mainpage_window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
