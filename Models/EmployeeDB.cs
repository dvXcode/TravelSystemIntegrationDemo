using System;
using System.Collections.Generic;
using System.Text;

namespace TravelSystemIntegration.Models
{
    public class EmployeeDB
    {
        public string FORNAVN { get; set; }
        public string ETTERNAVN { get; set; }
        public string BRUKER { get; set; }
        public string FIRMA { get; set; }
        public string RESSNR { get; set; }
        public string LEVNR { get; set; }
        public string E_MAIL { get; set; }
        public decimal GODKJENNER { get; set; }
        public string REISEREGEL { get; set; }
        public string BILTYPE { get; set; }
        public string ANSVAR { get; set; }
        public DateTime DATO_FRA { get; set; }
        public DateTime DATO_TIL { get; set; }
        public string STATUS_AGR { get; set; }
        public decimal STATUS_1_0 { get; set; }
        public string ADRESSE { get; set; }
        public string POSTNUMMER { get; set; }
        public string POSTSTED { get; set; }
        public string BANK_KONTO { get; set; }
        public string NAME { get; set; }
        public string RESS_TYPE { get; set; }
        public DateTime LAST_UPDATE { get; set; }
        public DateTime DATE_LOAADED { get; set; }

        public EmployeeDB()
        {
        }

        public EmployeeEntityDto.EmployeeEntity ToEmployee(int companyId)
        {

            var employeeEntity = new EmployeeEntityDto.EmployeeEntity();
            employeeEntity.Initials = $"{this.ETTERNAVN}, {this.FORNAVN} ({this.RESSNR})";//	Etternavn, Fornavn (ansattnummer)
            employeeEntity.Firstname = this.FORNAVN;
            employeeEntity.LastName = this.ETTERNAVN;
            employeeEntity.Username = this.E_MAIL;
            employeeEntity.Email1 = this.E_MAIL;
            employeeEntity.SsoRef = this.E_MAIL;
            employeeEntity.CsaRef = this.RESSNR;
            employeeEntity.IsActive = (this.STATUS_AGR != "C");//C - Sletted P - Parkert N -Aktive
            employeeEntity.EmployedDate = DATO_FRA.Date;

            if (this.STATUS_AGR == "P")
                employeeEntity.LeaveDate = LAST_UPDATE.Date;

            employeeEntity.CompanyId = companyId;
            employeeEntity.DepartmentId = 2;
            employeeEntity.EmployeeAccess = new EmployeeEntityDto.EmployeeAccess() { RoleEmployee = true, UseMobile = true, UseExpense = true, RoleProject = this.GODKJENNER == 1 };
            employeeEntity.EmployeeAddress = new EmployeeEntityDto.EmployeeAddress() { AddressLine1 = this.ADRESSE, PostAddress = this.POSTSTED, Zipcode = this.POSTNUMMER };
            employeeEntity.EmployeeDetail = new EmployeeEntityDto.EmployeeDetail()
            {
                EmployeeRef = this.RESSNR,
                EmployeeRefImport = this.RESSNR
            };
            return employeeEntity;
        }

        internal EmployeeEntityDto.EmployeeEntity ToEmployee(EmployeeEntityDto.EmployeeEntity employeeEntity, int companyId)
        {
            employeeEntity.Initials = $"{this.ETTERNAVN}, {this.FORNAVN} ({this.RESSNR})";//	Etternavn, Fornavn (ansattnummer)
            employeeEntity.Firstname = this.FORNAVN;
            employeeEntity.LastName = this.ETTERNAVN;
            employeeEntity.Username = this.E_MAIL;
            employeeEntity.Email1 = this.E_MAIL;
            employeeEntity.SsoRef = this.E_MAIL;
            employeeEntity.CsaRef = this.RESSNR;
            employeeEntity.IsActive = (this.STATUS_AGR != "C");//C - Sletted P - Parkert N -Aktive
            employeeEntity.EmployedDate = DATO_FRA.Date;

            if (this.STATUS_AGR == "P")
                employeeEntity.LeaveDate = LAST_UPDATE.Date;

            employeeEntity.CompanyId = companyId;
            employeeEntity.DepartmentId = 2; //Fra Sverre:Vi kan derfor foreløpig se bort fra denne og sette «DepartmentId» = 1 i ansatt-importen.
            employeeEntity.EmployeeAccess = new EmployeeEntityDto.EmployeeAccess() { RoleEmployee = true, UseMobile = true, UseExpense = true, RoleProject = this.GODKJENNER == 1 };
            employeeEntity.EmployeeAddress = new EmployeeEntityDto.EmployeeAddress() { AddressLine1 = this.ADRESSE, PostAddress = this.POSTSTED, Zipcode = this.POSTNUMMER };
            employeeEntity.EmployeeDetail = new EmployeeEntityDto.EmployeeDetail()
            {
                EmployeeRef = this.RESSNR,
                EmployeeRefImport = this.RESSNR
            };
            return employeeEntity;
        }
    }
}
