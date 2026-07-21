using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using Timer.Shared.EventAggregatorEvents;

namespace Timer.Shared.Extensions
{
    public static class IEventAggregatorExtensions
    {


        public static void PublishNotification(this IEventAggregator eventAggregator, TimeProvider timeProvider, string message, NotificationLevel notificationLevel, bool isRollback)
        {
            eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification(timeProvider.GetLocalNow(), message, notificationLevel, isRollback));
        }

        public static void PublishNotification(this IEventAggregator eventAggregator, TimeProvider timeProvider, string message, NotificationLevel notificationLevel)
        {
            eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification(timeProvider.GetLocalNow(), message, notificationLevel));
        }


        public static void PublishProgressNotification(this IEventAggregator eventAggregator, TimeProvider timeProvider, string message, NotificationLevel notificationLevel, Guid notificationId, int total)
        {
            eventAggregator.GetEvent<NotificationEvent>().Publish(new Notification(timeProvider.GetLocalNow(), message, notificationLevel, notificationId, total));
        }



        // authentication 
        public static void PublishInteractiveSignIn(this IEventAggregator eventAggregator, TimeProvider timeProvider, string userName)
        {
            eventAggregator.GetEvent<SignInEvent>().Publish(new AuthenticationPayload(timeProvider.GetLocalNow(), userName, AuthType.InteractiveSignIn));
        }

        public static void PublishSignOut(this IEventAggregator eventAggregator, TimeProvider timeProvider, string userName)
        {
            eventAggregator.GetEvent<SignOutEvent>().Publish(new AuthenticationPayload(timeProvider.GetLocalNow(), userName, AuthType.SignOut));
        }

        public static void PublishSilentSignIn(this IEventAggregator eventAggregator, TimeProvider timeProvider, string userName)
        {
            eventAggregator.GetEvent<SignInEvent>().Publish(new AuthenticationPayload(timeProvider.GetLocalNow(), userName, AuthType.SilentSignIn));
        }

    }

}
