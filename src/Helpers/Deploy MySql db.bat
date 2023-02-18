@ECHO OFF
set ProjectName=BusTour
set ChangeScriptsPath=%~dp0..\BusTour.Data\ChangeScripts
set JsonConfigurationFilePath=%~dp0..\Configs\appsettings.json
set JsonConnectionStringPath="DbConfig.ConnectionString"
set DbType="MySqlServer"

echo *******************
echo Deploy %ProjectName% (%ChangeScriptsPath%)
echo *******************

"%~dp0../../tools/DBMigrator/DBMigratorConsole.exe" -s -jcf="%JsonConfigurationFilePath%" -jscp="%JsonConnectionStringPath%" -dbType="%DbType%" -f="%ChangeScriptsPath%"

IF %ERRORLEVEL% == 0 GOTO EndSuccess
IF NOT %ERRORLEVEL% == 0 GOTO EndError

:EndError
pause
exit %ERRORLEVEL%

:EndSuccess
echo .