using System;
using System.IO;
using System.Linq;
using System.Text;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using Nuke.Common.Tools.OctoVersion;
using Serilog;

[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    const string CiBranchNameEnvVariable = "OCTOVERSION_CurrentBranch";
    
    readonly Configuration Configuration = Configuration.Release;

    [Solution] readonly Solution Solution;

    [Parameter("Whether to auto-detect the branch name - this is okay for a local build, but should not be used under CI.")] 
    readonly bool AutoDetectBranch = IsLocalBuild;
    
    [Parameter("Branch name for OctoVersion to use to calculate the version number. Can be set via the environment variable " + CiBranchNameEnvVariable + ".", Name = CiBranchNameEnvVariable)]
    string BranchName { get; set; }
    
    [OctoVersion(BranchMember = nameof(BranchName), AutoDetectBranchMember = nameof(AutoDetectBranch), Framework = "net6.0")]
    public OctoVersionInfo OctoVersionInfo;
    
    static AbsolutePath SourceDirectory => RootDirectory / "source";
    static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    static AbsolutePath PublishDirectory => RootDirectory / "publish";
    static AbsolutePath LocalPackagesDir => RootDirectory / ".." / "LocalPackages";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj", "**/TestResults").ForEach(d => d.DeleteDirectory());
            ArtifactsDirectory.CreateOrCleanDirectory();
            PublishDirectory.CreateOrCleanDirectory();
        });
    
    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            Log.Information("Building Octopus Data v{0}", OctoVersionInfo.FullSemVer);

            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetVersion(OctoVersionInfo.FullSemVer)
                .EnableNoRestore());
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore());
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Produces(ArtifactsDirectory / "*.nupkg")
        .Executes(() =>
        {
            Log.Information("Packing Octopus Data v{0}", OctoVersionInfo.FullSemVer);
            
            // This is done to pass the data to github actions
            GitHubActionsSetOutput("semver", OctoVersionInfo.FullSemVer);
            GitHubActionsSetOutput("prerelease_tag", OctoVersionInfo.PreReleaseTagWithDash);
            
            DotNetPack(_ => _
                .SetProject(Solution)
                .SetVersion(OctoVersionInfo.FullSemVer)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .EnableNoBuild()
                .DisableIncludeSymbols()
                .SetVerbosity(DotNetVerbosity.Normal)
                .SetProperty("NuspecProperties", $"Version={OctoVersionInfo.FullSemVer}"));
        });

    Target CopyToLocalPackages => _ => _
        .OnlyWhenStatic(() => IsLocalBuild)
        .TriggeredBy(Pack)
        .Executes(() =>
        {
            LocalPackagesDir.CreateDirectory();
            ArtifactsDirectory.GlobFiles("*.nupkg")
                .ForEach(package =>
                {
                    CopyFileToDirectory(package, LocalPackagesDir);
                });
        });

    Target OutputPackagesToPush => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {
            var nupkgs = ArtifactsDirectory.GlobFiles("*.nupkg");
            Assert.NotEmpty(nupkgs);
            
            var artifactPaths = nupkgs.Select(p => p.ToString());

            GitHubActionsSetOutput("packages_to_push", string.Join(',', artifactPaths));
        });

    Target Default => _ => _
        .DependsOn(OutputPackagesToPush);

    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Default);

    static void GitHubActionsSetOutput(string name, string value) => GitHubActionsWriteToEnvFile("GITHUB_OUTPUT", name, value);
    
    static void GitHubActionsWriteToEnvFile(string envFile, string name, string value)
    {
        if (value.Contains(Environment.NewLine)) throw new ArgumentException("multi-line output not supported yet");
        
        var outputEnvFile = Environment.GetEnvironmentVariable(envFile);
        if (outputEnvFile == null)
        {
            Console.WriteLine("{0} env var is not defined: Cannot write env var {1}={2}", envFile, name, value);
            return;
        }

        using var fileStream = new FileStream(outputEnvFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete);
        using var writer = new StreamWriter(fileStream, Encoding.UTF8);
        writer.WriteLine($"{name}={value}");
    }
}