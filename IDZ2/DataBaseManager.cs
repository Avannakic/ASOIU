using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDZ2
{
    /// <summary>
    /// Управление базой данных SQLite для предметной области "Музеи и Экспонаты".
    /// Инкапсулирует создание таблиц, импорт данных и CRUD-операции.
    /// </summary>
    public class DatabaseManager
    {
        private string _connectionString;

        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
        }

        /// <summary>
        /// Создает таблицы и выполняет первичный импорт данных из CSV-файлов
        /// </summary>
        public void InitializeDatabase(string museumsCsvPath, string ExposCsvPath)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS museums (
                museum_id INTEGER PRIMARY KEY AUTOINCREMENT,
                museum_name TEXT NOT NULL
            );
            CREATE TABLE IF NOT EXISTS Expos (
                Expo_id INTEGER PRIMARY KEY AUTOINCREMENT,
                museum_id INTEGER NOT NULL,
                Expo_name TEXT NOT NULL,
                value_k INTEGER NOT NULL,
                FOREIGN KEY (museum_id) REFERENCES museums(museum_id)
            );";
            cmd.ExecuteNonQuery();

            if (GetAllMuseums().Count == 0 && File.Exists(museumsCsvPath))
            {
                ImportMuseumsFromCsv(museumsCsvPath);
                Console.WriteLine($"Загружены музеи из {Path.GetFileName(museumsCsvPath)}");
            }

            if (GetAllExpos().Count == 0 && File.Exists(ExposCsvPath))
            {
                ImportExposFromCsv(ExposCsvPath);
                Console.WriteLine($"ыЗагружены экспонаты из {Path.GetFileName(ExposCsvPath)}");
            }
        }

        /// <summary>
        /// Загружает записи о музеях из CSV-файла
        /// </summary>
        private void ImportMuseumsFromCsv(string path)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');
                if (parts.Length < 2) continue;

                var cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO museums (museum_id, museum_name) VALUES (@id, @name)";
                cmd.Parameters.AddWithValue("@id", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@name", parts[1]);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Загружает записи об экспонатах из CSV-файла
        /// </summary>
        private void ImportExposFromCsv(string path)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            string[] lines = File.ReadAllLines(path);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');
                if (parts.Length < 4) continue;

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"INSERT INTO Expos (Expo_id, museum_id, Expo_name, value_k) 
                                VALUES (@eId, @mId, @name, @val)";

                cmd.Parameters.AddWithValue("@eId", int.Parse(parts[0]));
                cmd.Parameters.AddWithValue("@mId", int.Parse(parts[1]));
                cmd.Parameters.AddWithValue("@name", parts[2]);
                cmd.Parameters.AddWithValue("@val", int.Parse(parts[3]));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Возвращает полный список музеев
        /// </summary>
        public List<Museum> GetAllMuseums()
        {
            var result = new List<Museum>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT museum_id, museum_name FROM museums ORDER BY museum_id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(new Museum(reader.GetInt32(0), reader.GetString(1)));
            return result;
        }

        /// <summary>
        /// Возвращает полный список экспонатов
        /// </summary>
        public List<Expo> GetAllExpos()
        {
            var result = new List<Expo>();
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Expo_id, museum_id, Expo_name, value_k FROM Expos ORDER BY Expo_id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                result.Add(new Expo(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3)));
            return result;
        }

        /// <summary>
        /// Добавляет новую запись об экспонате
        /// </summary>
        public void AddExpo(Expo Expo)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Expos (museum_id, Expo_name, value_k) VALUES (@mId, @name, @val)";
            cmd.Parameters.AddWithValue("@mId", Expo.MusId);
            cmd.Parameters.AddWithValue("@name", Expo.Name);
            cmd.Parameters.AddWithValue("@val", Expo.Value);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Обновляет существующую запись об экспонате
        /// </summary>
        public void UpdateExpo(Expo Expo)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "UPDATE Expos SET museum_id=@mId, Expo_name=@name, value_k=@val WHERE Expo_id=@id";
            cmd.Parameters.AddWithValue("@id", Expo.Id);
            cmd.Parameters.AddWithValue("@mId", Expo.MusId);
            cmd.Parameters.AddWithValue("@name", Expo.Name);
            cmd.Parameters.AddWithValue("@val", Expo.Value);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Удаляет экспонат по идентификатору
        /// </summary>
        public void DeleteExpo(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM Expos WHERE Expo_id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Ищет экспонат по идентификатору
        /// </summary>
        public Expo GetExpoById(int id)
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Expo_id, museum_id, Expo_name, value_k FROM Expos WHERE Expo_id=@id";
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return new Expo(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetInt32(3));
            return null;
        }

        /// <summary>
        /// Выполняет SQL-запрос для отчетов
        /// </summary>
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

        public void ExportToCsv(string museumPath, string expoPath)
        {
            museumPath = Path.Combine(AppContext.BaseDirectory, museumPath);
            expoPath = Path.Combine(AppContext.BaseDirectory , expoPath);
            var mLines = new List<string> { "museum_id; museum_name" };
            foreach (var m in GetAllMuseums())
                mLines.Add($"{m.Id}; {m.Name}");
            File.WriteAllLines(museumPath, mLines.ToArray());

            var eLines = new List<string> { "Expo_id; museum_id; Expo_name; value" };
            foreach (var e in GetAllExpos())
                eLines.Add($"{e.Id}; {e.MusId}; {e.Name}; {e.Value}");
            File.WriteAllLines(expoPath, eLines.ToArray());
        }
    }
}
