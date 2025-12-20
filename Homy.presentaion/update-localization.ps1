# Update all .cshtml files to use correct SharedResources namespace
$files = Get-ChildItem -Path "Views" -Filter "*.cshtml" -Recurse

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    
    # Replace the old namespace with the new one
    $newContent = $content -replace 'IStringLocalizer<Homy\.presentaion\.SharedResources>', 'IStringLocalizer<Homy.presentaion.Resources.SharedResources>'
    $newContent = $newContent -replace 'Microsoft\.Extensions\.Localization\.IStringLocalizer<Homy\.presentaion\.SharedResources>', 'Microsoft.Extensions.Localization.IStringLocalizer<Homy.presentaion.Resources.SharedResources>'
    
    if ($content -ne $newContent) {
        Set-Content -Path $file.FullName -Value $newContent -NoNewline
        Write-Host "Updated: $($file.FullName)"
    }
}

Write-Host "Done!"
