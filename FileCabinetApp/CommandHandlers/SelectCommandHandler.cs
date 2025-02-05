using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FileCabinetApp;

public class SelectCommandHandler(IFileCabinetService fileCabinetService)
    : ServiceCommandHandleBase(fileCabinetService, "select")
{
    protected override void HandleCore(string parameters)
    {
        if (string.IsNullOrWhiteSpace(parameters))
        {
            Console.WriteLine("This command requires parameters. Check the help command to see the syntax");
            return;
        }

        var parts = parameters.Split(["where"], StringSplitOptions.RemoveEmptyEntries);
        var selectedFields = parts[0].Split(',', StringSplitOptions.TrimEntries);
        var wherePart = parts.Length > 1 ? parts[1].Trim() : null;

        var conditions = ParseKeyValuePairs(wherePart);

        try
        {
            var filteredRecords = this.fileCabinetService.Find(conditions).ToList();
            WriteTable(filteredRecords, selectedFields, Console.Out);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
    }


    /// <summary>
    /// Write in table form to the text stream a set of elements of type T (<see cref="ICollection{T}"/>),
    /// where the state of each object of type T is described by public properties that have only build-in
    /// type (int, char, string etc.)
    /// </summary>
    /// <typeparam name="T">Type selector.</typeparam>
    /// <param name="collection">Collection of elements of type T.</param>
    /// <param name="writer">Text stream.</param>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="collection"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Throw if <paramref name="writer"/> is null.</exception>
    /// <exception cref="ArgumentException">Throw if <paramref name="collection"/> is empty.</exception>
    private static void WriteTable<T>(ICollection<T>? collection, IReadOnlyCollection<string> fields, TextWriter? writer)
    {
        _ = collection ?? throw new ArgumentNullException(nameof(collection));
        _ = collection.Count == 0
            ? throw new ArgumentException("Collection can't be empty", nameof(collection))
            : collection;
        _ = writer ?? throw new ArgumentNullException(nameof(writer));

        var type = typeof(T);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => fields.Contains(property.Name, StringComparer.InvariantCultureIgnoreCase));
        var propertiesDictionary = new Dictionary<PropertyInfo, int>();
        var borderLine = new StringBuilder("+");
        var nameLine = new StringBuilder("|");

        foreach (var property in properties)
        {
            var maxCollectionLength = collection.Max(el => property.GetValue(el)?.ToString()?.Length ?? 0);
            var maxLength = maxCollectionLength > property.Name.Length ? maxCollectionLength : property.Name.Length;
            propertiesDictionary.Add(property, maxLength);
            borderLine.Append('-', maxLength + 2);
            borderLine.Append('+');
            nameLine.Append(CultureInfo.InvariantCulture, $" {property.Name.PadRight(maxLength)} |");
        }

        writer.WriteLine(borderLine);
        writer.WriteLine(nameLine);
        writer.WriteLine(borderLine);

        foreach (var element in collection)
        {
            var elementLine = BuildElementLine(propertiesDictionary, element);
            writer.WriteLine(elementLine);
            writer.WriteLine(borderLine);
        }

        return;

        static StringBuilder BuildElementLine<TElement>(Dictionary<PropertyInfo, int> propertiesDictionary, TElement element)
        {
            var line = new StringBuilder("|");
            foreach (var property in propertiesDictionary.Keys)
            {
                if (property.PropertyType == typeof(string) || property.PropertyType == typeof(char))
                {
                    line.Append(CultureInfo.InvariantCulture, $" {property.GetValue(element)?.ToString()?.PadRight(propertiesDictionary[property])} |");
                }
                else
                {
                    line.Append(CultureInfo.InvariantCulture, $" {property.GetValue(element)?.ToString()?.PadLeft(propertiesDictionary[property])} |");
                }
            }

            return line;
        }
    }

    private static Dictionary<string, string> ParseKeyValuePairs(string? input)
    {
        var result = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(input))
        {
            return result;
        }

        var matches = Regex.Matches(input, @"(\w+)\s*=\s*'([^']*)'", RegexOptions.IgnoreCase);

        foreach (Match match in matches)
        {
            if (match.Success)
            {
                var key = match.Groups[1].Value.ToUpperInvariant();
                var value = match.Groups[2].Value;
                result[key] = value;
            }
        }

        return result;
    }
}