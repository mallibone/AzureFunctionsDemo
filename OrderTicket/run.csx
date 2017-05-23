#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;

public class Ticket
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }

    public string TicketId { get; set; }
    public string EventId { get; set; }
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }
}

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log, IAsyncCollector<Ticket> outputTable, IAsyncCollector<Ticket> outputQueueItem)
{
    log.Info($"Order Ticket WebHook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    Ticket ticket = JsonConvert.DeserializeObject<Ticket>(jsonContent);

    ticket.RowKey = ticket.TicketId;
    ticket.PartitionKey = ticket.EventId;

    if (ticket.Email == null || string.IsNullOrEmpty(ticket.EventId)) {
        log.Error($"Received invalid Ticketrequest");
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass first/last properties in the input object"
        });
    }

    log.Info($"Ticket for Event {ticket.EventId} ordered by {ticket.Firstname} {ticket.Lastname}");

    await outputTable.AddAsync(ticket);
    await outputQueueItem.AddAsync(ticket);

    return req.CreateResponse(HttpStatusCode.OK, new {
        greeting = $"Ticket purchased for {ticket.Firstname} {ticket.Lastname}!"
    });
}