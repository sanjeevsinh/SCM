using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class ServiceValidationResult
    {
        private List<string> Messages = new List<string>();

        public bool IsValid { get; set; }

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
