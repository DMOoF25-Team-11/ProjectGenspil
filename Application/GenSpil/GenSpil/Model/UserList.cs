using System.Text.Json.Serialization;

namespace GenSpil.Model;

/// <summary>
/// Singleton class for handling a list of users.
/// </summary>
public sealed class UserList
{
    private static UserList? _instance;
    private static readonly object padlock = new object();
    public static UserList Instance
    {
        get
        {
            lock (padlock)
            {
                if (_instance == null)
                {
                    _instance = new UserList();
                }
                return _instance;
            }
        }

    }

    public List<User> Users { get; private set; }

    [JsonConstructor]
    UserList()
    {
        Users = new List<User>();
    }

    public void Add(User user)
    {
        Users.Add(user);
    }

    public void Remove(User user)
    {
        Users.Remove(user);
    }

    public void Clear()
    {
        Users.Clear();
    }
} ///> Singleton instance of the UserList
