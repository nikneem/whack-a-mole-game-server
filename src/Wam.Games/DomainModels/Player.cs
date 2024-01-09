﻿using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;

namespace Wam.Games.DomainModels;

public class Player : DomainModel<Guid>
{

    public string DisplayName { get; private set; }
    public string EmailAddress { get; private set; }
    public bool IsBanned { get; private set; }

    public Player(Guid id, string diplayName, string emailAddress, bool isBanned ) : base(id)
    {
        DisplayName = diplayName;
        EmailAddress = emailAddress;
        IsBanned = isBanned;
    }
}