/// <summary>
/// Класс Прямоугольник
/// </summary>
/// <param name="height">Высота</param>
/// <param name="width">Ширина</param>
/// <param name="type">Тип фигуры для удобства наследования</param>
internal class Rectangle(double height, double width, string type = "Прямоугольник") : Figure(type)
{
    public override double Area => width * height;
}
