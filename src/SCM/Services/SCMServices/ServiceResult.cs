using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class ServiceResult
    {
        public ServiceResult()
        {
            NetworkSyncServiceResults = new List<NetworkSyncServiceResult>();
        }

        private List<string> Messages = new List<string>();

        public bool IsSuccess { get; set; }

        public string GetMessage()
        {
            var messages = Messages.Concat(NetworkSyncServiceResults.Select(q => q.GetMessage()));
            return string.Concat(messages);
        }

        public List<string> GetMessageList()
        {
            return Messages;
        }

        public void Add(string message)
        {
            Messages.Add(message);
        }

        public void AddRange(IEnumerable<string> messages)
        {
            Messages.AddRange(messages);
        }

        public List<NetworkSyncServiceResult> NetworkSyncServiceResults { get; set; }

        public object Item { get; set; }
        public object Context { get; set; }
    }
}
