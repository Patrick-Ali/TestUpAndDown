using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections.Generic;
//using System.Web.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using TestingFileUpDown.Models;

namespace TestingFileUpDown.Controllers
{
    public class MyActionController : Controller
    {
        static HttpClient client = new HttpClient();

        public ActionResult Index() {
            return View("Upload");
        }

        static async Task<Uri> CreateEventAsync(EventIn eventIn)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "https://localhost:44389/api/1.0/event", eventIn);
            response.EnsureSuccessStatusCode();
            var tempURL = response.Headers.Location;
            Console.WriteLine(tempURL);
            return response.Headers.Location;
        }

        static async Task<EventIn> GetEventAsync(string path)
        {
            EventIn product = null;
            
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                product = await response.Content.ReadAsAsync<EventIn>();
            }
            return product;
        }

        [HttpPost]
        public async Task<ActionResult> Index2()
        {
            var test = Request.Form.Files;
            //foreach (var upload in Request.Form.Files)
            //{
                if (test[0].FileName != "") 
               {

                // read file to stream
                Stream hold = test[0].OpenReadStream();
                byte[] array = new byte[hold.Length];
                hold.Seek(0, SeekOrigin.Begin);
                hold.Read(array, 0, array.Length);
                EventIn temp = new EventIn()
                {
                    VideoURL = null,
                    Name = "Test",
                    Location = "York",
                    Date = "29/10/2020",
                    TimeStart = "09:00",
                    TimeEnd = "18:00",
                    Description = "A fun day out",
                    EventFile = array
                };
                await CreateEventAsync(temp);
                hold.Close();

                }
            //}
            return View("Upload");
        }

        public async Task<FileResult> Download(string ImageName)
        {
            EventIn tempEvent = await GetEventAsync("https://localhost:44389/api/1.0/event/5c7dd9607c49e47484670a8a");

            using (var stream = new MemoryStream())
            {
                stream.Write(tempEvent.EventFile, 0, tempEvent.EventFile.Length);
                stream.Seek(0, SeekOrigin.Begin);
                string filename = tempEvent.Name + tempEvent.Date + ".pdf";
                return File(stream.GetBuffer(), "application/pdf", filename);
            }

        }
    }
}