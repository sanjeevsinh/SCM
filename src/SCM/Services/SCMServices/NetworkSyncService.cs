using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace SCM.Services.SCMServices
{
    public class NetworkSyncService : INetworkSyncService
    {
        public NetworkSyncService()
        {
        }

        public async Task<NetworkSyncServiceResult> SyncToNetwork(Object item, string resource = "")
        {
            var syncResult = new NetworkSyncServiceResult();
            syncResult.IsSuccess = true;

            XmlSerializer xml = new XmlSerializer(item.GetType());
            MemoryStream memStream = new MemoryStream();
            var writer = new StreamWriter(memStream);
            xml.Serialize(writer, item);

            var xmlString = Encoding.ASCII.GetString(memStream.ToArray());
            syncResult.XmlResult = xmlString;

            var client = new HttpClient();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://10.65.127.30:8088/api/running/services" + resource),
                Method = HttpMethod.Put,
                Content = new StringContent(xmlString, Encoding.UTF8, "application/vnd.yang.data+xml")
            };

            client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW46YWRtaW4=");

            var httpResponse = await client.SendAsync(request);

            if (!httpResponse.IsSuccessStatusCode)
            {
                syncResult.Add(httpResponse.ReasonPhrase);
                syncResult.IsSuccess = false;
            }

            return syncResult;
        }
    }
}
