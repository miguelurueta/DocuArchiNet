$ErrorActionPreference = 'Stop'
[Console]::InputEncoding = New-Object System.Text.UTF8Encoding($false)
[Console]::OutputEncoding = New-Object System.Text.UTF8Encoding($false)
$doc120Result = [ordered]@{status='STOPPED';select=0;alterAttempted=$false;alterCompleted=$false;metadataUpdateAttempted=$false;metadataUpdated=$false;credentialsCleared=$false}
$doc120Connection = $null
$doc120Input = $null
function Read-Doc120Rows([string]$sql) {
    if($doc120Result.select -ge 12 -or $sql -notmatch '^SELECT '){throw 'READ_SCOPE_INVALID'}
    $doc120Result.select++
    $command=$doc120Connection.CreateCommand();$command.CommandText=$sql;$command.CommandTimeout=20
    try {$table=New-Object System.Data.DataTable;$reader=$command.ExecuteReader();try{$table.Load($reader)}finally{$reader.Dispose()};return ,$table}
    finally {$command.Dispose()}
}
function Write-Doc120Sql([string]$sql) {
    $command=$doc120Connection.CreateCommand();$command.CommandText=$sql;$command.CommandTimeout=120
    try {return $command.ExecuteNonQuery()}finally{$command.Dispose()}
}
try {
    $doc120Input=[Console]::In.ReadToEnd() | ConvertFrom-Json
    if($doc120Input.authorization -ne 'EGRESOS_CONTABIL_BENEFICIARIO_60_TO_250' -or $doc120Input.dsn -notmatch '^[A-Za-z0-9 _.-]{1,128}$' -or $doc120Input.expectedOriginHash -notmatch '^[a-f0-9]{64}$' -or [string]::IsNullOrWhiteSpace($doc120Input.user) -or [string]::IsNullOrWhiteSpace($doc120Input.password)){throw 'INPUT_INVALID'}
    $doc120Builder=New-Object System.Data.Odbc.OdbcConnectionStringBuilder
    $doc120Builder['DSN']=$doc120Input.dsn;$doc120Builder['UID']=$doc120Input.user;$doc120Builder['PWD']=$doc120Input.password
    $doc120Connection=New-Object System.Data.Odbc.OdbcConnection($doc120Builder.ConnectionString)
    $doc120Connection.Open()
    $doc120Origin=Read-Doc120Rows 'SELECT @@hostname AS host,@@port AS puerto,@@server_id AS servidor,DATABASE() AS base'
    if($doc120Origin.Rows.Count -ne 1){throw 'ORIGIN_CHANGED'}
    $doc120OriginParts=@($doc120Origin.Rows[0].ItemArray | ForEach-Object {[Convert]::ToString($_,[Globalization.CultureInfo]::InvariantCulture)})
    if(($doc120OriginParts -join '') -match '[^\x20-\x7e]'){throw 'ORIGIN_CHANGED'}
    $doc120Newton=Join-Path ([IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\..\..\..'))) 'bin\Newtonsoft.Json.dll'
    $null=[Reflection.Assembly]::LoadFrom($doc120Newton)
    $doc120OriginJson=[Newtonsoft.Json.JsonConvert]::SerializeObject([string[]]$doc120OriginParts)
    $doc120Sha=[Security.Cryptography.SHA256]::Create()
    try{$doc120OriginHash=[BitConverter]::ToString($doc120Sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($doc120OriginJson))).Replace('-','').ToLowerInvariant()}finally{$doc120Sha.Dispose()}
    if($doc120OriginHash -ne $doc120Input.expectedOriginHash){throw 'ORIGIN_CHANGED'}
    $doc120Before=Read-Doc120Rows "SELECT d.TIPO,c.DATA_TYPE,c.COLUMN_TYPE,c.CHARACTER_MAXIMUM_LENGTH,c.CHARACTER_SET_NAME,c.COLLATION_NAME,c.IS_NULLABLE,c.COLUMN_DEFAULT,c.EXTRA,c.COLUMN_COMMENT,t.ENGINE FROM system1 s INNER JOIN detalle_gabienete d ON d.id_gabinete=s.id_gabinete AND d.GABINETE=s.NOMBRE INNER JOIN information_schema.COLUMNS c ON c.TABLE_SCHEMA=DATABASE() AND c.TABLE_NAME=s.NOMBRE AND c.COLUMN_NAME=d.CAMPO INNER JOIN information_schema.TABLES t ON t.TABLE_SCHEMA=c.TABLE_SCHEMA AND t.TABLE_NAME=c.TABLE_NAME WHERE s.id_gabinete=10 AND s.NOMBRE='CONTABIL' AND d.id_detalle_gabinete=17 AND d.CAMPO='BENEFICIARIO' AND d.VISIBLE=1"
    if($doc120Before.Rows.Count -ne 1){throw 'TARGET_CHANGED'}
    $doc120Column=$doc120Before.Rows[0]
    if($doc120Column['COLUMN_TYPE'] -ne 'varchar(60)' -or $doc120Column['CHARACTER_SET_NAME'] -ne 'latin1' -or $doc120Column['COLLATION_NAME'] -ne 'latin1_swedish_ci' -or $doc120Column['IS_NULLABLE'] -ne 'YES' -or -not $doc120Column.IsNull('COLUMN_DEFAULT') -or [string]$doc120Column['EXTRA'] -ne '' -or [string]$doc120Column['COLUMN_COMMENT'] -ne '' -or $doc120Column['TIPO'] -notmatch '^\s*VARCHAR\s*(?:\(\s*60\s*\)|60)\s*$' -or $doc120Column['ENGINE'] -notin @('InnoDB','MyISAM')){throw 'COLUMN_CONTRACT_CHANGED'}
    $doc120Indexes=Read-Doc120Rows "SELECT s.INDEX_NAME,s.INDEX_TYPE,s.COLUMN_NAME,s.SUB_PART,c.DATA_TYPE,c.CHARACTER_OCTET_LENGTH,c.CHARACTER_MAXIMUM_LENGTH FROM information_schema.STATISTICS s INNER JOIN information_schema.COLUMNS c ON c.TABLE_SCHEMA=s.TABLE_SCHEMA AND c.TABLE_NAME=s.TABLE_NAME AND c.COLUMN_NAME=s.COLUMN_NAME WHERE s.TABLE_SCHEMA=DATABASE() AND s.TABLE_NAME='CONTABIL' AND s.INDEX_NAME IN (SELECT x.INDEX_NAME FROM information_schema.STATISTICS x WHERE x.TABLE_SCHEMA=DATABASE() AND x.TABLE_NAME='CONTABIL' AND x.COLUMN_NAME='BENEFICIARIO') ORDER BY s.INDEX_NAME,s.SEQ_IN_INDEX"
    $doc120IndexBytes=@{}
    foreach($doc120Index in $doc120Indexes.Rows){
        if($doc120Index['INDEX_TYPE'] -ne 'BTREE'){throw 'INDEX_REVIEW_REQUIRED'}
        $doc120Size=0
        if($doc120Index['COLUMN_NAME'] -eq 'BENEFICIARIO'){$doc120Size=if($doc120Index.IsNull('SUB_PART')){250}else{[Math]::Min(250,[int]$doc120Index['SUB_PART'])}}
        elseif(-not $doc120Index.IsNull('CHARACTER_OCTET_LENGTH')){
            $doc120Size=[long]$doc120Index['CHARACTER_OCTET_LENGTH']
            if(-not $doc120Index.IsNull('SUB_PART')){$doc120Size=[long]$doc120Index['SUB_PART']*[Math]::Ceiling($doc120Size/[long]$doc120Index['CHARACTER_MAXIMUM_LENGTH'])}
        }elseif($doc120Index['DATA_TYPE'] -in @('tinyint','smallint','mediumint','int','bigint','date','datetime','timestamp')){$doc120Size=8}
        else{throw 'INDEX_REVIEW_REQUIRED'}
        $doc120Name=[string]$doc120Index['INDEX_NAME'];$doc120IndexBytes[$doc120Name]+=$doc120Size
        if($doc120IndexBytes[$doc120Name] -gt 767){throw 'INDEX_CAPACITY_EXCEEDED'}
    }
    $doc120FingerprintSql="SELECT COUNT(*) AS registros,COALESCE(SUM(CRC32(CASE WHEN BENEFICIARIO IS NULL THEN 'N:' ELSE CONCAT('V:',BENEFICIARIO) END)),0) AS suma,COALESCE(BIT_XOR(CRC32(CASE WHEN BENEFICIARIO IS NULL THEN 'N:' ELSE CONCAT('V:',BENEFICIARIO) END)),0) AS xor_valores FROM CONTABIL"
    $doc120DataBefore=Read-Doc120Rows $doc120FingerprintSql
    if($doc120DataBefore.Rows.Count -ne 1){throw 'DATA_CONTROL_FAILED'}
    $doc120Result.alterAttempted=$true
    $null=Write-Doc120Sql 'ALTER TABLE `CONTABIL` MODIFY COLUMN `BENEFICIARIO` VARCHAR(250) CHARACTER SET latin1 COLLATE latin1_swedish_ci NULL DEFAULT NULL'
    $doc120Result.alterCompleted=$true
    $doc120Result.metadataUpdateAttempted=$true
    $doc120Command=$doc120Connection.CreateCommand()
    try{
        $doc120Command.CommandTimeout=20
        $doc120Command.CommandText="UPDATE detalle_gabienete SET TIPO='VARCHAR(250)' WHERE id_detalle_gabinete=17 AND id_gabinete=10 AND GABINETE='CONTABIL' AND CAMPO='BENEFICIARIO' AND TIPO=?"
        $doc120Parameter=$doc120Command.Parameters.Add('@before',[System.Data.Odbc.OdbcType]::VarChar,64);$doc120Parameter.Value=[string]$doc120Column['TIPO']
        if($doc120Command.ExecuteNonQuery() -ne 1){throw 'METADATA_UPDATE_UNCONFIRMED'}
    }finally{$doc120Command.Dispose()}
    $doc120Result.metadataUpdated=$true
    $doc120After=Read-Doc120Rows "SELECT d.TIPO,c.COLUMN_TYPE,c.CHARACTER_SET_NAME,c.COLLATION_NAME,c.IS_NULLABLE,c.COLUMN_DEFAULT,c.EXTRA,c.COLUMN_COMMENT FROM detalle_gabienete d INNER JOIN information_schema.COLUMNS c ON c.TABLE_SCHEMA=DATABASE() AND c.TABLE_NAME=d.GABINETE AND c.COLUMN_NAME=d.CAMPO WHERE d.id_detalle_gabinete=17 AND d.id_gabinete=10 AND d.GABINETE='CONTABIL' AND d.CAMPO='BENEFICIARIO'"
    if($doc120After.Rows.Count -ne 1 -or $doc120After.Rows[0]['COLUMN_TYPE'] -ne 'varchar(250)' -or $doc120After.Rows[0]['TIPO'] -ne 'VARCHAR(250)' -or $doc120After.Rows[0]['CHARACTER_SET_NAME'] -ne 'latin1' -or $doc120After.Rows[0]['COLLATION_NAME'] -ne 'latin1_swedish_ci' -or $doc120After.Rows[0]['IS_NULLABLE'] -ne 'YES' -or -not $doc120After.Rows[0].IsNull('COLUMN_DEFAULT') -or [string]$doc120After.Rows[0]['EXTRA'] -ne '' -or [string]$doc120After.Rows[0]['COLUMN_COMMENT'] -ne ''){throw 'AFTER_CONTRACT_INVALID'}
    $doc120DataAfter=Read-Doc120Rows $doc120FingerprintSql
    foreach($doc120Field in @('registros','suma','xor_valores')){if([string]$doc120DataBefore.Rows[0][$doc120Field] -ne [string]$doc120DataAfter.Rows[0][$doc120Field]){throw 'DATA_FINGERPRINT_CHANGED'}}
    $doc120Result.status='PASS_PHYSICAL_AND_METADATA_UPDATED';$doc120Result.beforeCapacity=60;$doc120Result.afterCapacity=250;$doc120Result.records=[string]$doc120DataAfter.Rows[0]['registros'];$doc120Result.aggregateFingerprintUnchanged=$true
}catch{
    $doc120Allowed=@('INPUT_INVALID','ORIGIN_CHANGED','TARGET_CHANGED','COLUMN_CONTRACT_CHANGED','INDEX_REVIEW_REQUIRED','INDEX_CAPACITY_EXCEEDED','DATA_CONTROL_FAILED','METADATA_UPDATE_UNCONFIRMED','AFTER_CONTRACT_INVALID','DATA_FINGERPRINT_CHANGED','READ_SCOPE_INVALID')
    $doc120Result.failure=if($_.Exception.Message -in $doc120Allowed){$_.Exception.Message}else{'DATABASE_OPERATION_UNCONFIRMED'}
    $doc120Exit=2
}finally{
    if($null -ne $doc120Connection){$doc120Connection.Dispose()}
    if($null -ne $doc120Input){$doc120Input.password=$null;$doc120Input.user=$null;$doc120Input.dsn=$null}
    if($null -ne $doc120Builder){$doc120Builder.Clear()}
    $doc120Result.credentialsCleared=$true
    $doc120Result | ConvertTo-Json -Depth 5 -Compress
}
if($doc120Exit -eq 2){exit 2}
