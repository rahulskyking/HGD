using System.Globalization;

namespace HireGameDev.Web.Display;

/// <summary>
/// Salaries are stored in rupees per year and shown in lakhs per annum (LPA), the figure
/// Indian candidates and studios actually talk in. 1 LPA = ₹1,00,000.
/// </summary>
public static class Salary
{
    private const decimal RupeesPerLakh = 100_000m;

    public static decimal ToLpa(decimal rupeesPerYear) =>
        decimal.Round(rupeesPerYear / RupeesPerLakh, 2);

    public static decimal ToRupees(decimal lpa) =>
        decimal.Round(lpa * RupeesPerLakh, 2);

    /// <summary>"₹12 LPA", "₹12.5 LPA".</summary>
    public static string FormatLpa(decimal rupeesPerYear) =>
        $"\u20B9{ToLpa(rupeesPerYear).ToString("0.##", CultureInfo.InvariantCulture)} LPA";
}
