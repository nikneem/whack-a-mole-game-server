using Azure.Data.Tables;
using HexMaster.RedisCache.Abstractions;
using Microsoft.Extensions.Options;
using System.Threading;
using Wam.Core.Configuration;
using Wam.Core.ExtensionMethods;
using Wam.Core.Identity;
using Wam.Users.DomainModels;
using Wam.Users.Entities;

namespace Wam.Users.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly ICacheClientFactory _cacheClientFactory;
    private readonly ICacheClient _createClient;

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
        if (response.Status.IsHttpSuccessCode())
        {
            await UpdateCache(entity);
            return true;
        }

        return false;
    }

    public async Task<User> Get(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = $"user:{userId}";
        var entity =  await _createClient.GetOrInitializeAsync(() => GetFromTableStorage(userId, cancellationToken), cacheKey);
        return new User(userId,
            entity.DisplayName,
            entity.EmailAddress,
            (byte?)entity.ExclusionReason);
    }

    private  Task UpdateCache(UserEntity entity)
    {
        var cacheKey = $"user:{entity.RowKey}";
        return _createClient.SetAsAsync(cacheKey, entity);
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
        _createClient = _cacheClientFactory.CreateClient();
        var tableStorageUrl = $"https://{configuration.Value.StorageAccountName}.table.core.windows.net";
        _tableClient = new TableClient(new Uri(tableStorageUrl), TableName,CloudIdentity.GetCloudIdentity());
    }
}