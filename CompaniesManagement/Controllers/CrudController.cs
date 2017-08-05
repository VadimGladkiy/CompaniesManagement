using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CompaniesManagement.Models;
using CompaniesManagement.Infrastructure;
using System.Web.Http.ModelBinding;

namespace CompaniesManagement.Controllers
{
    public class CrudController : ApiController
    {
        //get
        [HttpGet]
        // [Route("/api/Crud/Box")]
        public HttpResponseMessage GetBox()
        {
            // create db context
            DataContext _dbContext = new DataContext();

            CompaniesComparator CompsComparer = new CompaniesComparator();

            IEnumerable<Company> t = _dbContext.Companies.ToList();

            CompsComparer.setUnorderedList(t);

            IEnumerable<Company> getAll = t.OrderBy(x => x, CompsComparer);

            if (getAll != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, getAll);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
        }
        [HttpGet]
        [Route("api/Crud/{ids}/ByIds")]
        public HttpResponseMessage GetByIds([ModelBinder]int[] ids)
        {
            DataContext _dbContext = new DataContext();
            CompaniesComparator CompsComparer2 = new CompaniesComparator();

            List<Company> box = _dbContext.Companies
                .Where(x => ids.Contains(x.company_Identifier)).ToList();
            if (box == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    "Companies do not exist");
            }
            CompsComparer2.setUnorderedList(box);

            IEnumerable<Company> branch = box.OrderBy(x => x, CompsComparer2);
            return Request.CreateResponse(HttpStatusCode.OK, branch);
        }
        [HttpPost]
        public HttpResponseMessage PostOne([FromBody]CompanyInfo newComp)
        {

            DataContext _dbContext = new DataContext();
            var alreadyExists = _dbContext.Companies
                .Where(x => x.Name == newComp.company.Name).Any();
            if (alreadyExists == true)
            {
                return Request.CreateErrorResponse(HttpStatusCode.ResetContent,
                "the same company already exists");
            }
            List<Company> kidsRefer = new List<Company>();
            Company comData = newComp.company;
            if (newComp.parent != null)
            {
                comData.baseCompanyId = newComp.parent;
            }
            if (newComp.childs.Length > 0)
            {

                List<Company> kids = _dbContext.Companies.Where(x =>
                    newComp.childs.Contains(x.company_Identifier)).ToList();

                if (kids.First().baseCompanyId != kids.Last().baseCompanyId)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest,
                    "Companies aren't sublings ");
                }
                kidsRefer = kids;
            }

            _dbContext.Companies.Add(comData);
            _dbContext.SaveChanges();

            if (kidsRefer.Any())
            {
                var head = _dbContext.Companies.Where(
                    x => x.Name == newComp.company.Name &&
                    x.AnnualEarnings == newComp.company.AnnualEarnings).First();
                foreach (var p in kidsRefer)
                {
                    p.baseCompanyId = head.company_Identifier;
                }
                _dbContext.SaveChanges();
            }

            bool time = true;
            List<Company> box = new List<Company>();
            var key = _dbContext.Companies
            .Where(x => x.company_Identifier == comData.company_Identifier).First();
            key.baseCompanyId = comData.baseCompanyId.Value;
            while (time == true)
            {
                if (key.baseCompanyId == null)
                {
                    time = false;
                }
                var point = _dbContext.Companies
                    .Where(x => key.baseCompanyId == x.company_Identifier).First();
                box.Add(point);
                key = point;
            }
            foreach (var p in box)
            {
                p.AnnualEarnings += comData.AnnualEarnings;
            }

            _dbContext.SaveChanges();
            CompanyInfo responceObj = new CompanyInfo()
            {
                company = comData,
                childs = newComp.childs,
                parent = comData.baseCompanyId
            };

            var httpResp = Request.CreateResponse(HttpStatusCode.OK, responceObj);
            
            return httpResp;
        }
               
        [HttpDelete]
        [Route("api/Crud/One")]
        public HttpResponseMessage DeleteOne([FromBody]Company delComp)
        {
            DataContext _dbContext = new DataContext();
            
            var toDelete= _dbContext.Companies
                .First(x => x.company_Identifier == delComp.company_Identifier);
            if (toDelete == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    "Company with id " + delComp.company_Identifier.ToString() + " not found to delete");
            }
            else
            {   // find the relatives
                var relatItems = _dbContext.Companies
                    .Where(x => x.baseCompanyId == toDelete.company_Identifier).ToList();
                //    .Select(x=> x.company_Identifier).ToArray();
                Company MajorItem;
                int? parent;
                try
                {
                    MajorItem = _dbContext.Companies
                    .Where(x => x.company_Identifier == toDelete.baseCompanyId).First();
                }
                catch (Exception e) { MajorItem = null; }
                if (MajorItem != null)
                {
                    foreach (var p in relatItems)
                    {
                        p.baseCompanyId = MajorItem.company_Identifier;
                    }
                    parent = MajorItem.company_Identifier;
                }
                else
                {
                    foreach (var p in relatItems)
                    {
                        p.baseCompanyId = null;
                    }
                    parent = null;
                }
                _dbContext.Companies.Remove(toDelete);
                _dbContext.SaveChanges();

                int[] ch = relatItems.Select(x => x.company_Identifier).ToArray();
                CompanyInfo delInfo = new CompanyInfo()
                {
                    company = toDelete,
                    childs = ch,
                    parent = parent
                };
               
                return Request.CreateResponse(HttpStatusCode.OK, delInfo);
            }
        }
        
        [Route("api/Crud/put")]
        public HttpResponseMessage PutEmployee([FromBody] Company newData)
        {
            var _dbContext = new DataContext();
            var wantedComp = _dbContext.Companies
                .First(x => x.company_Identifier == newData.company_Identifier);
            if (wantedComp == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    "Company with id " + newData.company_Identifier.ToString() + " not found to update");
            }
            else
            {
               // wantedComp.company_Identifier = newData.company_Identifier;
                wantedComp.Name = newData.Name;
                wantedComp.AnnualEarnings = newData.AnnualEarnings;
              
                _dbContext.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, wantedComp);
            }
        }
    }
}
