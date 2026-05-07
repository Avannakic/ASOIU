using System;
using System.IO;
using IDZ2;

class Program
{
    static string dbPath = "museum_data.db";
    static string museumsCsv = FindFile("museum.csv");
    static string exposCsv = FindFile("expo.csv");

    static void Main(string[] args)
    {

        var db = new DatabaseManager(dbPath);
        db.InitializeDatabase(museumsCsv, exposCsv);

        bool exit = false;
        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("=== УЧЕТ МУЗЕЙНЫХ ЭКСПОНАТОВ ===");
            Console.WriteLine("1. Список всех музеев");
            Console.WriteLine("2. Список всех экспонатов");
            Console.WriteLine("3. Добавить новый экспонат");
            Console.WriteLine("4. Обновить сущ. экспонат");
            Console.WriteLine("5. Удалить сущ. экспонат");
            Console.WriteLine("6. Сформировать отчеты");
            Console.WriteLine("7. Экспорт данных в CSV");
            Console.WriteLine("0. Выход");
            Console.Write("\nВыберите пункт меню: ");

            switch (Console.ReadLine())
            {
                case "1": ListMuseums(db); break;
                case "2": ListExpos(db); break;
                case "3": CreateNewExpo(db); break;
                case "4": UpdateExExpo(db); break;
                case "5": DeleteExExpo(db); break;
                case "6": ShowReportsMenu(db); break;
                case "7": ExportToFiles(db); break;
                case "0": exit = true; break;
                default: Console.WriteLine("Неверный ввод."); break;
            }

            if (!exit)
            {
                Console.WriteLine("\nНажмите любую клавишу...");
                Console.ReadKey();
            }
        }
    }

    static void ListMuseums(DatabaseManager db)
    {
        var list = db.GetAllMuseums();
        Console.WriteLine("\n--- Список музеев ---");
        foreach (var m in list) Console.WriteLine($"ID: {m.Id} | Название: {m.Name}");
    }

    static void ListExpos(DatabaseManager db)
    {
        var list = db.GetAllExpos();
        Console.WriteLine("\n--- Реестр экспонатов ---");
        Console.WriteLine($"{"ID",-5} {"Музей ID",-10} {"Наименование",-25} {"Оценка (т.р.)",-15}");
        foreach (var e in list)
            Console.WriteLine($"{e.Id,-5} {e.MusId,-10} {e.Name,-25} {e.Value,-15}");
    }

    static void CreateNewExpo(DatabaseManager db)
    {
        Console.WriteLine("\n--- Регистрация нового экспоната ---");
        ListMuseums(db); 

        Console.Write("Введите ID музея: ");
        if (!int.TryParse(Console.ReadLine(), out int musId))
        {
            Console.WriteLine("Ошибка: ID музея должен быть целым числом.");
            return;
        }
        Console.Write("Название экспоната: ");
        string name = Console.ReadLine() ?? "Без названия";

        Console.Write("Оценочная стоимость (тыс. руб.): ");
        if (!int.TryParse(Console.ReadLine(), out int val))
        {
            Console.WriteLine("Ошибка: стоимость должна быть целым числом.");
            return;
        }

        db.AddExpo(new Expo(0, musId, name, val));
        Console.WriteLine("Экспонат успешно добавлен.");
    }

    static void UpdateExExpo(DatabaseManager db)
    {
        Console.WriteLine("--- Редактирование экспоната ---");
        Console.Write("Введите ID экспоната: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Ошибка: ID экспоната должен быть целым числом.");
            return;
        }

        var expo = db.GetExpoById(id);
        if (expo == null)
        {
            Console.WriteLine("Экспонат не найден.");
            return;
        }

        Console.WriteLine($"Текущие данные: {expo}");
        Console.WriteLine("(Нажмите Enter, чтобы не менять значение)");

        Console.Write($"Название [{expo.Name}]: ");
        string input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input)) expo.Name = input;

        Console.Write($"ID музея [{expo.MusId}]: ");
        input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input)) expo.MusId = int.Parse(input);

        Console.Write($"Стоимость [{expo.Value}]: ");
        input = Console.ReadLine();
        if (!string.IsNullOrEmpty(input)) expo.Value = double.Parse(input.Replace(',', '.'));

        db.UpdateExpo(expo);
        Console.WriteLine("Готово! Данные обновлены.");
    }

    static void DeleteExExpo(DatabaseManager db)
    {
        Console.WriteLine("--- Удаление экспоната ---");
        Console.Write("Введите ID экспоната: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Ошибка: ID экспоната должен быть целым числом.");
            return;
        }

        var expo = db.GetExpoById(id);
        if (expo == null)
        {
            Console.WriteLine("Экспонат не найден.");
            return;
        }

        Console.Write($"Удалить «{expo.Name}»? (д/н): ");
        if (Console.ReadLine().ToLower() == "д")
        {
            db.DeleteExpo(id);
            Console.WriteLine("Удалено.");
        }
        else
            Console.WriteLine("Отмена.");
    }

    static void ExportToFiles(DatabaseManager db)
    {
        db.ExportToCsv("museums_out.csv", "expos_out.csv");
        Console.WriteLine("[OK] Данные сохранены в файлы *_out.csv");
    }

    static string FindFile(string fileName)
    {
        string dir = AppContext.BaseDirectory;
        while (dir != null)
        {
            string path = Path.Combine(dir, fileName);
            if (File.Exists(path))
                return path;

            dir = Directory.GetParent(dir)?.FullName;
        }
        return null;
    }

    static void ShowReportsMenu(DatabaseManager db)
    {
        Console.Clear();
        Console.WriteLine("=== МЕНЮ ОТЧЕТОВ ===");
        Console.WriteLine("1. Полный список экспонатов с названиями музеев");
        Console.WriteLine("2. Количество экспонатов в каждом музее");
        Console.WriteLine("3. Средняя ценность экспонатов по музеям");
        Console.WriteLine("0. Назад");
        Console.Write("\nВыберите отчет: ");

        switch (Console.ReadLine())
        {
            case "1": ReportAllExposWithMuseums(db); break;
            case "2": ReportExposCountByMuseum(db); break;
            case "3": ReportAverageValueByMuseum(db); break;
            case "0": return;
            default: Console.WriteLine("Неверный выбор."); break;
        }
    }

    static void ReportAllExposWithMuseums(DatabaseManager db)
    {
        string sql = @"
        SELECT e.Expo_name, m.museum_name, e.value_k 
        FROM Expos e 
        JOIN museums m ON e.museum_id = m.museum_id 
        ORDER BY e.Expo_name ASC";

        new ReportBuilder(db)
            .Query(sql)
            .Title("ПОЛНЫЙ СПИСОК ЭКСПОНАТОВ")
            .Header("Экспонат", "Музей", "Стоимость (т.р.)")
            .ColumnWidths(30, 35, 30)
            .Footer("Всего экспонатов:")
            .Numbered()
            .Print();
    }

    static void ReportExposCountByMuseum(DatabaseManager db)
    {
        string sql = @"
        SELECT m.museum_name, COUNT(*) AS cnt 
        FROM Expos e 
        JOIN museums m ON e.museum_id = m.museum_id 
        GROUP BY m.museum_name
        ORDER BY cnt DESC";

        new ReportBuilder(db)
            .Query(sql)
            .Title("КОЛИЧЕСТВО ЭКСПОНАТОВ ПО МУЗЕЯМ")
            .Header("Название музея", "Кол-во объектов")
            .ColumnWidths(35, 35)
            .Print();
    }

    static void ReportAverageValueByMuseum(DatabaseManager db)
    {
        string sql = @"
        SELECT m.museum_name, ROUND(AVG(e.value_k), 2) as avg_val
        FROM Expos e 
        JOIN museums m ON e.museum_id = m.museum_id 
        GROUP BY m.museum_name
        ORDER BY avg_val DESC";

        new ReportBuilder(db)
            .Query(sql)
            .Title("СРЕДНЯЯ ЦЕННОСТЬ ФОНДОВ")
            .Header("Музей", "Средняя цена (т.р.)")
            .ColumnWidths(35, 20)
            .Print();
    }
}
