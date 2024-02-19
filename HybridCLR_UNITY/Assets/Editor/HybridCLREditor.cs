using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
using System.IO;
using UnityEditor;
using UnityEngine;

public class HybridCLREditor 
{
    [MenuItem("Tools/CompileDll")]
    public static void CompileDll()
    {

        CompileDllCommand.CompileDll(EditorUserBuildSettings.activeBuildTarget);

        PrebuildCommand.GenerateAll();

        CopyAOTAssembliesTo();

        CopyHotUpdateAssembliesTo();

    }
    static void CopyAOTAssembliesTo()
    {
        var target = EditorUserBuildSettings.activeBuildTarget;
        string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
        string aotAssembliesDstDir = Application.dataPath + "/../Buildin/Codes";

        foreach (var dll in SettingsUtil.AOTAssemblyNames)//patch aot 补充元数据文件
        {
            string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
            if (!File.Exists(srcDllPath))
            {
                 continue;
            }
            string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.bytes";
            File.Copy(srcDllPath, dllBytesPath, true);
            Debug.Log($"[CopyAOTAssembliesTo] copy AOT dll {srcDllPath} -> {dllBytesPath}");
        }
    }
    static void CopyHotUpdateAssembliesTo()//assembly 
    {
        var target = EditorUserBuildSettings.activeBuildTarget;

        string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
        string hotfixAssembliesDstDir = Application.dataPath + "/../Buildin/Codes";

        foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
        {
            string dllPath = $"{hotfixDllSrcDir}/{dll}";
            string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll.Replace(".dll", "")}.bytes";
            File.Copy(dllPath, dllBytesPath, true);
            Debug.Log($"[CopyHotUpdateAssembliesTo] copy hotfix dll {dllPath} -> {dllBytesPath}");
        }
    }
    //static void CopyHotFixMain()
    //{
    //    var ProjectDir = Directory.GetParent(Application.dataPath).ToString();
    //    var workDir = $"{ProjectDir}/HotFixMain";

    //    BashUtil.RunCommand(workDir,)
    //}
}
