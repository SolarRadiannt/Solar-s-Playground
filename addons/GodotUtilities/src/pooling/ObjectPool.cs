using System.Collections.Generic;
using System;
using Godot;

namespace GodotUtilities.Pooling;

/// <summary>
/// A generic object pool for plain C# objects. Reduces allocations by reusing instances
/// instead of creating and garbage-collecting them every time.
/// </summary>
/// <typeparam name="T">The type of object to pool. Works with any reference or value type.</typeparam>
public class ObjectPool<T>
{
    private readonly Func<T> factory;

    private readonly Action<T> onGet;
    private readonly Action<T> onRelease;

    private readonly HashSet<T> inUse    = new();
    private readonly Stack<T>   available = new();

    private readonly bool extendable;

    /// <summary>Total number of instances managed by this pool (active + available).</summary>
    public int TotalCount  => available.Count + inUse.Count;

    /// <summary>Number of instances currently checked out via <see cref="Get"/>.</summary>
    public int ActiveCount => inUse.Count;

    /// <summary>Number of instances currently sitting idle, ready to be retrieved.</summary>
    public int FreeCount   => available.Count;

    /// <summary>
    /// Creates a new object pool.
    /// </summary>
    /// <param name="factory">Factory delegate used to create new instances when the pool is empty.</param>
    /// <param name="initialSize">Number of instances to create immediately via <see cref="Prewarm"/>.</param>
    /// <param name="extendable">
    /// If <see langword="true"/> (default), the pool grows automatically when exhausted.
    /// If <see langword="false"/>, <see cref="Get"/> returns <see langword="default"/> and pushes a warning instead.
    /// </param>
    /// <param name="onGet">Optional callback invoked on every <see cref="Get"/>, after <see cref="IPoolable.OnGet"/>.</param>
    /// <param name="onRelease">Optional callback invoked on every <see cref="Release"/>, after <see cref="IPoolable.OnRelease"/>.</param>
    public ObjectPool(Func<T> factory, int initialSize = 0, bool extendable = true, Action<T> onGet = null, Action<T> onRelease = null)
    {
        this.extendable = extendable;
        this.factory = factory;
        this.onGet = onGet;
        this.onRelease = onRelease;
        Prewarm(initialSize);
    }

    /// <summary>
    /// Pre-creates <paramref name="count"/> instances and adds them to the available stack.
    /// Can be called multiple times to grow the pool incrementally.
    /// </summary>
    public void Prewarm(int count)
    {
        for (int i = 0; i < count; i++)
            available.Push(factory());
    }

    /// <summary>
    /// Retrieves an instance from the pool, creating one if none are available and the pool is extendable.<br/>
    /// Invokes <see cref="IPoolable.OnGet"/> if the object implements <see cref="IPoolable"/>,
    /// then the <c>onGet</c> delegate.
    /// </summary>
    /// <returns>
    /// A pooled instance, or <see langword="default"/> if the pool is exhausted and not extendable.
    /// </returns>
    public T Get()
    {
        T @object;

        if (available.Count > 0) @object = available.Pop();
        else if (extendable) @object = factory();
        else
        {
            GD.PushWarning($"Pool for {typeof(T).Name} exhausted!");
            return default;
        }

        inUse.Add(@object);

        if (@object is IPoolable poolable)
            poolable.OnGet();
        onGet?.Invoke(@object);

        return @object;
    }

    /// <summary>
    /// Returns an instance to the pool.<br/>
    /// Invokes <see cref="IPoolable.OnRelease"/> if the object implements <see cref="IPoolable"/>,
    /// then the <c>onRelease</c> delegate.<br/>
    /// Pushes a warning and does nothing if the object is not tracked as active.
    /// </summary>
    public void Release(T @object)
    {
        if (!inUse.Contains(@object))
        {
            GD.PushWarning($"{typeof(T).Name} returned to the wrong pool or already released.");
            return;
        }

        inUse.Remove(@object);
        available.Push(@object);

        if (@object is IPoolable poolable)
            poolable.OnRelease();
        onRelease?.Invoke(@object);
    }

    /// <summary>
    /// Attempts to release <paramref name="obj"/> back to the pool.
    /// </summary>
    /// <returns><see langword="true"/> if the object was active and has been released; <see langword="false"/> if it wasn't tracked.</returns>
    public bool TryRelease(T obj)
    {
        if (!inUse.Contains(obj))
            return false;
        Release(obj);
        return true;
    }

    /// <summary>
    /// Releases all active instances back to the pool, invoking <see cref="IPoolable.OnRelease"/>
    /// and the <c>onRelease</c> delegate on each.
    /// </summary>
    public void ReleaseAll()
    {
        foreach (var obj in inUse)
        {
            if (obj is IPoolable poolable)
                poolable.OnRelease();
            onRelease?.Invoke(obj);
            available.Push(obj);
        }
        inUse.Clear();
    }

    /// <summary>
    /// Shrinks the available stack down to <paramref name="keepCount"/> by discarding excess idle instances.
    /// Active instances are not affected. Useful for reclaiming memory after a burst.
    /// </summary>
    /// <param name="onTrim">Optional callback invoked on each discarded instance before it is dropped.</param>
    public void Trim(int keepCount, Action<T> onTrim = null)
    {
        while (available.Count > keepCount)
        {
            var obj = available.Pop();
            onTrim?.Invoke(obj);
        }
    }

    /// <summary>
    /// Releases all active instances via <see cref="ReleaseAll"/>, then clears the available stack.
    /// After this call the pool is empty but still usable.
    /// </summary>
    public void Clear()
    {
        ReleaseAll();
        available.Clear();
    }
}

