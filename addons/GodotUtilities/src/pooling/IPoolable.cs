namespace GodotUtilities.Pooling;

/// <summary>
/// Implement on any pooled type to receive lifecycle callbacks
/// when the object is retrieved from or returned to a pool.
/// </summary>
public interface IPoolable
{
    /// <summary>Called immediately after the object is retrieved from the pool via <see cref="NodePool{T}.Get"/>.</summary>
    void OnGet();

    /// <summary>Called immediately after the object is returned to the pool via <see cref="NodePool{T}.Release"/>.</summary>
    void OnRelease();
}

