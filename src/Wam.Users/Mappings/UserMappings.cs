using Wam.Users.DataTransferObjects;
using Wam.Users.DomainModels;
using Wam.Users.Enums;

namespace Wam.Users.Mappings;

public static class UserMappings
{

    public static UserDetailsDto ToDto(this User user)
    {
        var exclusionReason = user.ExclusionReason.HasValue
            ? ExclusionReason.All.First(r => r.ReasonId == user.ExclusionReason.Value)
            : null;

        return new UserDetailsDto(
            user.Id,
            user.DisplayName,
            user.EmailAddress,
            user.IsExcluded,
            exclusionReason?.TranslationKey);
       
    }

}