using System.Text.Json;
using System.Text.Json.Serialization;
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
    private JsonSerializerOptions _jsonSerializerOptions;

    ///> The options for the JSON serializer.
    /// <summary>
    /// Represents the data container for the JSON file.
    /// </summary>
    public class DataContainer
    {
        public Version Version { get; set; }
        public BoardGameList? BoardGames { get; set; }
        public UserList? Users { get; set; }
        public CustomerList? Customers { get; set; }

        public DataContainer()
        {
            Version = new Version(1, 0);
            BoardGames = BoardGameList.Instance; // Instance of BoardGameList when the class is created.
            Users = UserList.Instance; // Instance of UserList when the class is created.
            Customers = CustomerList.Instance;  // Instance of CustomerList when the class is created.
        }
    }

    private JsonFileHandler()
    {
        _dataContainer = new DataContainer()
        {
            Version = _version,
        };

        _jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, ///> Use camel case for property names
            Converters = { new JsonStringEnumConverter() }
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
            if (!File.Exists(filePath))
                File.Create(filePath);
            string jsonString = JsonSerializer.Serialize(_dataContainer, _jsonSerializerOptions);
            File.WriteAllText(filePath, jsonString);
        }
    }

    /// <summary>
    /// Imports data from a JSON file.
    /// Reassign owner object for Car objects. So owner object is the same in OwnerList and CarList.
    /// </summary>
    /// <param name="filePath">Optional. Default value from Constants.jsonFilePath</param>
    public void ImportData(string filePath)
    {
        lock (_lock)
        {
            try
            {
                CheckAndCreateEmptyJsonFile(filePath);

                if (File.Exists(filePath))
                {
                    BoardGameList.Instance.Clear();
                    UserList.Instance.Users.Clear();
                    CustomerList.Instance.Clear();
                    string jsonString = File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<DataContainer>(jsonString, _jsonSerializerOptions);

                    if (data == null)
                    {
                        Console.WriteLine("Ingen data fundet.");
                        return;
                    }
                    if (data.Version?.Major == 1)
                    {
                        for (int i = 0; i < data.BoardGames?.BoardGames.Count; i++)
                        {
                            BoardGameList.Instance.Add(data.BoardGames.BoardGames[i]);
                        }
                        for (int i = 0; i < data.Users?.Users.Count; i++)
                        {
                            UserList.Instance.Add(data.Users.Users[i]);
                        }
                        for (int i = 0; i < data.Customers?.Customers.Count; i++)
                        {
                            CustomerList.Instance.AddCustomer(data.Customers.Customers[i]);
                        }
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
    public void CheckAndCreateEmptyJsonFile(string filePath)
    {
        lock (_lock)
        {
            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                //var emptyDataContainer = new DataContainer
                //{
                //    Version = _version,
                //    BoardGames = new BoardGameList(),
                //    Users = new UserList(),
                //    Customers = new CustomerList()
                //};

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Converters = { new JsonStringEnumConverter() }
                };

                string jsonString = JsonSerializer.Serialize(_dataContainer, options);
                File.WriteAllText(filePath, jsonString);
            }
        }
    }
}
