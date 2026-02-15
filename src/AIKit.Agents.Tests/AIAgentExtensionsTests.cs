using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Moq;
using Xunit;

namespace AIKit.Agents.Tests;

public class AIAgentExtensionsTests
{
    [Fact]
    public async Task RunWithOptionsAsync_WithStringInput_CallsRunAsync()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var expectedResponse = new AgentResponse([]);
        mockAgent.Setup(a => a.RunAsync("test input", null, null, default)).ReturnsAsync(expectedResponse);

        // Act
        var result = await mockAgent.Object.RunWithOptionsAsync("test input");

        // Assert
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task RunWithOptionsAsync_WithMessages_CallsRunAsync()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var messages = new[] { new ChatMessage(ChatRole.User, "test") };
        var expectedResponse = new AgentResponse([]);
        mockAgent.Setup(a => a.RunAsync(messages, null, null, default)).ReturnsAsync(expectedResponse);

        // Act
        var result = await mockAgent.Object.RunWithOptionsAsync(messages);

        // Assert
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task RunStreamingWithOptionsAsync_WithStringInput_CallsRunStreamingAsync()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var updates = AsyncEnumerable.Empty<AgentResponseUpdate>();
        mockAgent.Setup(a => a.RunStreamingAsync("test input", null, null, default)).Returns(updates);

        // Act
        var result = mockAgent.Object.RunStreamingWithOptionsAsync("test input");

        // Assert
        Assert.Equal(updates, result);
    }

    [Fact]
    public async Task RunStreamingWithOptionsAsync_WithMessages_CallsRunStreamingAsync()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var messages = new[] { new ChatMessage(ChatRole.User, "test") };
        var updates = AsyncEnumerable.Empty<AgentResponseUpdate>();
        mockAgent.Setup(a => a.RunStreamingAsync(messages, null, null, default)).Returns(updates);

        // Act
        var result = mockAgent.Object.RunStreamingWithOptionsAsync(messages);

        // Assert
        Assert.Equal(updates, result);
    }

    [Fact]
    public async Task RunWithOptionsAsync_WithOptions_PassesOptions()
    {
        // Arrange
        var mockAgent = new Mock<AIAgent>();
        var options = new AgentRunOptions();
        var expectedResponse = new AgentResponse([]);
        mockAgent.Setup(a => a.RunAsync("test input", null, options, default)).ReturnsAsync(expectedResponse);

        // Act
        var result = await mockAgent.Object.RunWithOptionsAsync("test input", options: options);

        // Assert
        Assert.Equal(expectedResponse, result);
    }
}