AIKit.Agents
Phase 1 – Requirements Specification
Version: 0.1
Target Framework: Microsoft Agent Framework (MAF)
Language: C# (.NET 10)

1. Purpose
   AIKit.Agents is a lightweight developer-experience layer built on top of Microsoft Agent Framework.
   Its purpose is to:

- Simplify agent creation
- Reduce boilerplate
- Automate tool discovery
- Standardize configuration patterns
- Reuse MAF APIs directly (no re-implementation)
  AIKit.Agents MUST NOT:
- Replace MAF
- Abstract away core MAF primitives
- Introduce a parallel agent model
- Create unnecessary custom abstractions

2. Architectural Principles
   2.1 Foundation

- AIKit.Agents MUST use Microsoft Agent Framework internally.
- All agents created MUST ultimately be native MAF agents.
- All tool registration MUST use native MAF APIs.

  2.2 Abstraction Policy
  The library MUST:

- Add orchestration helpers
- Provide fluent builders
- Provide reflection-based tool discovery
  The library MUST NOT:
- Reimplement agent runtime
- Wrap every MAF type in custom types
- Introduce custom tool interfaces (Phase 1)

3. Supported Agent Types (Phase 1)
   3.1 Chat Agents

- LLM-backed conversational agents
- With tool support
- With system instructions
- With memory (if MAF supports)

  3.2 Workflow Agents (Basic Support)

- Ability to create workflow-style agents
- Delegate to MAF workflow primitives
- Minimal configuration layer

4. Core Components
   4.1 ChatAgentBuilder
   Responsibilities:

- Configure LLM
- Configure system message
- Configure tools
- Configure services
- Build native MAF agent

Required Methods:

- WithModel(string modelName)
- WithSystemMessage(string systemMessage)
- WithToolsFromAssembly(params Assembly[]? assemblies)
- WithServiceProvider(IServiceProvider provider)
- Build() -> returns native MAF agent

5. Tool Discovery Requirements
   5.1 ToolDiscovery Component

- Scans assemblies
- Identifies valid tool types
- Filters abstract and non-public types

  5.2 Tool Identification Rules

- Detect MAF-compatible tools
- Support detection via interface implementation or known attribute markers
- Phase 1 MUST NOT introduce custom tool interfaces

  5.3 Assembly Resolution

- Default to Assembly.GetEntryAssembly() if no assemblies provided
- Throw meaningful exception if entry assembly is null

6. Dependency Injection Integration

- Support optional IServiceProvider
- Instantiate tool types via ActivatorUtilities
- Fallback to Activator.CreateInstance if DI not provided

7. Configuration Constraints

- Phase 1 MUST NOT include complex policy engines, retry frameworks, custom memory stores, custom execution pipeline, observability layer

8. Error Handling Requirements

- Throw meaningful exceptions if:
  - No model configured
  - Tool instantiation fails
  - Assembly scanning fails

- Avoid silent failures

9. Non-Functional Requirements
   9.1 Performance

- Tool scanning must happen once per builder
- Avoid repeated reflection scanning
- Avoid dynamic proxies

  9.2 Simplicity

- Public API surface must remain small
- Maximum abstraction depth: 1 layer above MAF

  9.3 Compatibility

- Must compile against .NET 8+
- Must track latest stable MAF version

11. Developer Experience Goals

- Reduce setup code by at least 40%
- Remove manual reflection
- Standardize creation pattern

12. Out of Scope (Phase 1)

- Agent orchestration
- Agent-to-agent communication
- Distributed execution
- Memory abstraction
- Tool permission system
- Telemetry framework
- UI integration
- Multi-model routing

13. Phase 1 Deliverables

14. Core builder implementation

15. ToolDiscovery component

16. Basic WorkflowAgentBuilder

17. Unit tests for tool scanning, multiple assemblies, DI tool instantiation

18. Sample project

19. README documentation

20. Strategic Positioning
    AIKit.Agents is positioned as:
    "The Fluent Builder Layer for Microsoft Agent Framework"
    Not a competitor. Not a replac
