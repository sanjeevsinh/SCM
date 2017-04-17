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
            return string.Concat(Messages);
        }

        public List<string> GetMessageList()
        {
            return Messages;
        }

        public string GetHtmlListMessage()
        {
            var message = string.Concat(Messages.Select(q => $"<li>{q}</li>"));
            message += string.Concat(NetworkSyncServiceResults.SelectMany(q => q.Messages.Select(m => $"<li>{m}</li>")));

           return $"<ul>{message}</ul>";
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
    }
}
