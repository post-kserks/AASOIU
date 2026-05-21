/// <summary>
/// Интерфейс для взаимодействия с пустыми элементами
/// </summary>
/// <typeparam name="T">Класс, который предполагается помещать в ячейку матрицы</typeparam>
public interface IMatrixCheckEmpty<T>
{
    T GetEmptyElement();
    bool CheckEmptyElement(T element);
}
