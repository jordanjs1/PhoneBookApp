using Contact.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contact.Api.Infrastructure.EntityConfigurations
{
    public class ContactInformationEntityTypeConfiguration : IEntityTypeConfiguration<ContactInformation>
    {
        public void Configure(EntityTypeBuilder<ContactInformation> builder)
        {
            builder.ToTable("ContactInformation");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).IsRequired();

            builder.Property(x => x.ContactId)
                .IsRequired();

            builder.HasOne(x => x.Contact)
                .WithMany()
                .HasForeignKey(x => x.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Type)
                .IsRequired();

            builder.Property(x => x.Content)
                .HasMaxLength(300);
        }
    }
}
