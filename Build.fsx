// include Fake lib
#r @"./Tools/FAKE.Core/tools/FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open System.IO
open System.Linq

let authors = ["Nripendra Nath Newa"]

let assemblyGuid = "119618c8-98cd-477a-b42f-295256cc95a5"

let copyRight = "Copyright ©  2014 - Nripendra Nath Newa"

let projectName = "xGherkin.net"
let projectDescription = "xGherkin.net is a xspec flavored BDD framework, which tries to strike balance between xspec and xbehave nature. The basic goal of this project is to remain close to plain gherkin as much as feasible given the language constrain (given that we are using c# to write gherkin)."
let projectSummary = "xGherkin.net is a xspec flavored BDD framework that tries to mimic Gherkin syntax as closely as possible."

let packagingRoot = "./packaging/"
let packagingDir = packagingRoot @@ "xGherkin.net"

let releaseNotes = 
    ReadFile "ReleaseNotes.md"
    |> ReleaseNotesHelper.parseReleaseNotes

let baseDir = "./"
let assemblyInfoPath = baseDir + "Src/Properties/AssemblyInfo.cs"
let outdir = baseDir  + "Src/Bin/Release"
let testDir = baseDir  + "Tests/xGherkinTests/Bin/Release"
let packageDir = baseDir  + "lib/net35"
let nuspecFile = baseDir  + "nuspec"


let rec runWithRetries f retries =
    if retries <= 0 then
        f()
    else
        try
            f()
        with
        | exn -> runWithRetries f (retries-1)

// Targets
Target "Clean" (fun _ ->
    CleanDirs [outdir; testDir;]
)

RestorePackages()

open Fake.AssemblyInfoFile

Target "AssemblyInfo" (fun _ ->
    CreateCSharpAssemblyInfo assemblyInfoPath
        [Attribute.Title projectName
         Attribute.Description projectDescription
         Attribute.Guid assemblyGuid
         Attribute.Product projectName
         Attribute.Copyright copyRight
         Attribute.ComVisible false
         Attribute.Version releaseNotes.AssemblyVersion
         Attribute.InformationalVersion (releaseNotes.SemVer.ToString())
         Attribute.FileVersion releaseNotes.AssemblyVersion]
)
//Build Main project
Target "Build" (fun _ ->
 
    !! "Src/*.csproj"
        |> MSBuildRelease outdir "Build"
        |> Log "AppBuild-Output: "
)

Target "BuildTest" (fun _ ->
 
    !! "Tests/xGherkinTests/*.csproj"
        |> MSBuildRelease testDir "Build"
        |> Log "TestBuild-Output: "
)

Target "Test" (fun _ ->
 
    !! (testDir + @"\xGherkinTests.dll") 
      |> xUnit (fun p -> {p with 
                             OutputDir = testDir
                             HtmlOutput = true })
)

Target "CreatePackage" (fun _ ->
    let net35Dir = packagingDir @@ "lib/net35/"
    CleanDirs [packagingRoot]
    CleanDirs [net35Dir]

    // Copy all the package files into a package folder
    CopyFile net35Dir (outdir @@ "xGherkin.dll")
    CopyFile net35Dir (outdir @@ "xGherkin.pdb")
    CopyFile net35Dir (outdir @@ "xGherkin.dll.config")
    CopyFiles packagingDir ["LICENSE"; "README.md"; "ReleaseNotes.md"]


    NuGet (fun p -> 
        {p with
            Authors = authors
            Project = projectName
            Description = projectDescription                               
            OutputPath = packagingRoot
            Summary = projectSummary
            WorkingDir = packagingDir
            Version = releaseNotes.SemVer.ToString()
            ReleaseNotes = toLines releaseNotes.Notes
            //AccessKey = myAccesskey
            Dependencies =
                ["xunit", GetPackageVersion "./packages/" "xunit"]
            Publish = false }) 
            "xGherkin.net.nuspec"
)


// Default target
Target "Default" (fun _ ->
    trace "Completed Build!!"
)

"Clean"
  ==> "AssemblyInfo" 
  ==> "Build"
  ==> "BuildTest"
  ==> "Test"
  ==> "CreatePackage"
  ==> "Default"

// start build
RunTargetOrDefault "Default"