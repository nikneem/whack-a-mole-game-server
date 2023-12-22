using Azure.Data.Tables;
using HexMaster.RedisCache.Abstractions;
using Microsoft.Extensions.Options;
using Wam.Core.Configuration;
using Wam.Core.Identity;
using Wam.Users.DomainModels;
using Wam.Users.Entities;

namespace Wam.Users.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly ICacheClientFactory _cacheClientFactory;

    private const string TableName = "users";
    private const string PartitionKey = "users";
    private TableClient _tableClient;

    public async Task<bool> Save(User user, CancellationToken cancellationToken)
    {
        var entity = new UserEntity
        {
            PartitionKey = PartitionKey,
            RowKey = user.Id.ToString(),
            DisplayName = user.DisplayName,
            EmailAddress = user.EmailAddress,
            ExclusionReason = user.ExclusionReason,
            Timestamp = DateTimeOffset.UtcNow
        };
        var response = await _tableClient.UpsertEntityAsync(
            entity, 
            TableUpdateMode.Replace,
            cancellationToken);
        return response.Status == 204;
    }

    public async Task<User> Get(Guid userId, CancellationToken cancellationToken)
    {
        var cacheClient = _cacheClientFactory.CreateClient();
        var cacheKey = $"user:{userId}";
        var entity =  await cacheClient.GetOrInitializeAsync(() => GetFromTableStorage(userId, cancellationToken), cacheKey);
        return new User(userId,
            entity.DisplayName,
            entity.EmailAddress,
            entity.ExclusionReason);
    }

    private async Task<UserEntity> GetFromTableStorage(Guid userId, CancellationToken cancellationToken)
    {
        var cloudResponse = await _tableClient.GetEntityAsync<UserEntity>(PartitionKey, userId.ToString(),
            cancellationToken: cancellationToken);

        if (cloudResponse.HasValue)
        {
            return cloudResponse.Value;
        }

        throw new Exception("User not found");
    }

    public UsersRepository(
        IOptions<AzureServices> configuration, 
        ICacheClientFactory cacheClientFactory)
    {
        _cacheClientFactory = cacheClientFactory;
        var tableStorageUrl = $"https://{configuration.Value.StorageAccountName}.table.core.windows.net";
        _tableClient = new TableClient(new Uri(tableStorageUrl), TableName,CloudIdentity.GetCloudIdentity());
    }
}