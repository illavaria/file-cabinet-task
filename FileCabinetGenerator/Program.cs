
using System.Globalization;
using System.Xml;
using FileCabinetApp;

public static class Program
{

    private static Tuple<string, Action<List<FileCabinetRecord>, StreamWriter>>[] exportParams =
    [
        new Tuple<string, Action<List<FileCabinetRecord>, StreamWriter>>("csv", SaveToCsv),
        new Tuple<string, Action<List<FileCabinetRecord>, StreamWriter>>("xml", SaveToXml)
    ];
    
    public static void Main(string[] args)
    {
        if (args?.Length <= 0)
        {
            Console.WriteLine("Program must take arguments: output-type, output, records-amount, start-id");
            return;
        }

        if (!ParseArgs(args, out var outputType, out var outputFilename, out var recordsAmount, out var startId)) 
            return;
        
        var records = GenerateRecords(recordsAmount, startId);
        
        
        var index = Array.FindIndex(exportParams, i => i.Item1.Equals(outputType, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            Console.WriteLine("Wrong output type.");
            return;
        }

        var saveCommand = exportParams[index].Item2;
            
        var fileExtension = Path.GetExtension(outputFilename).Trim('.');
        if (!string.Equals(fileExtension, outputType, StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("File extension must be the same as chosen export type");
            return;
        }

        if (File.Exists(outputFilename))
        {
            Console.Write($"File '{outputFilename}' already exists. Overwrite? [Y/n] ");
            var input = Console.ReadLine()?.Trim();
            if (!string.Equals(input, "Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Export canceled.");
                return;
            }
        }

        using var streamWriter = new StreamWriter(outputFilename);
        saveCommand(records, streamWriter);

    }

    private static void SaveToCsv(List<FileCabinetRecord> records, StreamWriter streamWriter)
    {
        var csvWriter = new FileCabinetRecordCsvWriter(streamWriter);
        foreach (var record in records)
        {
            csvWriter.Write(record);
        }
    }
    
    
    private static void SaveToXml(List<FileCabinetRecord> records, StreamWriter streamWriter)
    {
        using var xmlWriter = new XmlTextWriter(streamWriter);
        var xmlRecordWriter = new FileCabinetRecordXmlWriter(xmlWriter);
        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("records");
        foreach (var record in records)
        {
            xmlRecordWriter.Write(record);
        }
        
        xmlWriter.WriteEndElement();
    }
    
    private static List<FileCabinetRecord> GenerateRecords(int amount, int startId)
    {
        var records = new List<FileCabinetRecord>();
        var random = new Random();

        for (int i = 0; i < amount; i++)
        {
            records.Add(new FileCabinetRecord
            {
                Id = startId + i,
                FirstName = GenerateRandomString(random, 2, 60), 
                LastName = GenerateRandomString(random, 2, 60),
                DateOfBirth = GenerateRandomDate(random, new DateTime(1950, 1, 1), DateTime.Today),
                NumberOfChildren = (short)random.Next(0, 11),
                YearIncome = GenerateRandomDecimal(random, 0, 250_000),
                Gender = GenerateRandomGender(random) 
            });
        }

        return records;
    }
    
    private static string GenerateRandomString(Random random, int minLength, int maxLength)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        int length = random.Next(minLength, maxLength + 1);
        var result = new char[length];

        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }

        return new string(result);
    }

    private static DateTime GenerateRandomDate(Random random, DateTime minDate, DateTime maxDate) =>
        minDate.AddDays(random.Next((maxDate - minDate).Days));

    private static decimal GenerateRandomDecimal(Random random, decimal min, decimal max)
    {
        var range = (double)(max - min);
        return Math.Round((decimal)(random.NextDouble() * range) + min, 3);
    }

    private static char GenerateRandomGender(Random random)
    {
        char[] genders = { 'M', 'F', 'N' };
        return genders[random.Next(genders.Length)];
    }
    private static bool ParseArgs(string[]? args, out string outputType, out string outputFilename, out int recordsAmount, out int startId)
    {
        outputType = string.Empty;
        outputFilename = string.Empty;
        recordsAmount = -1;
        startId = -1;
        
        for (int i = 0; i < args?.Length; i++)
        {
            if (args[i].StartsWith("--output-type=", StringComparison.OrdinalIgnoreCase))
            {
                outputType = args[i]["--output-type=".Length..];
            }
            else if (string.Equals(args[i], "-t", StringComparison.OrdinalIgnoreCase))
            {
                outputType = args[i + 1];
                i++;
            }
            else if (args[i].StartsWith("--output=", StringComparison.OrdinalIgnoreCase))
            {
                outputFilename = args[i]["--output=".Length..];
            }
            else if (string.Equals(args[i], "-o", StringComparison.OrdinalIgnoreCase))
            {
                outputFilename = args[i + 1];
                i++;
            }
            else if (args[i].StartsWith("--records-amount=", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(args[i]["--records-amount=".Length..], CultureInfo.InvariantCulture,
                        out recordsAmount) && recordsAmount >= 0) continue;
                Console.WriteLine("Invalid parameter: records amount");
                return false;
            }
            else if (string.Equals(args[i], "-a", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(args[i + 1], CultureInfo.InvariantCulture, out recordsAmount) ||
                    recordsAmount < 0)
                {
                    Console.WriteLine("Invalid parameter: records amount");
                    return false;
                }

                i++;
            }
            else if (args[i].StartsWith("--start-id=", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(args[i]["--start-id=".Length..], CultureInfo.InvariantCulture, out startId) &&
                    startId >= 0) continue;
                Console.WriteLine("Invalid parameter: start id");
                return false;
            }
            else if (string.Equals(args[i], "-i", StringComparison.OrdinalIgnoreCase))
            {
                if (!int.TryParse(args[i + 1], CultureInfo.InvariantCulture, out startId) || startId < 0)
                {
                    Console.WriteLine("Invalid parameter: records amount");
                    return false;
                }

                i++;
            }
        }

        return true;
    }

}