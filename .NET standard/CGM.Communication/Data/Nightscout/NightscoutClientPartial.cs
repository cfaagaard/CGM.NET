using CGM.Communication.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using CGM.Communication.Extensions;

namespace CGM.Communication.Data.Nightscout
{
    public partial class NightscoutClient
    {


        public string ApiKey { get; set; }

        public NightscoutClient(string baseUrl, string apiKey) : this(baseUrl)
        {
            this.ApiKey = apiKey;
        }

        partial void PrepareRequest(HttpClient client, HttpRequestMessage request, StringBuilder urlBuilder)
        {

        }
        partial void PrepareRequest(System.Net.Http.HttpClient client, System.Net.Http.HttpRequestMessage request, string url)
        {
            string key = this.ApiKey.Sha1Digest();

            request.Headers.Add("API-SECRET", key);
        }

        /// <summary>Add new entries.</summary>
        /// <param name="body">Entries to be uploaded.</param>
        /// <returns>Rejected list of entries.  Empty list is success.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public System.Threading.Tasks.Task AddDeviceStatusAsync(System.Collections.Generic.IEnumerable<DeviceStatus> body)
        {
            return AddDeviceStatusAsync(body, System.Threading.CancellationToken.None);
        }

        /// <summary>Add new entries.</summary>
        /// <param name="body">Entries to be uploaded.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Rejected list of entries.  Empty list is success.</returns>
        /// <exception cref="SwaggerException">A server side error occurred.</exception>
        public async System.Threading.Tasks.Task AddDeviceStatusAsync(System.Collections.Generic.IEnumerable<DeviceStatus> body, System.Threading.CancellationToken cancellationToken)
        {
            var urlBuilder_ = new System.Text.StringBuilder();
            urlBuilder_.Append(BaseUrl).Append("/devicestatus");

            var client_ = new System.Net.Http.HttpClient();
            try
            {
                using (var request_ = new System.Net.Http.HttpRequestMessage())
                {
                    var content_ = new System.Net.Http.StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(body));
                    // content_.Headers.ContentType.MediaType = "application/json";

                    content_.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    request_.Content = content_;
                    request_.Method = HttpMethod.Post;// new System.Net.Http.HttpMethod("POST");

                    PrepareRequest(client_, request_, urlBuilder_);
                    var url_ = urlBuilder_.ToString();
                    request_.RequestUri = new System.Uri(url_, System.UriKind.RelativeOrAbsolute);
                    PrepareRequest(client_, request_, url_);

                    var response_ = await client_.SendAsync(request_, System.Net.Http.HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                    try
                    {
                        var headers_ = System.Linq.Enumerable.ToDictionary(response_.Headers, h_ => h_.Key, h_ => h_.Value);
                        foreach (var item_ in response_.Content.Headers)
                            headers_[item_.Key] = item_.Value;

                        ProcessResponse(client_, response_);

                        var status_ = ((int)response_.StatusCode).ToString();
                        if (status_ == "405")
                        {
                            var responseData_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new SwaggerException("Invalid input", status_, responseData_, headers_, null);
                        }
                        else
                        if (status_ == "200")
                        {
                            return;
                        }
                        else
                        if (status_ != "200" && status_ != "204")
                        {
                            var responseData_ = await response_.Content.ReadAsStringAsync().ConfigureAwait(false);
                            throw new SwaggerException("The HTTP status code of the response was not expected (" + (int)response_.StatusCode + ").", status_, responseData_, headers_, null);
                        }
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


    public partial class Entry
    {

        private string _device;

        /// <summary>sgv, mbg, cal, etc</summary>
        [Newtonsoft.Json.JsonProperty("device", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Device
        {
            get { return _device; }
            set
            {
                if (_device != value)
                {
                    _device = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _id;

        /// <summary>sgv, mbg, cal, etc</summary>
        [Newtonsoft.Json.JsonProperty("_Id", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double? _mbg;
        [Newtonsoft.Json.JsonProperty("mbg", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public double? Mbg
        {
            get { return _mbg; }
            set
            {
                if (_mbg != value)
                {
                    _mbg = value;
                    RaisePropertyChanged();
                }
            }
        }


    }
}


