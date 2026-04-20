using MySqlConnector;

namespace edp_gui_app;

public sealed class SiteOwnerAuthService
{
    private readonly string _connectionString;

    public SiteOwnerAuthService(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<SiteOwner?> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT owner_id, owner_name, owner_email
            FROM site_owner
            WHERE owner_email = @email AND password = @password
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@password", password);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new SiteOwner(
            reader.GetInt32("owner_id"),
            reader.GetString("owner_name"),
            reader.GetString("owner_email"));
    }

    public async Task<CreateSiteOwnerResult> CreateOwnerAsync(
        string ownerName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        if (await OwnerEmailExistsAsync(connection, email, cancellationToken))
        {
            return new CreateSiteOwnerResult(CreateSiteOwnerStatus.EmailAlreadyExists, null);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO site_owner (owner_name, owner_email, password)
            VALUES (@name, @email, @password);
            """;
        command.Parameters.AddWithValue("@name", ownerName);
        command.Parameters.AddWithValue("@email", email);
        command.Parameters.AddWithValue("@password", password);

        await command.ExecuteNonQueryAsync(cancellationToken);

        return new CreateSiteOwnerResult(
            CreateSiteOwnerStatus.Created,
            new SiteOwner(Convert.ToInt32(command.LastInsertedId), ownerName, email));
    }

    public async Task<IReadOnlyList<OwnedSite>> LoadSitesByOwnerAsync(
        int ownerId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT site_id, site_name
            FROM site
            WHERE owner_id = @ownerId
            ORDER BY site_name;
            """;
        command.Parameters.AddWithValue("@ownerId", ownerId);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var sites = new List<OwnedSite>();
        while (await reader.ReadAsync(cancellationToken))
        {
            sites.Add(new OwnedSite(
                reader.GetInt32("site_id"),
                reader.GetString("site_name")));
        }

        return sites;
    }

    public async Task<OwnedSite> CreateSiteAsync(
        int ownerId,
        string siteName,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO site (site_name, owner_id)
            VALUES (@siteName, @ownerId);
            """;
        command.Parameters.AddWithValue("@siteName", siteName);
        command.Parameters.AddWithValue("@ownerId", ownerId);

        await command.ExecuteNonQueryAsync(cancellationToken);

        return new OwnedSite(Convert.ToInt32(command.LastInsertedId), siteName);
    }

    public async Task UpdateSiteAsync(
        int siteId,
        int ownerId,
        string siteName,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE site
            SET site_name = @siteName
            WHERE site_id = @siteId AND owner_id = @ownerId;
            """;
        command.Parameters.AddWithValue("@siteName", siteName);
        command.Parameters.AddWithValue("@siteId", siteId);
        command.Parameters.AddWithValue("@ownerId", ownerId);

        var rowsUpdated = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsUpdated == 0)
        {
            throw new InvalidOperationException("Site could not be updated.");
        }
    }

    public async Task DeleteSiteAsync(
        int siteId,
        int ownerId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            DELETE FROM site
            WHERE site_id = @siteId AND owner_id = @ownerId;
            """;
        command.Parameters.AddWithValue("@siteId", siteId);
        command.Parameters.AddWithValue("@ownerId", ownerId);

        var rowsDeleted = await command.ExecuteNonQueryAsync(cancellationToken);
        if (rowsDeleted == 0)
        {
            throw new InvalidOperationException("Site could not be deleted.");
        }
    }

    private static async Task<bool> OwnerEmailExistsAsync(
        MySqlConnection connection,
        string email,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT 1 FROM site_owner WHERE owner_email = @email LIMIT 1;";
        command.Parameters.AddWithValue("@email", email);

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }
}
