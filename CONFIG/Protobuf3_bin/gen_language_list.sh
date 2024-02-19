#!/bin/zsh


WORKSPACE=/c/Users/x8261/Desktop/LockStep/Config
LUBAN_DLL=Luban/Luban.dll
CONF_ROOT=DataTables
cd $WORKSPACE

dotnet ${LUBAN_DLL} -t all -d text-list \
--conf $CONF_ROOT/luban.conf \
--validationFailAsError \
-x l10n.textProviderFile=Sheet1@$CONF_ROOT/Datas/l10n/Language_ch.xlsx \
-x outputDataDir=$CONF_ROOT/Datas/l10n/Text \
-x l10n.textListFile=language.txt

