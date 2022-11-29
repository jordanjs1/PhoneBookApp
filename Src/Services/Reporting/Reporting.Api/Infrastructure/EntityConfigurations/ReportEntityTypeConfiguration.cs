using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Reporting.Api.Models;

namespace Reporting.Api.Infrastructure.EntityConfigurations
{
    public class ReportEntityTypeConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Report");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.Status);

            builder.Property(x => x.PathToReportFile)
                .HasMaxLength(2000);
        }
    }
}
