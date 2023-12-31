﻿using Wam.Users.DataTransferObjects;

namespace Wam.Users.Services;

public interface IUsersService
{
    Task<UserDetailsDto> Create(UserCreateDto dto, CancellationToken cancellationToken);
    Task<UserDetailsDto> Get(Guid id, CancellationToken cancellationToken);
    Task<UserDetailsDto> Ban(Guid id, byte reason, CancellationToken cancellationToken);
}