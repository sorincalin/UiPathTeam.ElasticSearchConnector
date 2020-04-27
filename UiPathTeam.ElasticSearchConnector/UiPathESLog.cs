using System;

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
}
