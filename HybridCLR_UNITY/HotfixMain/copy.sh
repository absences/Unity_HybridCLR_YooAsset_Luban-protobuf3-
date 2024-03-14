
UNITY="/d/Program Files/Unity 2021.3.35f1"


ENGINE="$UNITY/Editor/Data/Managed/UnityEngine"


CUR_DP=$(cd $(dirname $0); pwd)

cp -r "$ENGINE/UnityEngine.AudioModule.dll" "$CUR_DP"
cp -r "$ENGINE/UnityEngine.CoreModule.dll" "$CUR_DP"
cp -r "$ENGINE/UnityEngine.InputLegacyModule.dll" "$CUR_DP"
cp -r "$ENGINE/UnityEngine.UnityWebRequestModule.dll" "$CUR_DP"
cp -r "$ENGINE/UnityEngine.UnityWebRequestTextureModule.dll" "$CUR_DP"
