using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.Network
{
    /// <summary>
    /// 网络服务类型。
    /// </summary>
    public enum ServiceType : byte
    {
        /// <summary>
        /// TCP 网络服务。
        /// </summary>
        Tcp = 0,
        /// <summary>
        /// 快速可靠udp协议
        /// </summary>
        Kcp
    }
}