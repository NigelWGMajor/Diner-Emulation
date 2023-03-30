# A code generator for the project scaffold for the diner emulator
[CmdletBinding()]
param (
    [Parameter(Mandatory, HelpMessage="The solution name is required.")]
    [string]
    $slnName
)
./New-ImportLibrary.ps1 'KitchenService' "$slnName"
./New-ImportLibrary.ps1 'RestaurantService' "$slnName"
./New-ImportLibrary.ps1 'WorkflowManager' "$slnName"
./New-ConsoleApp.ps1 'ModelTester' "$slnName"
./New-RazorApp.ps1 'Emulator' "$slnName"
Start-Process "..\$slnName\$slnName.sln"