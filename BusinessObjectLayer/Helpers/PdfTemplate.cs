using BusinessObjectLayer.Dtos;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjectLayer.Helpers
{
    public static class PdfTemplate
    {
        public static string GetHTMLString(List<TaskDto> tasks)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(@"
                <html>
                    <head></head>
                    <body>
                        <div class='header'><h1>Scheduled tasks</h1></div>
                        <table align='center'>
                            <thead style='display: 'table-header-group;'>
                                <tr>
                                    <th>Name</th>
                                    <th>Responsible</th>
                                    <th>Start Date</th>
                                    <th>End Date</th>
                                </tr>
                            </thead>
                            <tbody>");

            foreach (var task in tasks)
            {
                if (task.StartDate != null)
                {
                    stringBuilder.AppendFormat(@"
                   <tr>
                        <td>{0}</td>
                        <td>{1}</td>
                        <td>{2}</td>
                        <td>{3}</td>
                    </tr>
                ", task.Name, task.ResponsibleDisplayName, task.StartDate.Value.ToString("dd/MMMM/yyyy"), task.EndDate.Value.ToString("dd/MMMM/yyyy"));
                }
            }

            stringBuilder.Append(@"
                        </tbody>
                    </table>
                    </body>
                </html>
            ");

            return stringBuilder.ToString();
        }
    }
}
