# AIKit.Agents

A lightweight C# library that provides a fluent builder layer on top of Microsoft Agent Framework (MAF) for building conversational AI agents. It simplifies agent creation by reducing boilerplate, automating tool discovery, and standardizing configuration patterns.

## Key Features

- **Fluent Builders**: Easy-to-use APIs for chat and workflow agents
- **Tool Discovery**: Automatic reflection-based tool registration from assemblies
- **Structured Output**: JSON schema-based response formatting
- **Dependency Injection**: Built-in support for DI containers
- **Observability**: OpenTelemetry integration for monitoring

## Quick Start

Install the package:

```bash
dotnet add package AIKit.Agents
```

Create a simple chat agent:

```csharp
using AIKit.Agents;
using AIKit.Clients.AzureOpenAI;

var chatClient = new ChatClientBuilder()
    .WithEndpoint("https://your-endpoint.openai.azure.com/")
    .WithModel("gpt-4o-mini")
    .WithDefaultAzureCredential()
    .Build();

var agent = new AgentBuilder()
    .WithChatClient(chatClient)
    .WithSystemMessage("You are a helpful assistant.")
    .WithToolsFromAssembly(typeof(YourTools).Assembly)
    .Build();

var response = await agent.RunAsync("Hello!");
Console.WriteLine(response.Text);
```

## Requirements

- .NET 10.0+
- Microsoft Agent Framework packages (included as dependencies)

## Documentation

- [Software Requirements Specification](docs/SRS.md)
- [Development Plan](docs/Plan.md)
- [Developer Guide](docs/DeveloperGuide.md) (planned)

## Samples

See [src/AIKit.Agents.Sample/](src/AIKit.Agents.Sample/) for complete examples including chat agents and workflow pipelines.

## Contributing

Contributions are welcome! Please see the development plan for roadmap items.
