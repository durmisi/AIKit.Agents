using System.Reflection;

namespace AIKit.Agents;

/// <summary>
/// Static entry point for creating AI agents using fluent builders.
/// </summary>
public static class AiKitAgentBuilder
{
    /// <summary>
    /// Creates a new chat agent builder.
    /// </summary>
    /// <returns>A <see cref="ChatAgentBuilder"/> instance.</returns>
    public static ChatAgentBuilder CreateChatAgent() => new();
}

