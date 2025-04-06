using GenSpil.Handler;

namespace Test;

[TestClass]
public class AuthenticationTests
{
    private Authentication? _authHandler;

    [TestInitialize]
    public void Setup()
    {
        _authHandler = new Authentication();
    }

    [TestMethod]
    public void TestValidLogin()
    {
        // Arrange
        var username = "user";
        var password = "user";

        // Act
        bool result = _authHandler!.Login(username, password);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void TestInvalidLogin()
    {
        // Arrange
        var username = "invalidUser";
        var password = "invalidPassword";

        // Act
        var result = _authHandler!.Login(username, password);

        // Assert
        Assert.IsFalse(result);
    }
}
