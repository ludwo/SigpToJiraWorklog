# SIGP to JIRA worklog


Fill JIRA worklog from SIGP work report (exported Excel file).
SIGP workreport description format `<IssueKey> [description]`

## ToDo list
- [x] Read exported Excel file
- [x] Check duplicates by issue/user/date/description
- [x] Fill JIRA issues
- [ ] Undo?
- [ ] Test project

## Usage
```console
SigpToJiraWorklog <JiraServerUrl> <JiraUserName> <JiraPassword> <SigpExportedExcelFile>

examples:
  SigpToJiraWorklog https://jira.domain.com user.name password workReport201708.xls
  SigpToJiraWorklog https://jira.domain.com user.name password "c:\dir with space\workReport201708.xls"
``` 
