namespace Candoumbe.Forms.ContinuousIntegration
{
    using Candoumbe.Pipelines.Components;
    using Candoumbe.Pipelines.Components.GitHub;
    using Candoumbe.Pipelines.Components.NuGet;
    using Candoumbe.Pipelines.Components.Workflows;

    using Nuke.Common;
    using Nuke.Common.CI.GitHubActions;
    using Nuke.Common.IO;
    using Nuke.Common.ProjectModel;

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
        OnPushBranchesIgnore = [IHaveMainBranch.MainBranchName],
        PublishArtifacts = true,
        InvokedTargets = [nameof(IUnitTest.UnitTests), nameof(IPack.Pack)],
        CacheKeyFiles = ["global.json", "src/**/*.csproj", "test/**/*.csproj"],
        ImportSecrets =
        [
            nameof(NugetApiKey),
            nameof(IReportCoverage.CodecovToken),
            nameof(IMutationTest.StrykerDashboardApiKey)
        ],
        OnPullRequestExcludePaths =
        [
            "docs/*",
            "README.md",
            "CHANGELOG.md",
            "LICENSE"
        ]
    )]
    [GitHubActions(
        "delivery",
        GitHubActionsImage.UbuntuLatest,
        FetchDepth = 0,
        OnPushBranches = [IHaveMainBranch.MainBranchName],
        InvokedTargets = [nameof(IUnitTest.UnitTests), nameof(IPushNugetPackages.Publish), nameof(ICreateGithubRelease.AddGithubRelease)],
        EnableGitHubToken = true,
        CacheKeyFiles = ["global.json", "src/**/*.csproj", "test/**/*.csproj"],
        PublishArtifacts = true,
        ImportSecrets =
        [
            nameof(NugetApiKey),
            nameof(IReportCoverage.CodecovToken),
            nameof(IMutationTest.StrykerDashboardApiKey)
        ],
        OnPullRequestExcludePaths =
        [
            "docs/*",
            "README.md",
            "CHANGELOG.md",
            "LICENSE"
        ]
    )]

    public class Build : EnhancedNukeBuild,
        IHaveSourceDirectory,
        IHaveTestDirectory,
        IGitFlowWithPullRequest,
        IClean,
        IRestore,
        IMutationTest,
        IBenchmark,
        IReportCoverage,
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
        IEnumerable<PushNugetPackageConfiguration> IPushNugetPackages.PublishConfigurations =>
        [
            new NugetPushConfiguration(apiKey: NugetApiKey,
                                       source: new Uri("https://api.nuget.org/v3/index.json"),
                                       () => NugetApiKey is not null),
            new GitHubPushNugetConfiguration(githubToken: this.Get<IHaveGitHubRepository>().GitHubToken,
                                             source: new Uri("https://nukpg.github.com/"),
                                             () => this.Get<ICreateGithubRelease>()?.GitHubToken is not null)
        ];

        ///<inheritdoc/>
        IEnumerable<MutationProjectConfiguration> IMutationTest.MutationTestsProjects =>
        [
            new (sourceProject: Solution.AllProjects.Single(csproj => string.Equals(csproj.Name, "Forms")),
                 testProjects: Solution.GetAllProjects("Forms.UnitTests"),
                 configurationFile: this.Get<IHaveTestDirectory>().TestDirectory / "Forms.UnitTests" / "stryker-config.json")
        ];
    }
}