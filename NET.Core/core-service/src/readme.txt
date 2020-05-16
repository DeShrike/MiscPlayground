
https://github.com/stevejgordon/IHostedServiceAsAService

https://www.stevejgordon.co.uk/running-net-core-generic-host-applications-as-a-windows-service

dotnet publish --configuration Release



sc create CoreService binPath= "...\bin\Release\netcoreapp2.1\win7-x64\publish\core-service.exe"


sc start CoreService


https://github.com/PeterKottas/DotNetCore.WindowsService
