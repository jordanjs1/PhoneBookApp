using Microsoft.EntityFrameworkCore;
using Reporting.Api.Models;

namespace Reporting.Api.Infrastructure.Repositories
{
    public class PgSqlReportRepository : IReportRepository
    {
        private readonly ReportingContext _reportingContext;
        public PgSqlReportRepository(ReportingContext dbContext)
        {
            _reportingContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        public async Task<ICollection<Report>> GetAllReportsAsync()
        {
            return await _reportingContext.Reports.ToListAsync();
        }


        public async Task<Report?> GetReportByIdAsync(Guid id)
        {
            return await _reportingContext.Reports.FirstOrDefaultAsync(report => report.Id == id);
        }


        public async Task<Guid> CreateNewReportAsync()
        {
            var newReport = new Report();

            await _reportingContext.Reports.AddAsync(newReport);
            await _reportingContext.SaveChangesAsync();

            return newReport.Id;
        }

        public async Task UpdateReportAsync(Report report)
        {
            _reportingContext.Reports.Update(report);

            await _reportingContext.SaveChangesAsync();
        }
    }
}
