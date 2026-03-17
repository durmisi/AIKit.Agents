using Microsoft.Agents.AI.Workflows;

namespace AIKit.Agents;

/// <summary>
/// Fluent builder for creating workflow agents.
/// </summary>
public class WorkflowAgentBuilder
{
    private string? _name;
    private string? _description;
    private Executor? _startExecutor;
    private readonly List<Executor> _executors = new();
    private readonly List<WorkflowEdge> _edges = new();

    /// <summary>
    /// Sets the workflow name.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The builder instance.</returns>
    public WorkflowAgentBuilder WithName(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    /// <summary>
    /// Sets the workflow description.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>The builder instance.</returns>
    public WorkflowAgentBuilder WithDescription(string description)
    {
        _description = description ?? throw new ArgumentNullException(nameof(description));
        return this;
    }

    /// <summary>
    /// Sets the start executor for the workflow.
    /// </summary>
    /// <param name="startExecutor">The start executor.</param>
    /// <returns>The builder instance.</returns>
    public WorkflowAgentBuilder WithStartExecutor(Executor startExecutor)
    {
        _startExecutor = startExecutor ?? throw new ArgumentNullException(nameof(startExecutor));
        return this;
    }

    /// <summary>
    /// Adds an executor to the workflow.
    /// </summary>
    /// <param name="executor">The executor to add.</param>
    /// <returns>The builder instance.</returns>
    public WorkflowAgentBuilder WithExecutor(Executor executor)
    {
        _executors.Add(executor ?? throw new ArgumentNullException(nameof(executor)));
        return this;
    }

    /// <summary>
    /// Adds an edge between two executors.
    /// </summary>
    /// <param name="source">The source executor.</param>
    /// <param name="target">The target executor.</param>
    /// <param name="condition">Optional condition for the edge.</param>
    /// <param name="label">Optional label for the edge.</param>
    /// <param name="idempotent">Whether the edge is idempotent.</param>
    /// <returns>The builder instance.</returns>
    public WorkflowAgentBuilder AddEdge(Executor source, Executor target, Func<object?, bool>? condition = null, string? label = null, bool idempotent = false)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        if (target == null) throw new ArgumentNullException(nameof(target));
        _edges.Add(new WorkflowEdge(source, target, condition, label, idempotent));
        return this;
    }

    /// <summary>
    /// Builds the workflow agent.
    /// </summary>
    /// <returns>The MAF Workflow instance.</returns>
    public Workflow Build()
    {
        Executor startExecutor;
        if (_startExecutor != null)
        {
            startExecutor = _startExecutor;
        }
        else if (_executors.Count > 0)
        {
            startExecutor = _executors[0];
        }
        else
        {
            throw new InvalidOperationException("At least one executor must be added or a start executor must be specified.");
        }

        var builder = new WorkflowBuilder(startExecutor)
            .WithName(_name ?? "DefaultWorkflow");

        if (_description != null)
        {
            builder.WithDescription(_description);
        }

        foreach (var edge in _edges)
        {
            builder.AddEdge(edge.Source, edge.Target, edge.Condition, edge.Label, edge.Idempotent);
        }

        return builder.Build();
    }

    private record WorkflowEdge(Executor Source, Executor Target, Func<object?, bool>? Condition, string? Label, bool Idempotent);

    private class InternalWorkflowExecutor : Executor
    {
        public InternalWorkflowExecutor() : base("InternalWorkflowExecutor", null, false)
        {
        }


        protected override ProtocolBuilder ConfigureProtocol(ProtocolBuilder protocolBuilder)
        {
            return protocolBuilder.ConfigureRoutes(routeBuilder =>
            {
                routeBuilder.AddHandler<object>((input, context) => ValueTask.CompletedTask);
            });
        }
    }
}