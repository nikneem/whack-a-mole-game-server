using HexMaster.DomainDrivenDesign;
using HexMaster.DomainDrivenDesign.ChangeTracking;
using Wam.Users.Enums;

namespace Wam.Users.DomainModels;

public class User : DomainModel<Guid>
{

    public string DisplayName { get; private set; }
    public string EmailAddress { get; private set; }
    public bool IsExcluded => ExclusionReason.HasValue;
    public byte? ExclusionReason { get; private set; }

    public void SetDisplayName(string value)
    {
        if (!Equals(DisplayName, value))
        {
            DisplayName = value;
            SetState(TrackingState.Modified);
        }
    }

    public void SetEmailAddress(string value)
    {
        if (!Equals(EmailAddress, value))
        {
            EmailAddress = value;
            SetState(TrackingState.Modified);
        }
    }

    public void Exclude(ExclusionReason reason)
    {
        if (!ExclusionReason.HasValue || !Equals(reason.ReasonId, ExclusionReason.Value))
        {
            ExclusionReason = reason.ReasonId;
            SetState(TrackingState.Modified);
        }
    }






    public User(
        Guid id, 
        string displayName, 
        string emailAddress, 
        byte? exclusionReason = null) : base(id)
    {
        DisplayName = displayName;
        EmailAddress = emailAddress;
        ExclusionReason = exclusionReason;
    }



    
    private User() : base(Guid.NewGuid(), TrackingState.New)
    {
    }

    public static User Create(string displayName, string emailAddress)
    {
        var user = new User();
        user.SetDisplayName(displayName);
        user.SetEmailAddress(emailAddress);
        return user;
    }

}