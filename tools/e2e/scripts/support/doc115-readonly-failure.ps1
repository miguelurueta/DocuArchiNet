# Closed ODBC classification, following the existing DOC-32 SELECT helper.
# Inputs contain SQLSTATE/native codes only; never SQL, credentials or messages.
function Get-Doc115ReadOnlyFailure([string]$Step,[string[]]$States=@(),[int[]]$Native=@()) {
    if($States -contains '28000' -or $Native -contains 1045){return 'DOC115_READONLY_AUTHENTICATION_FAILED'}
    if($States -contains 'IM002'){return 'DOC115_READONLY_DSN_UNAVAILABLE'}
    if($States -contains 'IM003'){return 'DOC115_READONLY_DRIVER_UNAVAILABLE'}
    if($Native -contains 1044){return 'DOC115_READONLY_DATABASE_ACCESS_DENIED'}
    if($Native -contains 1049){return 'DOC115_READONLY_DATABASE_UNAVAILABLE'}
    if(@($States|Where-Object {$_ -like '08*' -or $_ -like 'HYT*'}).Count -gt 0){return 'DOC115_READONLY_CONNECTION_UNAVAILABLE'}
    if($Native -contains 1142 -or $Native -contains 1143){return 'DOC115_READONLY_SELECT_DENIED'}
    if($Step -eq 'OPEN_CONNECTION'){return 'DOC115_READONLY_OPEN_FAILED'}
    if($States -contains '42S22'){return 'DOC115_READONLY_COLUMN_UNAVAILABLE'}
    if($States -contains '42S02'){return 'DOC115_READONLY_TABLE_UNAVAILABLE'}
    return 'DOC115_READONLY_QUERY_FAILED'
}

# Resolve public DSN target metadata only. OdbcConnection.DataSource is a driver
# description and is not a reliable MySQL Server value. Never read UID/PWD.
function Resolve-Doc115DsnTarget([string]$Dsn,[string]$ExpectedHash,[scriptblock]$ReadTarget=$null) {
    if($Dsn -notmatch '\A[A-Za-z0-9 _.-]{1,128}\z' -or $ExpectedHash -notmatch '\A[a-f0-9]{64}\z'){throw 'DOC115_DSN_TARGET_INVALID'}
    if($null -eq $ReadTarget){$ReadTarget={param($path)if(Test-Path -LiteralPath $path){$key=Get-Item -LiteralPath $path;return @{server=[string]$key.GetValue('SERVER');database=[string]$key.GetValue('DATABASE')}}}}
    $matches=@()
    foreach($base in @('HKCU:\Software\ODBC\ODBC.INI','HKLM:\SOFTWARE\ODBC\ODBC.INI','HKLM:\SOFTWARE\WOW6432Node\ODBC\ODBC.INI')){
        $target=& $ReadTarget (Join-Path $base $Dsn)
        if($null -eq $target){continue}
        if([string]::IsNullOrWhiteSpace($target.server) -or [string]::IsNullOrWhiteSpace($target.database)){throw 'DOC115_DSN_TARGET_INVALID'}
        $hash=[Security.Cryptography.SHA256]::Create()
        try{$fingerprint=([BitConverter]::ToString($hash.ComputeHash([Text.Encoding]::UTF8.GetBytes($target.server+'|'+$target.database)))).Replace('-','').ToLowerInvariant()}finally{$hash.Dispose()}
        if($fingerprint -cne $ExpectedHash){throw 'DOC115_DSN_TARGET_CHANGED'}
        $matches+=@{server=[string]$target.server;database=[string]$target.database}
    }
    if($matches.Count -eq 0){throw 'DOC115_DSN_TARGET_MISSING'}
    return $matches[0]
}
