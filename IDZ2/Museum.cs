using System;
using System.Collections.Generic;
using System.Text;

namespace IDZ2
{
    /// <summary>
    /// Музей
    /// </summary>
    public class Museum
    {
        public int Id { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">Имя музея</param>
        public Museum(int id, string name)
        {
            Id = id;
            Name = name;
        }
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Museum() : this(0, "") { }

        public override string ToString() => $"{Id} : {Name}";
    }
    /// <summary>
    /// Экспонат
    /// </summary>
    public class Expo
    {
        public int Id { get; set; }
        public int MusId { get; set; }
        public string Name { get; set; }
        public double _value;
        /// <summary>
        /// Ценность экспоната (она не может быть отрицательной)
        /// </summary>
        public double Value
        {
            get => _value;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Ценность объекта не может быть отрицательной");
                _value = value;
            }
        }
        /// <summary>
        /// Конструктор с параметрами
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="MusId">ID музея</param>
        /// <param name="name">Имя экспоната</param>
        /// <param name="value">Ценность экспоната</param>
        public Expo(int id, int musId, string name, double value)
        {
            Id = id;
            MusId = musId;
            Name = name;
            Value = value;
        }
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public Expo() : this(0, 0, "", 0) { }

        public override string ToString() => $"{Id} : Экспонат '{Name}' музея {MusId} ценностью {Value} тыс. руб.";
    }
}
