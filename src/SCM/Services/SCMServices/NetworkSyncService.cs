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

        /// <summary>
        /// URI for access to the NSO server. This is for testing only. Do NOT do this in production.
        /// Instead, store the URL in the appsettings.json file.
        /// </summary>
        private string NetworkBaseUri { get; } = "http://10.65.127.30:8088/api/running/services";

        public async Task<NetworkSyncServiceResult> SyncNetworkAsync(Object item, string resource)
        {
            return await SyncNetworkAsync(item, resource, HttpMethod.Put);
        }

        public async Task<NetworkSyncServiceResult> SyncNetworkAsync(Object item, string resource, HttpMethod method)
        {
            var syncResult = new NetworkSyncServiceResult { IsSuccess = true };
 
            var xmlStr = ObjectToXmlString(item);
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(NetworkBaseUri + resource),
                Method = method,
                Content = new StringContent(xmlStr, Encoding.UTF8, "application/vnd.yang.data+xml")
            };

            var networkResponse = await GetNetworkHttpResponse(request);

            // Return item to sender for any further processing

            syncResult.Item = item;

            if (!networkResponse.IsSuccess)
            {
                syncResult.IsSuccess = false;
                syncResult.StatusCode = networkResponse.StatusCode;
                syncResult.Messages.AddRange(networkResponse.Messages);
            }

            return syncResult;
        }

        public async Task<NetworkSyncServiceResult> CheckNetworkSyncAsync(Object item, string resource)
        {
            var checkSyncResult = new NetworkSyncServiceResult();
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(NetworkBaseUri + resource + "?deep"),
                Method = HttpMethod.Get
            };

            checkSyncResult =  await GetNetworkHttpResponse(request);

            // Return item to sender for any further processing

            checkSyncResult.Item = item;

            if (!checkSyncResult.IsSuccess)
            {
                return checkSyncResult;
            }

            // Parse string response to an object then to xml to perform the deep-equals check.
            // This process normalises the data. The resulting xml trees must also be sorted 
            // in order to check equality using deep-equals.

            var objectItem = XmlStringToObject(checkSyncResult.Content, item.GetType());

            // Perform comparison

            var xmlTree = XElement.Parse(ObjectToXmlString(objectItem));
            var sortedXmlTree = Sort(xmlTree);
            var xmlTreeToCompare = XElement.Parse(ObjectToXmlString(item));
            var sortedXmlTreeToCompare = Sort(xmlTreeToCompare);

            checkSyncResult.IsSuccess = XNode.DeepEquals(sortedXmlTree, sortedXmlTreeToCompare);
          
            if (!checkSyncResult.IsSuccess)
            {
                checkSyncResult.Messages.Add("The resource is not synchronised with the network.");
            }

            return checkSyncResult;
        }

        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(string resource)
        {
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(NetworkBaseUri + resource),
                Method = HttpMethod.Delete
            };

            return  await GetNetworkHttpResponse(request);
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

        private async Task<NetworkSyncServiceResult> GetNetworkHttpResponse(HttpRequestMessage httpRequest)
        {
            var result = new NetworkSyncServiceResult { IsSuccess = true };

            try
            {
                var client = new HttpClient();

                // Authorisation header here is statically set. Do NOT do this in production!
                // Code published to repositories such as GIT will allow the auth settings to be visible. 
                // Authentication settings must be sourced from a secure location.

                client.DefaultRequestHeaders.Add("Authorization", "Basic YWRtaW46YWRtaW4=");

                var httpResponse = await client.SendAsync(httpRequest);
                result.Content = await httpResponse.Content.ReadAsStringAsync();

                if (httpResponse.IsSuccessStatusCode)
                {
                    result.StatusCode = NetworkSyncStatusCode.Success;
                }
                else 
                {
                    result.Messages.Add("The network request failed.");

                    if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        result.StatusCode = NetworkSyncStatusCode.NotFound;
                        result.Messages.Add("The resource was not found.");
                    }

                    result.Messages.Add(httpResponse.ReasonPhrase);
                    result.IsSuccess = false;
                }
            }

            catch (HttpRequestException  /** ex **/ )
            {
                result.StatusCode = NetworkSyncStatusCode.RequestFailed;
                result.Messages.Add("Unable to complete the request.Perhaps the network is unavailable.Check logs for more details.");
                result.IsSuccess = false;
            }

            return result;
        }

        /// <summary>
        /// Recursively sorts the children of an XML element
        /// using the child element name followed by value as sort parameters.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static XElement Sort(XElement element)
        {
            XElement newElement = new XElement(element.Name,
                    from child in element.Elements()
                    orderby child.Name.ToString()
                    orderby child.Value.ToString()
                    select Sort(child));

            if (element.HasAttributes)
            {
                foreach (XAttribute attrib in element.Attributes())
                {
                    newElement.SetAttributeValue(attrib.Name, attrib.Value);
                }
            }

            if (!element.HasElements)
            {
                newElement.SetValue(element.Value);
            }

            return newElement;
        }
    }
}
