using System.Text.RegularExpressions;

namespace NivoMaxBot.Messaging.Routing;

/// <summary>
/// Представляет шаблон маршрута для callback-данных Telegram.
/// Поддерживает параметры в фигурных скобках, например: "category:view:{id:int}", "category:add:{parentId:int?}".
/// </summary>
public class RouteTemplate
{
    private readonly string _template;
    private readonly List<Segment> _segments;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="RouteTemplate"/>.
    /// </summary>
    /// <param name="template">Шаблон маршрута, например "category:view:{id:int}".</param>
    public RouteTemplate(string template)
    {
        _template = template;
        _segments = ParseTemplate(template);
    }

    /// <summary>
    /// Пытается сопоставить входящую строку с шаблоном и извлечь значения параметров.
    /// </summary>
    /// <param name="data">Входящая строка callback-данных.</param>
    /// <param name="values">Словарь извлечённых значений параметров (имя параметра -> значение).</param>
    /// <returns>true, если сопоставление успешно; иначе false.</returns>
    public bool Match(string data, out Dictionary<string, object> values)
    {
        values = new Dictionary<string, object>();
        var dataParts = data.Split(':');
        int templateIndex = 0;
        int dataIndex = 0;

        while (templateIndex < _segments.Count)
        {
            var segment = _segments[templateIndex];

            if (segment.IsParameter)
            {
                // Если это параметр и он необязательный, а данных недостаточно
                if (segment.IsOptional && dataIndex >= dataParts.Length)
                {
                    values[segment.Name] = null;
                    templateIndex++;
                    continue;
                }

                if (dataIndex >= dataParts.Length)
                    return false;

                // Если параметр необязательный и текущий сегмент данных пустой
                if (segment.IsOptional && string.IsNullOrEmpty(dataParts[dataIndex]))
                {
                    values[segment.Name] = null;
                    dataIndex++;
                    templateIndex++;
                    continue;
                }

                // Попытка преобразовать значение
                if (!TryConvert(dataParts[dataIndex], segment.Type, out var value))
                    return false;

                values[segment.Name] = value;
                dataIndex++;
                templateIndex++;
            }
            else
            {
                // Литерал
                if (dataIndex >= dataParts.Length || !string.Equals(segment.Text, dataParts[dataIndex], StringComparison.Ordinal))
                    return false;
                dataIndex++;
                templateIndex++;
            }
        }

        if (dataIndex < dataParts.Length)
            return false;

        return true;
    }

    private List<Segment> ParseTemplate(string template)
    {
        var segments = new List<Segment>();
        // Регулярное выражение ищет либо блок в фигурных скобках, либо последовательность символов, не являющихся двоеточием.
        var regex = new Regex(@"(\{[^}]+\})|([^:]+)");
        var matches = regex.Matches(template);

        foreach (Match match in matches)
        {
            if (match.Value.StartsWith("{") && match.Value.EndsWith("}"))
            {
                // Параметр
                var inner = match.Value.Substring(1, match.Value.Length - 2); // удаляем { и }
                var tokens = inner.Split(':', StringSplitOptions.RemoveEmptyEntries);
                var name = tokens[0].Trim();
                var typeSpec = tokens.Length > 1 ? tokens[1].Trim() : "string";
                bool isOptional = typeSpec.EndsWith("?");
                if (isOptional)
                    typeSpec = typeSpec.Substring(0, typeSpec.Length - 1);

                var type = typeSpec.ToLowerInvariant() switch
                {
                    "int" => typeof(int),
                    "long" => typeof(long),
                    "bool" => typeof(bool),
                    _ => typeof(string)
                };

                segments.Add(new Segment
                {
                    IsParameter = true,
                    Name = name,
                    Type = type,
                    IsOptional = isOptional
                });
            }
            else
            {
                // Литерал (может быть пустым, если в шаблоне подряд идут двоеточия)
                if (!string.IsNullOrEmpty(match.Value))
                {
                    segments.Add(new Segment
                    {
                        IsParameter = false,
                        Text = match.Value
                    });
                }
            }
        }
        return segments;
    }

    private bool TryConvert(string value, Type targetType, out object result)
    {
        result = null;
        try
        {
            if (targetType == typeof(int))
            {
                result = int.Parse(value);
                return true;
            }
            if (targetType == typeof(long))
            {
                result = long.Parse(value);
                return true;
            }
            if (targetType == typeof(bool))
            {
                result = bool.Parse(value);
                return true;
            }
            if (targetType == typeof(string))
            {
                result = value;
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    private class Segment
    {
        public bool IsParameter { get; set; }
        public string Text { get; set; } // для литералов
        public string Name { get; set; } // для параметров
        public Type Type { get; set; } // для параметров
        public bool IsOptional { get; set; } // для параметров
    }
}