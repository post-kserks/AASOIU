/// <summary>
/// Класс стек
/// </summary>
class SimpleStack<T> : SimpleList<T> where T : IComparable
{
    public void Push(T element) => Add(element);

    public T Pop()
    {
        T result;

        if (Count == 0)
            return default!;
        else if (Count == 1)
        {
            result = first!.Data;
            first = null;
            last = null;
        }
        else
        {
            var newLast = GetItem(Count - 2);
            result = newLast.Next!.Data;
            last = newLast;
            newLast.Next = null;
        }

        Count--;
        return result;
    }
}
