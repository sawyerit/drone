@ECHO OFF
SETLOCAL
CLS

set /P usr="Enter service account user: "
set /P pwd="Enter service account password: "


ECHO.
ECHO **************************************
ECHO Deleting Crunchbase Service
ECHO **************************************
sc delete "Drone.Crunchbase.Service"
ECHO --------------------------------------
ECHO.
ECHO.


echo.
ECHO **************************************
echo Deleting SocialMedia Service
ECHO **************************************
sc delete "Drone.SocialMedia.Service"
ECHO --------------------------------------
ECHO.
ECHO.

echo.
ECHO **************************************
echo Deleting MarketShare Service
ECHO **************************************
sc delete "Drone.MarketShare.Service"
ECHO --------------------------------------
ECHO.
ECHO.

echo.
ECHO **************************************
echo Deleting QueueProcessor Service
ECHO **************************************
sc delete "Drone.QueueProcessor.Service"
ECHO --------------------------------------
ECHO.
ECHO.




ECHO.
ECHO **************************************
ECHO Creating Crunchbase Service
ECHO **************************************
sc create "Drone.Crunchbase.Service" binPath= "\"D:\bizintel-data.int.godaddy.com\Drone\Drone.Service.exe\" Crunchbase" DisplayName= "Drone Crunchbase" obj= "%usr%@dc1.corp.gd" password= "%pwd%"
ECHO Setting Crunchbase Service description
ECHO --------------------------------------
sc description "Drone.Crunchbase.Service" "Crunchbase Service"
ECHO.
ECHO.


echo.
ECHO **************************************
echo Creating SocialMedia Service
ECHO **************************************
sc create "Drone.SocialMedia.Service" binPath= "\"D:\bizintel-data.int.godaddy.com\Drone\Drone.Service.exe\" SocialMedia" DisplayName= "Drone SocialMedia" obj= "%usr%@dc1.corp.gd" password= "%pwd%"
echo Setting SocialMedia Service description
ECHO --------------------------------------
sc description "Drone.SocialMedia.Service" "SocialMedia Service"
ECHO.
ECHO.

echo.
ECHO **************************************
echo Creating MarketShare Service
ECHO **************************************
sc create "Drone.MarketShare.Service" binPath= "\"D:\bizintel-data.int.godaddy.com\Drone\Drone.Service.exe\" MarketShare" DisplayName= "Drone MarketShare" obj= "%usr%@dc1.corp.gd" password= "%pwd%"
echo Setting MarketShare Service description
ECHO --------------------------------------
sc description "Drone.MarketShare.Service" "MarketShare Service"
ECHO.
ECHO.

echo.
ECHO **************************************
echo Creating QueueProcessor Service
ECHO **************************************
sc create "Drone.QueueProcessor.Service" binPath= "\"D:\bizintel-data.int.godaddy.com\Drone\Drone.Service.exe\" QueueProcessor" DisplayName= "Drone QueueProcessor" obj= "%usr%@dc1.corp.gd" password= "%pwd%"
echo Setting QueueProcessor Service description
ECHO --------------------------------------
sc description "Drone.QueueProcessor.Service" "QueueProcessor Service"
ECHO.
ECHO.


pause