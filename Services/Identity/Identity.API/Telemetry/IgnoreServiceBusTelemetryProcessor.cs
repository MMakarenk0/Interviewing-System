using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Identity.API.Telemetry;

public class IgnoreServiceBusTelemetryProcessor : ITelemetryProcessor
{
    private ITelemetryProcessor _next;

    public IgnoreServiceBusTelemetryProcessor(ITelemetryProcessor next)
    {
        _next = next;
    }

    public void Process(ITelemetry item)
    {
        if (item is DependencyTelemetry dependency)
        {
            if (dependency.Type == "Azure Service Bus")
            {
                return;
            }
        }

        _next.Process(item);
    }
}

