set WORKSPACE=..\..

set GEN_CLIENT=%WORKSPACE%\Luban\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\Luban\Configs

dotnet %GEN_CLIENT% ^
    -t all ^
    -c cs-simple-json ^
    -d json  ^
    --schemaPath %CONF_ROOT%\Defines\__root__.xml ^
    -x inputDataDir=%CONF_ROOT%\Datas ^
    -x outputCodeDir=%WORKSPACE%\Scripts\Configs ^
    -x outputDataDir=%WORKSPACE%\StreamingAssets\Json ^
    -x pathValidator.rootDir=D:\workspace2\luban_examples\Projects\Csharp_Unity_bin ^
    -x l10n.textProviderFile=*@%WORKSPACE%\Luban\Configs\Datas\l10n\texts.json

pause