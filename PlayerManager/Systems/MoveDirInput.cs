namespace PlayerManager.Systems;
using fennecs;
using Godot;

using PlayerManager.Components;
using SolFramework.Scheduler;
using SolFramework.Core;
using SolFramework.MoveManager;

public partial class MoveDirInput : Node, ISystem
{
    public int Priority => SPriority.Action - 1;
    public void Process(double _)
    {
        _main();
    }

    public void Init()
    {
        Scheduler.RegisterSystem(this);
    }

    public override void _Ready()
    {
        Init();
    }
    private static World world = Core.World;
    private static Stream<MoveDirection> _controllable = world.Query<MoveDirection>()
        .Has<Player>()
        .Stream();
    public void _main()
    {
        _controllable.For(
            static (ref MoveDirection moveDir) =>
            {
                var dir = Vector2.Zero;
                if (Input.IsActionPressed("move_up"))
                    dir += Vector2.Up;
                if (Input.IsActionPressed("move_down"))
                    dir += Vector2.Down;
                if (Input.IsActionPressed("move_left"))
                    dir += Vector2.Left;
                if (Input.IsActionPressed("move_right"))
                    dir += Vector2.Right;
                
                moveDir.Value = dir;
            });
    }
};