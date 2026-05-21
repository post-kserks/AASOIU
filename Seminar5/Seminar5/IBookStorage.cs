/// <summary>
/// Контракт для хранения книг.
/// </summary>
interface IBookStorage
{
    void Save(string title, string author);
}
