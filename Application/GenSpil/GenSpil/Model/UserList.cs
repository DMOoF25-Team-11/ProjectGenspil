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

    public UserList()
    {
        Users = new List<User>();
    }
} ///> Singleton instance of the UserList
