using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TravelSystemIntegration.Models
{
    public class CompanyDto
    {
        public List<Company> value { get; set; }
        public class Company
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? CompanyId { get; set; }
            public string CompanyName { get; set; }
            public string CompanyRef { get; set; }
            public string CompanyRefImport { get; set; }
            public object Address1 { get; set; }
            public object Address2 { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
