using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
   public class HttpService
    {
        private HttpClient _httpClient;
        private string _baseIPAddress;

        public HttpService(string ipaddress = "")
        {
            this._baseIPAddress = ipaddress;
            _httpClient = new HttpClient { BaseAddress = new Uri(_baseIPAddress) };
        }

        /// <summary>
        /// 以json的方式Post数据 返回string类型
        /// <para>最终以json的方式放置在http体中</para>
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="requestUri">例如/api/Files/UploadFile</param>
        /// <returns></returns>
        public string Post(object entity, string requestUri)
        {
            string request = string.Empty;
            if (entity != null)
                request = JsonConvert.SerializeObject(entity);
            HttpContent httpContent = new StringContent(request);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return Post(requestUri, httpContent);
        }

        private string Post(string requestUrl, HttpContent content)
        {
            var result = _httpClient.PostAsync(ConcatURL(requestUrl), content);
            var str = string.Empty;
            result.Result.Content.ReadAsStringAsync().ContinueWith((requestTask) =>
            {
                if (!requestTask.IsFaulted)
                    str = requestTask.Result;
                else
                    str = Newtonsoft.Json.JsonConvert.SerializeObject(new { suc = false, msg = requestTask.Exception.Message });
            }).Wait(60000);
            return str;
        }

        private string ConcatURL(string requestUrl)
        {
            return new Uri(_httpClient.BaseAddress, requestUrl).OriginalString;
        }
    }
}
