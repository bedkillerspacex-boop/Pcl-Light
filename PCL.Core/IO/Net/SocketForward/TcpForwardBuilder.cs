using System;
using System.Net;
using PCL.Core.Utils.Exts;

namespace PCL.Core.IO.Net.SocketForward;

public class TcpForwardBuilder
{
    private readonly TcpForwardConfig _cfg = new();

    public TcpForwardWorker Build()
    {
        ArgumentNullException.ThrowIfNull(_cfg.RemoteHost);
        ArgumentOutOfRangeCompatibility.ThrowIfZero(_cfg.RemotePort);
        
        if (_cfg.LocalHost.IsNullOrWhiteSpace()) _cfg.LocalHost = IPAddress.Loopback.ToString();
        
        return new TcpForwardWorker(_cfg);
    }

    public TcpForwardBuilder BindLocalRandom()
    {
        _cfg.LocalHost = IPAddress.Loopback.ToString();
        _cfg.LocalPort = 0;
        
        return this;
    }

    public TcpForwardBuilder BindLocal(ushort port)
    {
        ArgumentOutOfRangeCompatibility.ThrowIfZero(port);
        
        _cfg.LocalHost = IPAddress.Loopback.ToString();
        _cfg.LocalPort = port;

        return this;
    }

    public TcpForwardBuilder SetRemote(string host, ushort port)
    {
        ArgumentCompatibility.ThrowIfNullOrWhiteSpace(host);
        ArgumentOutOfRangeCompatibility.ThrowIfZero(port);

        _cfg.RemoteHost = host;
        _cfg.RemotePort = port;

        return this;
    }

    public TcpForwardBuilder SetRemote(IPEndPoint remote)
    {
        ArgumentNullException.ThrowIfNull(remote);

        _cfg.RemoteHost = remote.Address.ToString();
        _cfg.RemotePort = (ushort) remote.Port;
        
        return this;
    }

    public TcpForwardBuilder SetRemote(IPAddress host, ushort port)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentOutOfRangeCompatibility.ThrowIfZero(port);

        _cfg.RemoteHost = host.ToString();
        _cfg.RemotePort = port;

        return this;
    }

    public TcpForwardBuilder SetBufferSize(uint size)
    {
        ArgumentOutOfRangeCompatibility.ThrowIfGreaterThan(size, TcpForwardConfig.MaxBufferSize);
        ArgumentOutOfRangeCompatibility.ThrowIfLessThan(size, TcpForwardConfig.MinBufferSize);

        _cfg.BufferSize = size;

        return this;
    }

    public TcpForwardBuilder SetMaxAllowedActiveConnection(ushort maxConnectionCount)
    {
        ArgumentOutOfRangeCompatibility.ThrowIfZero(maxConnectionCount);

        _cfg.MaxConnection = maxConnectionCount;

        return this;
    }
}