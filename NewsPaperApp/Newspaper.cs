﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NewsPaperApp
{
    /*
     * Class that contains the information of a newspapers
     * The information will be extracted from the database
     */
    public class Newspaper
    {
        //Member variables
        private string name;
        private string publishingHouse;
        private DateTime publishingDate;

        //Constructor
        public Newspaper(string name, string publishingHouse, DateTime publishingDate)
        {
            this.name = name;
            this.publishingHouse = publishingHouse;
            this.publishingDate = publishingDate;
        }
        

        //Accessors
        public string GetName()
        {
            return this.name;
        }

        public string GetPublishingHouse()
        {
            return this.publishingHouse;
        }

        public DateTime GetPublishingDate()
        {
            return this.publishingDate;
        }

    }
}