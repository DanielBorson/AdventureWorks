using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdventureWorks.Models
{
    public class PersonModel
    {
        public List<Person> People { get; set; }

        public int PageSize { get; set; }

        public int TotalRecord { get; set; }

        public int NumberOfPages { get; set; }

    }
}