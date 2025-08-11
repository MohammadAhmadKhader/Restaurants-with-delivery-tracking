namespace Shared.Utils;

public static class DbContextUtils
{
    public static string GenerateConstraintName(string modelName, string propertyName, string constraintType)
    {
        return $"CK_{modelName}_{propertyName}_{constraintType}";
    }

    // \"{propertyName}\" instead of {propertyName} because if we didnt quote it.
    // postgres will switch the column name to lowercase then an error will occur for not existing column
    public static string GetGuidNotEmptyConstraintPgSql(string propertyName)
    {
        return $"\"{propertyName}\" <> '00000000-0000-0000-0000-000000000000'";
    }
}