using Azure;
using Azure.Data.Tables;

namespace Wam.Users.Entities;

public class UserEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public string DisplayName { get; set; }
    public string EmailAddress { get; set; }
    public byte? ExclusionReason { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}