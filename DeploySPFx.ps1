[CmdletBinding()]
param (
    [Parameter(Mandatory = $true)]
    [string] $defaultWorkingDirectory,
    
    # source alias for the build pipeline as defined in the release pipeline artifact
    [Parameter(Mandatory = $false)]
    [string] $buildArtifactsSourceAlias = "_DW.Tasks.SPO.Dashboard_SPFx",
    
    # stored as a variable in the 'SPFx Deploy' variable group
    # refererences the app registration (also used for the DevOps service connection)
    [Parameter(Mandatory = $true)]
    [string] $clientId,
    
    # stored as a variable in the 'SPFx Deploy' variable group
    # the tenant ID for the app registration (also used for the DevOps service connection)
    [Parameter(Mandatory = $true)]
    [string] $tenantId,
    
    # stored as a key vault variable in the 'SPFx KeyVault Secrets' variable group
    # references a certificate (stored in the key vault) that is used to authenticate to the app registration (as defined in clientId & tenantId)
    # this app registration provides the required SPO permissions to an an app to the App Catalog
    [Parameter(Mandatory = $true)]
    [string] $spfxDevOpsPipeline,

    [Parameter(Mandatory = $false)]
    [string] $rootSiteUrl = "https://dwdev365.sharepoint.com",
    
    [Parameter(Mandatory = $false)]
    [string] $beezySiteRelativeUrl = "sites/beezy",
    
    [Parameter(Mandatory = $false)]
    [string] $spfxPackageName = "dw-tasks-spo-dashboard.sppkg"    
)

# arguments to call inline PS script:
# -defaultWorkingDirectory "$(System.DefaultWorkingDirectory)" -buildArtifactsSourceAlias "_DW.Tasks.SPO.Dashboard_SPFx" -clientId $(clientId) -tenantId $(tenantId) -spfxDevOpsPipeline $(SPFxDevOpsPipeline) -rootSiteUrl "https://dwdev365.sharepoint.com" -beezySiteRelativeUrl "sites/beezy" -spfxPackageName "dw-tasks-spo-dashboard.sppkg"

Install-Module PnP.PowerShell -AllowPrerelease -Scope "CurrentUser" -Verbose -AllowClobber -Force
            
# Test by getting site name
Connect-PnPOnline -Url "$rootSiteUrl/$beezySiteRelativeUrl" -ClientId $clientId -Tenant $tenantId -CertificateBase64Encoded $spfxDevOpsPipeline      
$web = Get-PnPWeb
Write-Host "Site title: $($web.Title)"
            
# Deploy & Publish the SPFx package
Connect-PnPOnline -Url $rootSiteUrl -ClientId $clientId -Tenant $tenantId -CertificateBase64Encoded $spfxDevOpsPipeline      
Write-Host "Package: $spfxPackageName"      
Add-PnPApp -Path "$defaultWorkingDirectory/$buildArtifactsSourceAlias/drop/$spfxPackageName" -Overwrite -Publish
