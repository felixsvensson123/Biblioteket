using System.ComponentModel.DataAnnotations;

namespace Grupp15.Shared.Models
{
    public class LectureModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; } = "";
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; } = "";
        [Required(ErrorMessage = "Lecturers name is required")]
        public string LecturerName { get; set; } = "";
        [Required(ErrorMessage = "Estimated length is required")]
        public int EstimatedLengthMins { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Max student count is required")]
        public int MaxStudentCount { get; set; }
        public int AttendingCount { get; set; }
        public List<AttendModel>? Attending { get; set; } //Users attending to this lecture
    }
}
