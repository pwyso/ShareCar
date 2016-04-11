//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Web.Mvc;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ShareCar.DateValidation
//{
//    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
//    public sealed class ValidDateAttribute : ValidationAttribute, IClientValidatable
//    {
//            public override bool IsValid(object value)
//            {
//                return value != null && (DateTime)value > DateTime.Today;
//            }

//        //protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        //{
//        //    if (value != null)
//        //    {
//        //        DateTime date = Convert.ToDateTime(value);
//        //        if (date < DateTime.Today)
//        //        {
//        //            return new ValidationResult("Past date not allowed.");
//        //        }
//        //    }
//        //    return ValidationResult.Success;
//        //}

//        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
//        {
//            ModelClientValidationRule mcvr = new ModelClientValidationRule();
//            mcvr.ErrorMessage = "Past date not allowed.";
//            mcvr.ValidationType = "validdate";
//            return new[] { mcvr };
//        }
//    }
//}
