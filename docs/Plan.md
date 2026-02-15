# Implementation Plan: Align AIKit.Agents with Microsoft Agent Framework

## Overview

This plan outlines the steps to enhance AIKit.Agents to fully align with Microsoft's Agent Framework (MAF), incorporating advanced features like multimodal support, structured output, RAG, background responses, observability, declarative agents, middleware, conversations, and expanded tools. The library will remain a builder layer, assuming `IChatClient` is provided externally (e.g., from the AIKit repo). Changes build incrementally on the existing codebase, prioritizing core MAF features.

## Key Assumptions

- `IChatClient` instances are created and passed in externally; no creation within AIKit.Agents.
- All changes reuse MAF APIs directly without reimplementation.
- Retain the fluent builder pattern for simplicity.
- Test thoroughly after each major change to avoid breaking existing functionality.

## Steps

1. **Update AgentBuilder.cs for Core Enhancements**
   - Add support for `AgentSession` to enable conversations and memory.
   - Implement streaming runs via `RunStreamingAsync`.
   - Add options for background responses (`AllowBackgroundResponses`) with continuation token handling.
   - Ensure all enhancements work with the passed `IChatClient`.

2. **Add Multimodal Support**
   - Modify `AgentBuilder.cs` to accept `ChatMessage` inputs with `UriContent` or `DataContent` for images/audio.
   - Validate model compatibility (e.g., GPT-4o) via the `IChatClient`.
   - Update samples to demonstrate multimodal runs.

3. **Implement Structured Output**
   - Integrate `ChatResponseFormat.ForJsonSchema` and `AIJsonUtilities.CreateJsonSchema` in `AgentBuilder.cs`.
   - Add deserialization helpers for responses.
   - Provide builder methods to configure schemas from C# types.

4. **Integrate RAG**
   - Add `AIContextProviderFactory` configuration in `AgentBuilder.cs` for `TextSearchProvider`.
   - Support custom search adapters and options (e.g., search timing).
   - Ensure context injection works with the `AIAgent`.

5. **Support Background Responses**
   - Extend `AgentBuilder.cs` with continuation token logic for polling and stream resumption.
   - Add backoff handling for long-running tasks.

6. **Incorporate Observability**
   - Use `WithOpenTelemetry` on the `AIAgent` and `UseOpenTelemetry` on the `IChatClient` in `AgentBuilder.cs`.
   - Configure exporters (e.g., Azure Monitor) via builder options.

7. **Expand Tool Support**
   - Update `ToolDiscovery.cs` to detect and wrap tools with `ApprovalRequiredAIFunction`.
   - Add support for code interpreter, search, and MCP tools beyond function tools.
   - Integrate approvals into the builder's tool list.

8. **Add Middleware**
   - Implement builder `Use` methods in `AgentBuilder.cs` for run, function, and chat interception.
   - Allow chaining middleware on the `IChatClient` and `AIAgent`.

9. **Create DeclarativeAgentBuilder.cs**
   - New file for loading agents from YAML/JSON using `AgentFactory.CreateFromYaml`.
   - Accept an external `IChatClient` for instantiation.

10. **Update Samples**
    - Enhance `ChatUseCase.cs` and others in `AIKit.Agents.Sample/` to demo new features (multimodal, RAG, structured output, observability).
    - Ensure samples use passed `IChatClient` instances.

11. **Enhance WorkflowAgentBuilder.cs**
    - Add agent-as-tool composition via `AsAIFunction`.
    - Support workflows as agents with `IChatClient` integration where applicable.

12. **Add ProviderFactory.cs**
    - New file for provider abstractions (Azure OpenAI, OpenAI, etc.) with feature matrices.
    - Guide `IChatClient` selection without creation.

13. **Update Tests**
    - Add unit tests in `AIKit.Agents.Tests/` for new builder methods and MAF integrations.
    - Include integration tests with mocked `IChatClient`.
    - Ensure coverage for advanced features.

14. **Revise Documentation**
    - Update `README.md` and samples with examples for new features.
    - Note external `IChatClient` dependency.

## Verification

- Run unit and integration tests after each step.
- Execute samples to validate features (e.g., streaming, multimodal).
- Use OpenTelemetry for observability checks.
- Manual testing for background responses and RAG.
- Ensure no regressions in existing functionality.

## Decisions

- Prioritize core MAF features (multimodal, structured output, RAG) first.
- Use MAF's built-in support (e.g., `WithOpenTelemetry`) over custom code.
- Incremental rollout to minimize risk.

## Timeline

- Phase 1 (Core Enhancements): 1-2 weeks
- Phase 2 (Advanced Features): 2-4 weeks
- Testing and Documentation: 1 week

## Risks

- MAF API changes; track updates closely.
- Performance impact from new features; profile and optimize.
- Complexity increase; maintain simplicity via builders.
