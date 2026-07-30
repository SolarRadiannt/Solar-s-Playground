namespace SolFramework.Systems;

using Godot;
using fennecs;

using SolFramework;
using SolFramework.Scheduler;
using SolFramework.Components;

public partial class ActualVelCalc : Node, ISystem 
{
    public int Priority => SPriority.Transformation + 5;
    public void Process(double _)
    {
        CalculateActualVelAndSpeed();
        UpdateLastPosition();
    }

    public void Init()
    {
        Scheduler.RegisterPhysicsSystem(this);
    }

    public override void _Ready() => Init();

    private static World world = Core.World;

    private static Stream<Node2DHandle, LastPosition> calculateables =
        world.Stream<Node2DHandle, LastPosition>(); 
    private static void CalculateActualVelAndSpeed() =>
        calculateables.For(static
        (in Entity entity, ref Node2DHandle handle, ref LastPosition lastPos) =>
        {
            var actualVel = handle.Value.GlobalPosition - lastPos.Value;
            float actualSpeed = actualVel.Length();

            if (entity.Has<ActualVelocity>())
                entity.Ref<ActualVelocity>().Value = actualVel;
            else
                entity.Add(new ActualVelocity(actualVel));

            
            if (entity.Has<ActualSpeed>())
                entity.Ref<ActualSpeed>().Value = actualSpeed;
            else
                entity.Add(new ActualSpeed(actualSpeed));
        });

    public static readonly Stream<Node2DHandle> toUpdate = world.Stream<Node2DHandle>();
	public static void UpdateLastPosition() =>
		toUpdate.For(static
		(in Entity entity, ref Node2DHandle handle) =>
		{
			if (entity.Has<LastPosition>())
				entity.Ref<LastPosition>().Value = handle.Value.GlobalPosition;
			else
				entity.Add(new LastPosition(handle.Value.GlobalPosition));
		});
}