using ProjecToxfordApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ProjecToxfordApi.Controllers
{
    public class ProjecToxfordClientHelper
    {
        
        private const string serviceHost = "https://api.cognitive.azure.cn/face/v1.0";
        private const string KEY = "";
        private HttpClient client;

        public ProjecToxfordClientHelper()
        {
            client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            client.DefaultRequestHeaders.Add("ContentType", "application/json");
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "d99b8a44376145e0b1099ab78bc0eb23");
        }

        //调用认知服务ＡＰＩ
        public async Task<ProjecToxfordResponseModels> PostAsync(string querkey, object body, Dictionary<string, string> querystr = null)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (querystr != null)
            {
                foreach (var entry in querystr)
                {
                    queryString[entry.Key] = entry.Value;
                }
            }
            var uri = string.Format("{0}/{1}?{2}", serviceHost, querkey, queryString);

            byte[] byteData = null;

            if (body.GetType() == typeof(byte[]))
            {
                byteData = (byte[])body;
            }

            else
            {
                var jsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(body);
                byteData = Encoding.UTF8.GetBytes(jsonStr);
            }

            HttpResponseMessage response;
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = body.GetType() == typeof(byte[]) ?
                    new MediaTypeHeaderValue("application/octet-stream") :
                    new MediaTypeHeaderValue("application/json");
                try
                {
                    response = SyncExecute(client.PostAsync(uri, content));
                    var msg = SyncExecute(response.Content.ReadAsStringAsync());
                    return new ProjecToxfordResponseModels(msg, response.StatusCode);
                }
                catch (Exception)
                {

                    throw;
                }
                // { "faceID1" : "8037dfa3-1994-4044-a794-bc2701493580", "faceID2" : "48cd53d8-4e0d-4cae-8ed7-6c3d891ab3c0", "confidence" : 0.17461, "isIdentical" : false }
            }
        }
        public T SyncExecute<T>(Task<T> taks)
        {
            return Task.Factory.StartNew<Task<T>>(() => taks).Unwrap<T>().GetAwaiter().GetResult();
        }

        public HttpResponseMessage CreateHttpResponseMessage(HttpRequestMessage request, ProjecToxfordResponseModels result)
        {
            if (result.StatusCode == HttpStatusCode.OK)
            {

                return request.CreateResponse(HttpStatusCode.OK, result.Message);

            }
            else
            {
                return request.CreateErrorResponse(result.StatusCode, result.Message);
            }
        }
        // private const string serviceHost = "https://api.projectoxford.ai/face/v1.0";
        // client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9977094f70e64a9499f2c3743cebb078");


    }
}