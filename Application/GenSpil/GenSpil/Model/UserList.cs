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
    public List<User> Users { get; private set; } ///> List of users 

    [JsonConstructor]
    UserList()
    {
        Users = new List<User>();
#if DEBUG
        Seed();
#endif
    }

    public void Add(User user)
    {
        Users.Add(user);
    }

    public void Remove(User user)
    {
        Users.Remove(user);
    }

# if DEBUG
    public void Seed()
    {
        Users.Add(new User("admin", "admin", Type.Role.Admin));
        Users.Add(new User("user", "user", Type.Role.User));
    }
#endif
} ///> Singleton instance of the UserList
