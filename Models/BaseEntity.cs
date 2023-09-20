using System.ComponentModel.DataAnnotations;

namespace museum.Models;

public interface IBaseEntity
{
    [DataType(DataType.Date)] public DateTime CreatedAt { get; set; }

    [DataType(DataType.Date)] public DateTime UpdatedAt { get; set; }
}