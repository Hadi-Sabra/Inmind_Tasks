namespace Task1.Services;

public class LanguageService : ILanguageService
{
    public string GetLanguage(HttpContext context)
    {
        return context.Items["PreferredLanguage"] as string ?? "en";
    }
}
