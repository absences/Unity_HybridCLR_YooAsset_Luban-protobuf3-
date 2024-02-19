#!/bin/zsh
WORKSPACE=/c/Users/x8261/Desktop/LockStep/Config
LUBAN_DLL=../Luban/Luban.dll
CONF_ROOT=../DataTables
PROTOC=$WORKSPACE/Protobuf3_bin/protoc.exe
cd $WORKSPACE/Protobuf3_bin

dotnet ${LUBAN_DLL} -t all -c protobuf3 \
-d protobuf-bin  \
--conf $CONF_ROOT/luban.conf \
-x outputCodeDir=pb_proto \
-x outputDataDir=pb_bin \
-x l10n.textProviderFile=Sheet1@$CONF_ROOT/Datas/l10n/Language_ch.xlsx

$PROTOC --csharp_out=pb_proto pb_proto/schema.proto

cp -r pb_proto/Schema.cs ../../LockStepUnity/Assets/Generated/Config

find pb_bin -name "*.bytes" | while read file
do
    cp -r "$file" "../../LockStepUnity/Assets/Builds/Config"
done