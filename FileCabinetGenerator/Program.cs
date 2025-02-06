using System.Globalization;
using System.Xml;
using FileCabinetApp;
using FileCabinetApp.ValidationSettings;
using Newtonsoft.Json;

namespace FileCabinetGenerator;

public static class Program
{
    private const string ValidationRuleFilePath = "validation-rules.json";
    private static readonly Tuple<string, Action<List<FileCabinetRecord>, StreamWriter>>[] exportParams =
    [
        new Tuple<string, Action<List<FileCabinetRecord>, StreamWriter>>("csv", SaveToCsv),
        new Tuple<string, Action<List<FileCabinetRecord>, StreamWriter>>("xml", SaveToXml)
    ];
    
    public static void Main(string[] args)
    {
        if (args?.Length <= 0)
        {
            Console.WriteLine("Program must take arguments: output-type, output, records-amount, start-id");
            Console.WriteLine($"Examples of command's arguments: FileCabinetGenerator.exe --output-type=csv --output=d:records.csv --records-amount=10000 --start-id=30. {Environment.NewLine}FileCabinetGenerator.exe -t xml -o records.xml -a 5000 -i 45");
            return;
        }

        if (!ParseArgs(args, out var outputType, out var outputFilename, out var recordsAmount, out var startId, out var validationRule)) 
            return;
        
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

        if (!File.Exists(ValidationRuleFilePath))
        {
            Console.WriteLine($"Configuration file {ValidationRuleFilePath} not found.");
            return;
        }

        var json = File.ReadAllText(ValidationRuleFilePath);
        var configuration = JsonConvert.DeserializeObject<ValidationSettings>(json);
        if (configuration is null)
        {
            Console.WriteLine("Configuration file must have correct parameters");
            return;
        }
        FileCabinetRecordGenerator generator;

        try
        {
            if (string.Equals(validationRule, "default", StringComparison.OrdinalIgnoreCase))
            {
                if (configuration.Default is null)
                {
                    Console.WriteLine("Check validation file. It must have default parameters.");
                    return;
                }
                generator = new FileCabinetRecordGenerator(configuration.Default);
                Console.WriteLine("Generating with default validation setting.");
            }
            else if (string.Equals(validationRule, "custom", StringComparison.OrdinalIgnoreCase))
            {
                if (configuration.Custom is null)
                {
                    Console.WriteLine("Check validation file. It must have custom parameters.");
                    return;
                }
                generator = new FileCabinetRecordGenerator(configuration.Custom);
                Console.WriteLine("Generating with custom validation setting.");
            }
            else
            {
                Console.WriteLine("Unknown validation rule");
                return;
            }
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine($"Couldn't get validation settings: {e.Message}");
            return;
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine($"Couldn't get validation settings: {e.Message}");
            return;
        }

        if (File.Exists(outputFilename))
        {
            Console.Write($"File '{outputFilename}' already exists. Overwrite? [Y/n] ");
            var input = Console.ReadLine()?.Trim();
            if (!string.Equals(input, "Y", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Generation canceled.");
                return;
            }
        }
        
        var records = generator.GenerateRecords(recordsAmount, startId);
        
        using var streamWriter = new StreamWriter(outputFilename);
        saveCommand(records, streamWriter);
        
        Console.WriteLine($"{recordsAmount} records were written to {outputFilename}");
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

    private static bool ParseArgs(string[]? args, out string outputType, out string outputFilename, out int recordsAmount, out int startId, out string validationRule)
    {
        outputType = string.Empty;
        outputFilename = string.Empty;
        recordsAmount = -1;
        startId = -1;
        validationRule = "default";
        
        for (var i = 0; i < args?.Length; i++)
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
            else if (args[i].StartsWith("--validation-rules=", StringComparison.OrdinalIgnoreCase))
            {
                validationRule = args[i]["--validation-rules=".Length..];
            }
            else if (string.Equals(args[i], "-v", StringComparison.OrdinalIgnoreCase))
            {
                validationRule = args[i + 1];
                i++;
            }
        }

        return true;
    }

}