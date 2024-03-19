using GameFramework.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace UnityGameFramework.Editor
{

    [CustomEditor(typeof(NetworkComponent))]
    public class NetworkComponentInspector : GameFrameworkInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying)
            {
                EditorGUILayout.HelpBox("Available during runtime only.", MessageType.Info);
                return;
            }

            NetworkComponent t = (NetworkComponent)target;

            if (IsPrefabInHierarchy(t.gameObject))
            {
                EditorGUILayout.LabelField("Network Channel Count", t.NetworkChannelCount.ToString());

                INetworkChannel[] networkChannels = t.GetAllNetworkChannels();
                foreach (INetworkChannel networkChannel in networkChannels)
                {
                    DrawNetworkChannel(networkChannel);
                }
            }

            Repaint();
        }

        private void OnEnable()
        {
        }

        private void DrawNetworkChannel(INetworkChannel networkChannel)
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.LabelField(networkChannel.Name, networkChannel.Connected ? "Connected" : "Disconnected");
                EditorGUILayout.LabelField("Service Type", networkChannel.ServiceType.ToString());
                EditorGUILayout.LabelField("Address Family", networkChannel.AddressFamily.ToString());
                // EditorGUILayout.LabelField("Local Address",
                //     networkChannel.Connected ? networkChannel.Socket.LocalEndPoint.ToString() : "Unavailable");

                // EditorGUILayout.LabelField("Remote Address",
                //    networkChannel.Connected ? networkChannel.Socket.RemoteEndPoint.ToString() : "Unavailable");
                // EditorGUILayout.LabelField("Send Packet",
                //    Utility.Text.Format("{0} / {1}", networkChannel.SendPacketCount.ToString(), networkChannel.SentPacketCount.ToString()));
                EditorGUILayout.LabelField("Ping",
                  string.Format("{0} ms", networkChannel.Ping.ToString("F2")));
                EditorGUILayout.LabelField("Miss Heart Beat Count",
                    networkChannel.MissHeartBeatCount.ToString());
                EditorGUILayout.LabelField("Heart Beat",
                    string.Format("{0} / {1}", networkChannel.HeartBeatElapseSeconds.ToString("F2"), networkChannel.HeartBeatInterval.ToString("F2")));
                EditorGUI.BeginDisabledGroup(!networkChannel.Connected);
                {
                    if (GUILayout.Button("Disconnect"))
                    {
                        networkChannel.Close();
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Separator();
        }
    }
}