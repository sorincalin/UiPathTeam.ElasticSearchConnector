using System;
using System.Activities;

namespace UiPathTeam.ElasticSearchConnector.Test
{
    class Program
    {
        static void Main()
        {
            var searchActivity = new ElasticSearchConnector.Search
            {
                AuthenticationRequired = false,
                Index = "default-*",
                URL = "http://localhost:9200/",
                MaxSize = 20,
                StartTime = new DateTime(2018,1,2),
                EndTime = DateTime.Now,
                RobotName = "StudioRobot",
            };

            var output = WorkflowInvoker.Invoke(searchActivity);
        }
    }
}
