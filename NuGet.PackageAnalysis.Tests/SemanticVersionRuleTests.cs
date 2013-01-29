using Moq;
using NuGet.PackageAnalysis.SemVer;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions;
using Strings = NuGet.PackageAnalysis.SemVer.Properties.Resources;

namespace NuGet.PackageAnalysis.Tests
{
    public sealed class SemanticVersionRuleTests
    {
        [Theory]
        [InlineData("1.0.0", true)]
        [InlineData("1.0.0-preRelease", true)]
        [InlineData("1.0", false)]
        [InlineData("1.0.0.0", false)]
        [InlineData("1.0-preRelease", false)]
        [InlineData("1.0.0.0-preRelease", false)]
        public void AppliesStrictSemVerSpecification(string versionString, bool isValid)
        {
            // arrange
            var packageVersion = new SemanticVersion(versionString);
            var packageMock = new Mock<IPackage>(MockBehavior.Strict);
            packageMock.Setup(m => m.Version).Returns(packageVersion).Verifiable();
            packageMock.Setup(m => m.Id).Returns("TestPackage").Verifiable();
            IPackage package = packageMock.Object;
            PackageIssue packageIssue = null;
            var rule = new SemanticVersionRule();

            // act
            IEnumerable<PackageIssue> validationResult = rule.Validate(package);

            // assert
            Assert.DoesNotThrow(() => packageIssue = validationResult.SingleOrDefault());

            if (!isValid)
            {
                Assert.NotNull(packageIssue);
                Assert.Equal(Strings.SemanticVersionRule_IssueTitle, packageIssue.Title);
                Assert.Equal(PackageIssueLevel.Error, packageIssue.Level);
            }
            else
            {
                Assert.Null(packageIssue);
            }
        }
    }
}
