using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TravelSystemIntegration.Models;

namespace TravelSystemIntegration.Helpers
{
    public class TravelSystemApi
    {
        private string _serviceRoot { get; }
        private string _base64UsernamePassword { get; }
        public TravelSystemApi(string serviceRoot, string base64UsernamePassword)
        {
            _serviceRoot = serviceRoot;
            _base64UsernamePassword = base64UsernamePassword;
        }

        #region Company definition  


        public async Task<CompanyDto> GetCompaniesEntityAsync()
        {
            var httpClient = new HttpClientHelper<CompanyDto>(_base64UsernamePassword);
            var companies = await httpClient.GetSingleItemRequest(
                $"{_serviceRoot}companies",
                CancellationToken.None);

            return companies;
        }


        #endregion

        #region Employee definition
        public async Task<EmployeeEntityDto> GetEmployeesEntityFilteredByEmailAsync(string email)
        {
            var httpClient = new HttpClientHelper<EmployeeEntityDto>(_base64UsernamePassword);
            var employeeEntities = await httpClient.GetSingleItemRequest(
                $"{_serviceRoot}employees?$filter=Username eq '{email}'&$expand=EmployeeAddress,EmployeeDetail,EmployeeAccess",
                CancellationToken.None);

            return employeeEntities;
        }


        public async Task<EmployeeEntityDto.EmployeeEntity> PostEmployeeEntityWithExtraFieldsAsync(EmployeeEntityDto.EmployeeEntity employeeEntity)
        {

            var httpClient = new HttpClientHelper<EmployeeEntityDto.EmployeeEntity>(_base64UsernamePassword);
            var employeeEntityTemp = await httpClient.PostRequest($"{_serviceRoot}employees", employeeEntity, CancellationToken.None);
            employeeEntity.EmployeeId = employeeEntityTemp.EmployeeId;
            return employeeEntity;
        }

        public async Task<bool> PutEmployeeEntityWithExtraFieldsAsync(EmployeeEntityDto.EmployeeEntity employeeEntity)
        {

            var httpClient = new HttpClientHelper<EmployeeEntityDto.EmployeeEntity>(_base64UsernamePassword);
            await httpClient.PutRequest($"{_serviceRoot}employees({employeeEntity.EmployeeId.Value})", employeeEntity, CancellationToken.None);

            return true;
        }

        public async Task<bool> PatchEmployeeEntityWithExtraFieldsAsync(EmployeeEntityDto.EmployeeEntityPatch employeeEntity)
        {

            var httpClient = new HttpClientHelper<EmployeeEntityDto.EmployeeEntityPatch>(_base64UsernamePassword);
            await httpClient.PatchRequest($"{_serviceRoot}employees({employeeEntity.EmployeeId.Value})", employeeEntity, CancellationToken.None);

            return true;
        }


        public async Task<bool> PatchEmployeeEntityWithEmployeeAccessAsync(EmployeeEntityDto.EmployeeEntity employeeEntity)
        {

            var httpClient = new HttpClientHelper<EmployeeEntityDto.EmployeeAccess>(_base64UsernamePassword);
            await httpClient.PatchRequest($"{_serviceRoot}employees({employeeEntity.EmployeeId.Value})/EmployeeAccess", employeeEntity.EmployeeAccess, CancellationToken.None);

            return true;
        }
        #endregion
    }
}
