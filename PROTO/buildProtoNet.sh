set -e
function DEFINE()
{
VS_PATH="/c/Program Files/Microsoft Visual Studio/2022/Professional/Common7/IDE/devenv.exe"
    CMAKE="/c/Program Files/CMake/bin/cmake.exe"
    WORKSPACE=$(cd $(dirname $0);pwd)
    protobuf="protobuf-26.0"
    PbFor="csharp_net"
    dirName=".temp"
    PROTOBUILD="$dirName/Build/Release/protoc.exe"
}
function UnzipMakeVs()
{
    bz x -o:"$dirName" $protobuf.zip

    bz x -o:"$dirName/$protobuf/third_party/abseil-cpp" "abseil-cpp.zip"

    cp "CMakeLists.txt" "$dirName/$protobuf"

    CMAKE -S "$dirName/$protobuf" -B "$dirName/Build"

    echo "####cmake done####"
}
function CopyCSharpSource()
{
    local dir="$dirName/$protobuf/src/google/protobuf/compiler/csharp"

    rm -rf "$dir"

    cp -r "$PbFor" "$dir"

    echo "####copy done####"
}
function BuildVSProject()
{
    local logPath="$dirName/build_vs.log"

    rm -rf $logPath
    #vs路径     sln解决方案                 选项               项目             log路径
    "$VS_PATH" "$dirName/Build/protobuf.sln" //build "Release" //Project protoc //out "$logPath"

    echo "####vsbuild done####"
}
function GenNetProto()
{
    find net -name "*.proto" | while read file
    do
        name=$(ls "$file" | cut -d. -f1) #获取文件名称 除了后缀
        "$PROTOBUILD"  --proto_path=./net --csharp_out=./net "$file"

        cp "$name.cs" "../HybridCLR_UNITY/Assets/Generated/netGen"
      #  mv "$name.cs" "../Server/Server/Gen/"
    done

    #拷贝文件夹下所有文件到目标目录下
    #cp -r "Gen" "../LockStepDemo/Assets/Scripts"
    echo "#####gen done####"
}

function MAIN()
{
    DEFINE

    cd "$WORKSPACE"
    if [ ! -d "$dirName" ]; then
		mkdir "$dirName"
        UnzipMakeVs
	fi

    CopyCSharpSource

    BuildVSProject

    GenNetProto
}
#入口
MAIN 