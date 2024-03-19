using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Google.Protobuf;

namespace GameFramework.Network
{
    /// <summary>
    /// 网络频道辅助器接口。
    /// </summary>
    public interface INetworkChannelHelper 
    {

        /// <summary>
        /// 初始化网络频道辅助器。
        /// </summary>
        /// <param name="networkChannel">网络频道。</param>
        void Initialize(INetworkChannel networkChannel);

        /// <summary>
        /// 关闭并清理网络频道辅助器。
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 发送心跳消息包。
        /// </summary>
        /// <returns>是否发送心跳消息包成功。</returns>
        bool SendHeartBeat();

        /// <summary>
        /// 反序列化消息包。
        /// </summary>
        /// <param name="packetHeader">消息包头。</param>
        /// <param name="source">要反序列化的来源流。</param>
        /// <param name="customErrorData">用户自定义错误数据。</param>
        /// <returns>反序列化后的消息包。</returns>
        IMessage DeserializePacket(byte[] array, int offset, int count, out object customErrorData);

    }
}