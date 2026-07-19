using Godot;
using Godot.Collections;
using GodotUtilities.Pooling;

namespace GodotUtilities;

/// <summary>
/// Singleton utility node providing pooled, allocation-friendly 2D physics queries.<br/>
/// Add one instance to your scene tree — all methods are static and route through it automatically.
/// </summary>
[Icon("uid://b5qprcthscj0f")]
public partial class PhysicsQuery2D : Node
{
    private const uint ALL_LAYERS = uint.MaxValue;
    private const int INITIAL_POOL_SIZE = 4;

    private static PhysicsQuery2D instance;

    private PhysicsDirectSpaceState2D spaceState;

    private ObjectPool<PhysicsRayQueryParameters2D> queryRayPool;
    private ObjectPool<PhysicsShapeQueryParameters2D> queryShapePool;
    private ObjectPool<CircleShape2D> sphereShapePool;

    private Timer timer;

    public override void _EnterTree() => instance = this;

    public override void _Ready()
    {
        spaceState = GetViewport().World2D.DirectSpaceState;

        queryRayPool = new(() => new(), INITIAL_POOL_SIZE);
        queryShapePool = new(() => new(), INITIAL_POOL_SIZE);
        sphereShapePool = new(() => new(), INITIAL_POOL_SIZE);

        timer = new() { WaitTime = 8.0 }; AddChild(timer);
        timer.Timeout += OnTimerTimeout;
    }

    private void OnTimerTimeout()
    {
        queryRayPool.Trim(INITIAL_POOL_SIZE);
        queryShapePool.Trim(INITIAL_POOL_SIZE);
        sphereShapePool.Trim(INITIAL_POOL_SIZE);
    }

    #region Raycast

    /// <summary>Casts a ray from <paramref name="origin"/> in <paramref name="direction"/> for up to <paramref name="distance"/> units against all layers.</summary>
    /// <returns><see langword="true"/> if anything was hit.</returns>
    public static bool Raycast(Vector2 origin, Vector2 direction, float distance) =>
        Raycast(origin, direction, distance, out var _);

    /// <inheritdoc cref="Raycast(Vector2, Vector2, float)"/>
    /// <param name="hit">Hit info. Default if nothing was hit.</param>
    public static bool Raycast(Vector2 origin, Vector2 direction, float distance, out RaycastHit hit) =>
        Raycast(origin, direction, distance, out hit, ALL_LAYERS);

    /// <inheritdoc cref="Raycast(Vector2, Vector2, float, out RaycastHit)"/>
    /// <param name="collisionMask">Layers to test against.</param>
    public static bool Raycast(Vector2 origin, Vector2 direction, float distance, out RaycastHit hit, uint collisionMask) =>
        Raycast(origin, origin + direction.Normalized() * distance, out hit, collisionMask);

    /// <summary>Casts a ray from <paramref name="pointA"/> to <paramref name="pointB"/> against all layers.</summary>
    /// <returns><see langword="true"/> if anything was hit.</returns>
    public static bool Raycast(Vector2 pointA, Vector2 pointB) =>
        Raycast(pointA, pointB, out var _);

    /// <inheritdoc cref="Raycast(Vector2, Vector2)"/>
    /// <param name="hit">Hit info. Default if nothing was hit.</param>
    public static bool Raycast(Vector2 from, Vector2 to, out RaycastHit hit) =>
        Raycast(from, to, out hit, ALL_LAYERS);

    /// <inheritdoc cref="Raycast(Vector2, Vector2, out RaycastHit)"/>
    /// <param name="collisionMask">Layers to test against.</param>
    public static bool Raycast(Vector2 pointA, Vector2 pointB, out RaycastHit hit, uint collisionMask)
    {
        var query = instance.queryRayPool.Get();

        query.From = pointA;
        query.To   = pointB;
        query.CollisionMask = collisionMask;

        var result = instance.spaceState.IntersectRay(query);
        instance.queryRayPool.Release(query);

        if (result.Count <= 0)
        {
            hit = default;
            return false;
        }

        hit = new RaycastHit
        {
            Normal     = result["normal"].AsVector2(),
            Position   = result["position"].AsVector2(),
            Collider   = result["collider"].AsGodotObject(),
            ColliderRid = result["rid"].AsRid()
        };

        return true;
    }

    #endregion

    #region Check Sphere

    /// <summary>
    /// Returns <see langword="true"/> if any collider overlaps a circle at <paramref name="position"/> with the given <paramref name="radius"/>.
    /// Tests against all layers.
    /// </summary>
    public static bool CheckSphere(Vector2 position, float radius) =>
        CheckSphere(position, radius, out _, ALL_LAYERS);

    /// <inheritdoc cref="CheckSphere(Vector2, float)"/>
    /// <param name="collisionMask">Layers to test against.</param>
    public static bool CheckSphere(Vector2 position, float radius, uint collisionMask) =>
        CheckSphere(position, radius, out _, collisionMask);

    /// <inheritdoc cref="CheckSphere(Vector2, float)"/>
    /// <param name="collider">The first overlapping collider, or <see langword="null"/> if none.</param>
    public static bool CheckSphere(Vector2 position, float radius, out GodotObject collider) =>
        CheckSphere(position, radius, out collider, ALL_LAYERS);

    /// <inheritdoc cref="CheckSphere(Vector2, float, out GodotObject)"/>
    /// <param name="collisionMask">Layers to test against.</param>
    public static bool CheckSphere(Vector2 position, float radius, out GodotObject collider, uint collisionMask)
    {
        var overlaps = IntersectSphere(position, radius, collisionMask, maxResults: 1);

        if (overlaps.Count > 0)
        {
            collider = overlaps[0]["collider"].AsGodotObject();
            return true;
        }

        collider = null;
        return false;
    }

    #endregion

    #region Overlap Sphere

    /// <summary>
    /// Returns all colliders overlapping a circle at <paramref name="position"/> with the given <paramref name="radius"/>.
    /// Tests against all layers. Capped at 16 results.
    /// </summary>
    public static GodotObject[] OverlapSphere(Vector2 position, float radius) =>
        OverlapSphere(position, radius, ALL_LAYERS);

    /// <inheritdoc cref="OverlapSphere(Vector2, float)"/>
    /// <param name="collisionMask">Layers to test against.</param>
    public static GodotObject[] OverlapSphere(Vector2 position, float radius, uint collisionMask) =>
        OverlapSphere(position, radius, collisionMask, maxResults: 16);

    /// <inheritdoc cref="OverlapSphere(Vector2, float)"/>
    /// <param name="maxResults">Maximum number of colliders to return.</param>
    public static GodotObject[] OverlapSphere(Vector2 position, float radius, int maxResults) =>
        OverlapSphere(position, radius, ALL_LAYERS, maxResults);

    /// <inheritdoc cref="OverlapSphere(Vector2, float, int)"/>
    /// <param name="collisionMask">Layers to test against.</param>
    public static GodotObject[] OverlapSphere(Vector2 position, float radius, uint collisionMask, int maxResults)
    {
        var overlaps = IntersectSphere(position, radius, collisionMask, maxResults);
        var result   = new GodotObject[overlaps.Count];

        for (int i = 0; i < overlaps.Count; i++)
            result[i] = overlaps[i]["collider"].AsGodotObject();

        return result;
    }

    #endregion

    private static Array<Dictionary> IntersectSphere(Vector2 position, float radius, uint mask, int maxResults)
    {
        var query = instance.queryShapePool.Get();
        var shape = instance.sphereShapePool.Get();

        shape.Radius         = radius;
        query.Shape          = shape;
        query.Transform      = new Transform2D(0f, position);
        query.CollisionMask  = mask;

        var overlaps = instance.spaceState.IntersectShape(query, maxResults);
        instance.queryShapePool.Release(query);
        instance.sphereShapePool.Release(shape);

        return overlaps;
    }
}

/// <summary>
/// Data returned by a successful <see cref="PhysicsQuery2D.Raycast"/> call.
/// </summary>
public readonly struct RaycastHit
{
    /// <summary>World-space point where the ray intersected the collider surface.</summary>
    public Vector2 Position { get; init; }

    /// <summary>Surface normal at the point of intersection.</summary>
    public Vector2 Normal { get; init; }

    /// <summary>The collider that was hit.</summary>
    public GodotObject Collider { get; init; }

    /// <summary>RID of the collider that was hit. Useful for low-level physics API calls.</summary>
    public Rid ColliderRid { get; init; }

    /// <summary>
    /// Casts <see cref="Collider"/> to <typeparamref name="T"/>.
    /// Returns <see langword="null"/> if the collider is not of that type.
    /// </summary>
    public readonly T GetCollider<T>() where T : Node2D => Collider as T;
}