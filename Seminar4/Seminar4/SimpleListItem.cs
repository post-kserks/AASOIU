/// <summary>
/// Контейнерный элемент списка
/// </summary>
/// <typeparam name="T">Тип элемента списка</typeparam>
/// <param name="data">Данные элемента</param>
public class SimpleListItem<T>(T data)
{
    public T Data { get; set; } = data;
    public SimpleListItem<T>? Next { get; set; }
}
