param([switch]$Apply)
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$connection=$null;$catalog=$null;$transaction=$null;$stage='PREFLIGHT'
$code='E2E-LEGACY-SCOPE-DOC110'
$table='doc110_fuera_prefijo_no_existe'
$reportPath=Join-Path $repoRoot 'tools/e2e/artifacts/doc110-legacy-provider-fixture-result.json'
function Command([string]$Sql,[hashtable]$Parameters=@{}) {
    $command=$connection.CreateCommand();$command.CommandText=$Sql;$command.CommandTimeout=10
    if($script:transaction){$command.Transaction=$script:transaction}
    foreach($key in $Parameters.Keys){[void]$command.Parameters.AddWithValue($key,$Parameters[$key])}
    return $command
}
function Rows([string]$Sql,[hashtable]$Parameters=@{}) {
    $command=Command $Sql $Parameters;$reader=$command.ExecuteReader()
    $rows=New-Object 'System.Collections.Generic.List[object]'
    try {while($reader.Read()){
        $row=[ordered]@{}
        for($i=0;$i -lt $reader.FieldCount;$i++){$row[$reader.GetName($i)]=if($reader.IsDBNull($i)){$null}else{$reader.GetValue($i)}}
        $rows.Add([pscustomobject]$row)
    }}finally{$reader.Dispose();$command.Dispose()}
    return ,$rows.ToArray()
}
try {
    Add-Type -Path (Join-Path $repoRoot 'bin/MySql.Data.dll')
    $config=[xml](Get-Content -LiteralPath (Join-Path $repoRoot 'Web.config') -Raw)
    $additional=@($config.configuration.appSettings.add | Where-Object key -eq 'Doc110.Fuentes.TablasAdicionales')
    if($additional.Count -gt 0 -and -not [string]::IsNullOrWhiteSpace($additional[0].value) -and $additional[0].value.Trim() -ne '[]'){throw 'EXCEPTIONS_CONFIGURED'}
    $catalog=New-Object Data.Odbc.OdbcConnection
    $catalog.ConnectionString=($config.configuration.connectionStrings.add | Where-Object name -eq 'OdbcServicesGestor').connectionString
    $catalog.Open();$cmd=$catalog.CreateCommand()
    $cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
    $reader=$cmd.ExecuteReader()
    try {
        if(-not $reader.Read()){throw 'MODULE_REQUIRED'}
        $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $builder.Server=[string]$reader[0];$builder.Database=[string]$reader[1];$builder.UserID=[string]$reader[2];$builder.Password=[string]$reader[3]
        if($reader.Read()){throw 'MODULE_AMBIGUOUS'}
    }finally{$reader.Dispose();$cmd.Dispose();$catalog.Dispose();$catalog=$null}
    $connection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString);$connection.Open()
    $users=Rows 'SELECT Id_Remit_Dest_Int AS usuario,Empresa_Gestion_Documental_id_empresa AS empresa,Estado_Usuario AS activo FROM remit_dest_interno WHERE Login_Usuario=@login LIMIT 2' @{'@login'='LUZ.AGUILERA'}
    if($users.Count -ne 1 -or [int]$users[0].activo -ne 1 -or [string]$users[0].usuario -ne '141' -or [string]$users[0].empresa -ne '2'){throw 'IDENTITY_CHANGED'}
    $permissions=Rows 'SELECT ADMINISTRAR_PERFILES_IMPORTACION AS perfiles,ADMINISTRAR_PROVEEDORES_IMPORTACION AS proveedores FROM remit_dest_interno_perfil_produccion WHERE remit_dest_interno_idremit_dest_interno=@user LIMIT 2' @{'@user'=$users[0].usuario}
    if($permissions.Count -ne 1 -or [int]$permissions[0].perfiles -ne 1 -or [int]$permissions[0].proveedores -ne 1){throw 'PERMISSION_REQUIRED'}
    $physical=Rows 'SELECT COUNT(*) AS cantidad FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@table' @{'@table'=$table}
    if([int]$physical[0].cantidad -ne 0){throw 'TABLE_MUST_NOT_EXIST'}
    $existing=Rows 'SELECT proveedor_id FROM ra_integracion_proveedor WHERE codigo=@code LIMIT 2' @{'@code'=$code}
    if($existing.Count -ne 0){throw 'FIXTURE_ALREADY_EXISTS_NO_CHANGE'}
    $engines=Rows "SELECT ENGINE FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME IN ('ra_integracion_proveedor','ra_integracion_proveedor_version')"
    if($engines.Count -ne 2 -or @($engines | Where-Object ENGINE -ne 'InnoDB').Count -ne 0){throw 'TRANSACTIONAL_TABLES_REQUIRED'}
    if(-not $Apply){[pscustomobject]@{status='READY';synthetic=$true;inactive=$true;tableAbsent=$true;exceptionsConfigured=$false} | ConvertTo-Json -Compress;exit 0}
    # Explicitly authorized test fixture. Deliberately bypasses UI validation to
    # represent an old persisted contract; never an operational provider/source.
    $stage='CREATE';$transaction=$connection.BeginTransaction()
    $parameters=@{'@ambito'=[string]$users[0].empresa;'@user'=[string]$users[0].usuario;'@code'=$code;'@table'=$table}
    $command=Command "INSERT INTO ra_integracion_proveedor(ambito_ref,codigo,nombre,activo,revision,creado_por,creado_utc) VALUES(@ambito,@code,'Fixture legado sintetico DOC-110',0,1,@user,UTC_TIMESTAMP())" $parameters
    try {if($command.ExecuteNonQuery() -ne 1){throw 'HEADER_FAILED'};$providerId=[string]$command.LastInsertedId}finally{$command.Dispose()}
    $parameters['@provider']=$providerId
    $parameters['@doc']='[{"Nombre":"valor","Columna":"valor","Tipo":"TEXTO"}]'
    $command=Command "INSERT INTO ra_integracion_proveedor_version(proveedor_id,numero,tipo,conexion_ref,tabla_nombre,clave_columna,clave_tipo,empresa_columna,esquema_config,propiedades_doc,creado_por,creado_utc,estado,revision) VALUES(@provider,1,'TABLA','RA',@table,'numero','TEXTO','empresa',1,@doc,@user,UTC_TIMESTAMP(),'BORRADOR',1)" $parameters
    try {if($command.ExecuteNonQuery() -ne 1){throw 'VERSION_FAILED'};$versionId=[string]$command.LastInsertedId}finally{$command.Dispose()}
    $check=Rows "SELECT p.activo,v.estado,v.revision,p.revision AS revision_proveedor FROM ra_integracion_proveedor p JOIN ra_integracion_proveedor_version v ON v.proveedor_id=p.proveedor_id WHERE p.proveedor_id=@provider AND v.version_id=@version AND p.codigo=@code AND p.ambito_ref=@ambito AND v.tabla_nombre=@table" @{'@provider'=$providerId;'@version'=$versionId;'@code'=$code;'@ambito'=[string]$users[0].empresa;'@table'=$table}
    if($check.Count -ne 1 -or [int]$check[0].activo -ne 0 -or $check[0].estado -cne 'BORRADOR' -or [int]$check[0].revision -ne 1 -or [int]$check[0].revision_proveedor -ne 1){throw 'FIXTURE_VERIFICATION_FAILED'}
    $transaction.Commit();$transaction.Dispose();$transaction=$null;$stage='COMMITTED'
    $report=[ordered]@{status='CREATED';synthetic=$true;inactive=$true;state='BORRADOR';providerId=$providerId;versionId=$versionId;code=$code;table=$table;connectionRef='RA';tableAbsent=$true;exceptionsConfigured=$false;finishedUtc=[DateTime]::UtcNow.ToString('o');scope='Synthetic legacy fixture only; no source authorization, publication, business row query or physical table creation'}
    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($reportPath))
    [IO.File]::WriteAllText($reportPath,($report | ConvertTo-Json),[Text.Encoding]::UTF8)
    $report | ConvertTo-Json -Compress
}catch{
    if($transaction){try{$transaction.Rollback()}catch{};$transaction.Dispose();$transaction=$null}
    [pscustomobject]@{status='FAIL';code='DOC110_LEGACY_FIXTURE_STOPPED';stage=$stage;committed=($stage -eq 'COMMITTED')} | ConvertTo-Json -Compress
    exit 1
}finally{if($connection){$connection.Dispose()};if($catalog){$catalog.Dispose()}}
