set -e
function DEFINE()
{
    VS_PATH="/c/Program Files/Microsoft Visual Studio/2022/Professional/Common7/IDE/devenv.exe"
    CMAKE="/c/Program Files/CMake/bin/cmake.exe"

    WORKSPACE=$(cd $(dirname $0);pwd)
    UnzipFile="luban-2.1.11"
    dirName=".temp"
}


function BuildVSProject()
{
    local logPath="$dirName/build_vs.log"

    rm -rf $logPath
    rm -rf "Luban"
    #vs路径     sln解决方案                 选项               项目 //Project Luban没有则是整个解决方案            log路径
    "$VS_PATH" "$dirName/$UnzipFile/src/Luban.sln" //build "Release"  //out "$logPath"

    echo "####vsbuild done####"

    cp -r "$dirName/$UnzipFile/src/Luban/bin/Release/net7.0" "Luban"
}

function MAIN()
{
    DEFINE

    cd "$WORKSPACE"
    if [ ! -d "$dirName" ]; then
		mkdir "$dirName"
        bz x -o:"$dirName" $UnzipFile.zip
	fi

   BuildVSProject
}

MAIN 