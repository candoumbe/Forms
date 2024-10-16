namespace Forms.ContinuousIntegration
{
    using Candoumbe.Pipelines;
    using Candoumbe.Pipelines.Components;
    using Candoumbe.Pipelines.Components.GitHub;
    using Candoumbe.Pipelines.Components.NuGet;
    using Candoumbe.Pipelines.Components.Workflows;

    using Nuke.Common;
    using Nuke.Common.CI;
    using Nuke.Common.CI.GitHubActions;
    using Nuke.Common.Execution;
    using Nuke.Common.IO;
    using Nuke.Common.ProjectModel;
    using Nuke.Common.Tools.DotNet;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using static Nuke.Common.IO.PathConstruction;

    /// <summary>
    /// Defines the pipeline used to build the project
    /// </summary>
    [GitHubActions(
        "integration",
        GitHubActionsImage.UbuntuLatest,
        FetchDepth = 0,
        OnPushBranchesIgnore = new[] { IHaveMainBranch.MainBranchName },
        PublishArtifacts = true,
        InvokedTargets = new[] { nameof(IUnitTest.UnitTests), nameof(IPack.Pack) },
        CacheKeyFiles = new[] { "global.json", "src/**/*.csproj", "test/**/*.csproj" },
        ImportSecrets = new[]
        {
            nameof(NugetApiKey),
            nameof(IReportCoverage.CodecovToken),
            nameof(IMutationTest.StrykerDashboardApiKey)
        },
        OnPullRequestExcludePaths = new[]
        {
            "docs/*",
            "README.md",
            "CHANGELOG.md",
            "LICENSE"
        }
    )]
    [GitHubActions(
        "delivery",
        GitHubActionsImage.UbuntuLatest,
        FetchDepth = 0,
        OnPushBranches = new[] { IHaveMainBranch.MainBranchName },
        InvokedTargets = new[] { nameof(IUnitTest.UnitTests), nameof(IPushNugetPackages.Publish), nameof(ICreateGithubRelease.AddGithubRelease) },
        EnableGitHubToken = true,
        CacheKeyFiles = new[] { "global.json", "src/**/*.csproj", "test/**/*.csproj" },
        PublishArtifacts = true,
        ImportSecrets = new[]
        {
            nameof(NugetApiKey),
            nameof(IReportCoverage.CodecovToken),
            nameof(IMutationTest.StrykerDashboardApiKey)
        },
        OnPullRequestExcludePaths = new[]
        {
            "docs/*",
            "README.md",
            "CHANGELOG.md",
            "LICENSE"
        }
    )]

    public class Build : EnhancedNukeBuild,
        IHaveSolution,
        IHaveSourceDirectory,
        IHaveTestDirectory,
        IGitFlowWithPullRequest,
        IClean,
        IRestore,
        ICompile,
        IUnitTest,
        IMutationTest,
        IBenchmark,
        IReportCoverage,
        IPack,
        IPushNugetPackages,
        ICreateGithubRelease
    {
        /// <summary>
        /// ApiKey required to publish packages to NuGet
        /// </summary>
        [Parameter("API key used to publish artifacts to Nuget.org")]
        [Secret]
        public readonly string NugetApiKey;

        [Solution]
        [Required]
        public readonly Solution Solution;

        ///<inheritdoc/>
        Solution IHaveSolution.Solution => Solution;

        ///<inheritdoc/>
        public static int Main() => Execute<Build>(x => ((ICompile)x).Compile);

        ///<inheritdoc/>
        IEnumerable<AbsolutePath> IClean.DirectoriesToDelete => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobDirectories("**/bin", "**/obj")
            .Concat(this.Get<IHaveTestDirectory>().TestDirectory.GlobDirectories("**/bin", "**/obj"));

        ///<inheritdoc/>
        AbsolutePath IHaveTestDirectory.TestDirectory => RootDirectory / "tests";


        ///<inheritdoc/>
        IEnumerable<Project> IUnitTest.UnitTestsProjects => this.Get<IHaveSolution>().Solution.GetAllProjects("*UnitTests");

        ///<inheritdoc/>
        IEnumerable<Project> IBenchmark.BenchmarkProjects => this.Get<IHaveSolution>().Solution.GetAllProjects("*.PerfomanceTests");

        ///<inheritdoc/>
        bool IReportCoverage.ReportToCodeCov => this.Get<IReportCoverage>().CodecovToken is not null;

        ///<inheritdoc/>
        IEnumerable<AbsolutePath> IPack.PackableProjects => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobFiles("**/*.csproj");

        ///<inheritdoc/>
        IEnumerable<PushNugetPackageConfiguration> IPushNugetPackages.PublishConfigurations => new PushNugetPackageConfiguration[]
        {
            new NugetPushConfiguration(apiKey: NugetApiKey,
                                       source: new Uri("https://api.nuget.org/v3/index.json"),
                                       () => NugetApiKey is not null),
            new GitHubPushNugetConfiguration(githubToken: this.Get<IHaveGitHubRepository>().GitHubToken,
                                             source: new Uri("https://nukpg.github.com/"),
                                             () => this is ICreateGithubRelease && this.Get<ICreateGithubRelease>()?.GitHubToken is not null)
        };

        ///<inheritdoc/>
        IEnumerable<MutationProjectConfiguration> IMutationTest.MutationTestsProjects => new[]{
            new MutationProjectConfiguration(sourceProject: Solution.AllProjects.Single(csproj => string.Equals(csproj.Name, "Forms")),
                                             testProjects: Solution.GetAllProjects("Forms.UnitTests"),
                                             configurationFile: this.Get<IHaveTestDirectory>().TestDirectory / "Forms.UnitTests" / "stryker-config.json")
        };

        ///<inheritdoc/>
        protected override void OnBuildCreated()
        {
            if (IsServerBuild)
            {
                EnvironmentInfo.SetVariable("DOTNET_ROLL_FORWARD", "LatestMajor");
            }
        }
    }
}