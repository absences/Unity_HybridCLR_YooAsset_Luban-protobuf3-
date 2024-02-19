using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using YooAsset.Editor;

public class AutoBuild
{


    [MenuItem("Tools/BuildWholePack")]
    static void BuildWholePack()
    {
        var version = AddAppVersion();
        SetPackageVersion(version.ToString());
        var scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
        var resource = GameObject.FindObjectOfType<ResourceComponent>();
        resource.AppVersion = version;
        resource.VersionType = "dev";
        resource.PlayMode = YooAsset.EPlayMode.OfflinePlayMode;
        resource.BackUpResourceSourceUrl = "";
        resource.ResourceSourceUrl = "";
        EditorSceneManager.SaveScene(scene);

        // HybridCLR编译代码
        HybridCLREditor.CompileDll();

        //收集shader变体
        //CollectSVC();

        var pkgversion = AddPackageVersion();
        //整包参数
        YooAssetsBuild(pkgversion, EBuildinFileCopyOption.ClearAndCopyAll, null, false, null);



        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;//构建平台
        string savePath = "";
        if (buildTarget == BuildTarget.Android)
        {
            bool aab = false;
            PlayerSettings.Android.bundleVersionCode = version;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "ENABLE_LOG");

            EditorUserBuildSettings.buildAppBundle = aab;
            PlayerSettings.Android.useAPKExpansionFiles = aab;

            savePath = Application.dataPath.Replace("Assets", "AutoBuild")
                + (aab ? "/AutoBuild.aab" : "/AutoBuild.apk");
        }

        else if (buildTarget == BuildTarget.iOS)
        {
            PlayerSettings.iOS.buildNumber = version.ToString();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "ENABLE_LOG");

            savePath = Application.dataPath.Replace("Assets", "AutoBuild") + "/clientIOS";
        }

        BuildPipeline.BuildPlayer(GetBuildScenes(), savePath, EditorUserBuildSettings.activeBuildTarget,
          false ? BuildOptions.Development : BuildOptions.None);
    }
    [MenuItem("Tools/BuildHotUpdatePack")]
    static void BuildHotUpdatePack()
    {
        var version = AddAppVersion();
        SetPackageVersion(version.ToString());
        var scene = EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);
        var resource = GameObject.FindObjectOfType<ResourceComponent>();
        resource.AppVersion = version;
        resource.VersionType = "release";
        resource.PlayMode = YooAsset.EPlayMode.HostPlayMode;
        resource.BackUpResourceSourceUrl = "http://192.168.3.60";
        resource.ResourceSourceUrl = "http://192.168.3.60";
        EditorSceneManager.SaveScene(scene);

        // HybridCLR编译代码
        HybridCLREditor.CompileDll();

        //收集shader变体
        //CollectSVC();

        var pkgversion = AddPackageVersion();
        //热更参数
        YooAssetsBuild(pkgversion, EBuildinFileCopyOption.ClearAndCopyByTags, "prefab", true, new string[] { "config" });

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;//构建平台
        string savePath = "";
        if (buildTarget == BuildTarget.Android)
        {
            bool aab = false;
            PlayerSettings.Android.bundleVersionCode = version;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "ENABLE_LOG");

            EditorUserBuildSettings.buildAppBundle = aab;
            PlayerSettings.Android.useAPKExpansionFiles = aab;

            savePath = Application.dataPath.Replace("Assets", "AutoBuild")
                + (aab ? "/AutoBuild.aab" : "/AutoBuild.apk");
        }

        else if (buildTarget == BuildTarget.iOS)
        {
            PlayerSettings.iOS.buildNumber = version.ToString();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "ENABLE_LOG");

            savePath = Application.dataPath.Replace("Assets", "AutoBuild") + "/clientIOS";
        }

        BuildPipeline.BuildPlayer(GetBuildScenes(), savePath, EditorUserBuildSettings.activeBuildTarget,
          false ? BuildOptions.Development : BuildOptions.None);
    }
    [MenuItem("Tools/BuildHotUpdate")]
    static void BuildHotUpdate()
    {
        HybridCLREditor.CompileDll();

        //收集shader变体
        //CollectSVC();

        var pkgversion = AddPackageVersion();
        //热更参数
        //热更包也可以使用zip
        //YooAssetsBuild(pkgversion, ECopyBuildinFileOption.None, null, true, new string[] { "config" });
        YooAssetsBuild(pkgversion);
    }

    private const string DefaultPackage = "DefaultPackage";
    static int AddPackageVersion()
    {
        var buildInfo = AssetDatabase.LoadAssetAtPath<BuildInfo>
           ("Assets/Editor/PackageBuildInfo.asset");
        var version = ++buildInfo.PackageVersion;

        EditorUtility.SetDirty(buildInfo);
        AssetDatabase.SaveAssetIfDirty(buildInfo);

        return version;
    }
    static int AddAppVersion()
    {
        var buildInfo = AssetDatabase.LoadAssetAtPath<BuildInfo>
           ("Assets/Editor/PackageBuildInfo.asset");
        var version = ++buildInfo.AppVersion;

        EditorUtility.SetDirty(buildInfo);
        AssetDatabase.SaveAssetIfDirty(buildInfo);

        return version;
    }
    /// <summary>
    /// 打包资源
    /// </summary>
    /// <param name="packageVersion">资源版本</param>
    /// <param name="eCopyBuildinFileOption">拷贝策略</param>
    /// <param name="buildinFileTags">拷贝tag</param>
    /// <param name="zipBuildinDelivery">压缩</param>
    /// <param name="zipTags">压缩tag</param>
    static void YooAssetsBuild(int packageVersion,
        EBuildinFileCopyOption eCopyBuildinFileOption = EBuildinFileCopyOption.None,
         string buildinFileTags = default,
        bool zipBuildinDelivery = false, string[] zipTags = default)
    {
        EBuildPipeline eBuildPipeline = EBuildPipeline.ScriptableBuildPipeline;//构建管线类型

        EBuildMode eBuildMode = EBuildMode.IncrementalBuild;//资源包流水线的构建模式

        ECompressOption eCompressOption = ECompressOption.LZ4;//AssetBundle压缩选项  LZ4读取效率比LZMA高  LZ4压缩率不及LZMA

        EFileNameStyle eOutputNameStyle = EFileNameStyle.HashName;//输出文件名称的样式

        // 打整包，全部资源拷贝至StreamingAssets
        // 热更包，一下必须的资源拷贝至StreamingAssets，剩下的可以压缩，然后放至CDN
        // ECopyBuildinFileOption eCopyBuildinFileOption = ECopyBuildinFileOption.ClearAndCopyByTags;//首包资源文件的拷贝方式

        BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;//构建平台

        //BuildParameters buildParameters = new BuildParameters
        //{
        //    StreamingAssetsRoot = AssetBundleBuilderHelper.GetDefaultStreamingAssetsRoot(),
        //    BuildOutputRoot = AssetBundleBuilderHelper.GetDefaultBuildOutputRoot(),
        //    BuildTarget = buildTarget,
        //    BuildPipeline = eBuildPipeline,
        //    BuildMode = eBuildMode,
        //    PackageName = DefaultPackage,
        //    PackageVersion = packageVersion.ToString(),
        //    VerifyBuildingResult = true,//验证构建结果
        //    SharedPackRule = new ZeroRedundancySharedPackRule(),//零冗余的共享资源打包规则
        //    //EncryptionServices = new FileOffsetEncryption(),//资源的加密方法
        //    CompressOption = eCompressOption,
        //    OutputNameStyle = eOutputNameStyle,
        //    CopyBuildinFileOption = eCopyBuildinFileOption,
        //    CopyBuildinFileTags = buildinFileTags,
        //    ZipBuildinDelivery = zipBuildinDelivery,//压缩分发
        //    ZipTags = zipTags//根据tag压缩
        //};

        //if (eBuildPipeline == EBuildPipeline.ScriptableBuildPipeline)
        //{
        //    buildParameters.SBPParameters = new BuildParameters.SBPBuildParameters();
        //    buildParameters.SBPParameters.WriteLinkXML = true;
        //}

        //var builder = new AssetBundleBuilder();
        //var buildResult = builder.Run(buildParameters);
    }


    public static void CollectSVC()
    {
        string savePath = ShaderVariantCollectorSetting.GeFileSavePath(ResourceManager.assetPackage);
        System.Action completedCallback = () =>
        {
            ShaderVariantCollection collection =
                AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(savePath);
            if (collection != null)
            {
                Debug.Log($"ShaderCount : {collection.shaderCount}");
                Debug.Log($"VariantCount : {collection.variantCount}");
            }
            else
            {
                throw new Exception("Failed to Collect shader Variants.");
            }

            EditorTools.CloseUnityGameWindow();
            EditorApplication.Exit(0);
        };
        ShaderVariantCollector.Run(savePath, ResourceManager.assetPackage, 1000, completedCallback);
    }

    const int spt = 3;
    static void SetPackageVersion(string bundleVersion)
    {
        string a = "0", b = "0", c = bundleVersion;
        int length = bundleVersion.Length;
        if (length > spt)
        {
            c = bundleVersion.Remove(0, length - spt);
            b = bundleVersion.Remove(length - spt, spt);
        }
        if (length > 2 * spt)
        {
            a = bundleVersion.Remove(length - spt * 2, spt * 2);
            b = bundleVersion.Remove(0, length - spt * 2).Remove(spt, spt);
        }
        PlayerSettings.bundleVersion = string.Format("{0}.{1}.{2:D3}", a, b, int.Parse(c));

    }
    static string[] GetBuildScenes()
    {
        List<string> pathList = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                //Assets/Scene/Main.unity
                pathList.Add(scene.path);
            }
        }
        return pathList.ToArray();
    }
}
