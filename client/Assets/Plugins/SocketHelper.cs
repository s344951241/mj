using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;

public class SocketHelper{

    private static bool? _isIpv6Only;

    public static bool IsIpv6Only {
        get {
            if (_isIpv6Only.HasValue)
            {
                return _isIpv6Only.Value;
            }
            else
            {
                IPAddress[] addrs;
                try
                {
                    addrs = Dns.GetHostAddresses("www.baidu.com");
                }
                catch {
                    _isIpv6Only = false;
                    return _isIpv6Only.Value;
                }

                if (addrs == null || addrs.Length == 0)
                {
                    _isIpv6Only = false;
                }
                else
                {
                    if (addrs[0].AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        _isIpv6Only = true;
                    }
                    else
                    {
                        _isIpv6Only = false;
                    }
                }
                return _isIpv6Only.Value;
            }
        }
    }

    public static AddressFamily addressFamily {
        get {
            if (IsIpv6Only)
            {
                return AddressFamily.InterNetworkV6;
            }
            else
            {
                return AddressFamily.InterNetwork;
            }
        }
    }
}
