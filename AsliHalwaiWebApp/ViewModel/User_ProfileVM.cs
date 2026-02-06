using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace AsliHalwaiWebApp.ViewModels
{
    public class User_ProfileVM
    {
        public int UserID { get; set; }
        public int UserTypeId { get; set; }
        public string UserType { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        public string ContactNo { get; set; }
        public string GenderTitle { get; set; }
        public string EmailAddress { get; set; }

        public DateTime RegistrationDate { get; set; }

        [DataType(DataType.MultilineText)]
        public string FullAddress { get; set; }

        public string UserStatus { get; set; }
        public int UserStatusID { get; set; }
        public DateTime? UserStatusChangeDate { get; set; }

        public DateTime UserDetailProvideDate { get; set; }

        public string PhotoPath { get; set; }

        //→ AadharCard
        [Display(Name = "Aadhar Card Number")]
        public string AadharCard { get; set; }

        public string EducationLevel { get; set; }
        public string ExperenceLevel { get; set; }

        public string EducationLastDegreePhotoPath { get; set; }
        public string ExperenceLastPhotoPath { get; set; }

        // ======================
        // FILE UPLOADS (ASP.NET Core)
        // ======================

        [NotMapped]
        [Display(Name = "Profile Photo")]
        public IFormFile UserPhoto { get; set; }

        [NotMapped]
        [Display(Name = "Education Last Degree Scan Photo")]
        public IFormFile EducationLastDegreePhoto { get; set; }

        [NotMapped]
        [Display(Name = "Last Experience Scan Photo")]
        public IFormFile ExperenceLastPhoto { get; set; }
    }
}
