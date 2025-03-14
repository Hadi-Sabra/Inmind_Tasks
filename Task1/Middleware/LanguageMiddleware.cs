using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;

public class LanguageMiddleware
{
    private readonly RequestDelegate _next;

    public LanguageMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        var lang = context.Request.Headers["Accept-Language"].ToString();
        if (string.IsNullOrEmpty(lang))
        {
            lang = "en"; // The default language is english
        }

        context.Items["PreferredLanguage"] = lang;
        await _next(context);
    }
}