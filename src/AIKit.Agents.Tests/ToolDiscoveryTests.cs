using System.ComponentModel;
using AIKit.Agents;

namespace AIKit.Agents.Tests;

public class ToolDiscoveryTests
{
    [Fact]
    public void DiscoverTools_WithDescriptionMethod_ReturnsTool()
    {
        // Arrange
        var assembly = typeof(ToolDiscoveryTests).Assembly;

        // Act
        var tools = ToolDiscovery.DiscoverTools([assembly]);

        // Assert
        Assert.NotEmpty(tools);
        Assert.Contains(tools, t => t.Name.Contains("SampleTool"));
    }

    [Fact]
    public void DiscoverTools_NoDescriptionMethods_ReturnsEmpty()
    {
        // Arrange
        var assembly = typeof(string).Assembly; // System assembly with no tools

        // Act
        var tools = ToolDiscovery.DiscoverTools([assembly]);

        // Assert
        Assert.Empty(tools);
    }

    [Description("A sample tool method.")]
    public static string SampleTool(string input) => $"Processed {input}";
}

public class AiKitAgentBuilderTests
{
    [Fact]
    public void CreateChatAgent_ReturnsChatAgentBuilder()
    {
        // Act
        var builder = AiKitAgentBuilder.CreateChatAgent();

        // Assert
        Assert.NotNull(builder);
        Assert.IsType<ChatAgentBuilder>(builder);
    }
}


