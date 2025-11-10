using System.Text.RegularExpressions;

namespace MonitoringDashboard.Helpers;

public static partial class EnumExtensions
{

    [GeneratedRegex("(\\B[A-Z])")]
    private static partial Regex PascalCaseRegex();
    
    public static string ToPascalCaseString(this Enum value)
    {
        return PascalCaseRegex().Replace(value.ToString(), " $1");
    }
}