namespace SolFramework.UtilityAI.Components;

using System.Collections.Generic;

public record struct AgentActions(List<BaseAction> Value);
public struct AgentDisabled;
public struct AgentIdle;