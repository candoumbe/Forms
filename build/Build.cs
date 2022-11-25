namespace Forms.ContinuousIntegration
{
    using Candoumbe.Pipelines;
    using Candoumbe.Pipelines.Components;
    using Candoumbe.Pipelines.Components.GitHub;
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

    using static Nuke.Common.IO.PathConstruction;

    /// <summary>
    /// Defines the pipeline used to build the project
    /// </summary>
    [GitHubActions(
        "integration",
        GitHubActionsImage.UbuntuLatest,
        OnPushBranchesIgnore = new[] { IHaveMainBranch.MainBranchName },
        PublishArtifacts = true,
        InvokedTargets = new[] { nameof(IUnitTest.UnitTests), nameof(IReportCoverage.ReportCoverage), nameof(IPack.Pack) },
        CacheKeyFiles = new[] { "global.json", "src/**/*.csproj" },
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
        OnPushBranches = new[] { IHaveMainBranch.MainBranchName },
        InvokedTargets = new[] { nameof(IUnitTest.UnitTests), nameof(IPublish.Publish), nameof(ICreateGithubRelease.AddGithubRelease) },
        EnableGitHubToken = true,
        CacheKeyFiles = new[] { "global.json", "src/**/*.csproj" },
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
    [UnsetVisualStudioEnvironmentVariables]
    [DotNetVerbosityMapping]
    [HandleVisualStudioDebugging]
    public class Build : NukeBuild,
        IHaveSourceDirectory,
        IHaveTestDirectory,
        IClean,
        IRestore,
        ICompile,
        IHaveSolution,
        IUnitTest,
        IMutationTest,
        IPack,
        ICreateGithubRelease,
        IPublish,
        IHaveMainBranch,
        IReportCoverage,
        IHaveDevelopBranch,
        IHaveGitVersion,
        IHaveGitHubRepository,
        IHaveTests,
        IHaveArtifacts,
        IHaveChangeLog,
        IGitFlowWithPullRequest
    {
        /// <summary>
        /// Github Actions
        /// </summary>
        [CI]
        public GitHubActions GitHubActions;

        /// <summary>
        /// Token used to interact with GitHub API
        /// </summary>
        [Parameter("Token used to interact with Nuget API")]
        [Secret]
        public readonly string NugetApiKey;

        /// <summary>
        /// Token used to interact with GitHub API
        /// </summary>
        [Solution]
        [Required]
        public Solution Solution { get; }

        /// <inheritdoc/>
        Solution IHaveSolution.Solution => Solution;

        /// <inheritdoc/>
        IEnumerable<Project> IUnitTest.UnitTestsProjects => this.Get<IHaveSolution>().Solution.GetProjects("*.UnitTests");

        /// <inheritdoc/>
        IEnumerable<AbsolutePath> IPack.PackableProjects => this.Get<IHaveSourceDirectory>().SourceDirectory.GlobFiles("**/*.csproj");

        /// <inheritdoc/>
        IEnumerable<Project> IMutationTest.MutationTestsProjects => this.Get<IUnitTest>().UnitTestsProjects;

        /// <inheritdoc/>
        IEnumerable<PublishConfiguration> IPublish.PublishConfigurations => new PublishConfiguration[]
        {
            new NugetPublishConfiguration(
                apiKey: NugetApiKey,
                source: new Uri("https://api.nuget.org/v3/index.json"),
                canBeUsed: () => NugetApiKey is not null
            ),
            new GitHubPublishConfiguration(
                githubToken: this.Get<ICreateGithubRelease>()?.GitHubToken,
                source: new Uri($"https://nuget.pkg.github.com/{GitHubActions?.RepositoryOwner}/index.json"),
                canBeUsed: () => this is ICreateGithubRelease createRelease && createRelease.GitHubToken is not null
            ),
        };

        /// <inheritdoc/>
        bool IReportCoverage.ReportToCodeCov => this.Get<IReportCoverage>()?.CodecovToken is not null;

        /// <summary>
        /// Defines the default target called when running the pipeline with no args
        /// </summary>
        public static int Main() => Execute<Build>(x => ((ICompile)x).Compile);
    }
}