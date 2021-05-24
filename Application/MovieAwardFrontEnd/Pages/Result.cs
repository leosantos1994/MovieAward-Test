using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MovieAwardFrontEnd.Pages
{
    public class ResultModel : PageModel
    {
        public string MaxDifference = "";
        public string GotConsecutiveWinnerProducer = "";

        public async Task OnGetAsync()
        {
            var client = new HttpClient();

            var response = await client.GetAsync("http://localhost:60645/api/MovieAward/GetConsecutiveWinner");
            GotConsecutiveWinnerProducer = response.Content.ReadAsStringAsync().Result;

            var responseDif = await client.GetAsync("http://localhost:60645/api/MovieAward/GetMaxDifference");
            MaxDifference = responseDif.Content.ReadAsStringAsync().Result;
        }

        public void OnPost()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:60645/api/");
            client.SendAsync(new HttpRequestMessage() { Method = HttpMethod.Delete, RequestUri = new Uri("http://localhost:60645/api/MovieAward/ClearData") });
        }
    }
}
