namespace SigpToJiraWorklog
{
    using System;
    using Atlassian.Jira;

    /// <summary>
    /// Sigp worklog entity.
    /// </summary>
    /// <seealso cref="Atlassian.Jira.Worklog" />
    /// <seealso cref="System.IEquatable{Atlassian.Jira.Worklog}" />
    public class SigpWorklog : Worklog, IEquatable<Worklog>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SigpWorklog"/> class.
        /// </summary>
        /// <param name="userLogin">The user login name.</param>
        /// <param name="manHours">Time spent in man hours.</param>
        /// <param name="startDate">The worklog start date.</param>
        /// <param name="description">The worklog description.</param>
        public SigpWorklog(string userLogin, double manHours, DateTime startDate, string description)
            : base(manHours.ToJiraTimeString(), startDate, description.ToJiraComment())
        {
            Author = userLogin;
            ManHours = manHours;
            Description = description;
        }

        /// <summary>
        /// Gets the worklog description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the man hours.
        /// </summary>
        public double ManHours { get; }

        /// <summary>
        /// Gets the issue key.
        /// </summary>
        public string IssueKey
        {
            get
            {
                return this.Description.ToJiraIssueKey();
            }
        }

        public bool Equals(Worklog other)
        {
            return other?.Author == this.Author &&
                other?.StartDate == this.StartDate &&
                other?.TimeSpent == this.TimeSpent &&
                other?.Comment == this.Comment;
        }

        public override bool Equals(Object obj)
        {
            return Equals(obj as Worklog);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator == (SigpWorklog sigp, Worklog jira)
        {
            if (((object)sigp) == null || ((object)jira) == null)
                return Object.Equals(sigp, jira);

            return sigp.Equals(jira);
        }

        public static bool operator != (SigpWorklog sigp, Worklog jira)
        {
            if (((object)sigp) == null || ((object)jira) == null)
                return !Object.Equals(sigp, jira);

            return !(sigp.Equals(jira));
        }
    }
}
