using System.ComponentModel.DataAnnotations;

namespace SqliteApplicationSettings.DTOs;

/// <summary>
/// A Data Transfer Object
/// </summary>
public class DTO
{
    [Key]
    public int Id { get; set; }
    public DateTime Created { get; set; } = DateTime.MinValue;
    public DateTime LastModified { get; set; } = DateTime.MinValue;
    public string InfoStringObject { get; set; } = string.Empty;
}


