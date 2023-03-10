namespace Rift.Events;

public class ServerArg: EventArgs
{
    public ServerArg(Server server)
    {
        Server = server;
    }

    public Server Server { get; }
}