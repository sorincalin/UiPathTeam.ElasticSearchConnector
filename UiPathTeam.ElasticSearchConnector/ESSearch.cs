using Nest;
using System;
using System.Activities;
using System.ComponentModel;
using System.Linq;
using System.Security;

namespace UiPathTeam.ElasticSearchConnector
{
    public class UiPathESLog
    {
        public string level { get; set; }
        public int levelOrdinal { get; set; }
        public DateTime timeStamp { get; set; }
        public string Source { get; set; }
        public int organizationUnitId { get; set; }
        public string machineName { get; set; }
        public string processName { get; set; }
        public string processVersion { get; set; }
        public string robotName { get; set; }
        public int totalExecutionTimeInSeconds { get; set; }
        public string totalExecutionTime { get; set; }
        public string fileName { get; set; }
        public string rawMessage { get; set; }
        public string windowsIdentity { get; set; }
        public string logType { get; set; }
    }

    public class Search: CodeActivity
    {
        [Category("Authentication")]
        [DisplayName("Is Auth required")]
        [RequiredArgument]
        public InArgument<bool> AuthenticationRequired { get; set; }

        [Category("Authentication")]
        public InArgument<string> Username { get; set; }

        [Category("Authentication")]
        public InArgument<SecureString> Password { get; set; }

        [Category("Input")]
        [DisplayName("Server URL")]
        [RequiredArgument]
        public InArgument<string> URL { get; set; }

        [Category("Input")]
        [DisplayName("Index")]
        [Description("A comma-separated list of index names to search; use the special string `_all` or Indices.All to perform the operation on all indices")]
        [RequiredArgument]
        public InArgument<string> Index { get; set; }

        [Category("Options")]
        [DisplayName("Max count")]
        [Description("Maximum number of logs to be retrieved. Maximum value is 10000")]
        [RequiredArgument]
        public InArgument<int> MaxSize{ get; set; }

        [Category("Options")]
        [DisplayName("Start time")]
        [Description("Start time for the query")]
        public InArgument<DateTime> StartTime{ get; set; }

        [Category("Options")]
        [DisplayName("End time")]
        [Description("End time for the query")]
        public InArgument<DateTime> EndTime { get; set; }

        [Category("Options")]
        [DisplayName("Process Name")]
        [Description("(Optional) specify the process name")]
        public InArgument<string> ProcessName { get; set; }

        [Category("Options")]
        [DisplayName("Robot Name")]
        [Description("(Optional) specify the Robot name")]
        public InArgument<string> RobotName { get; set; }

        [Category("Output")]
        public OutArgument<UiPathESLog[]> Logs { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var settings = new ConnectionSettings(new Uri(URL.Get(context)));
            settings.ThrowExceptions(alwaysThrow: true);
            settings.PrettyJson();

            if (AuthenticationRequired.Get(context) == true)
            {
                settings.BasicAuthentication(Username.Get(context), Password.Get(context));
            }
            

            var esClient = new ElasticClient(settings);
            var searchData = esClient.Search<UiPathESLog>(sd => sd
                                                                .Index(Index.Get(context))
                                                                .Size(MaxSize.Get(context))
                                                                .Query(q => q.
                                                                            Match(m => m
                                                                                        .Field(f => f.processName)
                                                                                        .Query(ProcessName.Get(context) == string.Empty ? "*" : ProcessName.Get(context))) &&
                                                                            q.
                                                                            Match(m => m
                                                                                        .Field(f => f.robotName)
                                                                                        .Query(RobotName.Get(context) == string.Empty ? "*" : RobotName.Get(context))) &&
                                                                            q
                                                                            .DateRange(r => r
                                                                                            .Field(f => f.timeStamp)
                                                                                            .GreaterThanOrEquals(StartTime.Get(context))
                                                                                            .LessThanOrEquals(EndTime.Get(context)))));

            Logs.Set(context, searchData.Documents.ToArray());
        }
    }
}
