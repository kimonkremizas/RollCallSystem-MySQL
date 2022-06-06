namespace RollCallSystem.Logic;

public static class HelperFunctions
{
    public static string GetRoleName(int? roleId)
    {
        switch (roleId)
        {
            default: return "Student";
            case 1: return "Teacher";
            case 2: return "Admin";
        }
    }
}
