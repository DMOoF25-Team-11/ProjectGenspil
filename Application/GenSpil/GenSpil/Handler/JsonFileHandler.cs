using System.Text.Json;
using GenSpil.Model;

namespace GenSpil.Handler;

/// <summary>
/// JsonFileHandler class for handling JSON file operations.
/// </summary>
public class JsonFileHandler
{
    private static JsonFileHandler? _instance;
    private static readonly object _lock = new object();
    public static JsonFileHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new JsonFileHandler();
                    }
                }
            }
            return _instance;
        }
    }

    private readonly Version _version = new Version(1, 0); ///> The _version of the JSON file.
    private readonly DataContainer _dataContainer = new DataContainer(); ///> The data container for the JSON file.

    /// <summary>
    /// Represents the data container for the JSON file.
    /// </summary>
    public class DataContainer
    {
        private Version _version;

        public Version Version { get; set; }
        public BoardGameList? BoardGames { get; set; }
        public CustomerList? Customers { get; set; }

        public DataContainer()
        {
            Version = new Version(1, 0);
            BoardGameList BoardGames = BoardGameList.Instance; // Instance of BoardGameList when the class is created.
            //Customers = null;  // Instance of CustomerList when the class is created.
        }
    }

    private JsonFileHandler()
    {
        _dataContainer = new DataContainer()
        {
            Version = _version,
        };
    } ///> Private constructor for singleton pattern


    /// <summary>
    /// Exports data to a JSON file.
    /// </summary>
    /// <param name="filePath">Optional. Default value from Constants.jsonFilePath</param>
    public void ExportData(string filePath)
    {
        lock (_lock)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(_dataContainer, options);
            File.WriteAllText(filePath, jsonString);
        }
    }

    /// <summary>
    /// Imports data from a JSON file.
    /// Reassign owner object for Car objects. So owner object is the same in OwnerList and CarList.
    /// </summary>
    /// <param name="filename">Optional. Default value from Constants.jsonFilePath</param>
    public void ImportData(string filename)
    {
        lock (_lock)
        {
            try
            {
                if (File.Exists(filename))
                {
                    BoardGameList.Instance.Clear();
                    CustomerList.Instance.Clear();
                    string jsonString = File.ReadAllText(filename);
                    var options = new JsonSerializerOptions { };
                    var data = JsonSerializer.Deserialize<DataContainer>(jsonString, options);

                    if (data == null)
                    {
                        Console.WriteLine("Ingen data fundet.");
                        return;
                    }
                    if (data.Version?.Major == 1)
                    {
                        //for (int i = 0; i < data.BoardGames?.Count; i++)
                        //{
                        //    BoardGame boardGame = data.BoardGames[i];
                        //    BoardGameList.Instance.AddOwner(boardGame);
                        //}
                    }
                    else
                    {
                        Console.WriteLine("File _version er ikke kompatible.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved importere json: {ex.Message}");
                Console.WriteLine("Tast for at forsætte");
                Console.ReadKey();
            }
        }
    }

}
