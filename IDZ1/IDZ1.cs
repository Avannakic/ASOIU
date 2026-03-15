while (true)
{
    Console.Write("Введите первую строку: ");
    string s1 = Console.ReadLine() ?? "";
    if (s1 == "exit") { break; }

    Console.Write("Введите вторую строку: ");
    string s2 = Console.ReadLine() ?? "";

    if (s1.Length < s2.Length)
    {
        string temp = s1;
        s1 = s2; s2 = temp;
    }

    int dist = Damerau_Levenstein(s1.ToLower(), s2.ToLower());
    Console.WriteLine($"Расстояние Дамерау-Левенштейна между {s1} и {s2}: {dist}");
}
static int Damerau_Levenstein(string s1, string s2)
{
    if (s1.Length == 0 && s2.Length == 0) { return 0; }
    else if (s1.Length == 0) { return s2.Length; }
    else if (s2.Length == 0) { return s1.Length; }

    int[,] D = new int[s1.Length + 1, s2.Length + 1];
    for (int i = 1; i <= s1.Length; i++) { D[i, 0] = i; }
    for (int j = 1; j <= s2.Length; j++) { D[0, j] = j; }
    for (int i = 1;  i <= s1.Length; i++)
    {
        for (int j = 1; j <= s2.Length; j++)
        {
            int m;
            if (s1[i - 1] == s2[j - 1]) { m = 0; }
            else { m = 1; }
            
            D[i, j] = Math.Min(Math.Min(D[i - 1, j] + 1, D[i, j - 1] + 1), D[i - 1, j - 1] + m);

            if ( (i > 1 && j > 1) && (s1[i- 1] == s2[j-2]) && (s1[i - 2] == s2[j - 1]) )
            {
                D[i, j] = Math.Min(D[i, j], D[i - 2, j - 2] + m);
            }
        }
    }
    printD(D);
    return D[s1.Length, s2.Length];
}

static void printD(int[,] D)
{
    for (int i = 0; i <= D.GetLength(0); i++)
    {
        for (int j = 0; j <= D.GetLength(1); j++)
        {
            Console.Write($"{D[i, j]} \t");
        }
        Console.WriteLine();
    }
}
