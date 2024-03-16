set PROTOC=protoc

%PROTOC% --csharp_out=./ Common.proto
%PROTOC% --csharp_out=./ RespFullyLst.proto
%PROTOC% --csharp_out=./ RqstFully.proto

xcopy Common.cs ..\..\HybridCLR_UNITY\Assets\Generated\netGen /y
xcopy RespFullyLst.cs ..\..\HybridCLR_UNITY\Assets\Generated\netGen /y
xcopy RqstFully.cs ..\..\HybridCLR_UNITY\Assets\Generated\netGen /y

pause

