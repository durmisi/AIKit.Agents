using AIKit.Agents;
using AIKit.Agents.Sample;

namespace AIKit.Agents.Sample;

/// <summary>
/// Demonstrates a real-world workflow agent use case.
/// </summary>
public static class WorkflowUseCase
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Workflow Agent Use Case: Document Processing Pipeline ===");
        Console.WriteLine("This workflow processes documents through validation, analysis, and summary generation.");
        Console.WriteLine();

        try
        {
            // Create workflow executors
            var validator = new DocumentProcessingWorkflow.DocumentValidator();
            var analyzer = new DocumentProcessingWorkflow.ContentAnalyzer();
            var summarizer = new DocumentProcessingWorkflow.SummaryGenerator();

            // Build the workflow
            var workflow = AiKitAgentBuilder.CreateWorkflowAgent()
                .WithName("DocumentProcessingWorkflow")
                .WithDescription("A workflow that validates, analyzes, and summarizes documents")
                .WithStartExecutor(validator)
                .WithExecutor(analyzer)
                .WithExecutor(summarizer)
                .AddEdge(validator, analyzer) // Validator -> Analyzer
                .AddEdge(analyzer, summarizer) // Analyzer -> Summarizer
                .Build();

            Console.WriteLine("Workflow created successfully!");
            Console.WriteLine("Workflow structure:");
            Console.WriteLine("- DocumentValidator -> ContentAnalyzer -> SummaryGenerator");
            Console.WriteLine();

            // Note: In a real application, you would integrate this workflow into an AI agent system
            // For example: var agent = workflow.AsAgent(); var response = await agent.RunAsync(messages);
            Console.WriteLine("This workflow can be used as an AI agent to process documents:");
            Console.WriteLine("- Input: Chat messages containing document data");
            Console.WriteLine("- Processing: Validation -> Analysis -> Summary generation");
            Console.WriteLine("- Output: Processed document summary");
            Console.WriteLine();

            // Demonstrate with sample documents (without execution)
            var sampleDocuments = new[]
            {
                new DocumentProcessingWorkflow.DocumentInput(
                    "Positive Review",
                    "I absolutely love this product! It's amazing and works perfectly. The quality is excellent and I'm very happy with my purchase."
                ),
                new DocumentProcessingWorkflow.DocumentInput(
                    "Technical Documentation",
                    "This class provides methods for data processing. Use the ProcessData() function to handle input validation. The algorithm is efficient and handles edge cases properly."
                ),
                new DocumentProcessingWorkflow.DocumentInput(
                    "Negative Feedback",
                    "This product is terrible. It doesn't work as advertised and I'm very disappointed. The customer service was also poor."
                )
            };

            foreach (var doc in sampleDocuments)
            {
                Console.WriteLine($"Sample document: '{doc.Title}'");
                Console.WriteLine($"Content preview: {doc.Content.Substring(0, Math.Min(50, doc.Content.Length))}...");
                Console.WriteLine("-> Would be validated, analyzed, and summarized by the workflow");
                Console.WriteLine();
            }

            Console.WriteLine("Workflow demonstration completed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize workflow: {ex.Message}");
        }
    }
}