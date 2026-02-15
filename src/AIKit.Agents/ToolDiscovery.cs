using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Reflection;

namespace AIKit.Agents;

/// <summary>
/// Component for discovering tools from assemblies.
/// </summary>
public static class ToolDiscovery
{
    /// <summary>
    /// Discovers tools from the specified assemblies.
    /// </summary>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <param name="serviceProvider">The service provider for DI.</param>
    /// <returns>A collection of AI tools.</returns>
    public static IEnumerable<AITool> DiscoverTools(Assembly[] assemblies, IServiceProvider? serviceProvider = null)
    {
        var tools = new List<AITool>();

        foreach (var assembly in assemblies)
        {
            try
            {
                var types = assembly.GetTypes()
                    .Where(t => t.IsPublic && !t.IsAbstract && !t.IsInterface);

                foreach (var type in types)
                {
                    var toolMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                        .Where(m => m.GetCustomAttribute<DescriptionAttribute>() != null);

                    if (!toolMethods.Any()) continue;

                    object? instance = null;
                    if (toolMethods.Any(m => !m.IsStatic))
                    {
                        try
                        {
                            if (serviceProvider != null)
                            {
                                instance = ActivatorUtilities.CreateInstance(serviceProvider, type);
                            }
                            else
                            {
                                instance = Activator.CreateInstance(type);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new ToolInstantiationException($"Failed to instantiate type {type.FullName}: {ex.Message}");
                        }
                    }

                    foreach (var method in toolMethods)
                    {
                        try
                        {
                            tools.Add(AIFunctionFactory.Create(method, instance));
                        }
                        catch (Exception ex)
                        {
                            throw new ToolInstantiationException($"Failed to create tool from method {method.Name}: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new AssemblyScanException($"Failed to scan assembly {assembly.FullName}: {ex.Message}");
            }
        }

        return tools;
    }
}