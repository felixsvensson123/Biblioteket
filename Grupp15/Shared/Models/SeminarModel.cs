using System.ComponentModel.DataAnnotations;

namespace Grupp15.Shared;

public class SeminarModel
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Lecturer { get; set; } = String.Empty;
    public int Length { get; set; }
    public bool IsBooked { get; set; }
}