param([ValidateSet('Doc110','Doc111Creation')][string]$Mode='Doc110')
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$catalog=$null;$connection=$null
function Scalar([string]$Sql){
    if($Sql -notmatch '\ASELECT\b' -or $Sql.Contains(';')){throw 'SELECT_REQUIRED'}
    $command=$connection.CreateCommand();$command.CommandText=$Sql;$command.CommandTimeout=5
    try{return $command.ExecuteScalar()}finally{$command.Dispose()}
}
function Rows([string]$Sql,[hashtable]$Parameters=@{}){
    if($Sql -notmatch '\ASELECT\b' -or $Sql.Contains(';')){throw 'SELECT_REQUIRED'}
    $command=$connection.CreateCommand();$command.CommandText=$Sql;$command.CommandTimeout=10
    foreach($key in $Parameters.Keys){[void]$command.Parameters.AddWithValue($key,$Parameters[$key])}
    $reader=$null;$items=New-Object 'System.Collections.Generic.List[object]'
    try{$reader=$command.ExecuteReader();while($reader.Read()){$row=[ordered]@{};for($i=0;$i -lt $reader.FieldCount;$i++){$row[$reader.GetName($i)]=if($reader.IsDBNull($i)){$null}else{$reader.GetValue($i)}};$items.Add([pscustomobject]$row)}}finally{if($reader){$reader.Dispose()};$command.Dispose()}
    return ,$items.ToArray()
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
    if($Mode -eq 'Doc111Creation'){
        # Reutiliza el mismo destino fijado por run-doc111-persistence.ps1.
        $sha=[Security.Cryptography.SHA256]::Create()
        try{$environmentHash=([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($builder.Server+'|'+$builder.Database)))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}
        if($environmentHash -ne '24cce9f4ae03e1b92b7f0cba1e89f395e9b4434f4c108f5ee571ae9515387b91'){throw 'DOC111_ENVIRONMENT_CHANGED'}
    }
    $connection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString);$connection.Open()
    if($Mode -eq 'Doc111Creation'){
        $profile=Rows 'SELECT p.ambito_ref,v.configuracion_doc FROM ra_integracion_perfil p INNER JOIN ra_integracion_perfil_version v ON v.perfil_id=p.perfil_id WHERE p.perfil_id=24 AND v.version_id=38'
        if($profile.Count -ne 1 -or [string]$profile[0].ambito_ref -ne '2'){throw 'DOC111_EXISTING_FIXTURE_REQUIRED'}
        $configuration=$profile[0].configuracion_doc|ConvertFrom-Json
        $parameters=@{'@empresa'=[uint32]2;'@instrumento'=[uint32]$configuration.InstrumentoRef;'@area'=[uint32]$configuration.AreaRef}
        if($parameters['@instrumento'] -eq 0 -or $parameters['@area'] -eq 0){throw 'DOC111_CLASSIFICATION_REQUIRED'}
        $engine=Rows 'SELECT VERSION() AS version_motor,@@SESSION.sql_mode AS modo_sql'
        $columns=Rows "SELECT TABLE_NAME,COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE,COLUMN_DEFAULT FROM information_schema.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='expediente_archivo' AND COLUMN_NAME IN ('ID_EXPEDIENTE','ID_EMPRESA_EXPEDIENTE','VOLUMEN_EXPEDIENTE','estado_expediente_electronico') ORDER BY ORDINAL_POSITION"
        $indexes=Rows "SELECT TABLE_NAME,INDEX_NAME,NON_UNIQUE,SEQ_IN_INDEX,COLUMN_NAME FROM information_schema.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('registro_organigrama','areas_depart_radicacion','ra_de_fondo_documental','ra_de_tipos_ciclos_archivo','ra_ruta_expediente') ORDER BY TABLE_NAME,INDEX_NAME,SEQ_IN_INDEX"
        $routes=[long](Scalar 'SELECT COUNT(*) FROM ra_ruta_expediente')
        $productionArchiveCycle=Rows 'SELECT Id_tipos_ciclo_archivo AS ciclo_id,Nombre_Tipo_ciclo_archivo AS nombre FROM ra_de_tipos_ciclos_archivo WHERE Nombre_Tipo_ciclo_archivo=@nombre LIMIT 2' @{'@nombre'='Archivo Gestión'}
        $duplicates=Rows 'SELECT COUNT(*) AS grupos_ambiguos FROM (SELECT NOMBRE_ORGANIGRAMA FROM registro_organigrama WHERE EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=@empresa GROUP BY NOMBRE_ORGANIGRAMA HAVING COUNT(*)>1) grupos' @{'@empresa'=$parameters['@empresa']}
        $classification=Rows 'SELECT i.id_instrumento,i.registro_organigrama_ID_ORGANIGRAMA AS organigrama,a.Codigo_Area AS area FROM ra_registro_instrumento_archivistico i INNER JOIN registro_organigrama o ON o.ID_ORGANIGRAMA=i.registro_organigrama_ID_ORGANIGRAMA INNER JOIN areas_depart_radicacion a ON a.Registro_Organigrama_Id_Organigrama=o.ID_ORGANIGRAMA WHERE o.EMPRESA_GESTION_DOCUMENTAL_ID_EMPRESA=@empresa AND i.id_instrumento=@instrumento AND a.Codigo_Area=@area LIMIT 2' $parameters
        $report=[pscustomobject]@{status='INSPECTED';mode=$Mode;environmentHash=$environmentHash;fixtureProfileId='24';fixtureVersionId='38';engine=$engine;columns=$columns;indexes=$indexes;routeConfigurations=$routes;productionArchiveCycle=$productionArchiveCycle;ambiguousOrganigramNames=$duplicates;classification=$classification;finishedUtc=[DateTime]::UtcNow.ToString('o');scope='SELECT only; existing configuration/fixture reused; no HTTP, activation, document effects, filesystem routes or credentials in report; inspection does not enable creation'}
        $file=Join-Path $repoRoot 'tools/e2e/artifacts/doc111-creation-compatibility-select.json'
        [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($file))
        [IO.File]::WriteAllText($file,($report|ConvertTo-Json -Depth 8),[Text.Encoding]::UTF8)
        [pscustomobject]@{status=$report.status;mode=$Mode;report='tools/e2e/artifacts/doc111-creation-compatibility-select.json';classificationMatches=$classification.Count;routeConfigurations=$routes;productionArchiveCycleMatches=$productionArchiveCycle.Count}|ConvertTo-Json -Compress
        exit 0
    }
    $outside=[long](Scalar "SELECT COUNT(*) FROM ra_integracion_proveedor_version WHERE conexion_ref IN ('RA','DA') AND tabla_nombre REGEXP '^[A-Za-z_][A-Za-z0-9_]{0,63}$' AND BINARY LEFT(tabla_nombre,6)<>BINARY 'imp_02'")
    $existing=[long](Scalar "SELECT COUNT(*) FROM information_schema.SCHEMATA WHERE SCHEMA_NAME='doc110_instalacion_prueba'")
    $profileTables=[long](Scalar "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('ra_integracion_perfil','ra_integracion_perfil_version','ra_integracion_perfil_regla')")
    $report=[pscustomobject]@{status='INSPECTED';providerVersionsOutsideLiteralPrefix=$outside;proposedInstallSchemaExists=($existing -gt 0);currentProfileTables=$profileTables;finishedUtc=[DateTime]::UtcNow.ToString('o');scope='SELECT only; existence is not acceptance of isolated installation or missing-schema behavior'}
    $file=Join-Path $repoRoot 'tools/e2e/artifacts/doc110-final-prerequisites.json'
    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($file))
    [IO.File]::WriteAllText($file,($report | ConvertTo-Json),[Text.Encoding]::UTF8)
    $report | ConvertTo-Json -Compress
}catch{
    $failureCode=if($Mode -eq 'Doc111Creation'){'DOC111_CREATION_INSPECTION_FAILED'}else{'DOC110_PREREQUISITE_INSPECTION_FAILED'}
    [pscustomobject]@{status='FAIL';code=$failureCode} | ConvertTo-Json -Compress
    exit 1
}finally{if($connection){$connection.Dispose()};if($catalog){$catalog.Dispose()}}
