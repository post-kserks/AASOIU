/// <summary>
/// Абстрактная фигура
/// </summary>
/// <param name="type">Название типа фигуры</param>
internal abstract class Figure(string type) : IComparable
{
    public string Type { get; } = type;

    public abstract double Area { get; }

    public override string ToString() =>
        $"{Type} площадью {Area}";

    public int CompareTo(object? obj) =>
        obj is Figure other
            ? Area.CompareTo(other.Area)
            : throw new ArgumentException("Объект не является фигурой");
}
