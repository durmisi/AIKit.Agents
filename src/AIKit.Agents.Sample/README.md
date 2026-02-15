# AIKit.Agents Sample Application

This sample application demonstrates real-world use cases for the AIKit.Agents library, showcasing both chat agents and workflow agents.

## Features

### Chat Agent Use Case

- Interactive AI assistant with multiple tools
- Tools include: calculator, time queries, sentiment analysis, text case conversion
- Uses Azure OpenAI for natural language processing
- Environment-based configuration

### Workflow Agent Use Case

- Document processing pipeline
- Multi-step workflow: validation → analysis → summary generation
- Demonstrates workflow builder patterns
- Shows how workflows can be integrated with AI agents

## Prerequisites

- .NET 10 SDK
- Azure OpenAI resource (for chat agent)
- Azure CLI installed and authenticated (for Azure OpenAI access)

## Configuration

Set the following environment variables for the chat agent:

```bash
export AZURE_OPENAI_ENDPOINT="https://your-resource.openai.azure.com/"
export AZURE_OPENAI_MODEL="gpt-4o-mini"  # or your preferred model
```

Alternatively, use Azure CLI authentication:

```bash
az login
```

## Running the Sample

1. Navigate to the sample directory:

   ```bash
   cd src/AIKit.Agents.Sample
   ```

2. Run the application:

   ```bash
   dotnet run
   ```

3. Choose a use case:
   - **1**: Chat Agent - Interactive AI assistant
   - **2**: Workflow Agent - Document processing demo
   - **3**: Run both sequentially

## Chat Agent Demo

The chat agent includes tools for:

- **CalculateExpression**: Evaluate mathematical expressions
- **GetCurrentDateTime**: Get current time in specified timezone
- **AnalyzeSentiment**: Analyze text sentiment (positive/negative/neutral)
- **ConvertCase**: Convert text to upper or lowercase

Example interactions:

- "Calculate 15 \* 23 + 7"
- "What time is it in Pacific Standard Time?"
- "Analyze the sentiment of: I love this product!"
- "Convert 'Hello World' to uppercase"

## Workflow Agent Demo

The workflow demonstrates document processing with three executors:

1. **DocumentValidator**: Validates document content and size
2. **ContentAnalyzer**: Analyzes word count, sentiment, and code detection
3. **SummaryGenerator**: Generates processing summary

The workflow shows how to:

- Chain multiple processing steps
- Handle different data types between steps
- Integrate with AI agent systems

## Architecture

The sample uses the AIKit.Agents fluent builders:

```csharp
// Chat Agent
var agent = new AgentBuilder()
    .WithChatClient(chatClient)
    .WithSystemMessage("You are a helpful assistant.")
    .WithToolsFromAssembly(typeof(SampleTools).Assembly)
    .Build();

// Workflow Agent
var workflow = new WorkflowAgentBuilder()
    .WithName("DocumentProcessingWorkflow")
    .WithExecutor(validator)
    .WithExecutor(analyzer)
    .AddEdge(validator, analyzer)
    .Build();
```

## Extending the Sample

To add new tools:

1. Create static methods with `[Description]` attributes
2. Add them to a class in the sample assembly
3. The tool discovery will automatically find them

To create new workflows:

1. Implement custom `Executor` classes
2. Define data flow between executors
3. Use the workflow builder to connect them
