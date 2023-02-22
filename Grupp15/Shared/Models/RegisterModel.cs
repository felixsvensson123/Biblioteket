using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grupp15.Shared.Models
{
    public class RegisterModel : LoginModel
    {
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
        public string? PersonName { get; set; }
        public string? Adress { get; set; }
    }
}
