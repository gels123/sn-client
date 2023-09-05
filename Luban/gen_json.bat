set WORKSPACE=..

set GEN_CLIENT=%WORKSPACE%\Luban\Tools\Luban\Luban.dll
set CONF_ROOT=%WORKSPACE%\Luban\Configs

dotnet %GEN_CLIENT% ^
    -t client ^
    -c cs-simple-json ^
    -d json  ^
    --schemaPath %CONF_ROOT%\Defines\__root__.xml ^
    -x inputDataDir=%CONF_ROOT%\Datas ^
    -x outputCodeDir=%WORKSPACE%\Assets\Scripts\Configs\Gen ^
    -x outputDataDir=%WORKSPACE%\Assets\StreamingAssets\Json ^
    -x l10n.textProviderFile=*@%WORKSPACE%\Luban\Configs\Datas\l10n\texts.xlsx
        
pause
