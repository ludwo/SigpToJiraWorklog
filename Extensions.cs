namespace SigpToJiraWorklog
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    static class Extensions
    {
        /// <summary>
        /// Convert decimal time to the jira time string.
        /// </summary>
        /// <param name="manHours">The man hours decimal value.</param>
        /// <returns>
        /// Jira spent time string.
        /// </returns>
        public static string ToJiraTimeString(this double manHours)
        {
            TimeSpan ts = TimeSpan.FromHours(manHours);
            StringBuilder sb = new StringBuilder();

            if ((ts.Days + (ts.Hours / 8))/5 > 0)
            {
                throw new NotSupportedException();
            }

            sb.AppendFormat("{0}", ts.Days + (ts.Hours / 8) > 0 ? $"{ts.Days + (ts.Hours/8)}d " : string.Empty);
            sb.AppendFormat("{0}", ts.Hours % 8 > 0 ? $"{ts.Hours % 8}h " : string.Empty);
            sb.AppendFormat("{0}", ts.Minutes > 0 ? $"{ts.Minutes}m " : string.Empty);

            return sb.ToString().Trim();
        }

        /// <summary>
        /// Get comment from Sigp worklog description.
        /// </summary>
        /// <param name="description">The Sigp worklog description.</param>
        /// <returns>Comment string.</returns>
        public static string ToJiraComment(this string description)
        {
            return description.Replace(description.ToJiraIssueKey(),string.Empty).Trim();
        }

        /// <summary>
        /// Gets the jira issue key from Sigp worklog description. Supported description format is "<IssueKey> "
        /// </summary>
        /// <param name="description">The Sigp worklog description.</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static string ToJiraIssueKey(this string description)
        {
            string issueKey = description?.Split(' ').FirstOrDefault();
            if (!string.IsNullOrEmpty(issueKey) && Regex.IsMatch(issueKey, @"((?<!([A-Za-z]{1,10})-?)[A-Z]+-\d+)"))
            {
                return issueKey.ToUpper();
            }

            throw new NotSupportedException($"Unsupported Issue key '{issueKey}'");
        }
    }
}
