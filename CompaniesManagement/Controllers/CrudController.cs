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
            return Request.CreateResponse(HttpStatusCode.OK , branch);
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

            CompanyInfo responceObj = new CompanyInfo()
            {
                company = comData,
                childs = newComp.childs,
                parent = comData.baseCompanyId
            };
            
            var httpResp = Request.CreateResponse(HttpStatusCode.OK , responceObj);
           
           
            return httpResp;
        }
        [HttpDelete]
        public HttpResponseMessage DeleteOne(int idComp)
        {
            DataContext _dbContext = new DataContext();
            var deleteComp= _dbContext.Companies
                .First(x => x.company_Identifier == idComp);
            if (deleteComp == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    "Company with id " + idComp.ToString() + " not found to delete");
            }
            else
            {
                _dbContext.Companies.Remove(deleteComp);
                _dbContext.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK);
            }

        }
        [HttpPut]
        public HttpResponseMessage PutEmployee([FromBody] CompanyInfo newData)
        {
            var _dbContext = new DataContext();
            var wantedComp = _dbContext.Companies
                .First(x => x.company_Identifier == newData.parent);
            if (wantedComp == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound,
                    "Company with id " + newData.parent.ToString() + " not found to update");
            }
            else
            {
                wantedComp.company_Identifier = newData.company.company_Identifier;
                wantedComp.Name = newData.company.Name;
                wantedComp.AnnualEarnings = newData.company.AnnualEarnings;
                wantedComp.baseCompanyId = newData.company.baseCompanyId;

                _dbContext.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, newData);
            }
        }
    }
}
