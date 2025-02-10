using Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repositories.Employees;

public class PhoneConfiguration : IEntityTypeConfiguration<UserPhone>
{
    public void Configure(EntityTypeBuilder<UserPhone> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PhoneNumber)
            .IsRequired();

        builder.Property(p => p.UserId)
            .IsRequired();
    }
}
