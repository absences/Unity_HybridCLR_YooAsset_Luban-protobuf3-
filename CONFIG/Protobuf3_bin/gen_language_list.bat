set WORKSPACE=..

set LUBAN_DLL=%WORKSPACE%\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\DataTables

dotnet %LUBAN_DLL% -t all -d text-list ^
--conf %CONF_ROOT%\luban.conf ^
--validationFailAsError ^
-x l10n.textProviderFile=Sheet1@%CONF_ROOT%\Datas\l10n\Language_ch.xlsx ^
-x outputDataDir=%CONF_ROOT%\Datas\l10n\Text ^
-x l10n.textListFile=language.txt

pause
@REM todo 将language.txt文本先读取后根据，新增的放后面