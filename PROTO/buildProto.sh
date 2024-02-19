set -e
function DEFINE()
{
    VS_PATH="/c/Program Files/Microsoft Visual Studio/2022/Professional/Common7/IDE/devenv.exe"
    CMAKE="/c/Program Files/CMake/bin/cmake.exe"
    WORKSPACE=$(cd $(dirname $0);pwd)
    protobuf="protobuf-25.3"
    PbFor="csharp_lite"
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
}
#入口
MAIN 