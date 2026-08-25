param(
    [Parameter(Mandatory = $true)]
    [string]$ProfilePath
)

$ErrorActionPreference = 'Stop'

try {
    $profile = Get-Content -LiteralPath $ProfilePath -Raw | ConvertFrom-Json
    $loginUri = [Uri]::new([Uri]$profile.baseUrl, 'gestor.aspx')
    $request = [System.Net.WebRequest]::Create($loginUri)
    $request.Method = 'GET'
    $request.Timeout = 30000
    $response = $request.GetResponse()
    try {
        $reader = New-Object System.IO.StreamReader($response.GetResponseStream())
        try {
            $html = $reader.ReadToEnd()
        } finally {
            $reader.Dispose()
        }
    } finally {
        $response.Dispose()
    }

    $select = [regex]::Match($html, '(?is)<select\b[^>]*\bid=["\x27]ContentPlacenter_DropDownListmodulos["\x27][^>]*>(.*?)</select>')
    if (-not $select.Success) {
        [Console]::Out.WriteLine('LOGIN_MODULE_SELECT_NOT_FOUND')
        exit 0
    }
    $options = [regex]::Matches($select.Groups[1].Value, '(?is)<option\b[^>]*\bvalue=["\x27]([^"\x27]+)["\x27][^>]*>(.*?)</option>')
    if ($options.Count -eq 0) {
        [Console]::Out.WriteLine('LOGIN_MODULE_OPTIONS_EMPTY')
        exit 0
    }
    foreach ($option in $options) {
        $value = $option.Groups[1].Value.Trim()
        $label = ([regex]::Replace($option.Groups[2].Value, '<[^>]+>', '')).Trim()
        if ($value -match '^[A-Za-z0-9_.-]{1,80}$' -and $label.Length -le 120) {
            [Console]::Out.WriteLine(('MODULE_VALUE={0}; LABEL={1}' -f $value, $label))
        }
    }
} catch {
    [Console]::Out.WriteLine('LOGIN_MODULE_OPTIONS_UNAVAILABLE')
    exit 1
}
