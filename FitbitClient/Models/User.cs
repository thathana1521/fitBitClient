using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitbitClient.Models
{
    public class User
    {
        public int age { get; set; }
        public Boolean ambassador { get; set; }
        public Boolean autoStrideEnabled { get; set; }
        public string avatar { get; set; }
        public string avatar150 { get; set; }
        public string avatar640 { get; set; }
        public int averageDailySteps { get; set; }
        public string clockTimeDisplayFormat { get; set; }
        public Boolean corporate { get; set; }
        public Boolean corporateAdmin { get; set; }
        public string country { get; set; }
        public DateTime dateOfBirth { get; set; }
        public string displayName { get; set; }
        public string displayNameSetting { get; set; }
        public string distanceUnit { get; set; }
        public string encodedId { get; set; }
        public Features features { get; set; }
        public string firstName { get; set; }
        public string foodsLocale { get; set; }
        public string fullName { get; set; }
        public string gender { get; set; }


    }
}
