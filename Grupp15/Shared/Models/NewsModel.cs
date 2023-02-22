using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grupp15.Shared.Models
{
    public class NewsModel
    {
        public int Id { get; set; }
        public string News { get; set; }
        public DateTime Created { get; set; }

        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public Models.ApplicationUser? User { get; set; }
    }
}
