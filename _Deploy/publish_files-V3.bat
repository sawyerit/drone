@ECHO OFF
SETLOCAL
CLS

SET Environment=Production-V3

ECHO ********************************************************************
ECHO Copying test bin to deploy path
ECHO ********************************************************************

:: WindowsService
for /f %%f in (ServiceList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug" "..\..\Build\%Environment%\WindowsService" %%f
ROBOCOPY "..\Drone.Service\bin\Debug" "..\..\Build\%Environment%\WindowsService" Drone.Service.exe.config

:: SocialMedia
for /f %%f in (SocialMediaList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug" "..\..\Build\%Environment%\WindowsService\SocialMedia" %%f
for /f %%f in (SocialMediaXMLList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug\XML" "..\..\Build\%Environment%\WindowsService\SocialMedia\XML" %%f

::Crunchbase
for /f %%f in (CrunchbaseList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug" "..\..\Build\%Environment%\WindowsService\Crunchbase" %%f
for /f %%f in (CrunchbaseXMLList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug\XML" "..\..\Build\%Environment%\WindowsService\Crunchbase\XML" %%f

::MarketShare
for /f %%f in (MarketShareList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug" "..\..\Build\%Environment%\WindowsService\MarketShare" %%f
for /f %%f in (MarketShareXMLList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug\XML" "..\..\Build\%Environment%\WindowsService\MarketShare\XML" %%f

::QueueProcessor
for /f %%f in (QueueList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug" "..\..\Build\%Environment%\WindowsService\QueueProcessor" %%f
for /f %%f in (QueueXMLList.txt) do ROBOCOPY "..\Drone.Test\bin\Debug\XML" "..\..\Build\%Environment%\WindowsService\QueueProcessor\XML" %%f

GOTO PAUSE


:PAUSE
ECHO. Press Any Key to Continue...
PAUSE>NUL
GOTO END

:END
ENDLOCAL