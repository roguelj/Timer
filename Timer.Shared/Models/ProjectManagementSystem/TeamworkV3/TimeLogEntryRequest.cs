using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{

    public class Rootobject
    {
        public Tag[] tags { get; set; }
        public Timelog timelog { get; set; }
        public Timelogoptions timelogOptions { get; set; }
    }

    public class Timelog
    {
        public Date date { get; set; }
        public string description { get; set; }
        public bool hasStartTime { get; set; }
        public int hours { get; set; }
        public int invoiceId { get; set; }
        public bool isBillable { get; set; }
        public bool isUtc { get; set; }
        public int minutes { get; set; }
        public int projectId { get; set; }
        public int[] tagIds { get; set; }
        public int taskId { get; set; }
        public int ticketId { get; set; }
        public Time time { get; set; }
        public int userId { get; set; }
    }

    public class Date
    {
    }

    public class Time
    {
    }

    public class Timelogoptions
    {
        public bool fireWebhook { get; set; }
        public bool logActivity { get; set; }
        public bool markTaskComplete { get; set; }
        public bool parseInlineTags { get; set; }
        public bool useNotifyViaTWIM { get; set; }
    }

}
