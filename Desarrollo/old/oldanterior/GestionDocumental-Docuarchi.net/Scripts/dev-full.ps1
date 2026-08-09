param(
  [string]$Url,
  [ValidateSet("Debug", "Release")]
  [string]$Configuration = "Debug",
  [int]$Retries = 30,
  [int]$DelaySeconds = 2,
  [switch]$SkipBuild,
  [switch]$OpenBrowser,
  [string]$MSBuildPath
)

$ErrorActionPreference = "Stop"

function Write-Step {
  param([string]$Message)
  Write-Host ""
  Write-Host "==> $Message"
}

function Fail {
  param([string]$Message)
  throw $Message
}

function Assert-Path {
  param(
    [string]$Path,
    [string]$Description
  )

  if (-not (Test-Path $Path)) {
    Fail "No se encontro $Description en '$Path'."
  }
}

function Resolve-MSBuild {
  param([string]$ExplicitPath)

  if ($ExplicitPath) {
    if (Test-Path $ExplicitPath) {
      return (Resolve-Path $ExplicitPath).Path
    }

    Fail "El MSBuildPath indicado no existe: '$ExplicitPath'."
  }

  $fromPath = Get-Command MSBuild.exe -ErrorAction SilentlyContinue
  if ($fromPath) {
    return $fromPath.Source
  }

  $vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
  if (Test-Path $vswhere) {
    $found = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
    if ($found) {
      return $found
    }
  }

  Fail "No se encontro MSBuild.exe. Instala Visual Studio Build Tools o agrega MSBuild al PATH."
}

function Get-IISUrlFromProject {
  param([string]$ProjectFile)

  $xml = New-Object System.Xml.XmlDocument
  $xml.Load($ProjectFile)

  $ns = New-Object System.Xml.XmlNamespaceManager($xml.NameTable)
  $ns.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003")

  $node = $xml.SelectSingleNode("//msb:IISUrl", $ns)
  if ($node -and -not [string]::IsNullOrWhiteSpace($node.InnerText)) {
    return $node.InnerText.Trim()
  }

  return "https://localhost/GestionDocumental-Docuarchi.net"
}

function Show-EndpointDiagnostics {
  param(
    [string]$EndpointUrl,
    [object]$LastError
  )

  Write-Host ""
  Write-Host "No fue posible confirmar que la aplicacion responda en:"
  Write-Host "  $EndpointUrl"
  Write-Host ""
  Write-Host "Causas probables:"
  Write-Host "  - IIS no tiene creada la aplicacion virtual 'GestionDocumental-Docuarchi.net'."
  Write-Host "  - El binding HTTPS o el certificado local de IIS no esta configurado."
  Write-Host "  - El Application Pool no usa .NET CLR v4.0 o no esta iniciado."
  Write-Host "  - Falta restaurar paquetes o copiar ensamblados al directorio bin."
  Write-Host "  - El DSN ODBC 'MembershipUsers' o MySQL local de Web.config no esta disponible."

  if ($LastError) {
    Write-Host ""
    Write-Host "Detalle:"
    Write-Host "  $($LastError.Exception.Message)"
  }
}

$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$projectFile = Join-Path $projectRoot "GestionDocumental-Docuarchi.net.vbproj"
$solutionFile = Join-Path $projectRoot "..\GestionDocumental-Docuarchi.net.sln"
$webConfig = Join-Path $projectRoot "Web.config"
$packagesConfig = Join-Path $projectRoot "packages.config"
$packagesRoot = Join-Path $projectRoot "..\packages"

Set-Location $projectRoot

Write-Step "Validando estructura del proyecto"
Assert-Path $projectFile "el proyecto VB.NET"
Assert-Path $webConfig "Web.config"
Assert-Path $packagesConfig "packages.config"

if (-not (Test-Path $packagesRoot)) {
  Write-Warning "No se encontro la carpeta de paquetes esperada: '$packagesRoot'. Puede requerirse restauracion NuGet."
}

if (-not $Url) {
  $Url = Get-IISUrlFromProject $projectFile
}

Write-Host "Proyecto: $projectFile"
Write-Host "URL local: $Url"

Write-Step "Validando IIS"
$iisService = Get-Service W3SVC -ErrorAction SilentlyContinue
if (-not $iisService) {
  Fail "No se encontro el servicio W3SVC. Habilita IIS antes de ejecutar la aplicacion legacy."
}

if ($iisService.Status -ne "Running") {
  Fail "El servicio W3SVC existe pero esta en estado '$($iisService.Status)'. Inicia IIS y vuelve a intentar."
}

Write-Host "IIS/W3SVC: Running"

if (-not $SkipBuild) {
  Write-Step "Compilando aplicacion"
  $resolvedMSBuild = Resolve-MSBuild $MSBuildPath
  $buildTarget = $projectFile

  if (Test-Path $solutionFile) {
    $buildTarget = (Resolve-Path $solutionFile).Path
  }

  Write-Host "MSBuild: $resolvedMSBuild"
  Write-Host "Target: $buildTarget"

  & $resolvedMSBuild $buildTarget /m /t:Build "/p:Configuration=$Configuration" "/p:Platform=Any CPU" /v:m
  if ($LASTEXITCODE -ne 0) {
    Fail "MSBuild fallo con codigo $LASTEXITCODE. Revisa el resumen anterior para errores de compilacion o referencias faltantes."
  }

  Write-Host "Build completado correctamente."
} else {
  Write-Step "Build omitido"
  Write-Host "Se uso -SkipBuild; se validara solo IIS y el endpoint."
}

Write-Step "Validando endpoint local"
$ready = $false
$lastError = $null

for ($attempt = 1; $attempt -le $Retries; $attempt++) {
  try {
    $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
    Write-Host "HTTP $($response.StatusCode): $Url"
    $ready = $true
    break
  } catch {
    $lastError = $_
    Write-Host "Esperando aplicacion... ($attempt/$Retries)"
    Start-Sleep -Seconds $DelaySeconds
  }
}

if (-not $ready) {
  Show-EndpointDiagnostics -EndpointUrl $Url -LastError $lastError
  exit 1
}

Write-Host ""
Write-Host "Entorno legacy listo:"
Write-Host "  $Url"

if ($OpenBrowser) {
  Start-Process $Url
}
