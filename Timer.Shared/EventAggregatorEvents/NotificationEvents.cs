using Prism.Events;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Timer.Shared.EventAggregatorEvents
{
    public class NotificationEvent : PubSubEvent<Notification> { }

    public class ProgressIncrementNotificationEvent : PubSubEvent<(Guid NotificationId, int IncrementByQuantity)> { }

    public sealed class Notification : INotifyPropertyChanging, INotifyPropertyChanged
    {

        // ------------------------
        // member variables
        private int _current;


        // ------------------------
        // core properties
        public Guid NotificationId { get; }

        public required DateTimeOffset Timestamp { get; set; }

        public required string Message { get; set; }

        public NotificationLevel NotificationLevel { get; set; }

        public bool? IsRollback { get; set; }


        // ------------------------
        // bound progress properties
        public int? Total
        {
            get;
            set => this.Set(value, out field);
        }

        public int Current
        {
            get => this._current;
            set => this.Set(value, out this._current);
        }

        public decimal ProgressValue
        {
            get => field;
            set => this.Set(value, out field);
        }

        public string ProgressMessage
        {
            get => field;
            set => this.Set(value, out field);
        }

        public bool CanDismiss
        {
            get => field;
            set => this.Set(value, out field);
        }


        [SetsRequiredMembers]
        public Notification(DateTimeOffset timestamp, String message, NotificationLevel notificationLevel)
        {
            this.Timestamp = timestamp;
            this.Message = message;
            this.NotificationLevel = notificationLevel;
            this.NotificationId = Guid.NewGuid();
            this.ProgressMessage = string.Empty;
            this.ProgressValue = 0;
            this.CanDismiss = true;
        }

        [SetsRequiredMembers]
        public Notification(DateTimeOffset timestamp, String message, NotificationLevel notificationLevel, Guid notificationId, int total) : this(timestamp, message, notificationLevel)
        {
            this.NotificationId = notificationId;
            this.Total = total;
            this.CanDismiss = false;
        }

        [SetsRequiredMembers]
        public Notification(DateTimeOffset timestamp, String message, NotificationLevel notificationLevel, bool isRollback) : this(timestamp, message, notificationLevel)
        {
            this.IsRollback = isRollback;
        }

        public void Increment(int incrementByQuantity)
        {

            Interlocked.Add(ref this._current, incrementByQuantity);

            if (this.Total.HasValue)
            {
                this.ProgressValue = 100 * (this.Current / (decimal)this.Total.Value);
                this.ProgressMessage = $"{this.Current} of {this.Total.Value} completed.";
            }

            if (this.Total.HasValue && this.Current == this.Total.Value)
            {
                this.CanDismiss = true;
            }

        }



        // ------------------------
        // private methods
        private void Set<T>(T value, out T field, [CallerMemberName] string propertyName = "")
        {
            this.NotifyChanging(propertyName);
            field = value;
            this.NotifyChanged(propertyName);
        }


        // ------------------------
        // events
        public event PropertyChangingEventHandler? PropertyChanging;
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void NotifyChanging(string propertyName)
            => PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));

    }

    public enum NotificationLevel
    {
        Information,
        Warning,
        Error,
        Critical
    }
}
