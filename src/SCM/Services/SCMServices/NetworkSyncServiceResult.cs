using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Net;

namespace SCM.Services.SCMServices
{
    public enum NetworkSyncStatusCode
    {
        NotFound = 404,
        RequestFailed = 1000,
        Success = 1001
    }
    public class NetworkSyncServiceResult
    {
        public NetworkSyncServiceResult()
        {
            Messages = new List<string>();
        }
        public string GetMessage()
        {
            return string.Concat(Messages);
        }
        public List<string> Messages { get; set; }
        public bool IsSuccess { get; set; }
        public string Content { get; set; }
        public object Item { get; set; }
        public NetworkSyncStatusCode StatusCode { get; set; }
     
    }
}
