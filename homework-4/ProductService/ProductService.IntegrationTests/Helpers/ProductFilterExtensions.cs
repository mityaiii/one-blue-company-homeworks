using ProductService.Domain.Models;

namespace ProductService.IntegrationTests.Helpers;

public static class ProductFilterExtensions
{
    public static string ToQueryString(this ProductFilter filter)
    {
        var properties = filter.GetType().GetProperties()
            .Where(p => p.GetValue(filter) != null)
            .Select(p => $"{FirstLetterToLower(p.Name)}={Uri.EscapeDataString(p.GetValue(filter).ToString())}");

        var enumerable = properties as string[] ?? properties.ToArray();
        return !enumerable.Any() ? string.Empty : "?" + string.Join("&", enumerable);
    }

    private static string FirstLetterToLower(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToLower(input[0]) + input.Substring(1);
    }
}