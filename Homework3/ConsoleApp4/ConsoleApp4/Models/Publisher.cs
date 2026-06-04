using System.ComponentModel.DataAnnotations;

namespace ConsoleApp4.Models;

/// <summary>
/// Издательство, справочная таблица варианта 17 и сторона «один» связи.
/// </summary>
public class Publisher
{
    /// <summary>
    /// Идентификатор издательства.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Название издательства.
    /// </summary>
    [Required(ErrorMessage = "Название издательства обязательно.")]
    [StringLength(100, ErrorMessage = "Название издательства не должно превышать 100 символов.")]
    [Display(Name = "Издательство")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Журналы, выпускаемые издательством.
    /// </summary>
    public ICollection<Magazine> Magazines { get; set; } = new List<Magazine>();
}
