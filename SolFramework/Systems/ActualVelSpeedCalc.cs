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

    private static Stream<Node2D, LastPosition> calculateables =
        world.Stream<Node2D, LastPosition>(); 
    private static void CalculateActualVelAndSpeed() =>
        calculateables.For(static
        (in Entity entity, ref Node2D handle, ref LastPosition lastPos) =>
        {
            var actualVel = handle.GlobalPosition - lastPos.Value;
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

    public static readonly Stream<Node2D> toUpdate = world.Stream<Node2D>();
	public static void UpdateLastPosition() =>
		toUpdate.For(static
		(in Entity entity, ref Node2D handle) =>
		{
			if (entity.Has<LastPosition>())
				entity.Ref<LastPosition>().Value = handle.GlobalPosition;
			else
				entity.Add(new LastPosition(handle.GlobalPosition));
		});
}