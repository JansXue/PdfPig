param (
  [Parameter(Position=0,mandatory=$true)]
  [string]$version
)

$root = (Split-Path -parent $PSCommandPath)

$projs = Get-ChildItem "$root/../src" -Recurse | Where-Object { $_.extension -eq ".csproj" -and $_.name.IndexOf("Tests") -lt 0 }
$projs | ForEach-Object {
  $xml = New-Object XML
  $xml.Load($_.FullName)
  $xml.Project.PropertyGroup[0].Version = $version
  $xml.Save($_.FullName)
}
Write-Host $projs.Length