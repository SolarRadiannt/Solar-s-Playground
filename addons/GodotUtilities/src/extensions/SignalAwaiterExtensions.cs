using Godot;
using System.Threading.Tasks;

namespace GodotUtilities;

public static class SignalAwaiterExtensions
{
    public static Task<Variant[]> ToTask(this SignalAwaiter awaiter)
    {
        return Task.Run(async () => await awaiter);
    }
}