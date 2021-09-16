using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HttpBeezyConnect
{
    public static class SNOW_Request
    {
        [FunctionName("SNOW_Request")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            var results = await SendSNOWRequest(name);

            return new OkObjectResult(results);
        }

        public static async Task<string> SendSNOWRequest(string s) {

            using (var client = new HttpClient()) {
                var snowAPI = "https://api-preprod.systems.uk.hsbc/api/drm/hsbc-cst-e-sn-ext/1.0.0/sn-now-dev"; // System.Environment.GetEnvironmentVariable("snowAPI").Split(':')[1];
                var snowClient = "80716a6385a04f0b9df2a922e0cfd312"; // System.Environment.GetEnvironmentVariable("client_id").Split(':')[1];
                var snowSecuet = "3B6F6966407aAE8A8D69C8d3eB45A259"; // System.Environment.GetEnvironmentVariable("client_secret").Split(':')[1];
                var snowAuthroization = "Basic ODA3MTZHNjM4NWEwNGYwZDlkZjJhOTIyZTBjZmQzMTI6M0I2RjY5NjY0MDdhNEU4QThENjlDOGQzZUI0NUEyNTk"; // System.Environment.GetEnvironmentVariable("authorization").Split(':')[1];
                var snowRequestKey = "5F3AB969-62C3-4B20-A165-DEC55D8916CE"; // System.Environment.GetEnvironmentVariable("X-REQUEST-ID").Split(':')[1];
                var snowContentType = "application/json"; // System.Environment.GetEnvironmentVariable("Content-Type").Split(':')[1];

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Add("client_id", snowClient);
                client.DefaultRequestHeaders.Add("client_secret", snowSecuet);
                client.DefaultRequestHeaders.Add("authorization", snowAuthroization);
                client.DefaultRequestHeaders.Add("X-REQUEST-ID", snowRequestKey);

                //https://nzpcmad.blogspot.com/2017/07/aspnet-misused-header-name-make-sure.html
                //remove below to mediaTypeHeaderValue
                //client.DefaultRequestHeaders.Add("Content-Type", snowContentType);

                HttpResponseMessage response;
                Dictionary<string, string> dicVal = new Dictionary<string, string>();
                dicVal.Add("url", "hsbcc/chat_boat_api/announcements?psid=43783105");
                dicVal.Add("type", "get");                

                string json = JsonConvert.SerializeObject(dicVal);
                HttpContent requestData = new StringContent(json);
                requestData.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response = await client.PostAsync(String.Format(snowAPI), requestData);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadAsStringAsync();

                return result;

            }

        }
    }
}
