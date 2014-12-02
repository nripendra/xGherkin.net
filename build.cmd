"tools\nuget\nuget.exe" "update" "-self"
"tools\nuget\nuget.exe" "install" "FAKE.Core" "-OutputDirectory" "tools" "-ExcludeVersion" "-version" "3.9.9"

"tools\FAKE.Core\tools\Fake.exe" "build.fsx"