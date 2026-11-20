$ErrorActionPreference='Stop'
$taskRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$binary=Join-Path $taskRoot 'bin/GestionDocumental-Docuarchi.net.dll'
if((Get-FileHash -LiteralPath $binary -Algorithm SHA256).Hash.ToLowerInvariant() -ne '8b27a942458d41aa87d8ddd0df684bf70b7b89ec7702f0607958169a91cc571d'){throw 'DOC120_QA_NATIVE_BINARY_CHANGED'}
[void][Reflection.Assembly]::LoadFrom((Join-Path $taskRoot 'bin/Newtonsoft.Json.dll'))
$assembly=[Reflection.Assembly]::LoadFrom($binary)
$serializer=$assembly.GetType('GestionDocumental_Docuarchi.net.PerfilImportacionJson',$true)
# Synthetic request only. Invokes the pure serializer, never repository/service.
$sample=[ordered]@{planId='90';revision=[uint32]1;huella=('a'*64)}
$json=[string]$serializer.GetMethod('Escribir').Invoke($null,[object[]]@($sample))
$sha=[Security.Cryptography.SHA256]::Create()
try{$digest=([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::ASCII.GetBytes($json)))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}
[Console]::Out.WriteLine((@{json=$json;sha256=$digest}|ConvertTo-Json -Compress))
