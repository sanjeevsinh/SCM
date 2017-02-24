using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Net;

namespace SCM.Services.SCMServices
{
    public class NetworkSyncService : INetworkSyncService
    {

        private string NetworkBaseUri { get; } = "http://10.65.127.30:8088/api/running/services";

        public NetworkSyncService()
        {
        }

        public async Task<NetworkSyncServiceResult> SyncNetworkAsync(Object item, string resource)
        {
            var syncResult = new NetworkSyncServiceResult();
            syncResult.IsSuccess = true;

            var xmlStr = ObjectToXmlString(item);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(NetworkBaseUri + resource),
                Method = HttpMethod.Put,
                Content = new StringContent(xmlStr, Encoding.UTF8, "application/vnd.yang.data+xml")
            };

            var networkResponse = await GetNetworkHttpResponse(request);

            if (!networkResponse.IsSuccess)
            {
                syncResult.IsSuccess = false;
                syncResult.Add(networkResponse.GetMessage());
            }

            return syncResult;
        }

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(Object item, string resource)
        {
            var checkSyncResult = new NetworkCheckSyncServiceResult();

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(NetworkBaseUri + resource + "?deep"),
                Method = HttpMethod.Get
            };

            var networkResponse = await GetNetworkHttpResponse(request);
            checkSyncResult.NetworkSyncServiceResult.NetworkHttpResponse = networkResponse;

            if (!networkResponse.IsSuccess)
            {
                checkSyncResult.NetworkSyncServiceResult.Add("The network request failed.");

                if (networkResponse.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    checkSyncResult.NetworkSyncServiceResult.Add("The resource was not found.");
                }

                return checkSyncResult;
            }

            checkSyncResult.NetworkSyncServiceResult.IsSuccess = true;

            // Parse string response to an object then to xml to perform the deep-equals check.
            // This process normalises the data. The resulting xml trees must also be sorted 
            // in order to check equality using deep-equals.

            var objectItem = XmlStringToObject(networkResponse.Content, item.GetType());
            var xmlTree = XElement.Parse(ObjectToXmlString(objectItem));
            var sortedXmlTree = Sort(xmlTree);
            var xmlTreeToCompare = XElement.Parse(ObjectToXmlString(item));
            var sortedXmlTreeToCompare = Sort(xmlTreeToCompare);

            checkSyncResult.InSync = XNode.DeepEquals(sortedXmlTree, sortedXmlTreeToCompare);
          
            return checkSyncResult;
        }

        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(string resource)
        {
            var syncResult = new NetworkSyncServiceResult();
            syncResult.IsSuccess = true;

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(NetworkBaseUri + resource),
                Method = HttpMethod.Delete
            };

            var networkResponse = await GetNetworkHttpResponse(request);
            syncResult.NetworkHttpResponse = networkResponse;

            if (!networkResponse.IsSuccess)
            {
                syncResult.IsSuccess = false;
            }

            return syncResult;
        }

        private string ObjectToXmlString(Object objectItem)
        {
            XmlSerializer serializer = new XmlSerializer(objectItem.GetType());
            using (MemoryStream memStream = new MemoryStream())
            {

                var writer = new StreamWriter(memStream);
                serializer.Serialize(writer, objectItem);

                return Encoding.ASCII.GetString(memStream.ToArray());
            }
        }

        private Object XmlStringToObject(string stringItem, Type objectType)
        {
            XmlSerializer serializer = new XmlSerializer(objectType);
            using (TextReader reader = new StringReader(stringItem))
            {
               return serializer.Deserialize(reader);
            }
        }

        private async Task<NetworkHttpResponse> GetNetworkHttpResponse(HttpRequestMessage httpRequest)
        {

            var result = new NetworkHttpResponse();
            result.IsSuccess = true;

            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW46YWRtaW4=");

                var httpResponse = await client.SendAsync(httpRequest);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    result.Add(httpResponse.ReasonPhrase);
                    result.IsSuccess = false;
                }

                result.HttpStatusCode = httpResponse.StatusCode;
                result.Content = await httpResponse.Content.ReadAsStringAsync();
            }

            catch (HttpRequestException /* ex */)
            {
                result.Add("Unable to complete the request. Perhaps the network is unavailable. Check logs for more details.");
                result.IsSuccess = false;
            }

            return result;
        }

        private static XElement Sort(XElement element)
        {
            XElement newElement = new XElement(element.Name,
                    from child in element.Elements()
                    orderby child.Name.ToString()
                    select Sort(child));

            if (element.HasAttributes)
            {
                foreach (XAttribute attrib in element.Attributes())
                {
                    newElement.SetAttributeValue(attrib.Name, attrib.Value);
                }
            }
            return newElement;
        }
    }
}
