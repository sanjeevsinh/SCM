﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace SCM.Services.SCMServices
{
    public class NetworkSyncServiceResult
    {
        private List<string> Messages = new List<string>();

        public bool IsSuccess { get; set; }

        public string GetMessage()
        {
            return string.Concat(Messages);
        }

        public string GetAllMessages()
        {
            var message = GetMessage();
            if (NetworkHttpResponse != null)
            {
                message += NetworkHttpResponse.GetMessage();
            }

            return message;
        }

        public List<string> GetMessageList()
        {
            var m = new List<string>();
            if (NetworkHttpResponse != null)
            {
                m.Add(NetworkHttpResponse.GetMessage());
            }

            m.AddRange(Messages);

            return m;
        }

        public void Add(string message)
        {
            Messages.Add(message);
        }

        public NetworkHttpResponse NetworkHttpResponse { get; set; }
    }
}
