param([Parameter(Mandatory=$true)][ValidatePattern('^[a-f0-9]{32}$')][string]$ProbeId)
$ErrorActionPreference='Stop'
$doc118Root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$doc118Directory=[IO.Path]::GetFullPath((Join-Path $doc118Root ('.tmp/doc118-staging-lock-probe-'+$ProbeId)))
if(-not $doc118Directory.StartsWith((Join-Path $doc118Root '.tmp/'),[StringComparison]::OrdinalIgnoreCase) -or (Test-Path -LiteralPath $doc118Directory)){throw 'DOC118_STAGING_PROBE_DIRECTORY_INVALID'}
New-Item -ItemType Directory -Path $doc118Directory|Out-Null
$doc118Source=Join-Path $doc118Root 'tests/doc118-staging-lock-probe.fixture.cs'
$doc118Framework=Join-Path $env:windir 'Microsoft.NET/Framework64/v4.0.30319'
$doc118Exe=Join-Path $doc118Directory 'Doc118StagingProbe.exe'
$doc118Compile=@(& (Join-Path $doc118Framework 'csc.exe') /nologo /optimize- /platform:x64 /target:exe ('/reference:'+(Join-Path $doc118Framework 'System.Web.Extensions.dll')) ('/out:'+$doc118Exe) $doc118Source)
if($LASTEXITCODE -ne 0){$doc118Compile|Set-Content -LiteralPath (Join-Path $doc118Directory 'compile-diagnostics.txt') -Encoding UTF8;[pscustomobject]@{exitCode=$LASTEXITCODE;outputLines=$doc118Compile.Count}|ConvertTo-Json -Compress|Set-Content -LiteralPath (Join-Path $doc118Directory 'compile-status.json') -Encoding UTF8;throw 'DOC118_STAGING_PROBE_COMPILE_UNCONFIRMED'}
$doc118Output=& $doc118Exe run $doc118Directory $doc118Root
$doc118Exit=$LASTEXITCODE
try{$doc118Result=$doc118Output|ConvertFrom-Json}catch{throw 'DOC118_STAGING_PROBE_OUTPUT_UNCONFIRMED'}
$doc118Result|Add-Member -NotePropertyName probeId -NotePropertyValue $ProbeId
$doc118Result|Add-Member -NotePropertyName fixtureSha256 -NotePropertyValue (Get-FileHash -LiteralPath $doc118Source -Algorithm SHA256).Hash.ToLowerInvariant()
$doc118Result|ConvertTo-Json -Depth 7|Set-Content -LiteralPath (Join-Path $doc118Directory 'result.json') -Encoding UTF8
$doc118Result|ConvertTo-Json -Depth 7 -Compress
if($doc118Exit -ne 0 -or $doc118Result.status -ne 'PASS'){exit 2}
