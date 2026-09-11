param(
    [ValidateRange(1, 2147483647)][int]$DocumentTypeId = 154,
    [ValidateRange(0, 2147483647)][int]$ProcedureId = 0
)

$ErrorActionPreference = 'Stop'
$confirmation = Read-Host 'Inspeccion DOC-56 de tipologia exclusivamente SELECT en PRUEBAS. Escriba SI para continuar'
if ($confirmation -cne 'SI') { throw 'DOC56_TYPOLOGY_INSPECTION_NOT_AUTHORIZED' }
$user = Read-Host 'Usuario MySQL de solo lectura'
$securePassword = Read-Host 'Contrasena MySQL de solo lectura' -AsSecureString
$pointer = [IntPtr]::Zero
$connection = $null
try {
    $pointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    $password = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($pointer)
    $builder = [System.Data.Odbc.OdbcConnectionStringBuilder]::new()
    $builder['DSN'] = 'workflowdocument'
    $builder['UID'] = $user.Trim()
    $builder['PWD'] = $password
    $connection = [System.Data.Odbc.OdbcConnection]::new($builder.ConnectionString)
    $connection.Open()
    $schemaCommand = $connection.CreateCommand()
    $schemaCommand.CommandText = @'
SELECT table_schema
FROM information_schema.tables
WHERE table_name IN ('ra_dig_tipos_docum_lista_chequeo','tipo_doc_series')
GROUP BY table_schema
HAVING COUNT(DISTINCT table_name)=2
ORDER BY table_schema
'@
    $schemaReader = $schemaCommand.ExecuteReader()
    $schemas = [System.Collections.Generic.List[string]]::new()
    try {
        while ($schemaReader.Read()) {
            $schema = [string]$schemaReader.GetValue(0)
            if ($schema -notmatch '^[A-Za-z0-9_]+$') { throw 'DOC56_TYPOLOGY_SCHEMA_INVALID' }
            $schemas.Add($schema)
        }
    } finally { $schemaReader.Dispose(); $schemaCommand.Dispose() }
    if ($schemas.Count -eq 0) { throw 'DOC56_TYPOLOGY_SCHEMA_NOT_FOUND' }

    $candidate = 0
    foreach ($schema in $schemas) {
        $candidate++
        $command = $connection.CreateCommand()
        $procedureFilter = if ($ProcedureId -gt 0) { '  AND rdt.tipo_doc_entrante_id_Tipo_Doc_Entrante=?' } else { '' }
        $command.CommandText = ((@'
SELECT rdt.ID_TIPO_DOCUMENTAL_CHEQUEO,
       rdt.tipo_doc_entrante_id_Tipo_Doc_Entrante,
       rdt.series_documentales_Id_Series,
       rdt.tipo_doc_series_Id_Tipo_Doc_Series,
       rdt.subseries_documentales_Id_SubSeries,
       rdt.tipos_doc_subseries_Id_Tipos_Doc_SubSerie,
       tds.Descripcion_Documento
FROM `{0}`.`ra_dig_tipos_docum_lista_chequeo` rdt
LEFT JOIN `{0}`.`tipo_doc_series` tds
  ON tds.Id_Tipo_Doc_Series=rdt.tipo_doc_series_Id_Tipo_Doc_Series
WHERE rdt.tipo_doc_series_Id_Tipo_Doc_Series=?
{1}
ORDER BY rdt.tipo_doc_entrante_id_Tipo_Doc_Entrante, rdt.ID_TIPO_DOCUMENTAL_CHEQUEO
'@) -f $schema, $procedureFilter)
        $parameter = $command.CreateParameter()
        $parameter.Value = $DocumentTypeId
        [void]$command.Parameters.Add($parameter)
        if ($ProcedureId -gt 0) {
            $procedureParameter = $command.CreateParameter()
            $procedureParameter.Value = $ProcedureId
            [void]$command.Parameters.Add($procedureParameter)
        }
        $reader = $command.ExecuteReader()
        try {
            if (-not $reader.Read()) {
                Write-Host ('DOC56_TYPOLOGY_CANDIDATE_{0}_154_NOT_FOUND=True' -f $candidate)
                continue
            }
            for ($i = 0; $i -lt $reader.FieldCount; $i++) {
                $value = if ($reader.IsDBNull($i)) { '[NULL]' } else { [string]$reader.GetValue($i) }
                Write-Host ('DOC56_TYPOLOGY_CANDIDATE_{0}_{1}={2}' -f $candidate, $reader.GetName($i).ToUpperInvariant(), $value)
            }
            while ($reader.Read()) {
                Write-Host ('DOC56_TYPOLOGY_CANDIDATE_{0}_ADDITIONAL_CHECKLIST_ID={1}' -f $candidate, [string]$reader.GetValue(0))
            }
        } finally { $reader.Dispose(); $command.Dispose() }
    }
    Write-Host 'DOC56_TYPOLOGY_INSPECTION_COMPLETE'
} catch {
    $safeCode = if ($_.Exception.Message -match '^DOC56_TYPOLOGY_[A-Z0-9_]+$') { $_.Exception.Message } else { 'DOC56_TYPOLOGY_INSPECTION_FAILED' }
    $safeDetail = ($_.Exception.Message -replace '[\r\n\t]+',' ' -replace '(?i)(password|pwd)\s*=\s*[^;\s]+','$1=[REDACTED]')
    if ($safeDetail.Length -gt 300) { $safeDetail = $safeDetail.Substring(0,300) }
    Write-Error "$safeCode`: $safeDetail. No se mostraron credenciales ni datos de conexion."
    exit 2
} finally {
    if ($connection) { $connection.Dispose() }
    if ($pointer -ne [IntPtr]::Zero) { [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($pointer) }
    $password = $null
    $securePassword = $null
}
