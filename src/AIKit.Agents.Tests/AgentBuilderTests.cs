using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace AIKit.Agents.Tests;

public class AgentBuilderTests
{
    [Fact]
    public void WithChatClient_SetsChatClient()
    {
        // Arrange
        var chatClient = new MockChatClient();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithSystemMessage_SetsSystemMessage()
    {
        // Arrange
        var chatClient = new MockChatClient();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .WithSystemMessage("Test system message")
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithName_SetsName()
    {
        // Arrange
        var chatClient = new MockChatClient();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .WithName("TestAgent")
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithDescription_SetsDescription()
    {
        // Arrange
        var chatClient = new MockChatClient();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .WithDescription("Test description")
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithToolsFromAssembly_SetsAssemblies()
    {
        // Arrange
        var chatClient = new MockChatClient();
        var assembly = typeof(AgentBuilderTests).Assembly;

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .WithToolsFromAssembly(assembly)
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithToolsFromCurrentAssembly_SetsCurrentAssembly()
    {
        // Arrange
        var chatClient = new MockChatClient();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .WithToolsFromCurrentAssembly()
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithServiceProvider_SetsServiceProvider()
    {
        // Arrange
        var chatClient = new MockChatClient();
        var services = new MockServiceProvider();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .WithServiceProvider(services)
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithLoggerFactory_SetsLoggerFactory()
    {
        // Arrange
        var chatClient = new MockChatClient();
        var loggerFactory = new MockLoggerFactory();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .WithLoggerFactory(loggerFactory)
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithTools_AddsManualTools()
    {
        // Arrange
        var chatClient = new MockChatClient();
        var tool = AIFunctionFactory.Create((Func<string, string>)TestTool, null);

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .WithTools(tool)
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void WithResponseFormat_SetsResponseFormat()
    {
        // Arrange
        var chatClient = new MockChatClient();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .UseStructuredOutput<string>()
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Fact]
    public void UseStructuredOutput_EnablesStructuredOutput()
    {
        // Arrange
        var chatClient = new MockChatClient();

        // Act
        var agent = new AgentBuilder()
            .WithChatClient(chatClient)
            .UseStructuredOutput<string>    ()
            .Build();

        // Assert
        Assert.NotNull(agent);
    }

    [Description("A test tool.")]
    public static string TestTool(string input) => input;

    private class MockChatClient : IChatClient
    {
        public ChatClientMetadata Metadata => new("Mock", null);

        public Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public object? GetService(Type serviceType, object? serviceKey = null) => null;

        public void Dispose()
        { }
    }

    private class MockServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }

    private class MockLoggerFactory : ILoggerFactory
    {
        public void Dispose()
        { }

        public ILogger CreateLogger(string categoryName) => new MockLogger();

        public void AddProvider(ILoggerProvider provider)
        { }
    }

    private class MockLogger : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        { }
    }
}