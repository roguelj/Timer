using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timer.Shared.Models.ProjectManagementSystem.TeamworkV3
{

    public class Rootobject
    {
        public int accumulatedEstimatedMinutes { get; set; }
        public Assigneecompany[] assigneeCompanies { get; set; }
        public int[] assigneeCompanyIds { get; set; }
        public int[] assigneeTeamIds { get; set; }
        public Assigneeteam[] assigneeTeams { get; set; }
        public int[] assigneeUserIds { get; set; }
        public Assigneeuser[] assigneeUsers { get; set; }
        public Assignee[] assignees { get; set; }
        public Attachment[] attachments { get; set; }
        public Capacity[] capacities { get; set; }
        public Card card { get; set; }
        public Changefollower[] changeFollowers { get; set; }
        public Column column { get; set; }
        public Commentfollower[] commentFollowers { get; set; }
        public string completedAt { get; set; }
        public int completedBy { get; set; }
        public string completedOn { get; set; }
        public string createdAt { get; set; }
        public int createdBy { get; set; }
        public int createdByUserId { get; set; }
        public int[] crmDealIds { get; set; }
        public string dateUpdated { get; set; }
        public int decimalDisplayOrder { get; set; }
        public int[] dependencyIds { get; set; }
        public string description { get; set; }
        public string descriptionContentType { get; set; }
        public int displayOrder { get; set; }
        public string dueDate { get; set; }
        public int dueDateOffset { get; set; }
        public int estimateMinutes { get; set; }
        public bool hasDeskTickets { get; set; }
        public bool hasReminders { get; set; }
        public int id { get; set; }
        public bool isArchived { get; set; }
        public bool isBlocked { get; set; }
        public int isPrivate { get; set; }
        public Latestupdate[] latestUpdates { get; set; }
        public Lockdown lockdown { get; set; }
        public Meta3 meta { get; set; }
        public string name { get; set; }
        public Originalduedate originalDueDate { get; set; }
        public bool outOfSequence { get; set; }
        public Parenttask parentTask { get; set; }
        public int parentTaskId { get; set; }
        public int[] predecessorIds { get; set; }
        public Predecessor[] predecessors { get; set; }
        public string priority { get; set; }
        public int progress { get; set; }
        public Proof[] proofs { get; set; }
        public Sequence sequence { get; set; }
        public Sequenceduedate sequenceDueDate { get; set; }
        public int sequenceId { get; set; }
        public bool sequenceRootTask { get; set; }
        public Sequencestartdate sequenceStartDate { get; set; }
        public int stageTaskDisplayOrder { get; set; }
        public string startDate { get; set; }
        public int startDateOffset { get; set; }
        public string status { get; set; }
        public int[] subTaskIds { get; set; }
        public int[] tagIds { get; set; }
        public Tasklist tasklist { get; set; }
        public int tasklistId { get; set; }
        public string templateRoleName { get; set; }
        public Timer timer { get; set; }
        public string updatedAt { get; set; }
        public int updatedBy { get; set; }
        public Userpermissions userPermissions { get; set; }
    }

    public class Card
    {
        public int id { get; set; }
        public Meta meta { get; set; }
        public string type { get; set; }
    }

    public class Column
    {
        public int id { get; set; }
        public Meta1 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta1
    {
    }

    public class Lockdown
    {
        public int id { get; set; }
        public Meta2 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta2
    {
    }

    public class Meta3
    {
    }

    public class Originalduedate
    {
    }

    public class Parenttask
    {
        public int id { get; set; }
        public Meta4 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta4
    {
    }

    public class Sequence
    {
        public int id { get; set; }
        public Meta5 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta5
    {
    }

    public class Sequenceduedate
    {
    }

    public class Sequencestartdate
    {
    }

    public class Tasklist
    {
        public int id { get; set; }
        public Meta6 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta6
    {
    }

    public class Timer
    {
        public int id { get; set; }
        public Meta7 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta7
    {
    }

    public class Userpermissions
    {
        public bool canAddSubtasks { get; set; }
        public bool canComplete { get; set; }
        public bool canEdit { get; set; }
        public bool canLogTime { get; set; }
        public bool canViewEstTime { get; set; }
    }

    public class Assigneecompany
    {
        public int id { get; set; }
        public Meta8 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta8
    {
    }

    public class Assigneeteam
    {
        public int id { get; set; }
        public Meta9 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta9
    {
    }

    public class Assigneeuser
    {
        public int id { get; set; }
        public Meta10 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta10
    {
    }

    public class Assignee
    {
        public int id { get; set; }
        public Meta11 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta11
    {
    }

    public class Attachment
    {
        public int id { get; set; }
        public Meta12 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta12
    {
    }

    public class Capacity
    {
        public int id { get; set; }
        public Meta13 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta13
    {
    }

    public class Changefollower
    {
        public int id { get; set; }
        public Meta14 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta14
    {
    }

    public class Commentfollower
    {
        public int id { get; set; }
        public Meta15 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta15
    {
    }

    public class Latestupdate
    {
        public object after { get; set; }
        public object before { get; set; }
        public string field { get; set; }
    }

    public class Predecessor
    {
        public int id { get; set; }
        public Meta16 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta16
    {
    }

    public class Proof
    {
        public int id { get; set; }
        public Meta17 meta { get; set; }
        public string type { get; set; }
    }

    public class Meta17
    {
    }

}
