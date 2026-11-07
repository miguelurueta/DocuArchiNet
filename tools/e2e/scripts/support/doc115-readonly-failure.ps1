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
