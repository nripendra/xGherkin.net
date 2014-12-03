REM update nuget
"tools\nuget\nuget.exe" "update" "-self"

REM install Fake.Core
"tools\nuget\nuget.exe" "install" "FAKE.Core" "-OutputDirectory" "tools" "-ExcludeVersion" "-version" "3.9.9"

REM copy updated nuget to .nuget folder
copy "tools\nuget\nuget.exe" ".nuget\nuget.exe" /y

REM Build
"tools\FAKE.Core\tools\Fake.exe" "build.fsx" %1