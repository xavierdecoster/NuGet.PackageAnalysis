using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Strings = NuGet.PackageAnalysis.SemVer.Properties.Resources;

namespace NuGet.PackageAnalysis.SemVer
{
    [Export(typeof(IPackageRule))]
    public sealed class SemanticVersionRule : IPackageRule
    {
        // Note: NuGet doesn't support build numbers (yet)
        private const string _strictSemanticVersionRegex = "^(\\d+(\\.\\d+){2})(-[a-z][0-9a-z-]*)?$";

        public IEnumerable<PackageIssue> Validate(IPackage package)
        {
            if (!IsValidSemanticVersionAccordingToSpec(package))
            {
                string description = string.Format(CultureInfo.InvariantCulture, Strings.SemanticVersionRule_IssueDescription, package.Id, package.Version);
                var issue = new PackageIssue(Strings.SemanticVersionRule_IssueTitle, description, Strings.SemanticVersionRule_IssueSolution, PackageIssueLevel.Error);
                return new ReadOnlyCollection<PackageIssue>(new[] { issue });
            }
            return Enumerable.Empty<PackageIssue>();
        }

        private bool IsValidSemanticVersionAccordingToSpec(IPackage package)
        {
            return Regex.IsMatch(package.Version.ToString(), _strictSemanticVersionRegex, RegexOptions.IgnoreCase);
        }
    }
}
