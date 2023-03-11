using System;
using System.Collections.Generic;
using Rift.Events;
using Rift.Player;

namespace Rift;

public class ServerData
{
    private readonly List<PlayerBlob> _players  = new();
    public int PlayerCount => _players.Count;
    public event EventHandler? PlayedAdded;

    public void AddPlayer(PlayerBlob player)
    {
        _players.Add(player);

        PlayedAdded?.Invoke(this, new PlayerArg(player));
    }

    public List<PlayerBlob> GetPlayers()
    {
        return _players;
    }
}