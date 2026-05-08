using System;
using System.Collections.Generic;
using System.Text;

namespace IDZ2
{
    /// <summary>
    /// Создатель отчетов на основе паттерна Fluent Interface.
    /// </summary>
    public class ReportBuilder
    {
        private DatabaseManager _db;
        private string _sql = "";
        private string _title = "";
        private string[] _headers = Array.Empty<string>();
        private int[] _widths = Array.Empty<int>();
        private bool _numbered = false;
        private string _footer = "";

        public ReportBuilder(DatabaseManager db) { _db = db; }

        /// <summary>
        /// Устанавливает SQL-запрос для получения данных отчёта
        /// </summary>
        /// <param name="sql"></param>
        public ReportBuilder Query(string sql) { _sql = sql; return this; }

        /// <summary>
        /// Заголовок отчёта
        /// </summary>
        /// <param name="title"></param>
        public ReportBuilder Title(string title) { _title = title; return this; }

        /// <summary>
        /// Устанавливает названия колонок таблицы
        /// </summary>
        /// <param name="columns"></param>
        public ReportBuilder Header(params string[] columns) { _headers = columns; return this; }

        /// <summary>
        /// Устанавливает ширину каждой колонки
        /// </summary>
        /// <param name="widths"></param>
        public ReportBuilder ColumnWidths(params int[] widths) { _widths = widths; return this; }

        /// <summary>
        /// Добавление нумерации строк
        /// </summary>
        public ReportBuilder Numbered() { _numbered = true; return this; }

        /// <summary>
        /// Добвление сообщения в конце
        /// </summary>
        /// <param name="footer"></param>
        /// <returns></returns>
        public ReportBuilder Footer(string footer) { _footer = footer; return this; }

        /// <summary>
        /// Формирует итоговую текстовую строку отчета, обращаясь к DatabaseManagerExpo
        /// </summary>
        public string Build()
        {
            var (columns, rows) = _db.ExecuteQuery(_sql);
            var sb = new StringBuilder();

            // Заголовок
            if (!string.IsNullOrEmpty(_title))
            {
                sb.AppendLine();
                sb.AppendLine($"=== {_title} ===");
            }

            string[] displayHeaders = _headers.Length > 0 ? _headers : columns;
            int colCount = displayHeaders.Length;
            int numWidth = _numbered ? 5 : 0;

            // Шапка таблицы
            if (_numbered) sb.Append("№".PadRight(numWidth));
            for (int i = 0; i < colCount; i++)
                sb.Append(displayHeaders[i].PadRight(_widths.Length > i ? _widths[i] : 45));

            sb.AppendLine();

            // Разделительная линия
            int totalWidth = numWidth;
            for (int i = 0; i < colCount; i++)
                totalWidth += (_widths.Length > i ? _widths[i] : 20);
            sb.AppendLine(new string('-', totalWidth));

            // Данные строк
            for (int r = 0; r < rows.Count; r++)
            {
                if (_numbered) sb.Append((r + 1).ToString().PadRight(numWidth));
                for (int c = 0; c < rows[r].Length && c < colCount; c++)
                {
                    sb.Append(rows[r][c].PadRight(_widths.Length > c ? _widths[c] : 45));
                }
                sb.AppendLine();
            }

            // Итоговое сообщение
            if (!string.IsNullOrEmpty(_footer))
            {
                sb.AppendLine(new string('-', totalWidth));
                sb.AppendLine($"{_footer} {rows.Count}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Вывод сформированного отчёта
        /// </summary>
        public void Print() { Console.Write(Build()); }
    }
}
