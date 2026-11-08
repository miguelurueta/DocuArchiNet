param([Parameter(Mandatory=$true)][ValidatePattern('^[a-f0-9]{32}$')][string]$RunId,[Parameter(Mandatory=$true)][ValidateRange(1,2147483647)][int]$WorkerId,[ValidateRange(1,3)][int]$Attempt=1)
$ErrorActionPreference='Stop'
. (Join-Path $PSScriptRoot 'doc118-debugger-protocol.ps1')
$doc118Root=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../../..'))
$doc118Directory=Join-Path $doc118Root ('.tmp/doc118-debugger-'+$RunId+'-'+$Attempt)
if(Test-Path -LiteralPath $doc118Directory){throw 'DOC118_OBSERVER_PREVIOUS_INSTANCE'}
New-Item -ItemType Directory -Path $doc118Directory|Out-Null
$doc118Status=Join-Path $doc118Directory 'status.json'
$doc118Assemblies='C:\Program Files\Microsoft Visual Studio\18\Enterprise\Common7\IDE\PublicAssemblies'
$doc118Owner=$null
$doc118Sequence=0
$doc118Failure=$null
$doc118Created=[DateTime]::UtcNow.ToString('o')
$doc118Hashes=[pscustomobject][ordered]@{
 controller=(Get-FileHash -LiteralPath (Join-Path $PSScriptRoot 'Doc118Debugger.cs') -Algorithm SHA256).Hash.ToLowerInvariant()
 protocol=(Get-FileHash -LiteralPath $PSCommandPath -Algorithm SHA256).Hash.ToLowerInvariant()
 commandProtocol=(Get-FileHash -LiteralPath (Join-Path $PSScriptRoot 'doc118-debugger-protocol.ps1') -Algorithm SHA256).Hash.ToLowerInvariant()
 source=(Get-FileHash -LiteralPath (Join-Path $doc118Root 'workflow/ClassAlmacenamiento.vb') -Algorithm SHA256).Hash.ToLowerInvariant()
}
$doc118CleanupConfirmed=$false
$doc118CurrentState='STARTING'
$doc118LastTrace=''
$doc118TraceEvents=0
function Save-Observer($state,$value){
 $script:doc118CurrentState=$state
 $projection=[pscustomobject]@{status=$state;runId=$RunId;sequence=$doc118Sequence;stage=if($doc118Owner){$doc118Owner.Stage}else{'LOAD_CONTROLLER'};observation=$value;failure=$doc118Failure;cleanupConfirmed=$doc118CleanupConfirmed}|ConvertTo-Json -Depth 6 -Compress
 if($projection -ne $script:doc118LastTrace){
  if($script:doc118TraceEvents -ge 2048){throw 'DOC118_OBSERVER_TRACE_LIMIT'}
  $trace=[pscustomobject]@{utc=[DateTime]::UtcNow.ToString('o');value=($projection|ConvertFrom-Json)}|ConvertTo-Json -Depth 8 -Compress
  [IO.File]::AppendAllText((Join-Path $doc118Directory 'observation-history.jsonl'),$trace+[Environment]::NewLine,(New-Object Text.UTF8Encoding($false)))
  $script:doc118LastTrace=$projection;$script:doc118TraceEvents++
 }
 $data=[pscustomobject]@{status=$state;runId=$RunId;pid=$PID;sequence=$doc118Sequence;observation=$value;failure=$doc118Failure;cleanupConfirmed=$doc118CleanupConfirmed;createdUtc=$doc118Created;heartbeatUtc=[DateTime]::UtcNow.ToString('o');implementationHashes=$doc118Hashes}|ConvertTo-Json -Depth 6 -Compress
 [Doc118AtomicStatus]::Write($doc118Status,$data)
}
try{
 Save-Observer 'STARTING' $null
 Add-Type -Path (Join-Path $doc118Assemblies 'Microsoft.VisualStudio.Interop.dll')
 Add-Type -Path (Join-Path $doc118Assemblies 'envdte.dll')
 Add-Type -Path (Join-Path $doc118Assemblies 'envdte80.dll')
 Add-Type -ReferencedAssemblies (Join-Path $doc118Assemblies 'Microsoft.VisualStudio.Interop.dll') -Path (Join-Path $PSScriptRoot 'Doc118Debugger.cs')
 [Doc118MessageFilter]::Install()
 $doc118Source=Join-Path $doc118Root 'workflow/ClassAlmacenamiento.vb'
 $doc118Lines=[IO.File]::ReadAllLines($doc118Source)
 $doc118Declaration=[Array]::FindIndex($doc118Lines,[Predicate[string]]{param($line) $line.Contains('Function AlmacenamientoDocumentoProduccionDocumental(')})
 $doc118Body=$doc118Declaration
 while($doc118Body -lt $doc118Lines.Length -and $doc118Lines[$doc118Body].Trim() -ne 'Dim ClassNeodynamic As New ClassNeodynamic'){$doc118Body++}
 if($doc118Declaration -lt 0 -or $doc118Body -gt $doc118Declaration+100){throw 'DOC118_OBSERVER_SOURCE_UNCONFIRMED'}
 $doc118Owner=New-Object Doc118Debugger
 $doc118Owner.Initialize($WorkerId,$doc118Source,($doc118Body+1))
 Save-Observer 'ATTACHED' ($doc118Owner.Inspect())
 $doc118Expires=[DateTime]::UtcNow.AddMinutes(25)
 while([DateTime]::UtcNow -lt $doc118Expires){
  $doc118Owner.Poll()
  $doc118Request=Join-Path $doc118Directory 'request.json'
  if(Test-Path -LiteralPath $doc118Request){
   $command=Get-Content -LiteralPath $doc118Request -Raw|ConvertFrom-Json
   Confirm-Doc118ObserverCommand -Command $command -ExpectedSequence ($doc118Sequence+1)
   $doc118Sequence=$command.sequence
   Move-Item -LiteralPath $doc118Request -Destination (Join-Path $doc118Directory ('request-'+$doc118Sequence+'.consumed.json'))
   switch($command.operation){
    'ARM' {$doc118Owner.Arm([int]$command.args.expA,[string]$command.args.nameA,[int]$command.args.expB,[string]$command.args.nameB,[string]$command.args.resourceSha256)}
    'INSPECT' {}
    'RESUME' {$doc118Owner.Resume()}
    'CLOSE' {break}
    default {throw 'DOC118_OBSERVER_COMMAND_INVALID'}
   }
   Save-Observer $command.operation ($doc118Owner.Inspect())
   if($command.operation -eq 'CLOSE'){break}
  }
  $observation=$doc118Owner.Inspect()
  if($observation.closed -or $observation.fault){Save-Observer 'REVIEW_REQUIRED' $observation;break}
  if($observation.frozen){Save-Observer 'INSIDE_FROZEN' $observation}else{Save-Observer $doc118CurrentState $observation}
  Start-Sleep -Milliseconds 100
 }
}catch{
 $doc118Exception=$_.Exception
 for($doc118Depth=0;$doc118Depth -lt 5 -and $null -ne $doc118Exception.InnerException;$doc118Depth++){$doc118Exception=$doc118Exception.InnerException}
 $doc118Failure=[pscustomobject]@{stage=if($doc118Owner){$doc118Owner.Stage}else{'LOAD_CONTROLLER'};hresult=$doc118Exception.HResult;exceptionType=$doc118Exception.GetType().Name}
 Save-Observer 'UNCONFIRMED' $null
}
finally{
 try{if($null -ne $doc118Owner){$doc118Owner.Dispose();$doc118CleanupConfirmed=$doc118Owner.CleanupConfirmed;if($doc118CleanupConfirmed){Save-Observer 'CLOSED' $null}else{Save-Observer 'RELEASE_UNCONFIRMED' $null}}}catch{Save-Observer 'RELEASE_UNCONFIRMED' $null}
 try{[Doc118MessageFilter]::Remove()}catch{}
}
