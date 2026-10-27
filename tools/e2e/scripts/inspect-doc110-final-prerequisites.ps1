$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$catalog=$null;$connection=$null
function Scalar([string]$Sql){
    if($Sql -notmatch '\ASELECT\b' -or $Sql.Contains(';')){throw 'SELECT_REQUIRED'}
    $command=$connection.CreateCommand();$command.CommandText=$Sql;$command.CommandTimeout=5
    try{return $command.ExecuteScalar()}finally{$command.Dispose()}
}
try{
    Add-Type -Path (Join-Path $repoRoot 'bin/MySql.Data.dll')
    $config=[xml](Get-Content -LiteralPath (Join-Path $repoRoot 'Web.config') -Raw)
    $catalog=New-Object Data.Odbc.OdbcConnection
    $catalog.ConnectionString=($config.configuration.connectionStrings.add | Where-Object name -eq 'OdbcServicesGestor').connectionString
    $catalog.Open();$cmd=$catalog.CreateCommand()
    $cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
    $reader=$cmd.ExecuteReader()
    try{
        if(-not $reader.Read()){throw 'MODULE_REQUIRED'}
        $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $builder.Server=[string]$reader[0];$builder.Database=[string]$reader[1];$builder.UserID=[string]$reader[2];$builder.Password=[string]$reader[3]
        if($reader.Read()){throw 'MODULE_AMBIGUOUS'}
    }finally{$reader.Dispose();$cmd.Dispose();$catalog.Dispose();$catalog=$null}
    $connection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString);$connection.Open()
    $outside=[long](Scalar "SELECT COUNT(*) FROM ra_integracion_proveedor_version WHERE conexion_ref IN ('RA','DA') AND tabla_nombre REGEXP '^[A-Za-z_][A-Za-z0-9_]{0,63}$' AND BINARY LEFT(tabla_nombre,6)<>BINARY 'imp_02'")
    $existing=[long](Scalar "SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME='doc110_instalacion_prueba'")
    $profileTables=[long](Scalar "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('ra_integracion_perfil','ra_integracion_perfil_version','ra_integracion_perfil_regla')")
    $report=[pscustomobject]@{status='INSPECTED';providerVersionsOutsideLiteralPrefix=$outside;proposedInstallSchemaExists=($existing -gt 0);currentProfileTables=$profileTables;finishedUtc=[DateTime]::UtcNow.ToString('o');scope='SELECT only; existence is not acceptance of isolated installation or missing-schema behavior'}
    $file=Join-Path $repoRoot 'tools/e2e/artifacts/doc110-final-prerequisites.json'
    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($file))
    [IO.File]::WriteAllText($file,($report | ConvertTo-Json),[Text.Encoding]::UTF8)
    $report | ConvertTo-Json -Compress
}catch{
    [pscustomobject]@{status='FAIL';code='DOC110_PREREQUISITE_INSPECTION_FAILED'} | ConvertTo-Json -Compress
    exit 1
}finally{if($connection){$connection.Dispose()};if($catalog){$catalog.Dispose()}}
