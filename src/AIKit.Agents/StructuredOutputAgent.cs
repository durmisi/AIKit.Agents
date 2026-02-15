using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AIKit.Agents;

/// <summary>
/// Represents configuration options for a <see cref="StructuredOutputAgent"/>.
/// </summary>
#pragma warning disable CA1812 // Instantiated via AIAgentBuilderExtensions.UseStructuredOutput optionsFactory parameter
internal sealed class StructuredOutputAgentOptions
#pragma warning restore CA1812
{
    /// <summary>
    /// Gets or sets the system message to use when invoking the chat client for structured output conversion.
    /// </summary>
    public string? ChatClientSystemMessage { get; set; }

    /// <summary>
    /// Gets or sets the chat options to use for the structured output conversion by the chat client
    /// used by the agent.
    /// </summary>
    /// <remarks>
    /// This property is optional. The <see cref="ChatOptions.ResponseFormat"/> should be set to a
    /// <see cref="ChatResponseFormatJson"/> instance to specify the expected JSON schema for the structured output.
    /// Note that if <see cref="AgentRunOptions.ResponseFormat"/> is provided when running the agent,
    /// it will take precedence and override the <see cref="ChatOptions.ResponseFormat"/> specified here.
    /// </remarks>
    public ChatOptions? ChatOptions { get; set; }
}


/// <summary>
/// Represents an agent response that contains structured output and
/// the original agent response from which the structured output was generated.
/// </summary>
internal sealed class StructuredOutputAgentResponse : AgentResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StructuredOutputAgentResponse"/> class.
    /// </summary>
    /// <param name="chatResponse">The <see cref="ChatResponse"/> containing the structured output.</param>
    /// <param name="agentResponse">The original <see cref="AgentResponse"/> from the inner agent.</param>
    public StructuredOutputAgentResponse(ChatResponse chatResponse, AgentResponse agentResponse) : base(chatResponse)
    {
        this.OriginalResponse = agentResponse;
    }

    /// <summary>
    /// Gets the original non-structured response from the inner agent used by chat client to produce the structured output.
    /// </summary>
    public AgentResponse OriginalResponse { get; }
}

/// <summary>
/// A delegating AI agent that converts text responses from an inner AI agent into structured output using a chat client.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="StructuredOutputAgent"/> wraps an inner agent and uses a chat client to transform
/// the inner agent's text response into a structured JSON format based on the specified response format.
/// </para>
/// <para>
/// This agent requires a <see cref="ChatResponseFormatJson"/> to be specified either through the
/// <see cref="AgentRunOptions.ResponseFormat"/> or the <see cref="StructuredOutputAgentOptions.ChatOptions"/>
/// provided during construction.
/// </para>
/// </remarks>
internal sealed class StructuredOutputAgent : DelegatingAIAgent
{
    private readonly IChatClient _chatClient;
    private readonly StructuredOutputAgentOptions? _agentOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructuredOutputAgent"/> class.
    /// </summary>
    /// <param name="innerAgent">The underlying agent that generates text responses to be converted to structured output.</param>
    /// <param name="chatClient">The chat client used to transform text responses into structured JSON format.</param>
    /// <param name="options">Optional configuration options for the structured output agent.</param>
    public StructuredOutputAgent(AIAgent innerAgent, IChatClient chatClient, StructuredOutputAgentOptions? options = null)
        : base(innerAgent)
    {
        this._chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        this._agentOptions = options;
    }

    /// <inheritdoc />
    protected override async Task<AgentResponse> RunCoreAsync(
        IEnumerable<ChatMessage> messages,
        AgentSession? session = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Run the inner agent first, to get back the text response we want to convert.
        var textResponse = await this.InnerAgent.RunAsync(messages, session, options, cancellationToken).ConfigureAwait(false);

        // Invoke the chat client to transform the text output into structured data.
        ChatResponse soResponse = await this._chatClient.GetResponseAsync(
            messages: this.GetChatMessages(textResponse.Text),
            options: this.GetChatOptions(options),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return new StructuredOutputAgentResponse(soResponse, textResponse);
    }

    private List<ChatMessage> GetChatMessages(string? textResponseText)
    {
        List<ChatMessage> chatMessages = [];

        if (this._agentOptions?.ChatClientSystemMessage is not null)
        {
            chatMessages.Add(new ChatMessage(ChatRole.System, this._agentOptions.ChatClientSystemMessage));
        }

        chatMessages.Add(new ChatMessage(ChatRole.User, textResponseText));

        return chatMessages;
    }

    private ChatOptions GetChatOptions(AgentRunOptions? options)
    {
        ChatResponseFormat responseFormat = _agentOptions?.ChatOptions?.ResponseFormat
            ?? this._agentOptions?.ChatOptions?.ResponseFormat
            ?? throw new InvalidOperationException($"A response format of type '{nameof(ChatResponseFormatJson)}' must be specified, but none was specified.");

        if (responseFormat is not ChatResponseFormatJson jsonResponseFormat)
        {
            throw new NotSupportedException($"A response format of type '{nameof(ChatResponseFormatJson)}' must be specified, but was '{responseFormat.GetType().Name}'.");
        }

        var chatOptions = this._agentOptions?.ChatOptions?.Clone() ?? new ChatOptions();
        chatOptions.ResponseFormat = jsonResponseFormat;
        return chatOptions;
    }
}