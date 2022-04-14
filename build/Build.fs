open Fake
open Fake.Core
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open BlackFox.Fake
open BuildHelpers
open BlackFox.CommandLine
open BlackFox
open System
open System.Text.RegularExpressions
open Fake.Api

let cwd = System.Environment.CurrentDirectory

let demoPath = Path.getFullName "demo"
let srcPath = Path.getFullName "src"

let gitOwner = "glutinum-org"
let gitName = "Glutinum.Feliz.ReactResizeDetector"

[<EntryPoint>]
let main args =
    BuildTask.setupContextFromArgv args

    let clean = BuildTask.create "Clean" [] {
        [
            demoPath </> "bin"
            demoPath </> "obj"
            srcPath </> "bin"
            srcPath </> "obj"
        ]
        |> Shell.cleanDirs

        !! (Glob.fableJs demoPath)
        ++ (Glob.fableJs srcPath)
        |> Seq.iter Shell.rm
    }

    let npmInstall = BuildTask.create "NpmInstall" [] {
        run npm "install" cwd
    }

    let watchDemo = BuildTask.create "WatchDemo" [ npmInstall; clean ] {
        // All for graceful shutdown on Ctrl+C while the processes are running
        Console.CancelKeyPress.AddHandler(fun _ ea ->
            ea.Cancel <- true
            printfn "Received Ctrl+C, shutting down..."
            Environment.Exit(0)
        )

        [
            "vite", npx "vite dev" demoPath
            "fable", dotnet "fable --watch" demoPath
        ]
        |> runParallel
    }

    let build = BuildTask.create "Build" [ npmInstall; clean ] {
        run dotnet "build" srcPath
    }

    let release = BuildTask.create "Release" [ ] {
        let nugetKey =
            match Environment.environVarOrNone "nuget_key" with
            | Some nugetKey ->
                nugetKey

            | None ->
                failwith "nuget_key environment variable is not set"

        let githubToken =
            match Environment.environVarOrNone "github_token" with
            | Some githubKey ->
                githubKey

            | None ->
                failwith "github_token environment variable is not set"

        let (stdout, _) =
            dotnet "pack -c Release" srcPath
            |> Proc.runWithCaptureOutputAndRedirect

        let m = Regex.Match(stdout, ".*'(?<nupkg_path>.*\.(?<version>.*\..*\..*)\.nupkg)'")

        if not m.Success then
            failwith "Could not find nupkg file in output"
        else
            let nupkgFile = m.Groups["nupkg_path"].Value
            let version =
                m.Groups["version"].Value
                |> SemVersion.SemanticVersion.Parse

            let (_, _, changelogData) =
                Changelog.findEntry "CHANGELOG.md" version

            let releaseNote =
                Option.map Changelog.Util.allReleaseNotesFor changelogData
                |> Option.defaultValue ""

            let args =
                CmdLine.empty
                |> CmdLine.append "push"
                |> CmdLine.append nupkgFile
                |> CmdLine.appendPrefix "--source" "nuget.org"
                |> CmdLine.appendPrefix "--api-key" nugetKey
                |> CmdLine.toString

            run dotnet args cwd

            let tagVersion = $"v{(version.ToString())}"

            GitHub.createClientWithToken githubToken
            |> GitHub.draftNewRelease gitOwner gitName tagVersion (String.IsNullOrEmpty version.Prerelease) [ releaseNote ]
            |> GitHub.publishDraft
            |> Async.RunSynchronously
    }

    BuildTask.runOrList ()
    0
