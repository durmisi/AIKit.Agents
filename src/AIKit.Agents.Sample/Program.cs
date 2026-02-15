using System.ComponentModel;
using AIKit.Agents;
using AIKit.Clients.AzureOpenAI;

// Sample tool class
public class WeatherTools
{
    [Description("Get the weather for a given location.")]
    public static string GetWeather([Description("The location to get the weather for.")] string location)
    {
        return $"The weather in {location} is sunny.";
    }
}

public class Program
{
    public static async Task Main()
    {
        // Example of creating a chat agent with tools using AIKit
        var chatClient = new ChatClientBuilder()
            .WithEndpoint("https://your-endpoint.openai.azure.com/")
            .WithModel("gpt-4o-mini")
            .WithDefaultAzureCredential()
            .Build();

        var builder = AiKitAgentBuilder.CreateChatAgent()
            .WithChatClient(chatClient)
            .WithSystemMessage("You are a helpful assistant.")
            .WithToolsFromAssembly(typeof(WeatherTools).Assembly);

        var agent = builder.Build();
        var response = await agent.RunAsync("What's the weather in Paris?");

        Console.WriteLine($"Agent response: {response}");

        Console.WriteLine("AIKit.Agents sample: Agent builder created successfully using AIKit.");

        // Example of flexible workflow agent with custom executors and edges
        var customExecutor1 = new CustomExecutor("Executor1");
        var customExecutor2 = new CustomExecutor("Executor2");

        var workflowBuilder = AiKitAgentBuilder.CreateWorkflowAgent()
            .WithName("SampleWorkflow")
            .WithDescription("A sample workflow with multiple executors")
            .WithExecutor(customExecutor1)
            .WithExecutor(customExecutor2)
            .AddEdge(customExecutor1, customExecutor2);

        var workflow = workflowBuilder.Build();

        Console.WriteLine("Flexible workflow created successfully with custom executors and edges.");

        Console.WriteLine("Workflow builder created.");
    }

    // Custom executor for demonstration
    public class CustomExecutor : Microsoft.Agents.AI.Workflows.Executor
    {
        public CustomExecutor(string id) : base(id, null, false) { }

        protected override Microsoft.Agents.AI.Workflows.RouteBuilder ConfigureRoutes(Microsoft.Agents.AI.Workflows.RouteBuilder routeBuilder)
        {
            routeBuilder.AddHandler<object>((input, context) => ValueTask.CompletedTask);
            return routeBuilder;
        }
    }
}


