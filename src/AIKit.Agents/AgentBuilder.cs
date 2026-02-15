using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AIKit.Agents;

/// <summary>
/// Fluent builder for creating chat agents.
/// </summary>
public class AgentBuilder
{
    private IChatClient? _chatClient;
    private string? _systemMessage;
    private string? _name;
    private string? _description;
    private Assembly[] _assemblies = [];
    private IServiceProvider? _serviceProvider;
    private List<AITool> _addedTools = [];
    private ILoggerFactory? _loggerFactory;
    private bool _enableOpenTelemetry = false;
    private string _openTelemetrySourceName = "AIKit.Agents";
    private bool _openTelemetryEnableSensitiveData = false;
    private ChatResponseFormat _responseFormat = ChatResponseFormat.Text;
    private List<Func<AIAgent, IServiceProvider?, AIAgent>> _middlewares = [];

    /// <summary>
    /// Sets the chat client to use for the agent.
    /// </summary>
    /// <param name="chatClient">The chat client instance.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithChatClient(IChatClient chatClient)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        return this;
    }

    /// <summary>
    /// Sets the system message.
    /// </summary>
    /// <param name="systemMessage">The system instructions.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithSystemMessage(string systemMessage)
    {
        _systemMessage = systemMessage ?? throw new ArgumentNullException(nameof(systemMessage));
        return this;
    }

    /// <summary>
    /// Sets the agent name.
    /// </summary>
    /// <param name="name">The agent name.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithName(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    /// <summary>
    /// Sets the agent description.
    /// </summary>
    /// <param name="description">The agent description.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithDescription(string description)
    {
        _description = description ?? throw new ArgumentNullException(nameof(description));
        return this;
    }

    /// <summary>
    /// Sets the assemblies to scan for tools.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithToolsFromAssembly(params Assembly[] assemblies)
    {
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        return this;
    }

    /// <summary>
    /// Includes tools from the current assembly (entry or calling assembly).
    /// </summary>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithToolsFromCurrentAssembly()
    {
        _assemblies = [Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()];
        return this;
    }

    /// <summary>
    /// Adds manual tools to the agent.
    /// </summary>
    /// <param name="tools">The tools to add.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithTools(params AITool[] tools)
    {
        _addedTools.AddRange(tools ?? throw new ArgumentNullException(nameof(tools)));
        return this;
    }

    /// <summary>
    /// Sets the service provider for dependency injection.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithServiceProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        return this;
    }

    /// <summary>
    /// Sets the logger factory for logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithLoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        return this;
    }

    /// <summary>
    /// Enables OpenTelemetry observability for the agent.
    /// </summary>
    /// <param name="sourceName">The source name for telemetry.</param>
    /// <param name="enableSensitiveData">Whether to enable sensitive data in telemetry.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder WithOpenTelemetry(string sourceName = "AIKit.Agents", bool enableSensitiveData = false)
    {
        _enableOpenTelemetry = true;
        _openTelemetrySourceName = sourceName;
        _openTelemetryEnableSensitiveData = enableSensitiveData;
        return this;
    }

    public AgentBuilder UseStructuredOutput<T>() where T : class
    {
        _responseFormat = ChatResponseFormat.ForJsonSchema<T>();
        return this;
    }

    /// <summary>
    /// Adds a middleware to the agent pipeline.
    /// </summary>
    /// <param name="middleware">The middleware function.</param>
    /// <returns>The builder instance.</returns>
    public AgentBuilder Use(Func<AIAgent, IServiceProvider?, AIAgent> middleware)
    {
        _middlewares.Add(middleware ?? throw new ArgumentNullException(nameof(middleware)));
        return this;
    }

    /// <summary>
    /// Builds the chat agent.
    /// </summary>
    /// <returns>The MAF AIAgent instance.</returns>
    public AIAgent Build()
    {
        if (_chatClient == null) throw new MissingModelException("ChatClient must be set.");

        var discoveredTools = ToolDiscovery.DiscoverTools(_assemblies, _serviceProvider);
        var allTools = discoveredTools.Concat(_addedTools).ToList();

        // Apply chat middleware if set
        var chatClient = _chatClient;

        // Apply OpenTelemetry to chat client
        if (_enableOpenTelemetry)
        {
            chatClient = chatClient.AsBuilder()
            .UseFunctionInvocation()
            .UseOpenTelemetry(
                sourceName: _openTelemetrySourceName,
                configure: cfg =>
                {
                    cfg.EnableSensitiveData = _openTelemetryEnableSensitiveData;
                }).Build();
        }

        var chatOptions = new ChatOptions
        {
            Instructions = _systemMessage,
            ResponseFormat = _responseFormat,
            Tools = allTools
        };

        AIAgent agent = chatClient.AsAIAgent(
            new ChatClientAgentOptions
            {
                Name = _name ?? "ChatAgent",
                Description = _description ?? "Chat agent created with AIKit.Agents.AgentBuilder",
                ChatOptions = chatOptions
            },
            loggerFactory: _loggerFactory,
            services: _serviceProvider);

        // Apply OpenTelemetry
        if (_enableOpenTelemetry)
        {
            agent = agent.AsBuilder()
            .UseOpenTelemetry(
                sourceName: _openTelemetrySourceName,
                configure: cfg =>
                {
                    cfg.EnableSensitiveData = _openTelemetryEnableSensitiveData;
                }).Build();
        }

        // Apply middlewares
        foreach (var middleware in _middlewares)
        {
            agent = middleware(agent, _serviceProvider);
        }

        if(_responseFormat is ChatResponseFormatJson)
        {
            agent = new StructuredOutputAgent(agent, chatClient, new StructuredOutputAgentOptions
            {
                ChatClientSystemMessage = _systemMessage,
                ChatOptions =chatOptions
            });
        }

        return agent;
    }
}