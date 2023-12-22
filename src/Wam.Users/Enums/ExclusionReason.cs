using System.Globalization;

namespace Wam.Users.Enums;

public abstract class ExclusionReason
{
    public static ExclusionReason CodeOfConductViolation;
    public static ExclusionReason GeneralSystemMisuse;
    public static ExclusionReason[] All;

    static ExclusionReason()
    {
        All =
        [
            CodeOfConductViolation = new CodeOfConductViolationExclusionReason(),
            GeneralSystemMisuse = new GeneralSystemMisuseExclusionReason()
        ];
    }

    public abstract byte ReasonId { get; }
    public abstract string Reason { get; }
    public abstract string TranslationKey { get; }

    public static ExclusionReason FromId(byte id)
    {
        return All.FirstOrDefault(x => x.ReasonId == id) ?? GeneralSystemMisuse;
    }

}

public sealed class CodeOfConductViolationExclusionReason : ExclusionReason
{
    public override byte ReasonId => 1;
    public override string Reason => "CodeOfConductViolation";
    public override string TranslationKey => $"Users.ExclusionReasons.{Reason}";
}

public sealed class GeneralSystemMisuseExclusionReason : ExclusionReason
{
    public override byte ReasonId => 2;
    public override string Reason => "GeneralSystemMisuse";
    public override string TranslationKey => $"Users.ExclusionReasons.{Reason}".ToLower(CultureInfo.InvariantCulture);
}