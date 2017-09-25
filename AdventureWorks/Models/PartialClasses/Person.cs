using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdventureWorks.Models
{
    //Enum
    public enum EmailPromo : int
    {
        None = 0,
        PromoOne = 1,
        PromoTwo = 2,
        PromoThree = 3
    }

    public partial class Person
    {
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public int NoOfPages { get; set; }

        public EmailPromo PromoName
        {
            get { return (EmailPromo)EmailPromotion; }
        }
    }
}