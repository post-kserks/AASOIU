using Microsoft.Data.Sqlite;

namespace ConsoleApp4;
class DatabaseManager
{
    private readonly string _connectionString;
    public DatabaseManager(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
    }
    public void InitializeDatabase(string pubCsvPath, string magCsvPath)
    {
        CreateTables();

        if (GetAllPublishers().Count == 0 && File.Exists(pubCsvPath))
        {
            ImportPublishersFromCsv(pubCsvPath);
            Console.WriteLine($"[OK] Загружены издательства из {pubCsvPath}");
        }

        if (GetAllMagazines().Count == 0 && File.Exists(magCsvPath))
        {
            ImportMagazinesFromCsv(magCsvPath);
            Console.WriteLine($"[OK] Загружены журналы из {magCsvPath}");
        }
    }
    private void CreateTables()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS pub (
                pub_id   INTEGER PRIMARY KEY AUTOINCREMENT,
                pub_name TEXT    NOT NULL
            );
            CREATE TABLE IF NOT EXISTS mag (
                mag_id            INTEGER PRIMARY KEY AUTOINCREMENT,
                pub_id            INTEGER NOT NULL,
                mag_name          TEXT    NOT NULL,
                mag_circulation_k INTEGER NOT NULL,
                FOREIGN KEY (pub_id) REFERENCES pub(pub_id)
            );";
        cmd.ExecuteNonQuery();
    }
    private void ImportPublishersFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 2) continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO pub (pub_id, pub_name) VALUES (@id, @name)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
    }
    private void ImportMagazinesFromCsv(string path)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        string[] lines = File.ReadAllLines(path);
        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(';');
            if (parts.Length < 4) continue;

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                INSERT INTO mag (mag_id, pub_id, mag_name, mag_circulation_k)
                VALUES (@id, @pubId, @name, @circ)";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@pubId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@circ", int.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
    }
    public List<Publisher> GetAllPublishers()
    {
        var result = new List<Publisher>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT pub_id, pub_name FROM pub ORDER BY pub_id";
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new Publisher(
                reader.GetInt32(0),
                reader.GetString(1)));
        }
        return result;
    }
    public List<Magazine> GetAllMagazines()
    {
        var result = new List<Magazine>();
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT mag_id, pub_id, mag_name, mag_circulation_k FROM mag ORDER BY mag_id";
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new Magazine(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3)));
        }
        return result;
    }
    public Magazine? GetMagazineById(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText =
            "SELECT mag_id, pub_id, mag_name, mag_circulation_k FROM mag WHERE mag_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            return new Magazine(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt32(3));
        }
        return null;
    }
    public void AddMagazine(Magazine mag)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO mag (pub_id, mag_name, mag_circulation_k)
            VALUES (@pubId, @name, @circ)";
        cmd.Parameters.AddWithValue("@pubId", mag.PublisherId);
        cmd.Parameters.AddWithValue("@name", mag.Name);
        cmd.Parameters.AddWithValue("@circ", mag.CirculationK);
        cmd.ExecuteNonQuery();
    }
    public void UpdateMagazine(Magazine mag)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            UPDATE mag
            SET pub_id = @pubId, mag_name = @name, mag_circulation_k = @circ
            WHERE mag_id = @id";
        cmd.Parameters.AddWithValue("@id", mag.Id);
        cmd.Parameters.AddWithValue("@pubId", mag.PublisherId);
        cmd.Parameters.AddWithValue("@name", mag.Name);
        cmd.Parameters.AddWithValue("@circ", mag.CirculationK);
        cmd.ExecuteNonQuery();
    }
    public void DeleteMagazine(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM mag WHERE mag_id = @id";
        cmd.Parameters.AddWithValue("@id", id);
        cmd.ExecuteNonQuery();
    }
    public (string[] columns, List<string[]> rows) ExecuteQuery(string sql)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        string[] columns = new string[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
            columns[i] = reader.GetName(i);

        var rows = new List<string[]>();
        while (reader.Read())
        {
            string[] row = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                row[i] = reader.GetValue(i)?.ToString() ?? "";
            rows.Add(row);
        }

        return (columns, rows);
    }
}
