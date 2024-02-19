set WORKSPACE=..

set LUBAN_DLL=%WORKSPACE%\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables

set PROTOC=%WORKSPACE%\Protobuf3_bin\protoc


dotnet %LUBAN_DLL% ^
    -t all ^
    -c protobuf3 ^
    -d protobuf-bin  ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=pb_proto ^
    -x outputDataDir=pb_bin ^
    -x l10n.textProviderFile=Sheet1@%CONF_ROOT%\Datas\l10n\Language_ch.xlsx

%PROTOC% --csharp_out=pb_proto pb_proto/schema.proto

xcopy pb_proto\Schema.cs ..\..\HybridCLR_UNITY\Assets\Generated\Config /y

xcopy "pb_bin\*.bytes" "..\..\HybridCLR_UNITY\Buildin\Config" /s /e /y

pause
