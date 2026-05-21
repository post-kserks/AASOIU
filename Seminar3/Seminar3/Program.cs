using Microsoft.Data.Sqlite;

Console.OutputEncoding = System.Text.Encoding.UTF8;

if (args.Length > 0)
{
    return RunPrototype(args);
}

const string dbFile = "developers.db";
const string devCsv = "dev.csv";
const string depCsv = "dep.csv";

CreateDatabase(dbFile);
LoadData(dbFile, devCsv, depCsv);

PrintData(dbFile, "dep");
PrintData(dbFile, "dev");

List<string> names = Projection(dbFile, "dev", "dev_name");
Console.WriteLine("\n=== Результат Projection(dev, dev_name) ===");
foreach (var name in names)
    Console.WriteLine(name);

List<string[]> rows = Where(dbFile, "dev", "dep_id", "2");
Console.WriteLine("\n=== Результат Where(dev, dep_id, 2) ===");
foreach (var row in rows)
    Console.WriteLine(string.Join(" | ", row));

var (columns, joinRows) = Join(dbFile, "dev", "dep", "dep_id", "dep_id");
Console.WriteLine("\n=== Результат Join(dev, dep, dep_id, dep_id) ===");
Console.WriteLine(string.Join(" | ", columns));
Console.WriteLine(new string('-', 80));
foreach (var row in joinRows)
    Console.WriteLine(string.Join(" | ", row));

var (gavgCols, gavgRows) = GroupAvg(dbFile, "dev", "dep_id", "dev_commits");
Console.WriteLine("\n=== Результат GroupAvg(dev, dep_id, dev_commits) ===");
Console.WriteLine(string.Join(" | ", gavgCols));
Console.WriteLine(new string('-', 40));
foreach (var row in gavgRows)
    Console.WriteLine(string.Join(" | ", row));

return 0;

// --- Прототип СУБД (режимы из аргументов командной строки) ---

static int RunPrototype(string[] args)
{
    string mode = args[0].ToLower();

    switch (mode)
    {
        case "projection":
        {
            var table = ReadCsv(Console.In, ';');
            string columnName = args[1];
            var result = CsvProjection(table, columnName);
            WriteCsv(Console.Out, result, ';');
            break;
        }
        case "where":
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine(
                    "Ошибка: режим 'where' требует два параметра: where <колонка> <значение>");
                return 1;
            }
            var table = ReadCsv(Console.In, ';');
            string columnName = args[1];
            string value = args[2];
            var result = CsvWhere(table, columnName, value);
            WriteCsv(Console.Out, result, ';');
            break;
        }
        case "join":
        {
            if (args.Length < 5)
            {
                Console.Error.WriteLine(
                    "Использование: program join <таблица1> <таблица2> <ключ1> <ключ2>");
                return 1;
            }
            using var reader1 = File.OpenText(args[1] + ".csv");
            using var reader2 = File.OpenText(args[2] + ".csv");
            var left = ReadCsv(reader1, ';');
            var right = ReadCsv(reader2, ';');
            var result = CsvJoin(left, right, args[3], args[4]);
            WriteCsv(Console.Out, result, ';');
            break;
        }
        case "group_avg":
        {
            if (args.Length < 3)
            {
                Console.Error.WriteLine(
                    "Использование: program group_avg <колонка_группировки> <колонка_значений>");
                return 1;
            }
            var table = ReadCsv(Console.In, ';');
            string groupColumn = args[1];
            string valueColumn = args[2];
            var result = CsvGroupAvg(table, groupColumn, valueColumn);
            WriteCsv(Console.Out, result, ';');
            break;
        }
        default:
            Console.Error.WriteLine(
                "Неизвестный режим. Доступны: projection, where, join, group_avg");
            return 1;
    }

    return 0;
}

// --- SQLite ---

static void CreateDatabase(string dbPath)
{
    if (File.Exists(dbPath))
        File.Delete(dbPath);

    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var command = connection.CreateCommand();
    command.CommandText = @"
CREATE TABLE dep (
    dep_id INTEGER PRIMARY KEY,
    dep_name TEXT NOT NULL
);";
    command.ExecuteNonQuery();

    command.CommandText = @"
CREATE TABLE dev (
    dev_id INTEGER PRIMARY KEY,
    dep_id INTEGER NOT NULL,
    dev_name TEXT NOT NULL,
    dev_commits INTEGER NOT NULL,
    FOREIGN KEY (dep_id) REFERENCES dep(dep_id)
);";
    command.ExecuteNonQuery();

    Console.WriteLine($"[OK] База данных «{dbPath}» создана, таблицы dep и dev готовы.");
}

static void LoadData(string dbPath, string devCsvPath, string depCsvPath)
{
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    using (var transaction = connection.BeginTransaction())
    {
        var lines = File.ReadAllLines(depCsvPath);
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(';');
            if (parts.Length < 2) continue;

            var cmd = connection.CreateCommand();
            cmd.CommandText = "INSERT INTO dep (dep_id, dep_name) VALUES (@id, @name);";
            cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@name", parts[1]);
            cmd.ExecuteNonQuery();
        }
        transaction.Commit();
        Console.WriteLine($"[OK] Загружено строк из «{depCsvPath}»: {lines.Length - 1}");
    }

    using (var transaction = connection.BeginTransaction())
    {
        var lines = File.ReadAllLines(devCsvPath);
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(';');
            if (parts.Length < 4) continue;

            var cmd = connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO dev (dev_id, dep_id, dev_name, dev_commits)
                                VALUES (@devId, @depId, @name, @commits);";
            cmd.Parameters.AddWithValue("@devId", int.Parse(parts[0]));
            cmd.Parameters.AddWithValue("@depId", int.Parse(parts[1]));
            cmd.Parameters.AddWithValue("@name", parts[2]);
            cmd.Parameters.AddWithValue("@commits", int.Parse(parts[3]));
            cmd.ExecuteNonQuery();
        }
        transaction.Commit();
        Console.WriteLine($"[OK] Загружено строк из «{devCsvPath}»: {lines.Length - 1}");
    }
}

static void PrintData(string dbPath, string tableName)
{
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var cmd = connection.CreateCommand();
    cmd.CommandText = $"SELECT * FROM {tableName} ORDER BY 1;";

    using var reader = cmd.ExecuteReader();
    int columnCount = reader.FieldCount;
    const int colWidth = 20;

    Console.WriteLine($"\n========== Таблица {tableName} ==========");
    for (int c = 0; c < columnCount; c++)
        Console.Write($"{reader.GetName(c),-colWidth}");
    Console.WriteLine();
    Console.WriteLine(new string('-', colWidth * columnCount));

    while (reader.Read())
    {
        for (int c = 0; c < columnCount; c++)
            Console.Write($"{reader.GetValue(c),-colWidth}");
        Console.WriteLine();
    }
}

static List<string> Projection(string dbPath, string tableName, string columnName)
{
    var result = new List<string>();
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var cmd = connection.CreateCommand();
    cmd.CommandText = $"SELECT {columnName} FROM {tableName} ORDER BY 1;";

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
        result.Add(reader.GetValue(0).ToString()!);

    return result;
}

static List<string[]> Where(string dbPath, string tableName, string columnName, string value)
{
    var result = new List<string[]>();
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var cmd = connection.CreateCommand();
    cmd.CommandText = $"SELECT * FROM {tableName} WHERE {columnName} = @val ORDER BY 1;";
    cmd.Parameters.AddWithValue("@val", value);

    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        var row = new string[reader.FieldCount];
        for (int c = 0; c < reader.FieldCount; c++)
            row[c] = reader.GetValue(c).ToString()!;
        result.Add(row);
    }

    return result;
}

static (string[] columns, List<string[]> rows) Join(
    string dbPath,
    string table1, string table2,
    string key1, string key2)
{
    var rows = new List<string[]>();
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var cmd = connection.CreateCommand();
    cmd.CommandText = $@"
SELECT *
FROM {table1}
INNER JOIN {table2}
ON {table1}.{key1} = {table2}.{key2}
ORDER BY 1;";

    using var reader = cmd.ExecuteReader();

    var columns = new string[reader.FieldCount];
    for (int c = 0; c < reader.FieldCount; c++)
        columns[c] = reader.GetName(c);

    while (reader.Read())
    {
        var row = new string[reader.FieldCount];
        for (int c = 0; c < reader.FieldCount; c++)
            row[c] = reader.GetValue(c).ToString()!;
        rows.Add(row);
    }

    return (columns, rows);
}

static (string[] columns, List<string[]> rows) GroupAvg(
    string dbPath,
    string tableName,
    string groupColumn,
    string avgColumn)
{
    var rows = new List<string[]>();
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    var cmd = connection.CreateCommand();
    cmd.CommandText = $@"
SELECT {groupColumn}, AVG({avgColumn}) AS avg_{avgColumn}
FROM {tableName}
GROUP BY {groupColumn}
ORDER BY 1;";

    using var reader = cmd.ExecuteReader();

    var columns = new string[reader.FieldCount];
    for (int c = 0; c < reader.FieldCount; c++)
        columns[c] = reader.GetName(c);

    while (reader.Read())
    {
        var row = new string[reader.FieldCount];
        for (int c = 0; c < reader.FieldCount; c++)
            row[c] = reader.GetValue(c).ToString()!;
        rows.Add(row);
    }

    return (columns, rows);
}

// --- CSV: чтение / запись ---

static CsvTable ReadCsv(TextReader reader, char separator)
{
    string? headerLine = reader.ReadLine();
    if (headerLine is null)
        throw new InvalidOperationException("Входной поток пуст — нет строки заголовков.");

    string[] headers = headerLine.Split(separator);
    var rows = new List<CsvRow>();

    string? line;
    while ((line = reader.ReadLine()) is not null)
    {
        if (string.IsNullOrWhiteSpace(line))
            continue;

        string[] parts = line.Split(separator);
        rows.Add(new CsvRow(parts));
    }

    return new CsvTable(headers, rows);
}

static void WriteCsv(TextWriter writer, CsvTable table, char separator)
{
    writer.WriteLine(string.Join(separator, table.Headers));
    foreach (var row in table.Rows)
        writer.WriteLine(string.Join(separator, row.Fields));
}

static int FindColumnIndex(CsvTable table, string columnName)
{
    int index = Array.IndexOf(table.Headers, columnName);
    if (index < 0)
        throw new ArgumentException(
            $"Колонка «{columnName}» не найдена. Доступные колонки: {string.Join(", ", table.Headers)}");
    return index;
}

// --- Операции реляционной алгебры на CSV ---

static CsvTable CsvProjection(CsvTable table, string columnName)
{
    int colIndex = FindColumnIndex(table, columnName);
    string[] newHeaders = [columnName];
    var newRows = new List<CsvRow>();

    foreach (var row in table.Rows)
    {
        string[] fields = [row.Fields[colIndex]];
        newRows.Add(new CsvRow(fields));
    }

    return new CsvTable(newHeaders, newRows);
}

static CsvTable CsvWhere(CsvTable table, string columnName, string value)
{
    int colIndex = FindColumnIndex(table, columnName);
    var newRows = new List<CsvRow>();

    foreach (var row in table.Rows)
    {
        if (row.Fields[colIndex] == value)
            newRows.Add(row);
    }

    return new CsvTable(table.Headers, newRows);
}

static CsvTable CsvJoin(CsvTable left, CsvTable right, string leftKey, string rightKey)
{
    int leftKeyIndex = FindColumnIndex(left, leftKey);
    int rightKeyIndex = FindColumnIndex(right, rightKey);

    var newHeaders = new string[left.Headers.Length + right.Headers.Length];
    for (int i = 0; i < left.Headers.Length; i++)
        newHeaders[i] = left.Headers[i];
    for (int i = 0; i < right.Headers.Length; i++)
        newHeaders[left.Headers.Length + i] = right.Headers[i];

    var newRows = new List<CsvRow>();

    foreach (var leftRow in left.Rows)
    {
        foreach (var rightRow in right.Rows)
        {
            if (leftRow.Fields[leftKeyIndex] == rightRow.Fields[rightKeyIndex])
            {
                var fields = new string[leftRow.Fields.Length + rightRow.Fields.Length];
                for (int i = 0; i < leftRow.Fields.Length; i++)
                    fields[i] = leftRow.Fields[i];
                for (int i = 0; i < rightRow.Fields.Length; i++)
                    fields[leftRow.Fields.Length + i] = rightRow.Fields[i];
                newRows.Add(new CsvRow(fields));
            }
        }
    }

    return new CsvTable(newHeaders, newRows);
}

static double Average(List<double> values)
{
    double sum = 0;
    for (int i = 0; i < values.Count; i++)
        sum += values[i];
    return sum / values.Count;
}

static CsvTable CsvGroupAvg(CsvTable table, string groupColumn, string valueColumn)
{
    int groupIndex = FindColumnIndex(table, groupColumn);
    int valueIndex = FindColumnIndex(table, valueColumn);

    var groups = new Dictionary<string, List<double>>();

    foreach (var row in table.Rows)
    {
        string key = row.Fields[groupIndex];
        double value = double.Parse(row.Fields[valueIndex]);

        if (!groups.ContainsKey(key))
            groups[key] = new List<double>();
        groups[key].Add(value);
    }

    string[] newHeaders = [groupColumn, "avg_" + valueColumn];
    var newRows = new List<CsvRow>();

    foreach (var pair in groups)
    {
        string avg = Average(pair.Value).ToString("F2");
        newRows.Add(new CsvRow([pair.Key, avg]));
    }

    return new CsvTable(newHeaders, newRows);
}

record CsvRow(string[] Fields);
record CsvTable(string[] Headers, List<CsvRow> Rows);
