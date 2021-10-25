$fileNames = Get-ChildItem ".\src\app\api\generated\api" -Recurse |
 select -expand fullname

foreach ($filename in $filenames) 
{
  (Get-Content $fileName) -replace "'http://localhost'", "'http://localhost:5000'" | Set-Content $fileName
  (Get-Content $fileName) -replace "localVarHeaders: localVarHeaders,", "headers: localVarHeaders," | Set-Content $fileName
}