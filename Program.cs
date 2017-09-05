namespace SigpToJiraWorklog
{
    using System;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Atlassian.Jira;

    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            if (args.Length != 4)
            {
                Console.WriteLine("usage: {0} <JiraServerUrl> <JiraUserName> <JiraPassword> <SigpExportedExcelFile>", Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase));
                return;
            }

            // Get app params
            var jiraServerUrl = args[0];
            var jiraUser = args[1];
            var jiraPwd = args[2];
            string worklogFile = args[3];

            // Create JIRA client
            var jira = Jira.CreateRestClient(jiraServerUrl, jiraUser, jiraPwd);

            // Load SIGP worklog from exported Excel file
            var sigpWorklog = SigpWorklogReader.GetSigpWorklog(jiraUser, worklogFile);

            // Report SIGP worklog summary to the user
            sigpWorklog.ForEach(wl => Console.WriteLine($"{wl.StartDate?.ToString("dd/MM")} {string.Format("{0,-8}", wl.IssueKey)} {string.Format("{0,3}", wl.TimeSpent)} {wl.Comment}"));
            Console.WriteLine($"Total hours: {sigpWorklog.Sum(wl => wl.ManHours)}");
            Console.WriteLine("Press Enter to update JIRA worklog");
            Console.ReadLine();

            // Update Jira worklog per issue key
            sigpWorklog.GroupBy(l => l.IssueKey).ToList().ForEach(l =>
            {
                // Get Jira issue for issue key
                var jiraIssue = jira.Issues.GetIssueAsync(l.Key).Result;

                // Get Jira worklog for the specific issue key
                var issueWorklog = jiraIssue.GetWorklogsAsync().Result.ToList();

                // Add missing logs from SIGP worklog
                var issuesToUpdate = l.Where(wl => !issueWorklog.Any(i => wl == i)).ToList();
                if (issuesToUpdate.Count > 0)
                {
                    // Update issue
                    Console.WriteLine($"Updating issue key {l.Key}");
                    var results = issuesToUpdate.Select(issue => jiraIssue.AddWorklogAsync(issue).Result).ToList();

                    // Save changes
                    jiraIssue.SaveChanges();

                }
            });
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
