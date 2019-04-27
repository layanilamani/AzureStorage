using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureStorage.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(string Container = "aptech")
        {
            CloudStorageAccount act = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = act.CreateCloudBlobClient();

            var cont = client.GetContainerReference(Container);
            var blobs =  cont.ListBlobs();

            List<CloudBlockBlob> bloblist = new List<CloudBlockBlob>();

            foreach (CloudBlockBlob item in blobs)
            {
                bloblist.Add(item);
            }

            ViewBag.blobs = bloblist;

            return View();
        }

        #region Blob
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, string Container)
        {
            CloudStorageAccount act = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = act.CreateCloudBlobClient();

            var cont = client.GetContainerReference(Container);

            if (!cont.Exists())
            {
                cont.Create();
                cont.SetPermissions(new BlobContainerPermissions()
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }

            var blob1 = cont.GetBlockBlobReference(file.FileName);

            blob1.UploadFromStream(file.InputStream);

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string blob, string Container = "aptech")
        {
            CloudStorageAccount act = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = act.CreateCloudBlobClient();

            var cont = client.GetContainerReference(Container);

            var blob1 = cont.GetBlockBlobReference(blob);

            blob1.DeleteIfExists();

            return RedirectToAction("Index");
        }

        #endregion

        public ActionResult GetTable()
        {
            CloudStorageAccount act = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = act.CreateCloudTableClient();

            var table = client.GetTableReference("Products");

            if (!table.Exists())
            {
                table.Create();
            }

            var query = new TableQuery<Models.Product>();
            query.Select(new string[] {"Id","Name" });
            query.Take(int.MaxValue);

            var productstable = table.ExecuteQuery<Models.Product>(query);
            List<Models.Product> products = new List<Models.Product>();

            foreach (var item in productstable)
            {
                products.Add(new Models.Product() { Id = item.Id, Name = item.Name });
            }

            ViewBag.products = products;

            return View(products);
        }

        public ActionResult AddInTable()
        {
            CloudStorageAccount act = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = act.CreateCloudTableClient();

            var table = client.GetTableReference("Products");

            if (!table.Exists())
            {
                table.Create();
            }

            Models.Product product = new Models.Product();

            var r = new Random();
            var randomint =  r.Next(0, int.MaxValue);

            product.Id = randomint;
            product.Name = "abc";
            product.PartitionKey = DateTime.Now.Year.ToString();
            product.Price = 265;
            product.RowKey = "Id";
            var operation = TableOperation.Insert(product);

            table.Execute(operation);            

            return RedirectToAction("GetTable");
        }

       public ActionResult AddQueue()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddQueue(string name)
        {
            CloudStorageAccount act = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = act.CreateCloudQueueClient();

            var que = client.GetQueueReference("aptechque");
            que.CreateIfNotExists();

            que.AddMessage(new Microsoft.WindowsAzure.Storage.Queue.CloudQueueMessage("salam"));



            return RedirectToAction("Index");
        }
    }
}