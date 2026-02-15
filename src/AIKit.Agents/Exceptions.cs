namespace AIKit.Agents;

/// <summary>
/// Exception thrown when required configuration is missing.
/// </summary>
public class MissingModelException : Exception
{
    public MissingModelException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when tool instantiation fails.
/// </summary>
public class ToolInstantiationException : Exception
{
    public ToolInstantiationException(string message) : base(message) { }
}

/// <summary>
/// Exception thrown when assembly scanning fails.
/// </summary>
public class AssemblyScanException : Exception
{
    public AssemblyScanException(string message) : base(message) { }
}

