using System.Text.RegularExpressions;

namespace MyLibrary.Application.Helpers
{
    public static class Transliterator
    {
        
        private static readonly (string Latin, string Cyrillic)[] Map =
        {
            ("shch", "щ"), ("Shch", "Щ"),
            ("zh", "ж"), ("Zh", "Ж"),
            ("kh", "х"), ("Kh", "Х"),
            ("ts", "ц"), ("Ts", "Ц"),
            ("ch", "ч"), ("Ch", "Ч"),
            ("sh", "ш"), ("Sh", "Ш"),
            ("yu", "ю"), ("Yu", "Ю"),
            ("ya", "я"), ("Ya", "Я"),
            ("yo", "ё"), ("Yo", "Ё"),

            ("A", "А"), ("B", "Б"), ("V", "В"), ("G", "Г"),
            ("D", "Д"), ("E", "Е"), ("Z", "З"), ("I", "И"),
            ("Y", "Й"), ("K", "К"), ("L", "Л"), ("M", "М"),
            ("N", "Н"), ("O", "О"), ("P", "П"), ("R", "Р"),
            ("S", "С"), ("T", "Т"), ("U", "У"), ("F", "Ф"),
            ("H", "Х"), ("C", "Ц"), ("", ""), 

            ("a", "а"), ("b", "б"), ("v", "в"), ("g", "г"),
            ("d", "д"), ("e", "е"), ("z", "з"), ("i", "и"),
            ("y", "й"), ("k", "к"), ("l", "л"), ("m", "м"),
            ("n", "н"), ("o", "о"), ("p", "п"), ("r", "р"),
            ("s", "с"), ("t", "т"), ("u", "у"), ("f", "ф"),
            ("h", "х"), ("c", "ц"),
            ("'", ""), ("\"", ""), // часто встречаются апострофы от твёрдого знака/ъ
            ("`", ""),
        };

        /// <summary>
        /// Переводит латинскую транслитерацию в кириллицу.
        /// Если строка уже содержит кириллицу — возвращает её как есть.
        /// </summary>
        public static string? LatinToCyrillic(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            // Если есть кириллица — ничего не делаем
            if (Regex.IsMatch(text, @"\p{IsCyrillic}"))
                return text;

            // Если латиницы нет (только цифры, знаки) — тоже не делаем
            if (!Regex.IsMatch(text, @"[a-zA-Z]"))
                return text;

            var result = text;

            foreach (var (latin, cyrillic) in Map)
            {
                if (string.IsNullOrEmpty(latin)) continue;
                result = result.Replace(latin, cyrillic);
            }

            return result;
        }
    }
}
