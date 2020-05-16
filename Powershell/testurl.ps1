$url = "https://tradews.outilac-demunter.be/Centric/CS/Trade/csprod/SalesPriceService.svc"

Write-Host ("GET " + $url)

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

try
{
	Write-Host ("TRY")
    $Response = Invoke-WebRequest -Uri $url -ErrorAction Stop -Method GET
	Write-Host ("DONE")
    # This will only execute if the Invoke-WebRequest is successful.
    $StatusCode = $Response.StatusCode
	Write-Host ("ENDTRY")
}
catch
{
	Write-Host ("ERROR")
	$_.Exception
    $StatusCode = $_.Exception.Response.StatusCode.value__
}
$StatusCode
