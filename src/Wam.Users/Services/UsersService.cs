using Wam.Users.DataTransferObjects;
using Wam.Users.DomainModels;
using Wam.Users.Mappings;
using Wam.Users.Repositories;

namespace Wam.Users.Services;

public class UsersService(IUsersRepository usersRepository) : IUsersService
{
    public async Task<UserDetailsDto> Create(UserCreateDto dto, CancellationToken cancellationToken)
    {
        var user = User.Create(dto.DisplayName, dto.EmailAddress);
        if (await usersRepository.Save(user, cancellationToken) == false)
        {
            throw new Exception("Failed to save user");
        }
        return user.ToDto();
    }

    public async Task<UserDetailsDto> Get(Guid id, CancellationToken cancellationToken)
    {
        var domainModel = await usersRepository.Get(id, cancellationToken);
        return domainModel.ToDto();
    }
}