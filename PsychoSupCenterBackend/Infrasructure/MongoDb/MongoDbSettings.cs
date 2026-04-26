namespace Infrasructure.MongoDb;

public sealed class MongoDbSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string RefreshTokensCollection { get; set; } = string.Empty;
}