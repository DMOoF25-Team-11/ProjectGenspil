
using System.Reflection.Metadata;
using GenSpil.Type;
using GenSpil.Model;

namespace GenSpil.Handler;

/// <summary>
/// Represents authentication system
/// </summary>
public class Authentication
{
    private UserList Users { get; set; } ///> List of users
    public string? User { get; private set; } ///> Current user

    /// <summary>
    /// Constructor for Authentication class
    /// Initializes the Users property with the UserList instance 
    /// </summary>
    public Authentication()
    {
        Users = UserList.Instance;
    }

    /// <summary>
    /// Try to login with the given username and password.
    /// </summary>
    /// <param name="password">The password to login with.</param>
    /// <param name="username">The username to login with.</param>
    /// <returns>True if the login was successful, false otherwise.</returns>

    public bool Login(string username, string password)
    {
        foreach (User user in Users.Users)
        {
            if (user.Name == username && user.Password == password)
            {
                User = username;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Logout the current user.
    /// </summary>
    public void Logout()
    {
        User = null;
    }

    /// <summary>
    /// Get the role of the given user.
    /// </summary>
    /// <param name="user">The user to get the role of.</param>
    /// <returns>The role of the user. Default is Guest if user not found.</returns>
    public Role GetRole(string? user)
    {
        if (user == null)
            return Role.Guest;

        foreach (User item in Users.Users)
        {
            if (item.Name == user)
            {
                return (Role)item.Role;
            }
        }
        return Role.Guest;
    }


}
