using AIKit.Agents.Sample;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("AIKit.Agents Sample Application");
        Console.WriteLine("Choose a use case to run:");
        Console.WriteLine("1. Chat Agent - Interactive AI Assistant");
        Console.WriteLine("2. Workflow Agent - Document Processing Pipeline");
        Console.WriteLine("3. Run Both (Sequential)");
        Console.Write("Enter your choice (1-3): ");

        var choice = Console.ReadLine()?.Trim();

        switch (choice)
        {
            case "1":
                await ChatUseCase.RunAsync();
                break;

            case "2":
                await WorkflowUseCase.RunAsync();
                break;

            case "3":
                await ChatUseCase.RunAsync();
                Console.WriteLine();
                Console.WriteLine("Press Enter to continue to workflow demo...");
                Console.ReadLine();
                await WorkflowUseCase.RunAsync();
                break;

            default:
                Console.WriteLine("Invalid choice. Running both use cases...");
                await ChatUseCase.RunAsync();
                Console.WriteLine();
                Console.WriteLine("Press Enter to continue to workflow demo...");
                Console.ReadLine();
                await WorkflowUseCase.RunAsync();
                break;
        }

        Console.WriteLine();
        Console.WriteLine("Sample application completed. Press any key to exit...");
        Console.ReadKey();
    }
}