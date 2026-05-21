/// <summary>
/// Реализация проверки на пустоту для геометрической фигуры
/// </summary>
internal class FigureMatrixCheckEmpty : IMatrixCheckEmpty<Figure>
{
    public Figure GetEmptyElement() => null!;

    public bool CheckEmptyElement(Figure element) => element is null;
}
