param([Parameter(Mandatory=$true)][ValidateRange(1,[long]::MaxValue)][long]$TaskId,[string]$Dsn='workflowdocument')
$ErrorActionPreference='Stop'
$connection=$null; $passwordPointer=[IntPtr]::Zero; $stage='START'
function Add-Parameters($command,[object[]]$values){ foreach($value in $values){$p=$command.CreateParameter();$p.Value=$value;[void]$command.Parameters.Add($p)} }
try {
  if((Read-Host 'Diagnostico DOC-67 exclusivamente SELECT en CERTIFICACION. Escriba SI') -cne 'SI'){throw 'DOC67_CACHE_DIAG_NOT_AUTHORIZED'}
  $user=Read-Host 'Usuario MySQL de solo lectura'; if([string]::IsNullOrWhiteSpace($user)){throw 'DOC67_CACHE_DIAG_USER_REQUIRED'}
  $securePassword=Read-Host 'Contraseña MySQL de solo lectura' -AsSecureString
  $passwordPointer=[Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
  $password=[Runtime.InteropServices.Marshal]::PtrToStringBSTR($passwordPointer)
  $builder=[System.Data.Odbc.OdbcConnectionStringBuilder]::new();$builder['DSN']=$Dsn;$builder['UID']=$user.Trim();$builder['PWD']=$password
  $connection=[System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString);$connection.Open()
  $stage='INTENT';$cmd=$connection.CreateCommand()
  try {
    $cmd.CommandText='SELECT intent_id,radicado FROM workflow_import_intent WHERE task_id=? ORDER BY created_utc DESC LIMIT 1';Add-Parameters $cmd @($TaskId)
    $reader=$cmd.ExecuteReader();try{if(-not $reader.Read()){throw 'DOC67_CACHE_DIAG_INTENT_NOT_FOUND'};$intentId=[string]$reader['intent_id'];$radicado=[string]$reader['radicado']}finally{$reader.Dispose()}
  }finally{$cmd.Dispose()}
  $stage='INSCRIPTIONS';$inscriptions=@();$cmd=$connection.CreateCommand()
  try {
    $cmd.CommandText="SELECT inscription_ordinal,CASE WHEN normalized_matricula IS NULL OR normalized_matricula='' THEN matricula ELSE normalized_matricula END identity_value,cabinet_name FROM workflow_import_inscription WHERE intent_id=? ORDER BY inscription_ordinal"
    Add-Parameters $cmd @($intentId);$reader=$cmd.ExecuteReader();try{while($reader.Read()){$inscriptions+=,[pscustomobject]@{Ordinal=[int]$reader['inscription_ordinal'];Identity=([string]$reader['identity_value']).Replace('S0','');Cabinet=[string]$reader['cabinet_name']}}}finally{$reader.Dispose()}
  }finally{$cmd.Dispose()}
  if($inscriptions.Count-eq 0){throw 'DOC67_CACHE_DIAG_INSCRIPTION_NOT_FOUND'}
  $stage='SCHEMA';$cmd=$connection.CreateCommand();try{$cmd.CommandText="SELECT TABLE_SCHEMA FROM information_schema.TABLES WHERE TABLE_NAME='ra_sii_cache_exepediente' ORDER BY TABLE_SCHEMA LIMIT 2";$reader=$cmd.ExecuteReader();$schemas=@();try{while($reader.Read()){$schemas+=,[string]$reader['TABLE_SCHEMA']}}finally{$reader.Dispose()}}finally{$cmd.Dispose()}
  if($schemas.Count-ne 1-or$schemas[0]-notmatch'^[A-Za-z0-9_]+$'){throw 'DOC67_CACHE_DIAG_SCHEMA_UNRESOLVED'}
  $cacheTable='`'+$schemas[0]+'`.ra_sii_cache_exepediente'
  foreach($inscription in $inscriptions){
    $stage='CACHE';$cacheRows=@();$cmd=$connection.CreateCommand();try{$cmd.CommandText="SELECT RadicadoSII,IdExpediente FROM $cacheTable WHERE Matricula=? AND NombreGabinete=? ORDER BY id_ra_sii_cache_exepediente LIMIT 10";Add-Parameters $cmd @($inscription.Identity,$inscription.Cabinet);$reader=$cmd.ExecuteReader();try{while($reader.Read()){$cacheRows+=,[pscustomobject]@{Radicado=[string]$reader['RadicadoSII'];Expediente=[int64]$reader['IdExpediente']}}}finally{$reader.Dispose()}}finally{$cmd.Dispose()}
    $rows=$cacheRows.Count;$exp=@($cacheRows|ForEach-Object{$_.Expediente}|Sort-Object -Unique).Count;$same=@($cacheRows|Where-Object{$_.Radicado-eq$radicado}).Count;$other=@($cacheRows|Where-Object{$_.Radicado-ne$radicado}).Count
    $classification=if($rows-gt 1){'DUPLICATE_IDENTITY'}elseif($rows-eq 1-and$other-gt 0){'RADICADO_MISMATCH'}elseif($rows-eq 1-and$same-eq 1){'SINGLE_ROW_REQUIRES_PHYSICAL_COMPARISON'}else{'CACHE_ABSENT'}
    Write-Host ('DOC67_CACHE_DIAG_ORDINAL='+$inscription.Ordinal);Write-Host ('DOC67_CACHE_DIAG_ROWS='+$rows);Write-Host ('DOC67_CACHE_DIAG_DISTINCT_EXPEDIENTS='+$exp);Write-Host ('DOC67_CACHE_DIAG_SAME_RADICADO='+$same);Write-Host ('DOC67_CACHE_DIAG_OTHER_RADICADO='+$other);Write-Host ('DOC67_CACHE_DIAG_CLASSIFICATION='+$classification)
  }
  Write-Host 'DOC67_CACHE_DIAG_COMPLETE'
}catch{$code=if($_.Exception.Message -match '^DOC67_CACHE_DIAG_[A-Z_]+$'){$_.Exception.Message}else{"DOC67_CACHE_DIAG_${stage}_QUERY_FAILED"};Write-Error "$code. No se mostraron secretos ni datos personales.";exit 2}
finally{if($connection){$connection.Dispose()};if($passwordPointer-ne[IntPtr]::Zero){[Runtime.InteropServices.Marshal]::ZeroFreeBSTR($passwordPointer)};$password=$null;$securePassword=$null}
