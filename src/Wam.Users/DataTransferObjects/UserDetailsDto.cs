namespace Wam.Users.DataTransferObjects;

public record UserDetailsDto(Guid Id, string DisplayName, string EmailAddress, bool IsExcluded, string? ExclusionReason);