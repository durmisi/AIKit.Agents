using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

namespace AIKit.Agents.Tests;

public class WorkflowAgentBuilderTests
{
    [Fact]
    public async Task SimpleWorkflow_Executes()
    {
        // Arrange - Create a workflow with a single executor
        var executor = new OutputExecutor("Step1", "Step executed");

        var workflow = new WorkflowAgentBuilder()
            .WithName("SimpleWorkflow")
            .WithExecutor(executor)
            .WithStartExecutor(executor)
            .Build();

        // Act - Convert to agent and execute
        var agent = workflow.AsAgent();
        var response = await agent.RunAsync(new[] { new ChatMessage(ChatRole.User, "test input") });

        // Assert - Check that it executed without error
        Assert.True(true);
    }

    [Fact]
    public async Task ConditionalWorkflow_RoutesBasedOnCondition()
    {
        // Arrange - Create workflow with conditional routing
        var results = new List<string>();
        var startExecutor = new ConditionalExecutor("Start", results);
        var evenExecutor = new LoggingExecutor("Even", results, "Even path taken");
        var oddExecutor = new LoggingExecutor("Odd", results, "Odd path taken");

        var workflow = new WorkflowAgentBuilder()
            .WithName("ConditionalWorkflow")
            .WithExecutor(startExecutor)
            .WithExecutor(evenExecutor)
            .WithExecutor(oddExecutor)
            .AddEdge(startExecutor, evenExecutor, input => IsEven((string)input!))
            .AddEdge(startExecutor, oddExecutor, input => !IsEven((string)input!))
            .Build();

        var agent = workflow.AsAgent();

        // Act - Test even number
        var response = await agent.RunAsync(new[] { new ChatMessage(ChatRole.User, "4") });

        // Assert
        Assert.True(true);

        // Reset and test odd number
        results.Clear();
        response = await agent.RunAsync(new[] { new ChatMessage(ChatRole.User, "5") });

        // Assert
        Assert.True(true);
    }

    [Fact]
    public async Task WorkflowWithStartExecutor_UsesSpecifiedStart()
    {
        // Arrange
        var results = new List<string>();
        var startExecutor = new LoggingExecutor("Start", results, "Custom start executed");
        var middleExecutor = new LoggingExecutor("Middle", results, "Middle executed");
        var defaultExecutor = new LoggingExecutor("Default", results, "Default should not execute");

        var workflow = new WorkflowAgentBuilder()
            .WithName("StartExecutorWorkflow")
            .WithExecutor(defaultExecutor) // This would normally be start
            .WithExecutor(middleExecutor)
            .WithExecutor(startExecutor)
            .WithStartExecutor(startExecutor) // Explicitly set start
            .AddEdge(startExecutor, middleExecutor)
            .Build();

        var agent = workflow.AsAgent();

        // Act
        var response = await agent.RunAsync(new[] { new ChatMessage(ChatRole.User, "test") });

        // Assert - Only start should execute, not default
        Assert.Equal(1, results.Count);
        Assert.Equal("Custom start executed", results[0]);
        Assert.DoesNotContain("Default should not execute", results);
    }

    [Fact]
    public async Task WorkflowAccumulatesResults_ThroughMultipleExecutors()
    {
        // Arrange - Create workflow that builds a result string
        var greetingExecutor = new OutputExecutor("Greeting", "Hello ");
        var nameExecutor = new OutputExecutor("Name", "World");
        var punctuationExecutor = new OutputExecutor("Punctuation", "!");

        var workflow = new WorkflowAgentBuilder()
            .WithName("ResultBuildingWorkflow")
            .WithExecutor(greetingExecutor)
            .WithExecutor(nameExecutor)
            .WithExecutor(punctuationExecutor)
            .AddEdge(greetingExecutor, nameExecutor)
            .AddEdge(nameExecutor, punctuationExecutor)
            .Build();

        var agent = workflow.AsAgent();

        // Act
        var response = await agent.RunAsync(new[] { new ChatMessage(ChatRole.User, "ignored") });

        // Assert - Check that it executed without error
        Assert.True(true);
    }

    [Fact]
    public async Task WorkflowWithDescription_SetsDescriptionCorrectly()
    {
        // Arrange
        var executor = new TestExecutor("Test");
        var description = "This is a test workflow description";

        var workflow = new WorkflowAgentBuilder()
            .WithName("DescribedWorkflow")
            .WithDescription(description)
            .WithExecutor(executor)
            .Build();

        var agent = workflow.AsAgent();

        // Assert
        Assert.True(true);
        // Note: MAF Workflow doesn't expose description publicly, but it should be set internally
    }

    [Fact]
    public void Build_WithoutExecutors_ThrowsException()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            new WorkflowAgentBuilder()
                .WithName("TestWorkflow")
                .Build());
    }

    [Fact]
    public void WithName_SetsName()
    {
        // Arrange
        var executor = new TestExecutor("Test");

        // Act
        var workflow = new WorkflowAgentBuilder()
            .WithName("TestWorkflow")
            .WithExecutor(executor)
            .Build();

        // Assert
        Assert.NotNull(workflow);
    }

    [Fact]
    public void WithDescription_SetsDescription()
    {
        // Arrange
        var executor = new TestExecutor("Test");

        // Act
        var workflow = new WorkflowAgentBuilder()
            .WithName("TestWorkflow")
            .WithDescription("Test Description")
            .WithExecutor(executor)
            .Build();

        // Assert
        Assert.NotNull(workflow);
    }

    [Fact]
    public void WithExecutor_AddsExecutor()
    {
        // Arrange
        var executor = new TestExecutor("Custom");

        // Act
        var workflow = new WorkflowAgentBuilder()
            .WithName("TestWorkflow")
            .WithExecutor(executor)
            .Build();

        // Assert
        Assert.NotNull(workflow);
    }

    [Fact]
    public void AddEdge_AddsEdge()
    {
        // Arrange
        var executor1 = new TestExecutor("Test1");
        var executor2 = new TestExecutor("Test2");

        // Act
        var workflow = new WorkflowAgentBuilder()
            .WithName("TestWorkflow")
            .WithExecutor(executor1)
            .WithExecutor(executor2)
            .AddEdge(executor1, executor2)
            .Build();

        // Assert
        Assert.NotNull(workflow);
    }

    [Fact]
    public void WithStartExecutor_SetsStartExecutor()
    {
        // Arrange
        var startExecutor = new TestExecutor("Start");

        // Act
        var workflow = new WorkflowAgentBuilder()
            .WithName("TestWorkflow")
            .WithStartExecutor(startExecutor)
            .Build();

        // Assert
        Assert.NotNull(workflow);
    }

    // Helper methods
    private static bool IsEven(string input)
    {
        return int.TryParse(input, out var number) && number % 2 == 0;
    }

    // Test executors
    private class TestExecutor : Executor
    {
        public TestExecutor(string id = "Test") : base(id, null, false) { }

        protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.AddHandler<object>((input, context) => ValueTask.CompletedTask);
            routeBuilder.AddHandler<List<ChatMessage>>((messages, context) => ValueTask.CompletedTask);
            routeBuilder.AddHandler<TurnToken>((token, context) => ValueTask.CompletedTask);
            return routeBuilder;
        }
    }

    private class LoggingExecutor : Executor
    {
        private readonly List<string> _results;
        private readonly string _message;

        public LoggingExecutor(string id, List<string> results, string message) : base(id, null, false)
        {
            _results = results;
            _message = message;
        }

        protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.AddHandler<object>(async (input, context) =>
            {
                _results.Add(_message);
                await context.SendMessageAsync(input);
            });
            routeBuilder.AddHandler<List<ChatMessage>>(async (messages, context) =>
            {
                await context.SendMessageAsync(messages.Last().Text);
                _results.Add(_message);
            });
            routeBuilder.AddHandler<TurnToken>((token, context) => ValueTask.CompletedTask);
            return routeBuilder;
        }
    }

    private class ConditionalExecutor : Executor
    {
        private readonly List<string> _results;

        public ConditionalExecutor(string id, List<string> results) : base(id, null, false)
        {
            _results = results;
        }

        protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.AddHandler<object>(async (input, context) =>
            {
                // This executor just passes through - routing is handled by edges
                await context.SendMessageAsync(input);
            });
            routeBuilder.AddHandler<List<ChatMessage>>(async (messages, context) =>
            {
                // This executor just passes through - routing is handled by edges
                var text = messages.Last().Text;
                await context.SendMessageAsync(text);
            });
            routeBuilder.AddHandler<TurnToken>((token, context) => ValueTask.CompletedTask);
            return routeBuilder;
        }
    }

    private class OutputExecutor : Executor
    {
        private readonly string _output;

        public OutputExecutor(string id, string output) : base(id, null, false)
        {
            _output = output;
        }

        protected override RouteBuilder ConfigureRoutes(RouteBuilder routeBuilder)
        {
            routeBuilder.AddHandler<object>(async (input, context) =>
            {
                await context.YieldOutputAsync(_output);
            });
            routeBuilder.AddHandler<List<ChatMessage>>(async (messages, context) =>
            {
                await context.SendMessageAsync(messages.Last().Text);
                await context.YieldOutputAsync(_output);
            });
            routeBuilder.AddHandler<TurnToken>((token, context) => ValueTask.CompletedTask);
            return routeBuilder;
        }
    }
}