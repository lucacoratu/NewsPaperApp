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

            foreach (var article in articles) 
            {
                this.articleUserControls.Add(new ArticleUserControl());
                this.articleUserControls[this.articleUserControls.Count - 1].lbl_author.Content = article.GetPublisherUsername();
                this.articleUserControls[this.articleUserControls.Count - 1].lbl_title.Content = article.GetTitle();
                this.articleUserControls[this.articleUserControls.Count - 1].txtblock_content.Text = article.GetContent();
                //this.lstbox_articles.Items.Add(this.articleUserControls[this.articleUserControls.Count - 1]);
            }

            if (articles.Count != 0)
            {
                this.lbl_title.Content = articles[0].GetTitle();
                this.txtblock_content.Text = articles[0].GetContent();

                this.lbl_articleCounter.Content = "Article 1 / " + this.articles.Count.ToString();
            }
            else
            {
                this.lbl_articleCounter.Content = "Article 0 / " + this.articles.Count.ToString();
            }
        }

        public ArticleViewer()
        {
            InitializeComponent();

            InitializeArticleListBox();
        }

        private void articleviewer_window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_decrementArticle_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Click left");
        }

        private void btn_incrementArticle_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
