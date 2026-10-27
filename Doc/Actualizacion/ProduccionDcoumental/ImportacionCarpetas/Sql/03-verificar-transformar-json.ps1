[CmdletBinding()]
param(
 [Parameter(Mandatory=$true)][string]$RepositoryRoot,
 [switch]$Apply,
 [switch]$PostCutover
)
$ErrorActionPreference='Stop'
$catalog=$null;$connection=$null;$tx=$null
$phase='INIT'
function Read-Table([string]$sql) {
 if(-not $sql.StartsWith('SELECT ')){throw 'SELECT_REQUIRED'}
 $cmd=$connection.CreateCommand();$cmd.CommandText=$sql;$cmd.CommandTimeout=30;$cmd.Transaction=$tx
 try {$reader=$cmd.ExecuteReader();try{$data=New-Object Data.DataTable;$data.Load($reader);if($data.Rows.Count -gt 50000){throw 'BATCH_LIMIT'};return ,$data}finally{$reader.Dispose()}}finally{$cmd.Dispose()}
}
function Run-Write([string]$sql,[object[]]$parameters) {
 $cmd=$connection.CreateCommand();$cmd.CommandText=$sql;$cmd.CommandTimeout=30;$cmd.Transaction=$tx
 try {
  for($i=0;$i -lt $parameters.Length;$i+=2){$null=$cmd.Parameters.AddWithValue([string]$parameters[$i],$parameters[$i+1])}
  return $cmd.ExecuteNonQuery()
 }finally{$cmd.Dispose()}
}
function Same($a,$b) {
 if($a -is [byte[]] -and $b -is [byte[]]){return [Convert]::ToBase64String($a) -ceq [Convert]::ToBase64String($b)}
 if($a -is [DBNull] -or $b -is [DBNull]){return ($a -is [DBNull] -and $b -is [DBNull])}
 return [object]::Equals($a,$b)
}
function Map-Id($value,$map) {
 if($value -is [DBNull]){return [DBNull]::Value}
 if(-not $map.ContainsKey([string]$value)){throw 'MAPPING_MISSING'}
 return [uint32]$map[[string]$value]
}
try {
 if($Apply -and $PostCutover){throw 'INCOMPATIBLE_MODES'}
 $RepositoryRoot=[IO.Path]::GetFullPath($RepositoryRoot)
 Add-Type -Path (Join-Path $RepositoryRoot 'bin/MySql.Data.dll')
 $jsonDll=Join-Path $RepositoryRoot 'bin/Newtonsoft.Json.dll'
 Add-Type -Path $jsonDll
 Add-Type -TypeDefinition (Get-Content (Join-Path $PSScriptRoot 'Doc110MigrationJson.cs') -Raw) -ReferencedAssemblies @($jsonDll,'System.Core.dll')
 $phase='CONNECT_RA'
 $config=[xml](Get-Content (Join-Path $RepositoryRoot 'Web.config') -Raw)
 $catalog=New-Object Data.Odbc.OdbcConnection
 $catalog.ConnectionString=($config.configuration.connectionStrings.add|Where-Object name -eq 'OdbcServicesGestor').connectionString
 $catalog.Open();$cmd=$catalog.CreateCommand()
 $cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
 $reader=$cmd.ExecuteReader()
 try {
  if(-not $reader.Read()){throw 'RA_MISSING'}
  $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
  $builder.Server=[string]$reader[0];$builder.Database=[string]$reader[1];$builder.UserID=[string]$reader[2];$builder.Password=[string]$reader[3]
  if($reader.Read()){throw 'RA_AMBIGUOUS'}
 }finally{$reader.Dispose();$cmd.Dispose()}
 $connection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString);$connection.Open()
 $phase='COMPARE_SNAPSHOT'
 # Los escritores deben estar detenidos durante todas las etapas manuales.
 $tx=$connection.BeginTransaction([Data.IsolationLevel]::Serializable)
 if(-not $PostCutover){
  $external=Read-Table "SELECT COUNT(*) AS total FROM information_schema.KEY_COLUMN_USAGE WHERE REFERENCED_TABLE_SCHEMA=DATABASE() AND REFERENCED_TABLE_NAME IN ('ra_integracion_perfil','ra_integracion_perfil_version','ra_integracion_perfil_regla') AND (TABLE_SCHEMA<>DATABASE() OR TABLE_NAME NOT IN ('ra_integracion_perfil_version','ra_integracion_perfil_regla'))"
  if([long]$external.Rows[0].total -ne 0){throw 'EXTERNAL_DEPENDENCIES'}
 }
 $oldSuffix=if($PostCutover){'_guid_respaldo'}else{''}
 $newSuffix=if($PostCutover){''}else{'_ai'}
 $tables=@{perfil='ra_integracion_perfil';version='ra_integracion_perfil_version';regla='ra_integracion_perfil_regla'}
 $keys=@{perfil='perfil_id';version='version_id';regla='regla_id'}
 $old=@{};$new=@{};$maps=@{};$index=@{}
 foreach($entity in @('perfil','version','regla')) {
  $table=$tables[$entity];$key=$keys[$entity]
  $metadata=Read-Table "SELECT COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,EXTRA,COLUMN_KEY FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$table$newSuffix'"
  foreach($idColumn in @('perfil_id','version_id','regla_id','version_publicada_id')){
   $columns=@($metadata.Rows|Where-Object {$_.COLUMN_NAME -ceq $idColumn})
   if($columns.Count -eq 0){continue}
   if($columns.Count -ne 1 -or [string]$columns[0].COLUMN_TYPE -notmatch '^int(\([0-9]+\))? unsigned$'){throw 'TARGET_COLUMN_TYPE'}
   $auto=([string]$columns[0].EXTRA).Contains('auto_increment')
   if($auto -ne ($idColumn -ceq $key)){throw 'TARGET_AUTO_INCREMENT'}
   if($idColumn -ceq $key -and [string]$columns[0].COLUMN_KEY -cne 'PRI'){throw 'TARGET_PRIMARY_KEY'}
  }
  $old[$entity]=Read-Table "SELECT * FROM $table$oldSuffix LIMIT 50001"
  $new[$entity]=Read-Table "SELECT * FROM $table$newSuffix LIMIT 50001"
  $mapRows=Read-Table "SELECT anterior_id,nuevo_id FROM ra_integracion_mapa_$entity LIMIT 50001"
  if($old[$entity].Rows.Count -ne $new[$entity].Rows.Count -or $old[$entity].Rows.Count -ne $mapRows.Rows.Count){throw 'COUNT_MISMATCH'}
  $maps[$entity]=@{};$index[$entity]=@{}
  foreach($row in $mapRows.Rows){$maps[$entity].Add([string]$row.anterior_id,[uint32]$row.nuevo_id)}
  foreach($row in $new[$entity].Rows){$index[$entity].Add([string]$row[$key],$row)}
 }
 if($PostCutover){
  foreach($relation in @(@('ra_integracion_perfil_version','perfil_id','ra_integracion_perfil','perfil_id'),@('ra_integracion_perfil_regla','version_id','ra_integracion_perfil_version','version_id'))){
   $fk=Read-Table "SELECT CONSTRAINT_NAME FROM information_schema.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='$($relation[0])' AND COLUMN_NAME='$($relation[1])' AND REFERENCED_TABLE_SCHEMA=DATABASE() AND REFERENCED_TABLE_NAME='$($relation[2])' AND REFERENCED_COLUMN_NAME='$($relation[3])'"
   if($fk.Rows.Count -ne 1){throw 'TARGET_FOREIGN_KEY'}
  }
 }
 foreach($entity in @('perfil','version','regla')) {
  foreach($row in $old[$entity].Rows){
   $targetId=Map-Id $row[$keys[$entity]] $maps[$entity]
   if(-not $index[$entity].ContainsKey([string]$targetId)){throw 'TARGET_MISSING'}
   $target=$index[$entity][[string]$targetId]
   foreach($column in $old[$entity].Columns){
    $name=$column.ColumnName
    if($name -in @('regla_doc','config_hash')){continue}
    $value=$row[$name]
    switch($name){
     'perfil_id' {$value=Map-Id $value $maps.perfil}
     'version_id' {$value=Map-Id $value $maps.version}
     'regla_id' {$value=Map-Id $value $maps.regla}
     'version_publicada_id' {$value=Map-Id $value $maps.version}
    }
    $actual=$target[$name]
    if($name -in @('perfil_id','version_id','regla_id','version_publicada_id') -and $actual -isnot [DBNull]){$actual=[uint32]$actual}
    if(-not (Same $value $actual)){$phase='COMPARE_'+$entity+'_'+$name;throw 'SNAPSHOT_MISMATCH'}
   }
  }
 }
 # Comprueba pertenencia de FK y referencia publicada incluso donde no existe FK física.
 foreach($v in $new.version.Rows){if(-not $index.perfil.ContainsKey([string]$v.perfil_id)){throw 'VERSION_ORPHAN'}}
 foreach($r in $new.regla.Rows){if(-not $index.version.ContainsKey([string]$r.version_id)){throw 'RULE_ORPHAN'}}
 foreach($p in $new.perfil.Rows){
  if($p.version_publicada_id -isnot [DBNull]){
   $v=$index.version[[string]$p.version_publicada_id]
   if($null -eq $v -or $v.perfil_id -ne $p.perfil_id -or $v.estado -cne 'PUBLICADA'){throw 'PUBLISHED_POINTER_INVALID'}
  }
 }
 $changes=New-Object Collections.Generic.List[object]
 $versions=New-Object Collections.Generic.List[object]
 $phase='VERIFY_ORIGINAL_HASHES'
 foreach($v in $old.version.Rows){
  $before=New-Object Collections.Generic.List[string];$after=New-Object Collections.Generic.List[string]
  foreach($r in $old.regla.Rows){
   if([string]$r.version_id -cne [string]$v.version_id){continue}
   $next=[string](Map-Id $r.regla_id $maps.regla)
   $json=[Doc110MigrationJson]::ConvertRule([string]$r.regla_doc,[string]$r.regla_id,$next)
   $before.Add([string]$r.regla_doc);$after.Add($json)
   $current=[string]$index.regla[$next].regla_doc
   if($PostCutover -and $current -cne $json){throw 'POST_JSON_MISMATCH'}
   if(-not $PostCutover -and $current -cne $json -and $current -cne [string]$r.regla_doc){throw 'STAGING_JSON_MISMATCH'}
   $changes.Add(@{id=[uint32]$next;json=$json})
  }
  $hash=[DBNull]::Value
  if($v.estado -ceq 'PUBLICADA'){
   $original=[Doc110MigrationJson]::Hash([string]$v.configuracion_doc,$before.ToArray())
   if(-not (Same $original $v.config_hash)){throw 'ORIGINAL_HASH_MISMATCH'}
   $hash=[Doc110MigrationJson]::Hash([string]$v.configuracion_doc,$after.ToArray())
  }elseif($v.estado -cne 'BORRADOR' -or $v.config_hash -isnot [DBNull]){throw 'VERSION_STATE_INVALID'}
  $newId=Map-Id $v.version_id $maps.version
  $current=$index.version[[string]$newId].config_hash
  if($PostCutover -and -not (Same $current $hash)){throw 'POST_HASH_MISMATCH'}
  if(-not $PostCutover -and -not (Same $current $hash) -and -not (Same $current $v.config_hash)){throw 'STAGING_HASH_MISMATCH'}
  $versions.Add(@{id=$newId;previous=$v.config_hash;next=$hash})
 }
 if($Apply){
  $phase='TRANSFORM_STAGING'
  foreach($r in $changes){$null=Run-Write 'UPDATE ra_integracion_perfil_regla_ai SET regla_doc=@doc WHERE regla_id=@id' @('@doc',$r.json,'@id',$r.id)}
  foreach($v in $versions){
   $null=Run-Write 'UPDATE ra_integracion_perfil_version_ai SET config_hash=@hash WHERE version_id=@id' @('@hash',$v.next,'@id',$v.id)
   $null=Run-Write 'INSERT INTO ra_integracion_migracion_huella (version_id,hash_anterior,hash_nuevo,verificado_utc) VALUES (@id,@old,@new,@utc) ON DUPLICATE KEY UPDATE hash_anterior=VALUES(hash_anterior),hash_nuevo=VALUES(hash_nuevo),verificado_utc=VALUES(verificado_utc)' @('@id',$v.id,'@old',$v.previous,'@new',$v.next,'@utc',[DateTime]::UtcNow)
  }
 }
 if($Apply -or $PostCutover){
  $phase='VERIFY_TRANSFORMED'
  $audit=Read-Table 'SELECT version_id,hash_anterior,hash_nuevo FROM ra_integracion_migracion_huella LIMIT 50001'
  if($audit.Rows.Count -ne $versions.Count){throw 'AUDIT_COUNT'}
  foreach($v in $versions){
   $matches=@($audit.Rows|Where-Object {[string]$_.version_id -ceq [string]$v.id})
   if($matches.Count -ne 1 -or -not (Same $matches[0].hash_anterior $v.previous) -or -not (Same $matches[0].hash_nuevo $v.next)){throw 'AUDIT_MISMATCH'}
  }
  if($Apply){
   $rows=Read-Table 'SELECT regla_id,regla_doc FROM ra_integracion_perfil_regla_ai LIMIT 50001'
   foreach($r in $rows.Rows){$expected=@($changes|Where-Object {[string]$_.id -ceq [string]$r.regla_id});if($expected.Count -ne 1 -or [string]$r.regla_doc -cne $expected[0].json){throw 'WRITTEN_JSON_MISMATCH'}}
   $rows=Read-Table 'SELECT version_id,config_hash FROM ra_integracion_perfil_version_ai LIMIT 50001'
   foreach($v in $rows.Rows){$expected=@($versions|Where-Object {[string]$_.id -ceq [string]$v.version_id});if($expected.Count -ne 1 -or -not (Same $v.config_hash $expected[0].next)){throw 'WRITTEN_HASH_MISMATCH'}}
  }
 }
 if($Apply){$tx.Commit()}else{$tx.Rollback()};$tx.Dispose();$tx=$null
 [pscustomobject]@{resultado='PASS';modo=if($Apply){'STAGING_TRANSFORMED'}elseif($PostCutover){'POST_CUTOVER_READ_ONLY'}else{'PREVIEW_READ_ONLY'};perfiles=$old.perfil.Rows.Count;versiones=$versions.Count;reglas=$changes.Count;fechaUtc=[DateTime]::UtcNow.ToString('o')}|ConvertTo-Json -Compress
}catch{
 if($tx){try{$tx.Rollback()}catch{}}
 $safeCode=if($_.Exception.Message -cmatch '^[A-Z_]+$'){$_.Exception.Message}else{$_.Exception.GetBaseException().GetType().Name}
 Write-Output ('MIGRATION_FAILED phase='+$phase+' code='+$safeCode+' line='+$_.InvocationInfo.ScriptLineNumber+'; No continuar con SQL 04.')
 exit 1
}finally{
 if($tx){$tx.Dispose()};if($connection){$connection.Dispose()};if($catalog){$catalog.Dispose()}
}
