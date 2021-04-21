using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using OracleClientExtensions;
using TravelSystemIntegration.Helpers;
using TravelSystemIntegration.Models;

namespace TravelSystemIntegration
{
    public static class ImportUsersToTravelSystem
    {
        private static string _oracleConnectionString;
        private static string _serviceRoot;
        private static string _base64UsernamePassword;
        private static OracleConnection _conn;
        private static string _employeeSql;

        [FunctionName("ImportUsersToTravelSystem")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            ConfigOracleConnection(log);

            try
            {
                await _conn.OpenAsync();
                SetOracleSessionProperties();

                var employeeDbs = await _conn.ExecuteEntitiesAsync<EmployeeDB>(_employeeSql);

                await CreateAndUpdateEmployeesAsync(employeeDbs, log);

            }
            catch (OracleException ex)
            {
                log.LogError(ex.Message);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);
            }
            finally
            {
                log.LogInformation($"Import Done");
                await _conn.CloseAsync();
                await _conn.DisposeAsync();
            }
        }

        private static void ConfigOracleConnection(ILogger log)
        {
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            _oracleConnectionString = config["connectionString"];
            _serviceRoot = config["serviceRoot"];
            _base64UsernamePassword = config["base64UsernamePassword"];
            _employeeSql = config["employeeSql"];

            log.LogInformation($"_connectionString: {_oracleConnectionString ?? null}");
            log.LogInformation($"_serviceRoot: {_serviceRoot ?? null}");
            log.LogInformation($"_base64UsernamePassword: {_base64UsernamePassword ?? null}");

            _conn = new OracleConnection(connectionString: _oracleConnectionString);
        }

        private static void SetOracleSessionProperties()
        {
            // Get session info from connection object
            OracleGlobalization info = _conn.GetSessionInfo();
            // Set the session's DateFormat for output
            info.DateFormat = "DD.MM.RRRR";
            _conn.SetSessionInfo(info);
        }

        private static async Task UpdateTimestampEmployee(string companyCode, string vismaUsername)
        {
            await _conn.ExecuteNonQueryAsync($"BEGIN agresso5.set_res_date_loaded('{companyCode}', '{vismaUsername}', CURRENT_DATE); END; ");
        }

        private static async Task CreateAndUpdateEmployeesAsync(IEnumerable<EmployeeDB> aviRrEmployees, ILogger log)
        {
            var travelSystemApi  = new TravelSystemApi(_serviceRoot, _base64UsernamePassword);

            var companies = await travelSystemApi.GetCompaniesEntityAsync();

            foreach (var aviRrEmployee in aviRrEmployees)
            {
                var employee = await travelSystemApi.GetEmployeesEntityFilteredByEmailAsync(aviRrEmployee.E_MAIL);
                if (employee == null)
                {
                    log.LogInformation($"Couldn't find employee with email {aviRrEmployee.E_MAIL}");
                    continue;
                }

                var company = companies.value.FirstOrDefault(c => c.CompanyRefImport == aviRrEmployee.FIRMA);
                if (company == null)
                {
                    log.LogInformation($"Couldn't find company with id {aviRrEmployee.FIRMA}. Employee Id:{aviRrEmployee.E_MAIL}");
                    continue;
                }

                if (employee.value.Count > 0)
                {
                    await UpdateEmployeeInTravelSystem(aviRrEmployee, employee, company, travelSystemApi);
                }
                else
                {
                    await CreateEmployeeInTravelSystem(aviRrEmployee, company, travelSystemApi);
                }
            }
        }

        private static async Task CreateEmployeeInTravelSystem(EmployeeDB aviRrEmployee, CompanyDto.Company company,
            TravelSystemApi travelSystemApi)
        {
            //create
            var empl = aviRrEmployee.ToEmployee(company.CompanyId.Value);
            var ret = await travelSystemApi.PostEmployeeEntityWithExtraFieldsAsync(empl);
            if (ret?.EmployeeId != null)
            {
                await travelSystemApi.PatchEmployeeEntityWithEmployeeAccessAsync(empl);
                await UpdateTimestampEmployee(aviRrEmployee.FIRMA, aviRrEmployee.RESSNR);
            }
        }

        private static async Task UpdateEmployeeInTravelSystem(EmployeeDB aviRrEmployee, EmployeeEntityDto employee,
            CompanyDto.Company company, TravelSystemApi travelSystemApi)
        {
            //Update
            var empl = aviRrEmployee.ToEmployee(employee.value.FirstOrDefault(), company.CompanyId.Value);

            var retPut = await travelSystemApi.PutEmployeeEntityWithExtraFieldsAsync(empl);

            //BUG UNIT4: temporary workaround: NOT ABLE TO CHANGE IsActive field with PUT Method (PutEmployeeEntityWithExtraFieldsAsync)
            var retPatch = await travelSystemApi.PatchEmployeeEntityWithExtraFieldsAsync(empl.ToEmployeeEntityPatch());

            await travelSystemApi.PatchEmployeeEntityWithEmployeeAccessAsync(empl);
            await UpdateTimestampEmployee(aviRrEmployee.FIRMA, aviRrEmployee.RESSNR);
        }
    }
}
