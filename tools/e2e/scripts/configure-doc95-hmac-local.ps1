[CmdletBinding()]
param(
    [switch]$Rotate
)

$ErrorActionPreference = 'Stop'
$repoRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..'))
$target = [IO.Path]::GetFullPath((Join-Path $repoRoot 'AppSettings.local.config'))
if (-not $target.StartsWith($repoRoot, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'DOC95_HMAC_TARGET_INVALID'
}

if ((Test-Path -LiteralPath $target) -and -not $Rotate) {
    Write-Output 'DOC95_HMAC_LOCAL_CONFIGURED=true'
    Write-Output 'DOC95_HMAC_LOCAL_CHANGED=false'
    exit 0
}

$keyBytes = New-Object byte[] 32
$random = [Security.Cryptography.RandomNumberGenerator]::Create()
try {
    $random.GetBytes($keyBytes)
} finally {
    $random.Dispose()
}

$keyId = 'local-doc95-v1'
$document = New-Object Xml.XmlDocument
$declaration = $document.CreateXmlDeclaration('1.0', 'utf-8', $null)
[void]$document.AppendChild($declaration)
$root = $document.CreateElement('appSettings')
[void]$document.AppendChild($root)

$active = $document.CreateElement('add')
$active.SetAttribute('key', 'LoginSecondFactorHmacActiveKeyId')
$active.SetAttribute('value', $keyId)
[void]$root.AppendChild($active)

$key = $document.CreateElement('add')
$key.SetAttribute('key', 'LoginSecondFactorHmacKey.' + $keyId)
$key.SetAttribute('value', [Convert]::ToBase64String($keyBytes))
[void]$root.AppendChild($key)

$settings = New-Object Xml.XmlWriterSettings
$settings.Encoding = New-Object Text.UTF8Encoding($false)
$settings.Indent = $true
$writer = [Xml.XmlWriter]::Create($target, $settings)
try {
    $document.Save($writer)
} finally {
    $writer.Dispose()
    [Array]::Clear($keyBytes, 0, $keyBytes.Length)
}

Write-Output 'DOC95_HMAC_LOCAL_CONFIGURED=true'
Write-Output 'DOC95_HMAC_LOCAL_CHANGED=true'
