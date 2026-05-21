using System.Text;

/// <summary>
/// Класс разреженной матрицы
/// </summary>
/// <typeparam name="T">Тип элементов матрицы</typeparam>
/// <param name="maxX">Максимальное количество колонок</param>
/// <param name="maxY">Максимальное количество строк</param>
/// <param name="checkEmpty">Проверка на пустоту</param>
public class Matrix<T>(int maxX, int maxY, IMatrixCheckEmpty<T> checkEmpty)
{
    readonly Dictionary<(int x, int y), T> _matrix = [];

    public T this[int x, int y]
    {
        set
        {
            CheckBounds(x, y);
            _matrix[(x, y)] = value;
        }
        get
        {
            CheckBounds(x, y);
            return _matrix.TryGetValue((x, y), out var element)
                ? element
                : checkEmpty.GetEmptyElement();
        }
    }

    void CheckBounds(int x, int y)
    {
        if (x < 0 || x >= maxX)
            throw new ArgumentOutOfRangeException(nameof(x), $"x={x} выходит за границы");
        if (y < 0 || y >= maxY)
            throw new ArgumentOutOfRangeException(nameof(y), $"y={y} выходит за границы");
    }

    public int ColumnWidth { get; set; } = 32;

    public override string ToString()
    {
        var b = new StringBuilder();
        for (int j = 0; j < maxY; j++)
        {
            b.Append('|');
            for (int i = 0; i < maxX; i++)
            {
                string cell = !checkEmpty.CheckEmptyElement(this[i, j])
                    ? $"{this[i, j]}"
                    : "-";
                b.Append(cell.PadRight(ColumnWidth));
                b.Append('|');
            }
            b.AppendLine();
        }
        return b.ToString();
    }
}
