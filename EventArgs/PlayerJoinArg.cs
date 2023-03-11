using System;
using Rift.Player;

namespace Rift.Events;

public class PlayerJoinArg: EventArgs
{
    public PlayerJoinArg(PlayerBlob player,Server server)
    {
        Server = server;
        Player = player;
    }

    public Server Server { get; }
    public PlayerBlob Player { get; }
}