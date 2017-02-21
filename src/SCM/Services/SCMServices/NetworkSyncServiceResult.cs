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
            return string.Join("\r\n", Messages);
        }
        public void Add(string message)
        {
            Messages.Add(message);
        }

        public string XmlResult { get; set; }
    }
}