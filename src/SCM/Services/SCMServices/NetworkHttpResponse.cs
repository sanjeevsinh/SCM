using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class NetworkHttpResponse
    {
        private List<string> Messages = new List<string>();

        public bool IsSuccess { get; set; }
        public string Content { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }

        public string GetMessage()
        {
            return string.Join("\r\n", Messages);
        }
        public void Add(string message)
        {
            Messages.Add(message);
        }
    }
}
