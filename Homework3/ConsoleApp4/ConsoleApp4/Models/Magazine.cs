using System.ComponentModel.DataAnnotations;

namespace ConsoleApp4.Models;

/// <summary>
/// Журнал, основная таблица варианта 17 и сторона «много» связи.
/// </summary>
public class Magazine
{
    /// <summary>
    /// Идентификатор журнала.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор связанного издательства.
    /// </summary>
    [Display(Name = "Издательство")]
    public int PublisherId { get; set; }

    /// <summary>
    /// Связанное издательство.
    /// </summary>
    public Publisher? Publisher { get; set; }

    /// <summary>
    /// Название журнала.
    /// </summary>
    [Required(ErrorMessage = "Название журнала обязательно.")]
    [StringLength(120, ErrorMessage = "Название журнала не должно превышать 120 символов.")]
    [Display(Name = "Журнал")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Тираж журнала в тысячах экземпляров.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Тираж не может быть отрицательным.")]
    [Display(Name = "Тираж, тыс. экз.")]
    public int CirculationK { get; set; }
}
