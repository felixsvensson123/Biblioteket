using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grupp15.Shared.Models
{
    public class AttendModel
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Lecture))]
        public int LectureId { get; set; }
        public LectureModel? Lecture { get; set; }

        [ForeignKey(nameof(User))]
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
