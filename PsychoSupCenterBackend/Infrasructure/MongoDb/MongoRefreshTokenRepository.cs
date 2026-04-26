using Infrasructure.MongoDb;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using PsychoSupCenterBackend.Application.Common.Interfaces;
using PsychoSupCenterBackend.Application.Common.Models;

namespace PsychoSupCenterBackend.Infrasructure.MongoDb;

public sealed class MongoRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IMongoCollection<RefreshToken> _collection;

    static MongoRefreshTokenRepository()
    {
        if (!BsonClassMap.IsClassMapRegistered(typeof(RefreshToken)))
        {
            BsonClassMap.RegisterClassMap<RefreshToken>(cm =>
            {
                cm.AutoMap();
                cm.MapIdMember(c => c.Id)
                    .SetSerializer(new StringSerializer(BsonType.String));

                cm.MapMember(c => c.UserId)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
            });
        }
    }

    public MongoRefreshTokenRepository(IOptions<MongoDbSettings> settings)
    {
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
        _collection = database.GetCollection<RefreshToken>(settings.Value.RefreshTokensCollection);

        EnsureIndexesCreated();
    }

    private void EnsureIndexesCreated()
    {
        var tokenIndex = new CreateIndexModel<RefreshToken>(
            Builders<RefreshToken>.IndexKeys.Ascending(x => x.Token),
            new CreateIndexOptions { Unique = true });

        var ttlIndex = new CreateIndexModel<RefreshToken>(
            Builders<RefreshToken>.IndexKeys.Ascending(x => x.ExpiresAt),
            new CreateIndexOptions { ExpireAfter = TimeSpan.Zero });

        _collection.Indexes.CreateMany(new[] { tokenIndex, ttlIndex });
    }

    public async Task SaveTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        await _collection.InsertOneAsync(token, cancellationToken: cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _collection.Find(x => x.Token == token).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var update = Builders<RefreshToken>.Update.Set(x => x.IsRevoked, true);

        var result = await _collection.UpdateOneAsync(
            x => x.Token == token,
            update,
            cancellationToken: cancellationToken);

        return result.ModifiedCount > 0;
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var update = Builders<RefreshToken>.Update.Set(x => x.IsRevoked, true);

        await _collection.UpdateManyAsync(
            x => x.UserId == userId && !x.IsRevoked,
            update,
            cancellationToken: cancellationToken);
    }

    public async Task DeleteExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        await _collection.DeleteManyAsync(x => x.ExpiresAt <= DateTime.UtcNow, cancellationToken);
    }
}