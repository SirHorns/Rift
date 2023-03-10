using Rift.Player;

namespace Rift.Events;

public class PlayerArg : EventArgs
{
    public PlayerArg(PlayerBlob player)
    {
        Player = player;
    }

    public PlayerBlob Player { get; }
}