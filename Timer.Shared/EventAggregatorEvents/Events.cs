namespace Timer.Shared.EventAggregatorEvents
{
    public class SelectedProjectChangeEvent : Prism.Events.PubSubEvent { }

    public class ReleaseNotesChangedEvent : Prism.Events.PubSubEvent<string> { }

    public class LoadAllDataRequested : Prism.Events.PubSubEvent { }

}
