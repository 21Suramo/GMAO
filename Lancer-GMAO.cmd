@echo off
title GMAO Datex-Ohmeda - Lanceur MEDICANA
cd /d "%~dp0"

echo ============================================
echo   GMAO Datex-Ohmeda - MEDICANA
echo ============================================
echo.

echo [1/2] Demarrage du serveur de notifications...
start "GMAO - Serveur Notifications" /D "%~dp0servers\notification-server" cmd /k node server.js

ping -n 3 127.0.0.1 >nul

echo [2/2] Demarrage de l'application...
rem Build Debug (toujours la plus a jour pendant le developpement) ; sinon build publiee.
set "APP=%~dp0src\GMAO.Presentation.Wpf\bin\Debug\net10.0-windows\GMAO.Presentation.Wpf.exe"
if not exist "%APP%" set "APP=%~dp0publish\GMAO-app\GMAO.Presentation.Wpf.exe"

if not exist "%APP%" (
  echo.
  echo ERREUR : application introuvable. Compilez d'abord avec build.ps1 ou dotnet build.
  pause
  exit /b 1
)

start "" "%APP%"
echo.
echo Application lancee. Connexion : admin / Admin@123
ping -n 3 127.0.0.1 >nul
exit
