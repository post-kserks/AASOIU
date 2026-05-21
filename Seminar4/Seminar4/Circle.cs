/// <summary>
/// Класс Круг
/// </summary>
/// <param name="radius">Радиус</param>
internal class Circle(double radius) : Figure("Круг")
{
    public override double Area => Math.PI * radius * radius;
}
