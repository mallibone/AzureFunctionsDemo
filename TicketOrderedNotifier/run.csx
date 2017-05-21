#r "Microsoft.Azure.NotificationHubs"
#r "Newtonsoft.Json"

using System;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;

public class Ticket
{
    public string TicketId { get; set; }
    public string EventId { get; set; }
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
}

public static async Task Run(Ticket myQueueItem, TraceWriter log, IAsyncCollector<Notification> notification)
{
    log.Info($"Sending WNS toast notification for ticket order {myQueueItem.TicketId}");    
    string wnsNotificationPayload = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                                    "<toast><visual><binding template=\"ToastText01\">" +
                                        "<text id=\"1\">" + 
                                            "Ticket Ordered by (" + myQueueItem.Email + ")" + 
                                        "</text>" +
                                    "</binding></visual></toast>";

    log.Info($"{wnsNotificationPayload}");
    await notification.AddAsync(new WindowsNotification(wnsNotificationPayload)); 
}