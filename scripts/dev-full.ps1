$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..")
$apiUrl = "http://127.0.0.1:5055"
$healthUrl = "$apiUrl/api/accout/SolicitaEstructuraEmpresa?page=1&pageSize=1"
$viteCmd = Join-Path $root "node_modules\.bin\vite.cmd"

Set-Location $root

Write-Host "Starting DocuArchi API on $apiUrl ..."
$apiJob = Start-Job -Name "DocuArchiApiDev" -ArgumentList $root -ScriptBlock {
  param($workingDirectory)
  Set-Location $workingDirectory

  $env:ASPNETCORE_ENVIRONMENT = "Development"
  $env:ASPNETCORE_URLS = "http://127.0.0.1:5055"
  $env:Logging__EventLog__LogLevel__Default = "None"

  dotnet .\DocuArchi.Api.dll
}

try {
  $ready = $false
  for ($attempt = 1; $attempt -le 30; $attempt++) {
    Start-Sleep -Seconds 1

    if ($apiJob.State -ne "Running") {
      Receive-Job $apiJob
      throw "DocuArchi API stopped during startup."
    }

    try {
      Invoke-WebRequest -Uri $healthUrl -UseBasicParsing -TimeoutSec 2 | Out-Null
      $ready = $true
      break
    } catch {
      Write-Host "Waiting for API... ($attempt/30)"
    }
  }

  if (-not $ready) {
    Receive-Job $apiJob -Keep
    throw "DocuArchi API did not respond on $healthUrl"
  }

  Write-Host "DocuArchi API is ready."
  Write-Host "Starting Vite..."
  & $viteCmd
} finally {
  if ($apiJob) {
    Stop-Job $apiJob -ErrorAction SilentlyContinue
    Remove-Job $apiJob -Force -ErrorAction SilentlyContinue
  }
}
