using System;
using System.Collections.Generic;
using System.Text;

namespace CGM.Communication.Data.Nightscout
{
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "9.2.4.0")]
    public class Notification
    {
        [Newtonsoft.Json.JsonProperty("type", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Type { get; set; }

        [Newtonsoft.Json.JsonProperty("text", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Text { get; set; }

        [Newtonsoft.Json.JsonProperty("date", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Date { get; set; }


        //[Newtonsoft.Json.JsonProperty("date", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        //public string EventDate { get; set; }

        public static Notification FromJson(string data)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Notification>(data);
        }
    }


    public class NotificationClient
    {

        public string BaseUrl { get; set; }

        public NotificationClient(string baseurl)
        {
            this.BaseUrl = baseurl;
        }

        public async System.Threading.Tasks.Task AddNotificationAsync(Notification body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl);

            var client_ = new System.Net.Http.HttpClient();
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                    content_.Headers.ContentType.MediaType = "application/json";
                    request_.Content = content_;
                    request_.Method = new System.Net.Http.HttpMethod("POST");

  
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);


                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;

                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "200")
                        {
                            return;
                        }
                        else
                        if (status_ == "405")
                        {
                            var responseData_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new SwaggerException("Invalid input", status_, responseData_, headers_, null);
                        }
                        //else
                        //if (status_ != "200" && status_ != "204")
                        //{
                        //    var responseData_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                        //    throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, headers_, null);
                        //}
                    }
                    finally
                    {
                        if (response_ != null)
                            response_.Dispose();
                    }
                }
            }
            finally
            {
                if (client_ != null)
                    client_.Dispose();
            }
        }
    }


}
