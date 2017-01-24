using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ViewModels
{
    public class BundleInterfaceViewModel
    {
        [JsonProperty(PropertyName = "id")]
        public int BundlePortID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "isTagged")]
        public bool IsTagged { get; set; }

        [JsonProperty(PropertyName = "memberPorts")]
        public IList<PortViewModel> MemberPorts { get; set; }
    }
    public class BundlePortsByTenantViewModel
    {
        [JsonProperty(PropertyName = "bundlePorts")]
        [Display(Name = "Bundle Ports")]
        public IList<BundleInterfaceViewModel> BundlePorts { get; set; }

        [JsonProperty(PropertyName = "tenantGuid")]
        [Display(Name = "Tenant Globally Unique ID")]
        public Guid TenantGuid { get; set; }
    }
}
