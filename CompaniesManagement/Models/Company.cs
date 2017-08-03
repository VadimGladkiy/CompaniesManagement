using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace CompaniesManagement.Models
{
    public class Company
    {
        // fields for EF Code First approach

        // set up a Primary Key
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int company_Identifier { set; get; }

        [Required]
        [MaxLength(50)]
        public string Name { set; get; }

        [Required]
        public double AnnualEarnings { set; get; }

        // link on a base company
        public int? baseCompanyId { set; get; }

    }
}