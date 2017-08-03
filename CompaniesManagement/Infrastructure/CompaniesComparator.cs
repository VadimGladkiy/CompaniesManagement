using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CompaniesManagement.Models;

namespace CompaniesManagement.Infrastructure
{
    public class CompaniesComparator : IComparer<Company>
    {
        private IEnumerable<Company> uoListToCompare;
        
        public int Compare(Company c1, Company c2)
        {
            Company t1 = c1, t2 = c2;
            int counter1 = 0;
            int counter2 = 0;
            while (t1.baseCompanyId != null) 
            {
                counter1++;
                t1 = getNext(t1);
            }
            while (t2.baseCompanyId != null)
            {
                counter2++;
                t2 = getNext(t2);
            }
            if (counter1 < counter2) return -1;
            else if (counter1 == counter2) return 0;
            else return 1;
        }
        private Company getNext( Company getPre )
        {
            return uoListToCompare.Where(x => x.company_Identifier
                                           == getPre.baseCompanyId).First();
        }
        public void setUnorderedList(IEnumerable<Company> uoList)
        {
            this.uoListToCompare = uoList;
        }
    }
}