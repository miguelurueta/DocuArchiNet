param([uint32]$UserId=141,[uint32]$CompanyId=2)
$ErrorActionPreference='Stop'
$repoRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../../..'))
$catalog=$null;$connection=$null;$stage='CONNECT'
function Rows([string]$Sql,[hashtable]$Parameters=@{}) {
    if($Sql -notmatch '\ASELECT\b' -or $Sql.Contains(';') -or $Sql -match '\b(INTO|OUTFILE|DUMPFILE|FOR\s+UPDATE|LOCK|SLEEP|BENCHMARK)\b'){throw 'SELECT_REQUIRED'}
    $command=$connection.CreateCommand();$command.CommandText=$Sql;$command.CommandTimeout=10
    foreach($key in $Parameters.Keys){[void]$command.Parameters.AddWithValue($key,$Parameters[$key])}
    $reader=$null;$items=New-Object 'System.Collections.Generic.List[object]'
    try{$reader=$command.ExecuteReader();while($reader.Read()){$row=[ordered]@{};for($i=0;$i -lt $reader.FieldCount;$i++){$row[$reader.GetName($i)]=if($reader.IsDBNull($i)){$null}else{$reader.GetValue($i)}};$items.Add([pscustomobject]$row)}}finally{if($reader){$reader.Dispose()};$command.Dispose()}
    return ,$items.ToArray()
}
try {
    # Reuses catalog resolution from verify-doc110-identities-readonly.ps1.
    Add-Type -Path (Join-Path $repoRoot 'bin/MySql.Data.dll')
    $config=[xml](Get-Content -LiteralPath (Join-Path $repoRoot 'Web.config') -Raw)
    $catalog=New-Object Data.Odbc.OdbcConnection
    $catalog.ConnectionString=($config.configuration.connectionStrings.add|Where-Object name -eq 'OdbcServicesGestor').connectionString
    $catalog.Open();$cmd=$catalog.CreateCommand();$cmd.CommandTimeout=10
    $cmd.CommandText="SELECT IP_SERVER_MODULO,DB_NAME_MODULO,USER_DBMS_MODULO,PASW_DBMS_MODULO FROM gestor_modulos WHERE ESTADO_MODULO=1 AND TIPO_MODULO='GESTOR DOCUMENTAL'"
    $reader=$cmd.ExecuteReader()
    try {
        if(-not $reader.Read()){throw 'MODULE_REQUIRED'}
        $builder=New-Object MySql.Data.MySqlClient.MySqlConnectionStringBuilder
        $builder.Server=[string]$reader[0];$builder.Database=[string]$reader[1];$builder.UserID=[string]$reader[2];$builder.Password=[string]$reader[3]
        if($reader.Read()){throw 'MODULE_AMBIGUOUS'}
    }finally{$reader.Dispose();$cmd.Dispose();$catalog.Dispose();$catalog=$null}
    $sha=[Security.Cryptography.SHA256]::Create()
    try{$environmentHash=([BitConverter]::ToString($sha.ComputeHash([Text.Encoding]::UTF8.GetBytes($builder.Server+'|'+$builder.Database)))).Replace('-','').ToLowerInvariant()}finally{$sha.Dispose()}
    if($environmentHash -ne '24cce9f4ae03e1b92b7f0cba1e89f395e9b4434f4c108f5ee571ae9515387b91'){throw 'ENVIRONMENT_CHANGED'}
    $connection=New-Object MySql.Data.MySqlClient.MySqlConnection($builder.ConnectionString);$connection.Open()
    $parameters=@{'@usuario'=$UserId;'@empresa'=$CompanyId}
    $stage='AUTHORITY'
    $authority=Rows 'SELECT u.Estado_Usuario AS active,u.Empresa_Gestion_Documental_id_empresa AS company,e.ID_EMPRESA AS existingCompany,p.IMPORTAR_CARPETAS AS canImport,p.ADMINISTRAR_PERFILES_IMPORTACION AS canAdmin FROM remit_dest_interno u INNER JOIN empresa_gestion_documental e ON e.ID_EMPRESA=u.Empresa_Gestion_Documental_id_empresa LEFT JOIN remit_dest_interno_perfil_produccion p ON p.remit_dest_interno_idremit_dest_interno=u.id_Remit_Dest_Int WHERE u.id_Remit_Dest_Int=@usuario LIMIT 2' $parameters
    $identityValid=$authority.Count -eq 1 -and [string]$authority[0].active -eq '1' -and [string]$authority[0].company -eq [string]$CompanyId -and [string]$authority[0].existingCompany -eq [string]$CompanyId
    $importGranted=$identityValid -and [string]$authority[0].canImport -eq '1'
    $adminGranted=$identityValid -and [string]$authority[0].canAdmin -eq '1'
    $stage='LEVELS'
    # Same owner-or-explicit-both-rights predicate as ResolverDestino(ABRIR).
    $levels=Rows 'SELECT n.id_nivel FROM ra_pro_niveles n INNER JOIN remit_dest_interno u ON u.id_Remit_Dest_Int=n.remit_dest_interno_id_Remit_Dest_Int WHERE n.estado_nivel=1 AND u.Empresa_Gestion_Documental_id_empresa=@empresa AND (u.id_Remit_Dest_Int=@usuario OR EXISTS(SELECT 1 FROM ra_pro_permisos_niveles p WHERE p.ra_pro_niveles_id_nivel=n.id_nivel AND p.remit_dest_interno_id_Remit_Dest_Int=@usuario AND p.estado_permiso=1 AND p.agregar_expediente=1 AND p.carga_archivo=1))' $parameters
    $stage='HISTORICAL_PROFILE'
    $profile=Rows "SELECT p.activo AS active,p.version_publicada_id AS publishedVersion,v.estado AS versionState FROM ra_integracion_perfil p LEFT JOIN ra_integracion_perfil_version v ON v.perfil_id=p.perfil_id AND v.version_id=p.version_publicada_id WHERE p.perfil_id=24 AND p.ambito_ref=@empresa LIMIT 2" $parameters
    $profileReady=$profile.Count -eq 1 -and [string]$profile[0].active -eq '1' -and [string]$profile[0].versionState -eq 'PUBLICADA'
    $report=[ordered]@{status='INSPECTED';scope='SELECT only; historical fixture account; no HTTP login or permission grants';userId=$UserId;companyId=$CompanyId;environmentMatchesHistorical=$true;identityActiveAndCompanyValid=$identityValid;importGranted=$importGranted;adminGranted=$adminGranted;levelsWithBothNativeRights=$levels.Count;historicalProfileId=24;historicalProfileReady=$profileReady;operationalBasePermission=if($importGranted){'GRANTED'}else{'DENIED'};finishedUtc=[DateTime]::UtcNow.ToString('o')}
    $output=Join-Path $repoRoot 'tools/e2e/artifacts/doc111-permissions-readonly.json'
    [void][IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($output))
    [IO.File]::WriteAllText($output,($report|ConvertTo-Json -Depth 4),[Text.UTF8Encoding]::new($false))
    $report|ConvertTo-Json -Compress
}catch{
    [pscustomobject]@{status='FAIL';stage=$stage;code='DOC111_READONLY_PERMISSION_INSPECTION_FAILED'}|ConvertTo-Json -Compress
    exit 1
}finally{if($connection){$connection.Dispose()};if($catalog){$catalog.Dispose()}}
