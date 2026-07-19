using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GodotUtilities.FSM;

public partial class StateMachine<T>
{
    public const int TRANSITION_CACHE_INITIAL_CAPACITY = 32;

    private readonly List<Transition<T>> globalTransitions = new();
    private Transition<T>[] transitionCache = new Transition<T>[TRANSITION_CACHE_INITIAL_CAPACITY];

    private int transitionCounter;
    private int transitionCacheCount;

    private void TransitionTo(T id, bool bypassExit = false, bool respectLocked = true)
    {
        if (!ValidateId(id)) return;
        if (locked && respectLocked) return;

        if (!bypassExit && currentState != null) currentState.Exit?.Invoke();
        if (currentState != null) previousId = currentState.Id;

        currentState = states[id];
        elapsed = 0.0;
        currentState.Enter?.Invoke();

        StateChanged?.Invoke(previousId, currentState.Id);
        cacheDirty = true;
    }

    #region Transitions Evaluation

    private bool ShouldSkipTransition(Transition<T> transition) =>
        transition.IsExcluded(currentState.Id) ||
        !transition.IsIncluded(currentState.Id) ||
        GetState(transition.To).IsOnCooldown(totalTime) ||
        transition.IsGuardBlocked() ||
        !transition.IsMinTimeExceeded(elapsed, currentState.MinDuration);

    private void OnTransitionSelected(Transition<T> transition)
    {
        GetState(transition.To).LastFiredTime = totalTime;
        transition.Callback?.Invoke();
        TransitionTo(transition.To);
    }

    private void EvaluateTransitions()
    {
        var span = transitionCache.AsSpan(0, transitionCacheCount);

        foreach (ref readonly var transition in span)
        {
            if (ShouldSkipTransition(transition))
                continue;

            if (transition.Condition?.Invoke() ?? false)
            {
                OnTransitionSelected(transition);
                return;
            }
        }
    }

    private void RebuildTransitionCache()
    {
        cacheDirty = false;

        var local = currentState.Transitions;
        var global = globalTransitions;

        transitionCacheCount = local.Count + global.Count;

        if (transitionCacheCount > transitionCache.Length)
            Array.Resize(ref transitionCache, transitionCacheCount * 2);

        CollectionsMarshal.AsSpan(local).CopyTo(transitionCache);
        CollectionsMarshal.AsSpan(global).CopyTo(transitionCache.AsSpan(local.Count));
    }

    #endregion

    #region Transition Registry

    /// <summary>
    /// Adds a transition from <paramref name="fromId"/> to <paramref name="toId"/> and returns it for fluent configuration.<br/>
    /// Transitions are evaluated in priority-then-insertion order each tick via <see cref="Tick"/>.
    /// </summary>
    /// <param name="fromId">The state this transition originates from.</param>
    /// <param name="toId">The state this transition leads to.</param>
    /// <returns>
    /// The new <see cref="Transition{T}"/> to configure with <c>.When()</c>, <c>.IfOnly()</c>, <c>.OnEvent()</c>, etc.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if either <paramref name="fromId"/> or <paramref name="toId"/> is not a registered state.</exception>
    public Transition<T> AddTransition(T fromId, T toId)
    {
        if (!ValidateId(fromId) || !ValidateId(toId))
            throw new ArgumentException($"Invalid state id in AddTransition({fromId}, {toId})");

        var transition = states[fromId].AddTransition(toId, transitionCounter++);
        cacheDirty     = true;

        return transition;
    }

    /// <summary>
    /// Adds a transition to <paramref name="id"/> that is evaluated regardless of the current active state.<br/>
    /// Useful for universal rules such as a global "dead" or "paused" state that can be reached from anywhere.
    /// </summary>
    /// <param name="id">The target state id.</param>
    /// <returns>
    /// The new <see cref="Transition{T}"/> to configure with <c>.When()</c>, <c>.IfOnly()</c>, <c>.OnEvent()</c>, etc.
    /// </returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is not a registered state.</exception>
    public Transition<T> AddGlobalTransition(T id)
    {
        if (!ValidateId(id)) throw new ArgumentException($"Invalid state id in AddGlobalTransition({id})");

        var transition = new Transition<T>(default, id, isGlobal: true) { InsertionIndex = transitionCounter++ };

        SortedInsert(globalTransitions, transition);
        cacheDirty = true;
        return transition;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Returns <see langword="true"/> if the machine can transition to <paramref name="id"/> right now —
    /// meaning the machine is unlocked, the id is valid, <see cref="State{T}.MinDuration"/> has been exceeded and 
    /// <see cref="State{T}"/> is not on cooldown
    /// </summary>
    /// <param name="id">The target state id to check.</param>
    public bool CanTransition(T id)
    {
        if (currentState == null) return false;
        return ValidateId(id) && elapsed > currentState.MinDuration && !GetState(id).IsOnCooldown(totalTime) && !locked;
    }

    /// <summary>
    /// Attempts to transition to <paramref name="id"/>, respecting all normal rules
    /// (lock, min duration, cooldown, valid id).<br/>
    /// Use <see cref="ForceTransition"/> to bypass all of them.
    /// </summary>
    public bool TryTransition(T id)
    {
        if (!CanTransition(id)) return false;
        TransitionTo(id);
        return true;
    }

    /// <summary>
    /// Transitions to <paramref name="id"/> immediately, bypassing all guards —
    /// lock, cooldown, and min duration are all ignored.<br/>
    /// Prefer <see cref="TryTransition"/> for normal flow.
    /// </summary>
    public void ForceTransition(T id) => TransitionTo(id, respectLocked: false);

    #endregion
}

