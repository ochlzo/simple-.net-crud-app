using edp_gui_app;
using MySqlConnector;

[assembly: DoNotParallelize]

namespace edp_gui_app.Tests;

[TestClass]
public sealed class SiteOwnerAuthServiceTests
{
    private const string ConnectionString =
        "Server=127.0.0.1;Port=3306;Database=site_management;User ID=root;Password=NewStrongPassword123!;";

    [TestMethod]
    public async Task AuthenticateAsync_ReturnsOwner_WhenCredentialsMatch()
    {
        var email = BuildTestEmail();
        const string password = "SamplePassword123!";
        await InsertOwnerAsync(email, password);

        try
        {
            var service = new SiteOwnerAuthService(ConnectionString);

            var owner = await service.AuthenticateAsync(email, password);

            Assert.IsNotNull(owner);
            Assert.AreEqual(email, owner.OwnerEmail);
        }
        finally
        {
            await DeleteOwnerAsync(email);
        }
    }

    [TestMethod]
    public async Task AuthenticateAsync_ReturnsNull_WhenPasswordDoesNotMatch()
    {
        var email = BuildTestEmail();
        const string password = "SamplePassword123!";
        await InsertOwnerAsync(email, password);

        try
        {
            var service = new SiteOwnerAuthService(ConnectionString);

            var owner = await service.AuthenticateAsync(email, "WrongPassword123!");

            Assert.IsNull(owner);
        }
        finally
        {
            await DeleteOwnerAsync(email);
        }
    }

    [TestMethod]
    public async Task CreateOwnerAsync_CreatesOwner_WhenEmailIsNew()
    {
        var email = BuildTestEmail();
        const string password = "SamplePassword123!";
        const string ownerName = "Signup Owner";

        try
        {
            var service = new SiteOwnerAuthService(ConnectionString);

            var result = await service.CreateOwnerAsync(ownerName, email, password);

            Assert.AreEqual(CreateSiteOwnerStatus.Created, result.Status);
            Assert.IsNotNull(result.Owner);
            Assert.AreEqual(ownerName, result.Owner.OwnerName);
            Assert.AreEqual(email, result.Owner.OwnerEmail);
        }
        finally
        {
            await DeleteOwnerAsync(email);
        }
    }

    [TestMethod]
    public async Task CreateOwnerAsync_ReturnsDuplicate_WhenEmailAlreadyExists()
    {
        var email = BuildTestEmail();
        const string password = "SamplePassword123!";
        await InsertOwnerAsync(email, password);

        try
        {
            var service = new SiteOwnerAuthService(ConnectionString);

            var result = await service.CreateOwnerAsync("Duplicate Owner", email, "NewPassword123!");

            Assert.AreEqual(CreateSiteOwnerStatus.EmailAlreadyExists, result.Status);
            Assert.IsNull(result.Owner);
            Assert.AreEqual(1, await CountOwnersByEmailAsync(email));
        }
        finally
        {
            await DeleteOwnerAsync(email);
        }
    }

    private static string BuildTestEmail() => $"codex_{Guid.NewGuid():N}@example.com";

    private static async Task InsertOwnerAsync(string email, string password)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO site_owner (owner_name, owner_email, password)
            VALUES (@name, @email, @password);
            """;
        command.Parameters.AddWithValue("@name", "Codex Test Owner");
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@password", password);

        await command.ExecuteNonQueryAsync();
    }

    private static async Task DeleteOwnerAsync(string email)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM site_owner WHERE owner_email = @email;";
        command.Parameters.AddWithValue("@email", email);

        await command.ExecuteNonQueryAsync();
    }

    private static async Task<int> CountOwnersByEmailAsync(string email)
    {
        await using var connection = new MySqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM site_owner WHERE owner_email = @email;";
        command.Parameters.AddWithValue("@email", email);

        return Convert.ToInt32(await command.ExecuteScalarAsync());
    }
}
