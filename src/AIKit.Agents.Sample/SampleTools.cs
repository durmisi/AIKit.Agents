using System.ComponentModel;

namespace AIKit.Agents.Sample;

/// <summary>
/// Sample tools for demonstration purposes.
/// </summary>
public static class SampleTools
{
    [Description("Calculates the result of a mathematical expression.")]
    public static string CalculateExpression([Description("The mathematical expression to evaluate (e.g., '2 + 3 * 4').")] string expression)
    {
        try
        {
            // Simple evaluation - in real world, use a proper math parser
            var result = EvaluateSimpleExpression(expression);
            return $"Result: {result}";
        }
        catch (Exception ex)
        {
            return $"Error calculating expression: {ex.Message}";
        }
    }

    [Description("Gets current date and time information.")]
    public static string GetCurrentDateTime([Description("The timezone (optional, defaults to UTC).")] string? timezone = null)
    {
        var now = DateTime.UtcNow;
        if (!string.IsNullOrEmpty(timezone))
        {
            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById(timezone);
                now = TimeZoneInfo.ConvertTimeFromUtc(now, tz);
                return $"Current time in {timezone}: {now:yyyy-MM-dd HH:mm:ss}";
            }
            catch
            {
                return $"Invalid timezone '{timezone}'. Current UTC time: {now:yyyy-MM-dd HH:mm:ss}";
            }
        }
        return $"Current UTC time: {now:yyyy-MM-dd HH:mm:ss}";
    }

    [Description("Analyzes the sentiment of the given text.")]
    public static string AnalyzeSentiment([Description("The text to analyze.")] string text)
    {
        // Simple sentiment analysis - in real world, use ML model
        var positiveWords = new[] { "good", "great", "excellent", "amazing", "wonderful", "fantastic", "love", "like", "happy", "joy" };
        var negativeWords = new[] { "bad", "terrible", "awful", "hate", "dislike", "sad", "angry", "horrible", "worst", "poor" };

        var words = text.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var positiveCount = words.Count(w => positiveWords.Contains(w));
        var negativeCount = words.Count(w => negativeWords.Contains(w));

        string sentiment;
        if (positiveCount > negativeCount)
            sentiment = "Positive";
        else if (negativeCount > positiveCount)
            sentiment = "Negative";
        else
            sentiment = "Neutral";

        return $"Sentiment analysis: {sentiment} (Positive words: {positiveCount}, Negative words: {negativeCount})";
    }

    [Description("Converts text to uppercase or lowercase.")]
    public static string ConvertCase([Description("The text to convert.")] string text,
                                     [Description("The case to convert to ('upper' or 'lower').")] string caseType)
    {
        return caseType.ToLower() switch
        {
            "upper" => text.ToUpper(),
            "lower" => text.ToLower(),
            _ => $"Invalid case type '{caseType}'. Use 'upper' or 'lower'."
        };
    }

    private static double EvaluateSimpleExpression(string expression)
    {
        // Very basic evaluator - replace with proper math library in production
        var dataTable = new System.Data.DataTable();
        return Convert.ToDouble(dataTable.Compute(expression, string.Empty));
    }
}