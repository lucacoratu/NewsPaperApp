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
    /// Interaction logic for ArticleViewer.xaml
    /// </summary>
    public partial class ArticleViewer : Window
    {
        List<Article> articles = new List<Article>();
        List<ArticleUserControl> articleUserControls = new List<ArticleUserControl>();
        void InitializeArticleListBox()
        {
            this.articles = DatabaseConnection.GetNewspaperArticles(ClientData.GetCurrentNewspaper());
        }

        public ArticleViewer()
        {
            InitializeComponent();
        }

        private void articleviewer_window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
