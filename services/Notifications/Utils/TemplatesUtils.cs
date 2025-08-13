namespace Notifications.Utils;

public static class TemplatesUtils
{
    public static readonly string ForgotPasswordTemplate;
    public static readonly string RestaurantInvitationTemplate;
    
    static TemplatesUtils()
    {
        var basePath = AppContext.BaseDirectory;
        ForgotPasswordTemplate = File.ReadAllText(Path.Combine(basePath, "Utils", "Templates", "ForgotPassword.html"));
        RestaurantInvitationTemplate = File.ReadAllText(Path.Combine(basePath, "Utils", "Templates", "RestaurantInvitation.html"));
    }
}