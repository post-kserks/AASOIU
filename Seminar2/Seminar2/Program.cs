// ============================================================
// Тестовые матрицы
// ============================================================

// Рисунок 1 — COO (5×6, 6 ненулевых)
int[,] matrixCOO = {
    { 1, 0, 0, 0, 0, 1 },
    { 0, 2, 0, 0, 0, 0 },
    { 3, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 4 },
    { 0, 0, 5, 0, 0, 0 }
};

// Рисунок 2 — LIL (4×6, 6 ненулевых)
int[,] matrixLIL = {
    { 0, 1, 0, 2, 0, 0 },
    { 0, 0, 0, 0, 0, 3 },
    { 4, 0, 5, 0, 0, 0 },
    { 0, 0, 0, 6, 0, 0 }
};

// Рисунок 3 — CSR (7×5)
int[,] matrixCSR = {
    { 8, 0, 2, 0, 0 },
    { 0, 0, 5, 0, 0 },
    { 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0 },
    { 0, 0, 7, 1, 2 },
    { 0, 0, 0, 0, 0 },
    { 0, 0, 0, 9, 0 }
};

// ============================================================
// ЗАДАНИЕ 1: COO
// ============================================================

PrintSeparator("ЗАДАНИЕ 1: COO - КООРДИНАТНЫЙ ФОРМАТ ХРАНЕНИЯ");

int rCOO = matrixCOO.GetLength(0);
int cCOO = matrixCOO.GetLength(1);
int nCOO = CountNonZero(matrixCOO);

Console.WriteLine("Исходная плотная матрица (Рисунок 1, 5x6):");
PrintMatrix(matrixCOO);

Console.WriteLine("\n--- Анализ эффективности COO ---");
Console.WriteLine($"  Размерность матрицы      : {rCOO} x {cCOO} = {rCOO * cCOO} ячеек (плотное хранение)");
Console.WriteLine($"  Ненулевых элементов (N)  : {nCOO}");
Console.WriteLine($"  COO хранение (N*3)       : {nCOO}*3 = {nCOO * 3} ячеек");
Console.WriteLine($"  Порог эффективности R*C/3: {rCOO}*{cCOO}/3 = {(double)(rCOO * cCOO) / 3:F2}");
Console.WriteLine($"  Условие (N*3 < R*C)      : {nCOO * 3} < {rCOO * cCOO} => isCOOEffective = {isCOOEffective(matrixCOO)}");

Console.WriteLine("\n--- Преобразование Dense -> COO ---");
DenseToCOO(matrixCOO, out int[] rowCOO, out int[] colCOO, out int[] dataCOO);
PrintArray("Row   ", rowCOO);
PrintArray("Column", colCOO);
PrintArray("Data  ", dataCOO);

Console.WriteLine("\n--- Преобразование COO -> Dense ---");
int[,] restoredCOO = COOToDense(rowCOO, colCOO, dataCOO, rCOO, cCOO);
PrintMatrix(restoredCOO);
Console.WriteLine($"  Матрица восстановлена верно: {MatricesEqual(matrixCOO, restoredCOO)}");

// ============================================================
// ЗАДАНИЕ 2: LIL
// ============================================================

PrintSeparator("ЗАДАНИЕ 2: LIL - ХРАНЕНИЕ В ФОРМЕ СВЯЗНЫХ СПИСКОВ");

int rLIL = matrixLIL.GetLength(0);
int cLIL = matrixLIL.GetLength(1);
int nLIL = CountNonZero(matrixLIL);

Console.WriteLine("Исходная плотная матрица (Рисунок 2, 4x6):");
PrintMatrix(matrixLIL);

Console.WriteLine("\n--- Анализ эффективности LIL ---");
Console.WriteLine($"  Размерность матрицы      : {rLIL} x {cLIL} = {rLIL * cLIL} ячеек (плотное хранение)");
Console.WriteLine($"  Ненулевых элементов (N)  : {nLIL}");
Console.WriteLine($"  LIL хранение (N*2)       : {nLIL}*2 = {nLIL * 2} ячеек");
Console.WriteLine($"  Порог эффективности R*C/2: {rLIL}*{cLIL}/2 = {(double)(rLIL * cLIL) / 2:F2}");
Console.WriteLine($"  Условие (N*2 < R*C)      : {nLIL * 2} < {rLIL * cLIL} => isLILEffective = {isLILEffective(matrixLIL)}");

Console.WriteLine("\n--- Преобразование Dense -> LIL ---");
DenseToLIL(matrixLIL, out int[][] rowsLIL, out int[][] dataLIL);
Console.WriteLine("  Rows[i] — индексы столбцов ненулевых элементов строки i:");
PrintJaggedArray("Rows", rowsLIL);
Console.WriteLine("  Data[i] — значения ненулевых элементов строки i:");
PrintJaggedArray("Data", dataLIL);

Console.WriteLine("\n--- Преобразование LIL -> Dense ---");
int[,] restoredLIL = LILToDense(rowsLIL, dataLIL, rLIL, cLIL);
PrintMatrix(restoredLIL);
Console.WriteLine($"  Матрица восстановлена верно: {MatricesEqual(matrixLIL, restoredLIL)}");

// ============================================================
// ЗАДАНИЕ 3: CSR
// ============================================================

PrintSeparator("ЗАДАНИЕ 3: CSR - КОМПАКТНОЕ ХРАНЕНИЕ РАЗРЕЖЕННЫХ МАТРИЦ");

int rCSR = matrixCSR.GetLength(0);
int cCSR = matrixCSR.GetLength(1);
int nCSR = CountNonZero(matrixCSR);

Console.WriteLine("Исходная плотная матрица (Рисунок 3, 7x5):");
PrintMatrix(matrixCSR);

Console.WriteLine("\n--- Анализ эффективности CSR ---");
Console.WriteLine($"  Размерность матрицы           : {rCSR} x {cCSR} = {rCSR * cCSR} ячеек (плотное хранение)");
Console.WriteLine($"  Ненулевых элементов (N)       : {nCSR}");
Console.WriteLine($"  CSR хранение (2*N + R + 1)  : 2*{nCSR} + {rCSR} + 1 = {2 * nCSR + rCSR + 1} ячеек");
Console.WriteLine($"  Порог: (R*C - R - 1) / 2      : ({rCSR * cCSR} - {rCSR} - 1) / 2 = {(double)(rCSR * cCSR - rCSR - 1) / 2:F2}");
Console.WriteLine($"  Условие (2N+R+1 < R*C)      : {2 * nCSR + rCSR + 1} < {rCSR * cCSR} => isCSREffective = {isCSREffective(matrixCSR)}");

Console.WriteLine("\n--- Преобразование Dense -> CSR ---");
DenseToCSR(matrixCSR, out int[] dataCSR, out int[] indicesCSR, out int[] ipCSR);
PrintArray("Data          ", dataCSR);
PrintArray("Indices       ", indicesCSR);
PrintArray("IndexPointers ", ipCSR);

Console.WriteLine("\n--- Пошаговое декодирование строк из CSR ---");
for (int i = 0; i < rCSR; i++)
{
    int start = ipCSR[i];
    int end = ipCSR[i + 1];
    Console.Write($"  Row {i}: IP[{i}]={start}, IP[{i + 1}]={end} => ");
    if (start == end)
        Console.WriteLine("(пустая строка)");
    else
    {
        for (int k = start; k < end; k++)
            Console.Write($"matrix[{i},{indicesCSR[k]}]={dataCSR[k]}  ");
        Console.WriteLine();
    }
}

Console.WriteLine("\n--- Преобразование CSR -> Dense ---");
int[,] restoredCSR = CSRToDense(dataCSR, indicesCSR, ipCSR, rCSR, cCSR);
PrintMatrix(restoredCSR);
Console.WriteLine($"  Матрица восстановлена верно: {MatricesEqual(matrixCSR, restoredCSR)}");

Console.WriteLine("\n=== ПРОГРАММА ЗАВЕРШЕНА ===");

// ============================================================
// Вспомогательные функции
// ============================================================

static void PrintSeparator(string title)
{
    string line = new string('=', 62);
    Console.WriteLine($"\n{line}");
    Console.WriteLine($"  {title}");
    Console.WriteLine($"{line}\n");
}

static void PrintMatrix(int[,] m)
{
    int rows = m.GetLength(0);
    int cols = m.GetLength(1);
    for (int i = 0; i < rows; i++)
    {
        Console.Write("  |");
        for (int j = 0; j < cols; j++)
            Console.Write($" {m[i, j],3}");
        Console.WriteLine(" |");
    }
}

static void PrintArray(string name, int[] arr)
{
    Console.Write($"  {name}: [ ");
    for (int i = 0; i < arr.Length; i++)
        Console.Write($"{arr[i],3}{(i < arr.Length - 1 ? "," : " ")}");
    Console.WriteLine("]");
}

static void PrintJaggedArray(string name, int[][] arr)
{
    Console.WriteLine($"  {name}:");
    for (int i = 0; i < arr.Length; i++)
    {
        Console.Write($"    [{i}]: [ ");
        if (arr[i].Length == 0)
            Console.WriteLine("(пусто) ]");
        else
        {
            for (int k = 0; k < arr[i].Length; k++)
                Console.Write($"{arr[i][k],3}{(k < arr[i].Length - 1 ? "," : " ")}");
            Console.WriteLine("]");
        }
    }
}

static int CountNonZero(int[,] m)
{
    int count = 0;
    for (int i = 0; i < m.GetLength(0); i++)
        for (int j = 0; j < m.GetLength(1); j++)
            if (m[i, j] != 0)
                count++;
    return count;
}

static bool MatricesEqual(int[,] a, int[,] b)
{
    if (a.GetLength(0) != b.GetLength(0) || a.GetLength(1) != b.GetLength(1))
        return false;
    for (int i = 0; i < a.GetLength(0); i++)
        for (int j = 0; j < a.GetLength(1); j++)
            if (a[i, j] != b[i, j])
                return false;
    return true;
}

// ============================================================
// COO
// ============================================================

static void DenseToCOO(int[,] dense, out int[] row, out int[] col, out int[] data)
{
    int rows = dense.GetLength(0);
    int cols = dense.GetLength(1);
    int count = CountNonZero(dense);
    row = new int[count];
    col = new int[count];
    data = new int[count];
    int idx = 0;
    for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            if (dense[i, j] != 0)
            {
                row[idx] = i;
                col[idx] = j;
                data[idx] = dense[i, j];
                idx++;
            }
}

static int[,] COOToDense(int[] row, int[] col, int[] data, int numRows, int numCols)
{
    int[,] dense = new int[numRows, numCols];
    for (int k = 0; k < data.Length; k++)
        dense[row[k], col[k]] = data[k];
    return dense;
}

static bool isCOOEffective(int[,] dense)
{
    int r = dense.GetLength(0);
    int c = dense.GetLength(1);
    int n = CountNonZero(dense);
    return n * 3 < r * c;
}

// ============================================================
// LIL
// ============================================================

static void DenseToLIL(int[,] dense, out int[][] rows, out int[][] data)
{
    int numRows = dense.GetLength(0);
    int numCols = dense.GetLength(1);
    rows = new int[numRows][];
    data = new int[numRows][];
    for (int i = 0; i < numRows; i++)
    {
        int count = 0;
        for (int j = 0; j < numCols; j++)
            if (dense[i, j] != 0)
                count++;
        rows[i] = new int[count];
        data[i] = new int[count];
        int idx = 0;
        for (int j = 0; j < numCols; j++)
            if (dense[i, j] != 0)
            {
                rows[i][idx] = j;
                data[i][idx] = dense[i, j];
                idx++;
            }
    }
}

static int[,] LILToDense(int[][] rows, int[][] data, int numRows, int numCols)
{
    int[,] dense = new int[numRows, numCols];
    for (int i = 0; i < numRows; i++)
        for (int k = 0; k < rows[i].Length; k++)
            dense[i, rows[i][k]] = data[i][k];
    return dense;
}

static bool isLILEffective(int[,] dense)
{
    int r = dense.GetLength(0);
    int c = dense.GetLength(1);
    int n = CountNonZero(dense);
    return n * 2 < r * c;
}

// ============================================================
// CSR
// ============================================================

static void DenseToCSR(int[,] dense, out int[] data, out int[] indices, out int[] indexPointers)
{
    int numRows = dense.GetLength(0);
    int numCols = dense.GetLength(1);
    int n = CountNonZero(dense);
    data = new int[n];
    indices = new int[n];
    indexPointers = new int[numRows + 1];
    int idx = 0;
    for (int i = 0; i < numRows; i++)
    {
        indexPointers[i] = idx;
        for (int j = 0; j < numCols; j++)
            if (dense[i, j] != 0)
            {
                data[idx] = dense[i, j];
                indices[idx] = j;
                idx++;
            }
    }
    indexPointers[numRows] = n;
}

static int[,] CSRToDense(int[] data, int[] indices, int[] indexPointers, int numRows, int numCols)
{
    int[,] dense = new int[numRows, numCols];
    for (int i = 0; i < numRows; i++)
    {
        int start = indexPointers[i];
        int end = indexPointers[i + 1];
        for (int k = start; k < end; k++)
            dense[i, indices[k]] = data[k];
    }
    return dense;
}

static bool isCSREffective(int[,] dense)
{
    int r = dense.GetLength(0);
    int c = dense.GetLength(1);
    int n = CountNonZero(dense);
    return 2 * n + r + 1 < r * c;
}
