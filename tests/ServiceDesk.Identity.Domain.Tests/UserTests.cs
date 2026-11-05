
using ServiceDesk.Identity.Domain.Users;

namespace ServiceDesk.Identity.Domain.Tests;

public sealed class UserTests
{
    [Fact]
    public void NewUser_ShouldBeActive()
    {
        // Arrange
        var email = "ana@empresa.com";
        var name = "Ana García";

        // Act
        var user = new User(email, name);

        // Assert
        Assert.True(user.IsActive);
    }

    [Fact]
    public void NewUser_ShouldNormalizeCorporateEmail()
    {
        // Arrange
        var email = "  ANA@EMPRESA.COM  ";

        // Act
        var user = new User(email, "Ana García");

        // Assert
        Assert.Equal("ana@empresa.com", user.CorporateEmail);
    }

    [Fact]
    public void NewUser_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var email = "ana@empresa.com";
        var name = "";

        // Act
        var exception = Assert.Throws<ArgumentException>(
            () => new User(email, name));

        // Assert
        Assert.NotNull(exception);
    }


    [Fact]
    public void Deactivate_ShouldMakeUserInactive()
    {
        // Arrange
        var user = new User("ana@empresa.com", "Ana García");

        // Act
        user.Deactivate();

        // Assert
        Assert.False(user.IsActive);
    }

    [Fact]
    public void Activate_ShouldReactivateInactiveUser()
    {
        // Arrange
        var user = new User("ana@empresa.com", "Ana García");
        user.Deactivate();

        // Act
        user.Activate();

        // Assert
        Assert.True(user.IsActive);
    }


    [Fact]
    public void NewUser_ShouldHaveValidId()
    {
        // Arrange & Act
        var user = new User("ana@empresa.com", "Ana García");

        // Assert
        Assert.NotEqual(Guid.Empty, user.Id);
    }

    [Fact]
    public void DifferentUsers_ShouldHaveDifferentIds()
    {
        // Arrange & Act
        var user1 = new User("ana@empresa.com", "Ana García");
        var user2 = new User("luis@empresa.com", "Luis Pérez");

        // Assert
        Assert.NotEqual(user1.Id, user2.Id);
    }

}
