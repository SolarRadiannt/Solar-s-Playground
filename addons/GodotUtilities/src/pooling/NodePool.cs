using Godot;
using System;
using System.Collections.Generic;

namespace GodotUtilities.Pooling;

/// <summary>
/// A scene-based object pool for Godot <see cref="Node"/> instances. Keeps nodes in the scene tree
/// but disables processing and hides them when idle, avoiding the cost of frequent
/// <see cref="Node.QueueFree"/> / <see cref="PackedScene.Instantiate"/> calls.
/// </summary>
/// <typeparam name="T">A <see cref="Node"/> subtype matching the root node of <c>prefab</c>.</typeparam>
public class NodePool<T> where T : Node
{
    private readonly PackedScene prefab;
    private readonly Node parent;

    private readonly Stack<T> available = new();
    private readonly HashSet<T> inUse = new();

    private readonly Action<T> onInstantiate;

    private readonly bool extendable;

    /// <summary>Total number of instances managed by this pool (active + available).</summary>
    public int TotalCount => available.Count + inUse.Count;

    /// <summary>Number of instances currently checked out via <see cref="Get"/>.</summary>
    public int ActiveCount => inUse.Count;

    /// <summary>Number of instances currently sitting idle, ready to be retrieved.</summary>
    public int FreeCount => available.Count;

    /// <summary>
    /// Creates a new node pool and begins prewarming if <paramref name="initialSize"/> is greater than zero.
    /// </summary>
    /// <param name="prefab">The scene to instantiate nodes from.</param>
    /// <param name="parent">The node that will own all pooled instances as children.</param>
    /// <param name="initialSize">Number of instances to prewarm asynchronously. See <see cref="Prewarm"/>.</param>
    /// <param name="extendable">
    /// If <see langword="true"/> (default), the pool grows automatically when exhausted.
    /// If <see langword="false"/>, <see cref="Get"/> returns <see langword="null"/> and pushes a warning instead.
    /// </param>
    /// <param name="onInstantiate">Optional callback invoked once per instance, immediately after instantiation.</param>
    public NodePool(PackedScene prefab, Node parent, int initialSize = 0, bool extendable = true, Action<T> onInstantiate = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.extendable = extendable;
        this.onInstantiate = onInstantiate;

        Prewarm(initialSize);
    }

    /// <summary>
    /// Asynchronously adds <paramref name="count"/> instances to the pool, one per frame,
    /// so instantiation cost is spread over multiple frames rather than spiking on a single one.<br/>
    /// Calls to <see cref="Get"/> before this completes will still succeed if the pool is extendable,
    /// but will allocate new instances rather than using prewarmed ones.
    /// </summary>
    public async void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
        {
            await parent.GetTree().ToSignal(parent.GetTree(), SceneTree.SignalName.ProcessFrame);
            available.Push(CreateNew());
        }
    }

    /// <summary>
    /// Instantiates a new node from the prefab, invokes <c>onInstantiate</c>,
    /// disables it, and adds it as a child of <c>parent</c>.
    /// </summary>
    private T CreateNew()
    {
        var instance = prefab.Instantiate<T>();
        onInstantiate?.Invoke(instance);
        SetActive(instance, false);
        parent.AddChild(instance);
        return instance;
    }

    /// <summary>
    /// Retrieves an instance from the pool, creating one if none are available and the pool is extendable.<br/>
    /// Activates the node via <see cref="SetActive"/>, then invokes <see cref="IPoolable.OnGet"/>
    /// if the node implements <see cref="IPoolable"/>.
    /// </summary>
    /// <returns>An active node instance, or <see langword="null"/> if the pool is exhausted and not extendable.</returns>
    public T Get()
    {
        T node;

        if (available.Count > 0) node = available.Pop();
        else if (extendable) node = CreateNew();
        else
        {
            GD.PushWarning($"Pool for {typeof(T).Name} exhausted!");
            return null;
        }

        SetActive(node, true);
        inUse.Add(node);

        if (node is IPoolable poolable)
            poolable.OnGet();

        return node;
    }

    /// <summary>
    /// Returns a node to the pool. Disables it via <see cref="SetActive"/>,
    /// then invokes <see cref="IPoolable.OnRelease"/> if applicable.<br/>
    /// Pushes a warning and does nothing if the node is not tracked as active.
    /// </summary>
    public void Release(T node)
    {
        if (!inUse.Contains(node))
        {
            GD.PushWarning($"{typeof(T).Name} returned to the wrong pool or already released.");
            return;
        }

        SetActive(node, false);

        inUse.Remove(node);
        available.Push(node);

        if (node is IPoolable poolable)
            poolable.OnRelease();
    }

    /// <summary>
    /// Attempts to release <paramref name="node"/> back to the pool.
    /// </summary>
    /// <returns><see langword="true"/> if the node was active and has been released; <see langword="false"/> if it wasn't tracked.</returns>
    public bool TryRelease(T node)
    {
        if (!inUse.Contains(node))
            return false;
        Release(node);
        return true;
    }

    /// <summary>
    /// Releases all active nodes back to the pool, disabling each and invoking
    /// <see cref="IPoolable.OnRelease"/> where applicable.
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var node in inUse)
        {
            SetActive(node, false);

            if (node is IPoolable poolable)
                poolable.OnRelease();
            available.Push(node);
        }
        inUse.Clear();
    }

    /// <summary>
    /// Permanently destroys all pooled instances (both active and available) via <see cref="Node.QueueFree"/>.
    /// Skips any instances that are no longer valid. The pool is empty after this call.
    /// </summary>
    public void Destroy()
    {
        foreach (var node in inUse)    if (GodotObject.IsInstanceValid(node)) node.QueueFree();
        foreach (var node in available) if (GodotObject.IsInstanceValid(node)) node.QueueFree();

        available.Clear();
        inUse.Clear();
    }

    /// <summary>
    /// Shrinks the available stack down to <paramref name="keepCount"/> by permanently destroying
    /// excess idle instances. Active instances are not affected.<br/>
    /// Invokes <see cref="IPoolable.OnRelease"/> on each trimmed node before freeing it.
    /// </summary>
    /// <param name="onTrim">Optional callback invoked on each instance just before <see cref="Node.QueueFree"/>.</param>
    public void Trim(int keepCount, Action<T> onTrim = null)
    {
        while (available.Count > keepCount)
        {
            var node = available.Pop();

            if (node is IPoolable poolable)
                poolable.OnRelease();

            onTrim?.Invoke(node);
            node.QueueFree();
        }
    }

    /// <summary>
    /// Sets a node's active state by toggling its process mode and visibility.<br/>
    /// Handles <see cref="CanvasItem"/> (2D), <see cref="Node3D"/> (3D), and plain <see cref="Node"/> types.
    /// </summary>
    private static void SetActive(Node node, bool value)
    {
        node.ProcessMode = value
            ? Node.ProcessModeEnum.Inherit
            : Node.ProcessModeEnum.Disabled;

        switch (node)
        {
            case CanvasItem ci:  ci.Visible = value; break;
            case Node3D n3d: n3d.Visible = value; break;
        }
    }
}

