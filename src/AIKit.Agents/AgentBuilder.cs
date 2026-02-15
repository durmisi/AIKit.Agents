using System.Reflection;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

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
    /// Builds the chat agent.
    /// </summary>
    /// <returns>The MAF AIAgent instance.</returns>
    public AIAgent Build()
    {
        if (_chatClient == null) throw new MissingModelException("ChatClient must be set.");

        var discoveredTools = ToolDiscovery.DiscoverTools(_assemblies, _serviceProvider);
        var allTools = discoveredTools.Concat(_addedTools).ToList();

        return _chatClient.AsAIAgent(
            instructions: _systemMessage,
            name: _name ?? "ChatAgent",
            description: _description ?? "Chat agent created with AIKit.Agents.AgentBuilder",
            tools: allTools,
            loggerFactory: _loggerFactory,
            services: _serviceProvider);
    }
}

