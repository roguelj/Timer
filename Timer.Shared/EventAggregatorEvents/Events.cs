namespace Timer.Shared.EventAggregatorEvents
{
    public class SelectedProjectChangeEvent : Prism.Events.PubSubEvent { }

    public class ReleaseNotesChangedEvent : Prism.Events.PubSubEvent<string> { }


    public class ProjectSearchCriteriaChangedEvent : Prism.Events.PubSubEvent { }
    public class TaskSearchCriteriaChangedEvent : Prism.Events.PubSubEvent { }
    public class TagSearchCriteriaChangedEvent : Prism.Events.PubSubEvent { }

    public class LoadAllDataRequested : Prism.Events.PubSubEvent { }

}
