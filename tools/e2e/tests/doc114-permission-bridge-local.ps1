param([Parameter(Mandatory=$true)][string]$BridgePath,[Parameter(Mandatory=$true)][string]$FixtureDirectory)
$ErrorActionPreference='Stop'
$tokens=$null;$errors=$null
$ast=[Management.Automation.Language.Parser]::ParseFile($BridgePath,[ref]$tokens,[ref]$errors)
if($errors.Count){throw 'DOC114_BRIDGE_PARSE_FAILED'}
foreach($name in @('Get-Doc110OtherHash','Save-Doc110Recovery','Set-Doc110Permission','Restore-Doc110Permission')){
    $node=$ast.Find({param($n) $n -is [Management.Automation.Language.FunctionDefinitionAst] -and $n.Name -eq $name},$true)
    if(-not $node){throw 'DOC114_BRIDGE_FUNCTION_MISSING'}
    . ([scriptblock]::Create($node.Extent.Text))
}
# Solo se ejecutan funciones y la cláusula REVOKE con un proveedor en memoria.
# No se ejecutan inicialización, Web.config, conexión, SELECT ni UPDATE reales.
function Read-Doc110Target {return $script:fixtureRow}
$script:raConnection=[pscustomobject]@{}
$script:raConnection | Add-Member ScriptMethod CreateCommand {
    $command=[pscustomobject]@{CommandText='';CommandTimeout=0;Parameters=[pscustomobject]@{Values=@{}}}
    $command.Parameters | Add-Member ScriptMethod AddWithValue {param($name,$value) $this.Values[$name]=$value}
    $command | Add-Member ScriptMethod ExecuteNonQuery {
        $script:lastSql=$this.CommandText
        if($script:fixtureRow.PSObject.Properties[$script:permissionColumn].Value -cne [string]$this.Parameters.Values['@expected']){return 0}
        $script:fixtureRow.PSObject.Properties[$script:permissionColumn].Value=[string]$this.Parameters.Values['@value']
        return 1
    }
    $command | Add-Member ScriptMethod Dispose {}
    return $command
}
$switch=$ast.Find({param($n) $n -is [Management.Automation.Language.SwitchStatementAst]},$true)
$clause=$switch.Clauses | Where-Object {$_.Item1.Value -eq 'REVOKE'}
if(-not $clause){throw 'DOC114_REVOKE_CLAUSE_MISSING'}
$body=$clause.Item2.Extent.Text
$revoke=[scriptblock]::Create($body.Substring(1,$body.Length-2))
foreach($column in @('ADMINISTRAR_PERFILES_IMPORTACION','IMPORTAR_CARPETAS')){
    $script:permissionColumn=$column
    $script:fixtureRow=[pscustomobject]@{id='1';IMPORTAR_CARPETAS='1';ADMINISTRAR_PERFILES_IMPORTACION='1';Other='original'}
    $script:targetUser='1';$script:targetCompany='2';$script:primaryColumn='id';$script:primaryValue='1';$script:environmentHash='synthetic'
    $script:recoveryPath=Join-Path $FixtureDirectory ($column+'.json');$script:restoreRequired=$false
    $script:otherHash=Get-Doc110OtherHash $script:fixtureRow
    . $revoke | Out-Null
    $record=Get-Content -LiteralPath $script:recoveryPath -Raw | ConvertFrom-Json
    if($record.stage -ne 'REVOKED' -or $record.restored -or $record.permissionColumn -cne $column -or -not $script:restoreRequired){throw 'DOC114_REVOKE_JOURNAL_INVALID'}
    if($script:fixtureRow.PSObject.Properties[$column].Value -cne '0' -or (Get-Doc110OtherHash $script:fixtureRow) -cne $script:otherHash){throw 'DOC114_REVOKE_SCOPE_INVALID'}
    if(-not $script:lastSql.Contains('SET `'+$column+'`=@value') -or -not $script:lastSql.Contains('AND `'+$column+'`=@expected')){throw 'DOC114_COMPARE_AND_SET_MISSING'}
    Restore-Doc110Permission
    $record=Get-Content -LiteralPath $script:recoveryPath -Raw | ConvertFrom-Json
    if(-not $record.restored -or $script:restoreRequired -or $script:fixtureRow.PSObject.Properties[$column].Value -cne '1'){throw 'DOC114_RESTORE_INVALID'}
    $script:fixtureRow.PSObject.Properties[$column].Value='0';$script:restoreRequired=$true;Save-Doc110Recovery 'REVOKED' $false
    $script:fixtureRow.Other='external-change'
    $failed=$false;try{Restore-Doc110Permission}catch{$failed=$true}
    $record=Get-Content -LiteralPath $script:recoveryPath -Raw | ConvertFrom-Json
    if(-not $failed -or $record.restored -or -not $script:restoreRequired){throw 'DOC114_FALSE_RESTORATION_ACCEPTED'}
}
'DOC114_PERMISSION_BRIDGE_LOCAL=PASS; NO_DATABASE=true'
