namespace SigpToJiraWorklog
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using ExcelDataReader;

    /// <summary>
    /// Parser for the exported SIGP worklog Excel file.
    /// </summary>
    public class SigpWorklogReader
    {
        private const string DateColumn = "Date";
        private const string DescriptionColumn = "Description";
        private const string WorkerColumn = "Worker";
        private const string ManhoursColumn = "Manhours";
        private const string PurchaseOrderColumn = "Purchase order";
        private const string ProjectColumn = "Project";

        private const int TableIndex = 0;
        private const int NumberOfColumns = 6;
        private const int HeaderRowIndex = 0;
        private const int DateColumnIndex = 0;
        private const int DescriptionColumnIndex = 1;
        private const int WorkerColumnIndex = 2;
        private const int ManhoursColumnIndex = 3;
        private const int PurchaseOrderColumnIndex = 4;
        private const int ProjectColumnIndex = 5;

        /// <summary>
        /// Gets the sigp worklog from the exported Excel file.
        /// </summary>
        /// <param name="userLogin">The JIRA user login.</param>
        /// <param name="worklogFile">The Excel worklog file.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">No data.</exception>
        public static List<SigpWorklog> GetSigpWorklog(string userLogin, string worklogFile)
        {
            using (var stream = File.Open(worklogFile, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    return WorklogData(reader.AsDataSet()?.Tables?[TableIndex]).AsEnumerable().Skip(1).Select(r =>
                        {
                            return new SigpWorklog(userLogin, r.Field<double>(ManhoursColumnIndex), r.Field<DateTime>(DateColumnIndex), r.Field<string>(DescriptionColumnIndex));
                        }).OrderBy(wl => wl.StartDate).ThenBy(wl => wl.ManHours).ToList();
                }
            }

            throw new InvalidOperationException("No data.");
        }

        /// <summary>
        /// Check worklog DataTable.
        /// </summary>
        /// <param name="dt">The worklog DataTable.</param>
        /// <returns>The worklog DataTable</returns>
        /// <exception cref="NotSupportedException">Unsupported Excel file.</exception>
        private static DataTable WorklogData(DataTable dt)
        {
            if (dt != null && dt.Rows.Count > 1 && dt.Columns.Count == NumberOfColumns &&
                dt.Rows[HeaderRowIndex][DateColumnIndex] as string == DateColumn &&
                dt.Rows[HeaderRowIndex][DescriptionColumnIndex] as string == DescriptionColumn &&
                dt.Rows[HeaderRowIndex][WorkerColumnIndex] as string == WorkerColumn &&
                dt.Rows[HeaderRowIndex][ManhoursColumnIndex] as string == ManhoursColumn &&
                dt.Rows[HeaderRowIndex][PurchaseOrderColumnIndex] as string == PurchaseOrderColumn &&
                dt.Rows[HeaderRowIndex][ProjectColumnIndex] as string == ProjectColumn)
            {
                return dt;
            }

            throw new NotSupportedException("Unsupported Excel file.");
        }
    }
}
