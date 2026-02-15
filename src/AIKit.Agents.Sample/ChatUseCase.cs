using AIKit.Clients.AzureOpenAI;
using Microsoft.Agents.AI;

namespace AIKit.Agents.Sample;

/// <summary>
/// Demonstrates a real-world chat agent use case.
/// </summary>
public static class ChatUseCase
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Chat Agent Use Case: AI Assistant ===");
        Console.WriteLine("This agent can help with calculations, time queries, sentiment analysis, and text manipulation.");
        Console.WriteLine();

        // In a real application, load these from configuration or environment variables
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? "https://your-endpoint.openai.azure.com/";
        var model = Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL") ?? "gpt-4o-mini";

        if (endpoint.Contains("your-endpoint"))
        {
            Console.WriteLine("Please set AZURE_OPENAI_ENDPOINT environment variable to your Azure OpenAI endpoint.");
            return;
        }

        try
        {
            var chatClient = new ChatClientBuilder()
                .WithEndpoint(endpoint)
                .WithModel(model)
                .WithDefaultAzureCredential()
                .Build();

            var agent = new AgentBuilder()
                .WithChatClient(chatClient)
                .WithSystemMessage("""
                    You are a helpful AI assistant that can use various tools to help users.
                    You have access to tools for calculations, time queries, sentiment analysis, and text manipulation.
                    Always be polite and provide clear, concise answers.
                    When using tools, explain what you're doing and why.
                    """)
                .WithName("AIAssistant")
                .WithDescription("A versatile AI assistant with multiple tools")
                .WithToolsFromAssembly(typeof(SampleTools).Assembly)
                .Build();

            // Interactive chat loop with session support
            Console.WriteLine("Chat with the AI assistant! Type 'exit' to quit.");
            Console.WriteLine("Try asking things like:");
            Console.WriteLine("- Calculate 15 * 23 + 7");
            Console.WriteLine("- What time is it in Pacific Standard Time?");
            Console.WriteLine("- Analyze the sentiment of: I love this product!");
            Console.WriteLine("- Convert 'Hello World' to uppercase");
            Console.WriteLine();

            var session = await agent.CreateSessionAsync(); // Maintain conversation context
            var options = new AgentRunOptions(); // Default options

            while (true)
            {
                Console.Write("You: ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    break;

                try
                {
                    Console.Write("Assistant: ");
                    var response = await agent.RunWithOptionsAsync(input, session, options);
                    Console.WriteLine(response.Text);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize chat agent: {ex.Message}");
        }
    }
}