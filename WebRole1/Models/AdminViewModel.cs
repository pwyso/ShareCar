using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ShareCar.Models
{
    public class RoleViewModel
    {
        [Key]
        public string RoleId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression("[a-zA-Z]+", ErrorMessage = "Role Name: only letters allowed.")]
        [Display(Name = "Role Name"), StringLength(30, ErrorMessage = "Role Name: max. lenght is 30.")]
        public string Name { get; set; }
    }

    public class EditUserViewModel
    {
        [Key]
        public string Id { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false)]
        [RegularExpression("[a-zA-Z0-9]+", ErrorMessage = "Name: only numbers and letters allowed.")]
        [Display(Name = "Name"), StringLength(30, ErrorMessage = "Name: max. 30 characters allowed.")]
        public string Name { get; set; }

        [RegularExpression("[0-9]{8,12}", ErrorMessage = "Phone No: allowed numbers only, lenght 8 - 12.")]
        [Display(Name = "Phone No.")]
        public string PhNo { get; set; }

        [Display(Name = "Locked till")]
        public DateTime? LockedByDate { get; set; }

        [Required]
        [Display(Name = "Smoker")]
        public IsSmoking Smoker { get; set; }

        [Required]
        [Display(Name = "Age"), Range(18, 99, ErrorMessage = "Age: you must be min. 18 years old.")]
        public int Age { get; set; }

        public IEnumerable<SelectListItem> RolesList { get; set; }
    }
}