using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TravelSystemIntegration.Models
{
    public class EmployeeEntityDto
    {
        public List<EmployeeEntity> value { get; set; }
        public class EmployeeAccess
        {
            public bool RoleDepartment { get; set; }
            public bool RoleEmployee { get; set; }
            public bool RoleProject { get; set; }
            public bool RoleCertifier { get; set; }
            public bool UseMobile { get; set; }
            public bool UseExpense { get; set; }
        }

        public class EmployeeAddress
        {
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string Country { get; set; }
            public string Mobile { get; set; }
            public string Phone1 { get; set; }
            public string Phone2 { get; set; }
            public string PostAddress { get; set; }
            public string PostState { get; set; }
            public string Zipcode { get; set; }
        }

        public class EmployeeTime
        {
            public int? CostRate { get; set; }
            public bool FlexiTime { get; set; }
            public bool Overtime { get; set; }
            public int? PercentWork { get; set; }
            public int? Rate { get; set; }
            public bool ChangeSalaryCode { get; set; }
            public bool Timesheet { get; set; }
            public bool Vacation { get; set; }
        }

        public class EmployeeDetail
        {
            public string DateOfBirth { get; set; }
            public string EmployeeRef { get; set; }
            public string EmployeeRefImport { get; set; }
        }

        public class EmployeeEntityPatch
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? EmployeeId { get; set; }
            public bool IsActive { get; set; }
        }

        public class EmployeeEntity
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? EmployeeId { get; set; }
            public bool AllocatedProjects { get; set; }
            public bool IsActive { get; set; }
            public string Initials { get; set; }
            public string Firstname { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Email1 { get; set; }
            public string Email2 { get; set; }
            public string Email3 { get; set; }
            public string SsoRef { get; set; }
            public string CsaRef { get; set; }
            public int? CompanyId { get; set; }

            [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
            [DefaultValue(2)]
            public int? DepartmentId { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? JobCodeId { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? ActcatId { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public int? EmployeeCategoryId { get; set; }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
            public DateTime? EmployedDate { get; set; }//BUG in Reiseregning API
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore), JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
            public DateTime? LeaveDate { get; set; }//BUG in Reiseregning API
            public EmployeeAccess EmployeeAccess { get; set; }
            public EmployeeAddress EmployeeAddress { get; set; }
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public EmployeeTime EmployeeTime { get; set; }
            public EmployeeDetail EmployeeDetail { get; set; }

            public EmployeeEntityPatch ToEmployeeEntityPatch()
            {
                return new EmployeeEntityPatch()
                {
                    EmployeeId = this.EmployeeId,
                    IsActive = this.IsActive
                };
            }
        }
    }

    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}
