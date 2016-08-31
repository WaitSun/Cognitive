using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;

namespace SrpAPI.Common
{
    public class HttpHelper
    {
        public static HttpHelper Instance { get { return new HttpHelper(); } }

        private HttpClient client = null;

        

        public string GetSend(string Url)
        {
            try
            {
                if (client == null)
                {
                    client = new HttpClient();
                }
                return client.GetAsync(Url).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            { throw new Exception("GetSend:" + ex.Message); }
        }

        
    }
}
