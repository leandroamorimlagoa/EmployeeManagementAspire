using Domain.Employees;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Repositories.Employees;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
            .IsRequired();

        builder.Property(u => u.LastName)
            .IsRequired();

        builder.Property(u => u.Email)
            .IsRequired();

        builder.Property(u => u.DocNumber)
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(u => u.DocType)
            .IsRequired();

        builder.Property(u => u.ManagerId)
            .IsRequired();

        builder.Property(u => u.BirthDate)
            .IsRequired();

        builder.Property(u => u.Status)
            .IsRequired();

        builder.HasIndex(u => u.DocNumber)
            .IsUnique();

        builder.HasQueryFilter(u => u.Status == EmployeeStatus.Active);

        builder
            .HasMany(u => u.Phones)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        builder
            .HasOne(u => u.Manager)
            .WithMany()
            .HasForeignKey(u => u.ManagerId);
    }
}
