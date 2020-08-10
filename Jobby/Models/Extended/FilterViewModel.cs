using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jobby.Models.Extended
{
    public class FilterViewModel
    {
        public List<FilterVM> CareerLevelFilters { get; set; } = new List<FilterVM>()
        {
            new FilterVM() { Value = "Volunteer" },
            new FilterVM() { Value = "Internship" },
            new FilterVM() { Value = "Student" },
            new FilterVM() { Value = "Entry Level" },
            new FilterVM() { Value = "Experienced" },
            new FilterVM() { Value = "Manager" },
            new FilterVM() { Value = "Senior Management" }
        };
        public List<FilterVM> PostDateFilters { get; set; } = new List<FilterVM>()
        {
            new FilterVM() { Value = "1" },
            new FilterVM() { Value = "7" },
            new FilterVM() { Value = "30" }
        };
        public List<FilterVM> CountryFilters { get; set; } = new List<FilterVM>()
        {
            new FilterVM() { Value = "Egypt" },
            new FilterVM() { Value = "Saudi Arabia" },
            new FilterVM() { Value = "United Arab Emirates" },
            new FilterVM() { Value = "Kuwait" },
            new FilterVM() { Value = "Qatar" },
            new FilterVM() { Value = "Iraq" }
        };
        public List<FilterVM> SalaryFilters { get; set; } = new List<FilterVM>()
        {
            new FilterVM() { Value = "3000" },
            new FilterVM() { Value = "6000" },
            new FilterVM() { Value = "9000" }
        };
    }
    public class FilterVM
    {
        public string Value { get; set; }
        public bool IsChecked { get; set; }
    }

}