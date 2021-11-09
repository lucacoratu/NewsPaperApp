using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPaperApp
{
    class Article
    {
        private string Title;
        private string PublisherUsername;
        private string Content;

        public Article(string title, string publisherUsername, string content)
        {
            this.Title = title;
            this.PublisherUsername = publisherUsername;
            this.Content = content;
        }

        public string GetTitle()
        {
            return this.Title;
        }

        public string GetPublisherUsername()
        {
            return this.PublisherUsername;
        }

        public string GetContent()
        {
            return this.Content;
        }
    }
}
