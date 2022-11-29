using EventBus.Abstractions;
using Reporting.Api.Enums;
using Reporting.Api.Infrastructure.Repositories;
using Reporting.Api.IntegrationEvents.Events;

namespace Reporting.Api.IntegrationEvents.EventHandling;

public class ReportCompletedIntegrationEventHandler : IIntegrationEventHandler<ReportCompletedIntegrationEvent>
{
    private readonly IReportRepository _reportRepository;

    public ReportCompletedIntegrationEventHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task HandleAsync(ReportCompletedIntegrationEvent evt)
    {
        var report = await _reportRepository.GetReportByIdAsync(evt.ReportId);

        if (report == null)
            throw new InvalidOperationException("A report completed event was received but the database record of the report couldn't be found.");

        report.PathToReportFile = evt.PathToReportFile;
        report.Status = evt.Status;
        await _reportRepository.UpdateReportAsync(report);
    }
}