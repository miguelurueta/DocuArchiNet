param([Parameter(Mandatory=$true)][ValidatePattern('^[a-f0-9]{32}$')][string]$ProbeId,[switch]$CompileOnly)
$ErrorActionPreference='Stop'
$doc118Root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$doc118Directory=[IO.Path]::GetFullPath((Join-Path $doc118Root ('.tmp/doc118-release-probe-'+$ProbeId)))
if(-not $doc118Directory.StartsWith((Join-Path $doc118Root '.tmp/'),[StringComparison]::OrdinalIgnoreCase) -or (Test-Path -LiteralPath $doc118Directory)){throw 'DOC118_PROBE_DIRECTORY_INVALID'}
New-Item -ItemType Directory -Path $doc118Directory|Out-Null
$doc118Child=$null
$doc118FilterInstalled=$false
$doc118Result=[pscustomobject]@{issueKey='DOC-118';probeId=$ProbeId;scope='SYNTHETIC_PROCESS_ONLY';recordedUtc=[DateTime]::UtcNow.ToString('o');status='STARTING';stage='COMPILE_FIXTURE';failure=$null;appLoginAttempts=0;httpRequests=0;selectStatements=0;documentEffects=0;legacyRuntimeQualified=$false;childExited=$false;observation=$null;controllerSha256=(Get-FileHash -LiteralPath (Join-Path $PSScriptRoot 'support/Doc118Debugger.cs') -Algorithm SHA256).Hash.ToLowerInvariant()}
try{
 $doc118Source=Join-Path $doc118Root 'tests/doc118-release-probe.fixture.cs'
 $doc118Exe=Join-Path $doc118Directory 'Doc118Probe.exe'
 $doc118Csc=Join-Path $env:windir 'Microsoft.NET/Framework64/v4.0.30319/csc.exe'
 & $doc118Csc /nologo /debug:full /optimize- /platform:x64 /target:exe ('/out:'+$doc118Exe) ('/pdb:'+(Join-Path $doc118Directory 'Doc118Probe.pdb')) $doc118Source | Out-Null
 if($LASTEXITCODE -ne 0){throw 'DOC118_PROBE_COMPILE_FAILED'}
 $doc118Result.stage='LOAD_SDK_RUNNER'
 $doc118Interop='C:\Program Files\Microsoft Visual Studio\18\Enterprise\Common7\IDE\PublicAssemblies\Microsoft.VisualStudio.Interop.dll'
 Add-Type -Path $doc118Interop
 $doc118Combined=[IO.File]::ReadAllText((Join-Path $PSScriptRoot 'support/Doc118Debugger.cs'))+[Environment]::NewLine+[IO.File]::ReadAllText((Join-Path $doc118Root 'tests/doc118-debugger-probe-runner.fixture.cs'))
 Add-Type -ReferencedAssemblies $doc118Interop -TypeDefinition $doc118Combined
 if($CompileOnly){$doc118Result.stage='COMPILED';$doc118Result.status='COMPILE_PASS';return}
 [Doc118MessageFilter]::Install()
 $doc118FilterInstalled=$true
 $doc118Result.stage='START_FIXTURE'
 $doc118Child=Start-Process -FilePath $doc118Exe -ArgumentList @($doc118Directory) -WorkingDirectory $doc118Directory -WindowStyle Hidden -PassThru
 $doc118Until=[DateTime]::UtcNow.AddSeconds(5)
 while(-not (Test-Path -LiteralPath (Join-Path $doc118Directory 'ready.flag')) -and [DateTime]::UtcNow -lt $doc118Until){Start-Sleep -Milliseconds 50}
 if(-not (Test-Path -LiteralPath (Join-Path $doc118Directory 'ready.flag'))){throw 'DOC118_PROBE_NOT_READY'}
 $doc118Result.stage='RUN_SDK_PROBE';$doc118Result.observation=[Doc118ProbeRunner]::Run($doc118Child.Id,$doc118Directory,$doc118Source)
 $doc118Result.childExited=$doc118Child.WaitForExit(10000)
 $doc118Result.status=if($doc118Result.observation.status -eq 'RELEASE_PROBE_PASS' -and $doc118Result.observation.cleanupConfirmed -and $doc118Result.childExited){'PASS'}else{'UNCONFIRMED'}
}catch{$doc118Result.status='UNCONFIRMED';$doc118Result.failure=[pscustomobject]@{exceptionType=$_.Exception.GetType().Name;hresult=$_.Exception.HResult}}
finally{
 if($doc118Child -and -not $doc118Result.childExited){[IO.File]::WriteAllText((Join-Path $doc118Directory 'stop.flag'),'STOP');$doc118Result.childExited=$doc118Child.WaitForExit(10000)}
 if($doc118FilterInstalled){try{[Doc118MessageFilter]::Remove()}catch{}}
 $doc118Result|ConvertTo-Json -Depth 7|Set-Content -LiteralPath (Join-Path $doc118Directory 'result.json') -Encoding UTF8
 $doc118Result|ConvertTo-Json -Depth 7 -Compress
}
if($doc118Result.status -notin @('PASS','COMPILE_PASS')){exit 2}
