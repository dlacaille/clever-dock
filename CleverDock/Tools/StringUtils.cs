namespace CleverDock.Tools;

public class StringUtils
{
    /// <summary>
    ///     Limits characters according to the lower limit but keeps the string whole if it is shorter than the upper limit.
    ///     Tries to cut at words if spaces are found within the lower and upper limit.
    /// </summary>
    /// <param name="input">Text to limit.</param>
    /// <param name="lowerLimit">
    ///     Lower limit to start cutting words./param>
    ///     <param name="upperLimit">Maximum string size.</param>
    ///     <returns></returns>
    public static string LimitCharacters(string input, int lowerLimit, int upperLimit)
    {
        if (input.Length < upperLimit)
            return input;
        var margin = input.Substring(lowerLimit, upperLimit - lowerLimit);
        if (margin.Contains(' '))
        {
            var lastSpace = margin.LastIndexOf(' ');
            return input.Substring(0, lastSpace + lowerLimit) + "...";
        }

        return input.Substring(0, lowerLimit) + "...";
    }
}