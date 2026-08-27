# =====================================================================
#  Build & publication — GMAO Datex-Ohmeda (MEDICANA)
#  Restaure, compile, teste, publie l'app WPF et prépare le serveur Node.
# =====================================================================

$ErrorActionPreference = "Stop"

# .NET hors PATH sur certains postes : on l'ajoute si nécessaire.
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    $env:PATH = "C:\Program Files\dotnet;$env:PATH"
}

$racine = $PSScriptRoot
Set-Location $racine

Write-Host "==> Restauration..." -ForegroundColor Cyan
dotnet restore GMAO.slnx

Write-Host "==> Compilation (Release)..." -ForegroundColor Cyan
dotnet build GMAO.slnx -c Release --nologo

Write-Host "==> Tests unitaires..." -ForegroundColor Cyan
dotnet test tests/GMAO.Tests.Unit -c Release --nologo

Write-Host "==> Publication de l'application WPF..." -ForegroundColor Cyan
dotnet publish src/GMAO.Presentation.Wpf -c Release -r win-x64 --self-contained false -o publish/GMAO-app --nologo

Write-Host "==> Dépendances du serveur de notifications (npm)..." -ForegroundColor Cyan
Push-Location servers/notification-server
npm install
Pop-Location

Write-Host ""
Write-Host "Build terminée." -ForegroundColor Green
Write-Host "  Application : publish/GMAO-app/GMAO.Presentation.Wpf.exe"
Write-Host "  Serveur     : cd servers/notification-server ; node server.js"
