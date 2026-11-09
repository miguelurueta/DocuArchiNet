param([switch]$CheckOnly,[switch]$Execute)
$ErrorActionPreference='Stop'
$doc118OcRoot=(Get-Location).Path
$doc118OcChange='openspec/changes/doc-118-produccion-importacion-carga-carpeta-lote-recuperacion'
$doc118OcReviewPath=Join-Path $doc118OcChange 'v2-observer-continuation-review.json'
$doc118OcPreviousPath='Doc/Actualizacion/ProduccionDcoumental/ImportacionCarpetas/Validacion/doc118-runtime-smoke-20261109.json'
$doc118OcPreviousAuth=Join-Path $doc118OcChange 'v2-runtime-smoke-authorization.json'
$doc118OcDirectory=Join-Path $doc118OcRoot '.tmp/doc118-p4b-observer-20261109-01'
$doc118OcOwner=$null
$doc118OcFilter=$false
$doc118OcOwnIde=$null
$doc118OcCreated=$false
$doc118OcClock=[Diagnostics.Stopwatch]::StartNew()
$doc118OcResult=[ordered]@{issueKey='DOC-118';scope='P4_B_REVIEWED_OBSERVER_ONLY_CONTINUATION';recordedUtc=[DateTime]::UtcNow.ToString('o');status='STARTING';stage='LOCAL_CHECKS';httpRequests=0;applicationLogins=0;selectStatements=0;documentEffects=0;observerConnectionAttempts=0;observerAttachmentConfirmed=$false;documentalBreakpointArmed=$false;threadsIntentionallyFrozen=0;activeBodyObservationQualified=$false;loadedMemoryIdentityVerified=$false;cleanupConfirmed=$false;ownIdeProcessId=$null;ownIdeProcessExitVerified=$false;preexistingIdesUntouched=$true;observation=$null}
function Get-Doc118OcHash([string]$path){return (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash.ToLowerInvariant()}
try{
 if($CheckOnly -and $Execute){throw 'DOC118_OC_ARGUMENT_INVALID'}
 $doc118OcStop=Get-Content -LiteralPath $doc118OcPreviousPath -Raw|ConvertFrom-Json
 $doc118OcAuth=Get-Content -LiteralPath $doc118OcPreviousAuth -Raw|ConvertFrom-Json
 if($doc118OcStop.status -ne 'STOPPED_HEAD_NOT_CONFIRMED_NO_OBSERVER_CONNECTION' -or $doc118OcStop.runtimeDelta.observerConnections -ne 0 -or $doc118OcStop.serverLogOutcome.httpStatus -ne '200' -or $doc118OcAuth.status -ne 'AUTHORIZED' -or $doc118OcAuth.observerAttachmentAuthorized -ne $true -or $doc118OcAuth.consumed.httpRequests -ne 1 -or $doc118OcAuth.consumed.observerConnections -ne 0 -or $doc118OcAuth.remainingInApprovedMaximum.httpRequests -ne 0 -or $doc118OcAuth.remainingInApprovedMaximum.observerConnections -ne 1){throw 'DOC118_OC_UNUSED_AUTHORIZATION_UNCONFIRMED'}
 if((Get-Doc118OcHash $doc118OcPreviousAuth) -ne $doc118OcStop.authorization.sha256 -or (Get-Doc118OcHash $doc118OcStop.headCapacity.path) -ne $doc118OcStop.headCapacity.sha256){throw 'DOC118_OC_PRIOR_EVIDENCE_CHANGED'}
 foreach($doc118OcFile in $doc118OcStop.pinnedFiles){if((Get-Doc118OcHash $doc118OcFile.path) -ne $doc118OcFile.sha256){throw 'DOC118_OC_PIN_CHANGED'}}
 if(Test-Path -LiteralPath 'app_offline.htm'){throw 'DOC118_OC_MAINTENANCE_PRESENT'}
 if(Test-Path -LiteralPath $doc118OcDirectory){throw 'DOC118_OC_PREVIOUS_CONNECTION_REQUIRES_REVIEW'}
 if($Execute){
  if(-not (Test-Path -LiteralPath $doc118OcReviewPath)){throw 'DOC118_OC_REVIEW_REQUIRED'}
  $doc118OcReview=Get-Content -LiteralPath $doc118OcReviewPath -Raw|ConvertFrom-Json
  if($doc118OcReview.status -ne 'REVIEWED_CONTINUE_EXISTING_OBSERVER_AUTHORIZATION' -or $doc118OcReview.scope -ne $doc118OcResult.scope -or $doc118OcReview.previousStop.sha256 -ne (Get-Doc118OcHash $doc118OcPreviousPath) -or $doc118OcReview.originalAuthorization.sha256 -ne (Get-Doc118OcHash $doc118OcPreviousAuth) -or $doc118OcReview.executorSha256 -ne (Get-Doc118OcHash $PSCommandPath) -or $doc118OcReview.maximum.httpRequests -ne 0 -or $doc118OcReview.maximum.observerConnections -ne 1 -or $doc118OcReview.newBudgetsAuthorized -ne $false){throw 'DOC118_OC_REVIEW_SCOPE_CHANGED'}
 }
 $doc118OcInterop='C:/Program Files/Microsoft Visual Studio/18/Enterprise/Common7/IDE/PublicAssemblies/Microsoft.VisualStudio.Interop.dll'
 Add-Type -Path $doc118OcInterop
 Add-Type -ReferencedAssemblies $doc118OcInterop -Path 'tools/e2e/scripts/support/Doc118Debugger.cs'
 Add-Type -TypeDefinition 'public static class Doc118OcOwnIdeWindow { [System.Runtime.InteropServices.DllImport("user32.dll")] static extern uint GetWindowThreadProcessId(System.IntPtr h, out uint p); public static int Pid(System.IntPtr h) { uint p; GetWindowThreadProcessId(h, out p); return checked((int)p); } }'
 if(-not $Execute){$doc118OcResult.status='LOCAL_CHECK_AND_COMPILE_PASS_NO_CONNECTION';$doc118OcResult.cleanupConfirmed=$true;return}
 $doc118OcResult.stage='IIS_SCOPE'
 $doc118OcAppcmd=Join-Path $env:windir 'System32/inetsrv/appcmd.exe'
 $doc118OcPhysical=(& $doc118OcAppcmd list vdir 'Default Web Site/GestionDocumental-Docuarchi.net/' /text:physicalPath)
 if($LASTEXITCODE -ne 0 -or [IO.Path]::GetFullPath([string]$doc118OcPhysical).TrimEnd('\') -ne $doc118OcRoot.TrimEnd('\')){throw 'DOC118_OC_IIS_ROOT_CHANGED'}
 $doc118OcPool=(& $doc118OcAppcmd list app 'Default Web Site/GestionDocumental-Docuarchi.net' /text:applicationPool)
 if($LASTEXITCODE -ne 0 -or $doc118OcPool -ne 'DefaultAppPool'){throw 'DOC118_OC_IIS_POOL_CHANGED'}
 $doc118OcWorkers=@(& $doc118OcAppcmd list wp '/apppool.name:DefaultAppPool' /text:WP.NAME)
 if($LASTEXITCODE -ne 0){throw 'DOC118_OC_WORKER_UNCONFIRMED'}
 $doc118OcWorkers=@($doc118OcWorkers|Where-Object{$_ -match '^[1-9][0-9]*$'})
 if($doc118OcWorkers.Count -ne 1){throw 'DOC118_OC_SINGLE_WORKER_REQUIRED'}
 $doc118OcResult.workerId=[int]$doc118OcWorkers[0]
 $doc118OcSource=Join-Path $doc118OcRoot 'workflow/ClassAlmacenamiento.vb'
 $doc118OcLines=[IO.File]::ReadAllLines($doc118OcSource)
 $doc118OcDeclaration=[Array]::FindIndex($doc118OcLines,[Predicate[string]]{param($line) $line.Contains('Function AlmacenamientoDocumentoProduccionDocumental(')})
 $doc118OcBody=$doc118OcDeclaration
 while($doc118OcBody -ge 0 -and $doc118OcBody -lt $doc118OcLines.Length -and $doc118OcLines[$doc118OcBody].Trim() -ne 'Dim ClassNeodynamic As New ClassNeodynamic'){$doc118OcBody++}
 if($doc118OcDeclaration -lt 0 -or $doc118OcBody -gt $doc118OcDeclaration+100){throw 'DOC118_OC_SOURCE_LINE_UNCONFIRMED'}
 if($doc118OcClock.Elapsed.TotalSeconds -gt 90){throw 'DOC118_OC_DURATION_EXCEEDED'}
 $doc118OcResult.stage='CLAIM_UNUSED_CONNECTION'
 New-Item -ItemType Directory -Path $doc118OcDirectory|Out-Null
 $doc118OcCreated=$true
 $doc118OcCapacity=Join-Path $doc118OcDirectory 'connection-attempt.json'
 $doc118OcBytes=[Text.Encoding]::UTF8.GetBytes('{"maximum":1,"attemptConsumed":true,"httpRequests":0}')
 $doc118OcStream=[IO.File]::Open($doc118OcCapacity,[IO.FileMode]::CreateNew,[IO.FileAccess]::Write,[IO.FileShare]::None)
 try{$doc118OcStream.Write($doc118OcBytes,0,$doc118OcBytes.Length)}finally{$doc118OcStream.Dispose()}
 $doc118OcResult.observerConnectionAttempts=1
 [Doc118MessageFilter]::Install();$doc118OcFilter=$true
 $doc118OcResult.stage='ATTACH_AND_BIND_FALSE_CONDITION'
 $doc118OcOwner=New-Object Doc118Debugger
 $doc118OcOwner.Initialize([int]$doc118OcWorkers[0],$doc118OcSource,$doc118OcBody+1)
 $doc118OcResult.observerAttachmentConfirmed=$true
 $doc118OcResult.stage='INSPECT'
 if($doc118OcClock.Elapsed.TotalSeconds -gt 90){throw 'DOC118_OC_DURATION_EXCEEDED'}
 $doc118OcView=$doc118OcOwner.Inspect()
 $doc118OcResult.observation=$doc118OcView
 if(-not $doc118OcView.sourceBound -or -not $doc118OcView.workerAlive -or -not $doc118OcView.otherThreadsRunning -or -not $doc118OcView.released -or $doc118OcView.frozen -or $doc118OcView.fault -or $doc118OcView.entriesA -ne 0 -or $doc118OcView.entriesB -ne 0){throw 'DOC118_OC_SOURCE_BINDING_UNCONFIRMED'}
 foreach($doc118OcFile in $doc118OcStop.pinnedFiles){if((Get-Doc118OcHash $doc118OcFile.path) -ne $doc118OcFile.sha256){throw 'DOC118_OC_PIN_CHANGED_DURING_INSPECTION'}}
 if((Get-Doc118OcHash $doc118OcStop.headCapacity.path) -ne $doc118OcStop.headCapacity.sha256){throw 'DOC118_OC_HEAD_CAPACITY_CHANGED'}
 $doc118OcResult.status='SOURCE_BINDING_OBSERVER_ONLY_PASS'
}catch{
 $doc118OcResult.status='UNCONFIRMED'
 $doc118OcResult.failure=[ordered]@{stage=$doc118OcResult.stage;controllerStage=if($doc118OcOwner){$doc118OcOwner.Stage}else{$null};knownCode=if($_.Exception.Message -match '^DOC118_OC_[A-Z_]+$'){$_.Exception.Message}else{'OPERATION_UNCONFIRMED'};exceptionType=$_.Exception.GetType().Name;hresult=$_.Exception.HResult}
}finally{
 if($doc118OcOwner){
  # The handle belongs only to the private IDE held by this controller. Never
  # infer ownership from other IDE processes or terminate one of them.
  try{$doc118OcIde=$doc118OcOwner.GetType().GetField('ide',[Reflection.BindingFlags]'NonPublic,Instance').GetValue($doc118OcOwner);if($doc118OcIde){$doc118OcOwnIdeId=[Doc118OcOwnIdeWindow]::Pid([IntPtr]$doc118OcIde.MainWindow.HWnd);if($doc118OcOwnIdeId -gt 0){$doc118OcOwnIde=[Diagnostics.Process]::GetProcessById($doc118OcOwnIdeId);$doc118OcResult.ownIdeProcessId=$doc118OcOwnIdeId}}}catch{$doc118OcResult.ownIdeIdentityObservation='UNCONFIRMED'}
  try{$doc118OcOwner.Dispose();$doc118OcResult.cleanupConfirmed=$doc118OcOwner.CleanupConfirmed;if(-not $doc118OcResult.cleanupConfirmed){$doc118OcResult.status='SDK_CLOSE_UNCONFIRMED_REVIEW_REQUIRED'}}catch{$doc118OcResult.status='SDK_CLOSE_UNCONFIRMED_REVIEW_REQUIRED'}
  if($doc118OcOwnIde){try{$doc118OcResult.ownIdeProcessExitVerified=$doc118OcOwnIde.WaitForExit(5000);if(-not $doc118OcResult.ownIdeProcessExitVerified){$doc118OcResult.status='PRIVATE_IDE_EXIT_UNCONFIRMED_REVIEW_REQUIRED'}}catch{$doc118OcResult.ownIdeIdentityObservation='EXIT_UNCONFIRMED'}finally{$doc118OcOwnIde.Dispose()}}
 }else{$doc118OcResult.cleanupConfirmed=$true}
 if($doc118OcFilter){try{[Doc118MessageFilter]::Remove()}catch{$doc118OcResult.status='MESSAGE_FILTER_CLOSE_UNCONFIRMED_REVIEW_REQUIRED'}}
 $doc118OcResult.completedUtc=[DateTime]::UtcNow.ToString('o')
 $doc118OcResult.elapsedMilliseconds=$doc118OcClock.ElapsedMilliseconds
 if($Execute -and $doc118OcCreated){$doc118OcResult|ConvertTo-Json -Depth 6|Set-Content -LiteralPath (Join-Path $doc118OcDirectory 'result.json') -Encoding UTF8}
 $doc118OcResult|ConvertTo-Json -Depth 6 -Compress
}
if($doc118OcResult.status -notin @('SOURCE_BINDING_OBSERVER_ONLY_PASS','LOCAL_CHECK_AND_COMPILE_PASS_NO_CONNECTION')){exit 2}
