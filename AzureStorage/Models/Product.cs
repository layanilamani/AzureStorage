using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.Models
{
    public class Product : TableEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }

        public string ImageUrl { get; set; }
    }
}