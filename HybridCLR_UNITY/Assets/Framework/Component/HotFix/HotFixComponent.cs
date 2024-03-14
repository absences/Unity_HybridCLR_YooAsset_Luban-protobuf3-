using Cysharp.Threading.Tasks;
using GameFramework;
using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class HotFixComponent : BaseGameComponent
{

    readonly List<string> aotMetaAssemblyFiles = new List<string>()
    {
        "mscorlib",
        "System",
        "System.Core",
    };

    private bool Inited = false;
    public async UniTask Init()
    {
        HomologousImageMode mode = HomologousImageMode.SuperSet;

        var resource = GameEnter.Resource;
        foreach (var aotDllName in aotMetaAssemblyFiles)
        {
            using var handle = resource.LoadRawFileAsync(aotDllName);
            await handle.ToUniTask(this);
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(handle.GetRawFileData(), mode);
            //Log.Info(string.Format("LoadMetadata{0} {1}", aotDllName, err));
        }

        {
            using var handle = resource.LoadRawFileAsync("MonoHot");//mono 
            await handle.ToUniTask(this);
            var assembly = Assembly.Load(handle.GetRawFileData());
            s_Assemblies[0] = assembly;
            gameObject.AddComponent(assembly.GetType("MonoHotEnter"));
        }
        {
            using var handle = resource.LoadRawFileAsync("ildll");//hotfix main
            await handle.ToUniTask(this);
            Assembly assembly = Assembly.Load(handle.GetRawFileData());
            s_Assemblies[1] = assembly;

            Type entryType = assembly.GetType("HotfixMain.HotFixActivity");
            MethodInfo method = entryType.GetMethod("Init");
            method.Invoke(null, null);//

            var updatemethod = entryType.GetMethod("Update");
            updateAction = Delegate.CreateDelegate(typeof(Action<float, float>), null, updatemethod) as Action<float, float>;


            var onDestroy = entryType.GetMethod("ShutDown");
            onDestroyAction = Delegate.CreateDelegate(typeof(Action), null, onDestroy) as Action;

            //Type netparseentryType = assembly.GetType("HotfixMain.ILNetHelper");
            //var netparse = netparseentryType.GetMethod("HandleMsg");

            //netParseHandle = (Action<int, CodedInputStream>)Delegate.CreateDelegate(typeof(Action<int, CodedInputStream>), null, netparse);
        }

        Inited = true;
    }
    private Action<float, float> updateAction;
    private Action onDestroyAction;

    private readonly Dictionary<string, Type> s_CachedTypes = new Dictionary<string, Type>(StringComparer.Ordinal);
    private readonly Assembly[] s_Assemblies = new Assembly[2];

    public Type GetType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            throw new GameFrameworkException("Type name is invalid.");
        }
        if (!Inited)
            return null;
        if (s_CachedTypes.Count == 0)
            foreach (var assembly in s_Assemblies)
            {
                foreach (var _type in assembly.GetTypes())
                {
                    s_CachedTypes.Add(_type.Name, _type);
                }
            }
        foreach (var typePair in s_CachedTypes)
        {
            if (typePair.Key.EndsWith(typeName))
            {
                return typePair.Value;  
            }
        }
        return null;
    }

    private void Update()
    {
        if (Inited)
        {
            updateAction.Invoke(Time.deltaTime, Time.realtimeSinceStartup);
        }
    }

    private void OnDestroy()
    {
        if (Inited)
        {
            onDestroyAction();
        }
    }
}
