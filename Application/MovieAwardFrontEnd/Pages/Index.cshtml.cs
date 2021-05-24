using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MovieAwardFrontEnd.Pages
{
    public class IndexModel : PageModel
    {
        private IHostingEnvironment _environment;
        public IndexModel(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        [BindProperty]
        public IFormFile Upload { get; set; }
        public async Task OnPostAsync()
        {
            List<Movie> movies = ConverFileToEntity();
            if (movies.Count > 0)
                await PostToAPI(movies);
        }

        private List<Movie> ConverFileToEntity()
        {
            var movies = new List<Movie>();
            try
            {
                if (Upload != null && Upload.Length > 0)
                {
                    bool ignoreFirst = true;
                    using (var rd = new StreamReader(Upload.OpenReadStream()))
                    {
                        while (!rd.EndOfStream)
                        {
                            var splits = rd.ReadLine().Split(';');
                            if (splits[0].Length <= 0 || ignoreFirst)
                            {
                                ignoreFirst = false;
                                continue;
                            }

                            movies.Add(new Movie()
                            {
                                Year = splits[0].AsInt(0),
                                Title = splits[1],
                                Studio = splits[2],
                                Producer = splits[3],
                                Winner = IsWinner(splits[4] ?? "")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Conversion failed." + ex.Message);
            }
            return movies;
        }

        private async Task PostToAPI(List<Movie> movies)
        {
            string json = JsonSerializer.Serialize(movies);
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:60645/api/");
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await client.SendAsync(new HttpRequestMessage() { Method = HttpMethod.Post, Content = content, RequestUri = new Uri("http://localhost:60645/api/MovieAward/Upload") });
        }
       
        private bool IsWinner(string data)
        {
            return data.Equals("yes") ? true : false;
        }
    }
}
