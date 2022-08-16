using Autofac;
using PriceHunter.Model.Notification;
using PriceHunter.Notification;
using PriceHunter.Notification.Email;
using PriceHunter.Notification.MobilePush;
using PriceHunter.Notification.Sms;

namespace PriceHunter.Container.Modules
{
    public class NotificationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmailNotifiactionAdapter>().Keyed<INotificationAdapter>(NotificationType.Email).InstancePerLifetimeScope();
            builder.RegisterType<SmsNotificationAdapter>().Keyed<INotificationAdapter>(NotificationType.Sms).InstancePerLifetimeScope();
            builder.RegisterType<MobilePushNotificationAdapter>().Keyed<INotificationAdapter>(NotificationType.MobilePush).InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
