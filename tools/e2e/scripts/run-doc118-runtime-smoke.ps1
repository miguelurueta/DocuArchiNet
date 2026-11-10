param([switch]$CheckOnly,[switch]$Execute)
$ErrorActionPreference='Stop'
$doc118P4bRoot=(Get-Location).Path
$doc118P4bChange='openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion'
$doc118P4bProposalPath=Join-Path $doc118P4bChange 'v2-runtime-smoke-proposal.json'
$doc118P4bAuthPath=Join-Path $doc118P4bChange 'v2-runtime-smoke-authorization.json'
$doc118P4bOwner=$null
$doc118P4bFilter=$false
$doc118P4bClock=[Diagnostics.Stopwatch]::StartNew()
$doc118P4bResult=[ordered]@{issueKey='DOC-118';scope='P4_B_HEAD_AND_SOURCE_BINDING_SMOKE';recordedUtc=[DateTime]::UtcNow.ToString('o');status='STARTING';stage='LOCAL_CHECKS';applicationLogins=0;httpRequests=0;selectStatements=0;documentEffects=0;observerAttached=$false;documentalBreakpointArmed=$false;threadsIntentionallyFrozen=0;activeBodyObservationQualified=$false;loadedMemoryIdentityVerified=$false;cleanupConfirmed=$false;executeRequested=[bool]$Execute;observation=$null}
try{
 if($CheckOnly -and $Execute){throw 'DOC118_P4B_ARGUMENT_INVALID'}
 if(-not (Test-Path -LiteralPath $doc118P4bProposalPath)){throw 'DOC118_P4B_PROPOSAL_REQUIRED'}
 $doc118P4bProposal=Get-Content -LiteralPath $doc118P4bProposalPath -Raw|ConvertFrom-Json
 if($doc118P4bProposal.status -ne 'PROPOSED_NOT_AUTHORIZED' -or $doc118P4bProposal.stage -ne 'P4_B_HEAD_AND_SOURCE_BINDING_SMOKE' -or $doc118P4bProposal.maximum.httpRequests -ne 1 -or $doc118P4bProposal.maximum.applicationLogins -ne 0 -or $doc118P4bProposal.maximum.selectStatements -ne 0 -or $doc118P4bProposal.maximum.observerConnections -ne 1){throw 'DOC118_P4B_PROPOSAL_CHANGED'}
 foreach($doc118P4bFile in $doc118P4bProposal.pinnedFiles){if((Get-FileHash -LiteralPath $doc118P4bFile.path -Algorithm SHA256).Hash.ToLowerInvariant() -ne $doc118P4bFile.sha256){throw 'DOC118_P4B_PIN_CHANGED'}}
 if(Test-Path -LiteralPath 'app_offline.htm'){throw 'DOC118_P4B_MAINTENANCE_PRESENT'}
 if(Test-Path -LiteralPath $doc118P4bProposal.localResultDirectory){throw 'DOC118_P4B_PREVIOUS_RESULT_REQUIRES_REVIEW'}
 $doc118P4bInterop='C:/Program Files/Microsoft Visual Studio/18/Enterprise/Common7/IDE/PublicAssemblies/Microsoft.VisualStudio.Interop.dll'
 if(-not (Test-Path -LiteralPath $doc118P4bInterop)){throw 'DOC118_P4B_SDK_UNAVAILABLE'}
 Add-Type -Path $doc118P4bInterop
 Add-Type -ReferencedAssemblies $doc118P4bInterop -Path 'tools/e2e/scripts/support/Doc118Debugger.cs'
 if(-not $Execute){$doc118P4bResult.status='LOCAL_CHECK_AND_SDK_COMPILE_PASS_NO_HTTP_OR_ATTACH';$doc118P4bResult.cleanupConfirmed=$true;return}
 $doc118P4bResult.stage='AUTHORIZATION'
 if(-not (Test-Path -LiteralPath $doc118P4bAuthPath)){throw 'DOC118_P4B_AUTHORIZATION_REQUIRED'}
 $doc118P4bAuth=Get-Content -LiteralPath $doc118P4bAuthPath -Raw|ConvertFrom-Json
 $doc118P4bProposalHash=(Get-FileHash -LiteralPath $doc118P4bProposalPath -Algorithm SHA256).Hash.ToLowerInvariant()
 if($doc118P4bAuth.status -ne 'AUTHORIZED' -or $doc118P4bAuth.stage -ne $doc118P4bProposal.stage -or $doc118P4bAuth.proposalSha256 -ne $doc118P4bProposalHash -or $doc118P4bAuth.observerAttachmentAuthorized -ne $true -or $doc118P4bAuth.headRequestAuthorized -ne $true){throw 'DOC118_P4B_AUTHORIZATION_SCOPE_INVALID'}
 $doc118P4bAppcmd=Join-Path $env:windir 'System32/inetsrv/appcmd.exe'
 $doc118P4bPhysical=(& $doc118P4bAppcmd list vdir 'Default Web Site/GestionDocumental-Docuarchi.net/' /text:physicalPath)
 if($LASTEXITCODE -ne 0 -or [IO.Path]::GetFullPath([string]$doc118P4bPhysical).TrimEnd('\') -ne $doc118P4bRoot.TrimEnd('\')){throw 'DOC118_P4B_IIS_ROOT_UNCONFIRMED'}
 $doc118P4bPool=(& $doc118P4bAppcmd list app 'Default Web Site/GestionDocumental-Docuarchi.net' /text:applicationPool)
 if($LASTEXITCODE -ne 0 -or $doc118P4bPool -ne 'DefaultAppPool'){throw 'DOC118_P4B_IIS_POOL_CHANGED'}
 $doc118P4bDirectory=[IO.Path]::GetFullPath((Join-Path $doc118P4bRoot $doc118P4bProposal.localResultDirectory))
 if(-not $doc118P4bDirectory.StartsWith((Join-Path $doc118P4bRoot '.tmp/'),[StringComparison]::OrdinalIgnoreCase)){throw 'DOC118_P4B_RESULT_PATH_INVALID'}
 New-Item -ItemType Directory -Path $doc118P4bDirectory|Out-Null
 $doc118P4bResult.stage='HEAD_ONCE'
 $doc118P4bHeadText=(& node tools/e2e/scripts/support/doc118-runtime-smoke-head.cjs)
 $doc118P4bHeadExit=$LASTEXITCODE
 $doc118P4bHead=$doc118P4bHeadText|ConvertFrom-Json
 $doc118P4bResult.httpRequests=[int]$doc118P4bHead.httpRequests
 $doc118P4bResult.http=$doc118P4bHead
 if($doc118P4bHeadExit -ne 0 -or $doc118P4bHead.failure -or $doc118P4bHead.status -notin @(200,302)){throw 'DOC118_P4B_HEAD_NOT_ACCEPTED'}
 if($doc118P4bClock.Elapsed.TotalSeconds -gt 90){throw 'DOC118_P4B_DURATION_EXCEEDED'}
 $doc118P4bResult.stage='RESOLVE_SINGLE_WORKER'
 $doc118P4bWorkers=@(& $doc118P4bAppcmd list wp '/apppool.name:DefaultAppPool' /text:WP.NAME)
 if($LASTEXITCODE -ne 0){throw 'DOC118_P4B_WORKER_UNCONFIRMED'}
 $doc118P4bWorkers=@($doc118P4bWorkers|Where-Object{$_ -match '^[1-9][0-9]*$'})
 if($doc118P4bWorkers.Count -ne 1){throw 'DOC118_P4B_SINGLE_WORKER_REQUIRED'}
 $doc118P4bSource=Join-Path $doc118P4bRoot 'workflow/ClassAlmacenamiento.vb'
 $doc118P4bLines=[IO.File]::ReadAllLines($doc118P4bSource)
 $doc118P4bDeclaration=[Array]::FindIndex($doc118P4bLines,[Predicate[string]]{param($line) $line.Contains('Function AlmacenamientoDocumentoProduccionDocumental(')})
 $doc118P4bBody=$doc118P4bDeclaration
 while($doc118P4bBody -ge 0 -and $doc118P4bBody -lt $doc118P4bLines.Length -and $doc118P4bLines[$doc118P4bBody].Trim() -ne 'Dim ClassNeodynamic As New ClassNeodynamic'){$doc118P4bBody++}
 if($doc118P4bDeclaration -lt 0 -or $doc118P4bBody -gt $doc118P4bDeclaration+100){throw 'DOC118_P4B_SOURCE_LINE_UNCONFIRMED'}
 [Doc118MessageFilter]::Install();$doc118P4bFilter=$true
 $doc118P4bResult.stage='ATTACH_AND_BIND_FALSE_CONDITION'
 $doc118P4bOwner=New-Object Doc118Debugger
 $doc118P4bOwner.Initialize([int]$doc118P4bWorkers[0],$doc118P4bSource,$doc118P4bBody+1)
 $doc118P4bResult.observerAttached=$true
 $doc118P4bResult.stage='INSPECT'
 if($doc118P4bClock.Elapsed.TotalSeconds -gt 90){throw 'DOC118_P4B_DURATION_EXCEEDED'}
 $doc118P4bObservation=$doc118P4bOwner.Inspect()
 $doc118P4bResult.observation=$doc118P4bObservation
 if(-not $doc118P4bObservation.sourceBound -or -not $doc118P4bObservation.workerAlive -or -not $doc118P4bObservation.otherThreadsRunning -or -not $doc118P4bObservation.released -or $doc118P4bObservation.frozen -or $doc118P4bObservation.fault -or $doc118P4bObservation.entriesA -ne 0 -or $doc118P4bObservation.entriesB -ne 0){throw 'DOC118_P4B_SOURCE_BINDING_UNCONFIRMED'}
 foreach($doc118P4bFile in $doc118P4bProposal.pinnedFiles){if((Get-FileHash -LiteralPath $doc118P4bFile.path -Algorithm SHA256).Hash.ToLowerInvariant() -ne $doc118P4bFile.sha256){throw 'DOC118_P4B_PIN_CHANGED_DURING_INSPECTION'}}
 $doc118P4bResult.status='SOURCE_BINDING_SMOKE_PASS_NO_ACTIVE_CALL'
}catch{$doc118P4bResult.status='UNCONFIRMED';$doc118P4bResult.failure=[ordered]@{stage=$doc118P4bResult.stage;knownCode=if($_.Exception.Message -match '^DOC118_P4B_[A-Z_]+$'){$_.Exception.Message}else{'OPERATION_UNCONFIRMED'};exceptionType=$_.Exception.GetType().Name;hresult=$_.Exception.HResult}}
finally{
 if($doc118P4bOwner){try{$doc118P4bOwner.Dispose();$doc118P4bResult.cleanupConfirmed=$doc118P4bOwner.CleanupConfirmed;if(-not $doc118P4bResult.cleanupConfirmed){$doc118P4bResult.status='CLOSE_UNCONFIRMED_REVIEW_REQUIRED'}}catch{$doc118P4bResult.status='CLOSE_UNCONFIRMED_REVIEW_REQUIRED'}}elseif($doc118P4bResult.httpRequests -eq 0){$doc118P4bResult.cleanupConfirmed=$true}
 if($doc118P4bFilter){try{[Doc118MessageFilter]::Remove()}catch{$doc118P4bResult.status='CLOSE_UNCONFIRMED_REVIEW_REQUIRED'}}
 $doc118P4bResult.completedUtc=[DateTime]::UtcNow.ToString('o')
 $doc118P4bResult.elapsedMilliseconds=$doc118P4bClock.ElapsedMilliseconds
 if($Execute -and $doc118P4bDirectory -and (Test-Path -LiteralPath $doc118P4bDirectory)){$doc118P4bResult|ConvertTo-Json -Depth 6|Set-Content -LiteralPath (Join-Path $doc118P4bDirectory 'result.json') -Encoding UTF8}
 $doc118P4bResult|ConvertTo-Json -Depth 6 -Compress
}
if($doc118P4bResult.status -notin @('SOURCE_BINDING_SMOKE_PASS_NO_ACTIVE_CALL','LOCAL_CHECK_AND_SDK_COMPILE_PASS_NO_HTTP_OR_ATTACH')){exit 2}
