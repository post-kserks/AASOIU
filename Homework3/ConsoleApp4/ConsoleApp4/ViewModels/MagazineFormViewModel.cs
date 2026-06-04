using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsoleApp4.ViewModels;

/// <summary>
/// Данные формы добавления и редактирования журнала.
/// </summary>
public class MagazineFormViewModel
{
    /// <summary>
    /// Идентификатор журнала при редактировании.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Идентификатор выбранного издательства.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Выберите издательство.")]
    [Display(Name = "Издательство")]
    public int PublisherId { get; set; }

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

    /// <summary>
    /// Список издательств для выпадающего списка.
    /// </summary>
    public IEnumerable<SelectListItem> Publishers { get; set; } = Array.Empty<SelectListItem>();
}
