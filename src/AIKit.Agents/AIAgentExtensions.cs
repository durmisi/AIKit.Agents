using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace AIKit.Agents;

/// <summary>
/// Extension methods for AIAgent to support running with options.
/// </summary>
public static class AIAgentExtensions
{
    /// <summary>
    /// Runs the agent with the specified input and options.
    /// </summary>
    /// <param name="agent">The agent to run.</param>
    /// <param name="input">The input string.</param>
    /// <param name="session">The agent session for conversation continuity.</param>
    /// <param name="options">The run options.</param>
    /// <returns>The agent response.</returns>
    public static Task<AgentResponse> RunWithOptionsAsync(this AIAgent agent, string input, AgentSession? session = null, AgentRunOptions? options = null)
    {
        return agent.RunAsync(input, session, options);
    }

    /// <summary>
    /// Runs the agent with the specified messages and options.
    /// </summary>
    /// <param name="agent">The agent to run.</param>
    /// <param name="messages">The input messages.</param>
    /// <param name="session">The agent session for conversation continuity.</param>
    /// <param name="options">The run options.</param>
    /// <returns>The agent response.</returns>
    public static Task<AgentResponse> RunWithOptionsAsync(this AIAgent agent, IEnumerable<ChatMessage> messages, AgentSession? session = null, AgentRunOptions? options = null)
    {
        return agent.RunAsync(messages, session, options);
    }

    /// <summary>
    /// Runs the agent in streaming mode with the specified input and options.
    /// </summary>
    /// <param name="agent">The agent to run.</param>
    /// <param name="input">The input string.</param>
    /// <param name="session">The agent session for conversation continuity.</param>
    /// <param name="options">The run options.</param>
    /// <returns>An enumerable of agent response updates.</returns>
    public static IAsyncEnumerable<AgentResponseUpdate> RunStreamingWithOptionsAsync(this AIAgent agent, string input, AgentSession? session = null, AgentRunOptions? options = null)
    {
        return agent.RunStreamingAsync(input, session, options);
    }

    /// <summary>
    /// Runs the agent in streaming mode with the specified messages and options.
    /// </summary>
    /// <param name="agent">The agent to run.</param>
    /// <param name="messages">The input messages.</param>
    /// <param name="session">The agent session for conversation continuity.</param>
    /// <param name="options">The run options.</param>
    /// <returns>An enumerable of agent response updates.</returns>
    public static IAsyncEnumerable<AgentResponseUpdate> RunStreamingWithOptionsAsync(this AIAgent agent, IEnumerable<ChatMessage> messages, AgentSession? session = null, AgentRunOptions? options = null)
    {
        return agent.RunStreamingAsync(messages, session, options);
    }
}