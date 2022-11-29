using Reporting.Api.Daos;

namespace Reporting.Api.Services;

public interface IReportingService
{
    Task DoReportGenerationAsync(Guid reportId);

    Task<ICollection<ContactInformationDao>?> RequestContactInformationAsync();

    Task<string?> GenerateReportFileAsync(Guid reportId, ICollection<ContactInformationDao> contactInformation);
}