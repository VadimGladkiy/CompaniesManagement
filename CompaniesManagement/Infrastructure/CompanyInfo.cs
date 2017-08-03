using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CompaniesManagement.Models;

namespace CompaniesManagement.Infrastructure
{
    public class CompanyInfo
    {
        private int[] kinders;
        public Company company { set; get; }
        public int[] childs
        {
            set { kinders = value; }
            get {
                    if (kinders != null) return kinders;
                    else return kinders = new int[]{ };
                }
        }
        public int? parent { set; get; }
    }
}