using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AIKit.Agents.Sample;

/// <summary>
/// Workflow executors for document processing.
/// </summary>
public class DocumentProcessingWorkflow
{
    /// <summary>
    /// Executor that validates and parses input document.
    /// </summary>
    public class DocumentValidator : Executor
    {
        public DocumentValidator() : base("DocumentValidator", null, false)
        {
        }

        protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
        {
            return protocolBuilder.ConfigureRoutes(routeBuilder =>
            {
                routeBuilder.AddHandler<List<ChatMessage>>(async (messages, context) =>
                {
                    var input = messages.Last().Text;
                    Console.WriteLine($"[DocumentValidator] Validating input: {input}");

                    // Parse the document from the message
                    var doc = System.Text.Json.JsonSerializer.Deserialize<DocumentInput>(input.Replace("Process document: ", ""));

                    if (string.IsNullOrWhiteSpace(doc?.Content))
                    {
                        await context.SendMessageAsync("Error: Document content cannot be empty");
                        return;
                    }

                    if (doc.Content.Length > 10000)
                    {
                        await context.SendMessageAsync("Error: Document content too large (max 10000 characters)");
                        return;
                    }

                    Console.WriteLine($"[DocumentValidator] Document '{doc.Title}' validated successfully");
                    await context.YieldOutputAsync(doc);
                });
            });
        }
    }

    /// <summary>
    /// Executor that analyzes document content.
    /// </summary>
    public class ContentAnalyzer : Executor
    {
        public ContentAnalyzer() : base("ContentAnalyzer", null, false)
        {
        }

        protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
        {
            return protocolBuilder.ConfigureRoutes(routeBuilder =>
            {
                routeBuilder.AddHandler<DocumentInput>(async (input, context) =>
                {
                    Console.WriteLine($"[ContentAnalyzer] Analyzing content for: {input.Title}");

                    var analysis = new DocumentAnalysis(
                        WordCount: input.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
                        CharacterCount: input.Content.Length,
                        SentenceCount: input.Content.Split('.', StringSplitOptions.RemoveEmptyEntries).Length,
                        HasCode: input.Content.Contains("```") || input.Content.Contains("function") || input.Content.Contains("class"),
                        Sentiment: AnalyzeSentiment(input.Content)
                    );

                    await context.YieldOutputAsync(analysis);
                    Console.WriteLine($"[ContentAnalyzer] Analysis complete: {analysis.WordCount} words, {analysis.SentenceCount} sentences");
                });
            });
        }

        private string AnalyzeSentiment(string text)
        {
            var positiveWords = new[] { "good", "great", "excellent", "amazing", "wonderful", "fantastic", "love", "like", "happy", "joy" };
            var negativeWords = new[] { "bad", "terrible", "awful", "hate", "dislike", "sad", "angry", "horrible", "worst", "poor" };

            var words = text.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var positiveCount = words.Count(w => positiveWords.Contains(w));
            var negativeCount = words.Count(w => negativeWords.Contains(w));

            if (positiveCount > negativeCount) return "Positive";
            if (negativeCount > positiveCount) return "Negative";
            return "Neutral";
        }
    }

    /// <summary>
    /// Executor that generates a summary and stores results.
    /// </summary>
    public class SummaryGenerator : Executor
    {
        public SummaryGenerator() : base("SummaryGenerator", null, false)
        {
        }

        protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
        {
            return protocolBuilder.ConfigureRoutes(routeBuilder =>
            {
                routeBuilder.AddHandler<DocumentAnalysis>(async (analysis, context) =>
                {
                    Console.WriteLine($"[SummaryGenerator] Generating summary for analyzed document");

                    var summary = new ProcessingResult(
                        Analysis: analysis,
                        Summary: $"Document contains {analysis.WordCount} words and {analysis.SentenceCount} sentences. " +
                                 $"Overall sentiment: {analysis.Sentiment}. " +
                                 $"Contains code: {analysis.HasCode}.",
                        ProcessedAt: DateTime.UtcNow,
                        Status: "Completed"
                    );

                    // In a real application, this would save to database or file
                    Console.WriteLine($"[SummaryGenerator] Processing result: {summary.Summary}");
                    await context.SendMessageAsync(summary.Summary);
                });
            });
        }
    }

    /// <summary>
    /// Input data for document processing.
    /// </summary>
    public record DocumentInput(string Title, string Content);

    /// <summary>
    /// Analysis results.
    /// </summary>
    public record DocumentAnalysis(int WordCount, int CharacterCount, int SentenceCount, bool HasCode, string Sentiment);

    /// <summary>
    /// Final processing result.
    /// </summary>
    public record ProcessingResult(DocumentAnalysis Analysis, string Summary, DateTime ProcessedAt, string Status);
}