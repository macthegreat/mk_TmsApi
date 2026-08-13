using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MK_TmsApi.Entities;

namespace MK_TmsApi.Entities.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.RegistrationNumber)
            .IsRequired();

        builder.Property(s => s.Name)
            .IsRequired();

        builder.Property(s => s.GPA)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.HasMany(s => s.Enrollments)
            .WithOne(e => e.Student)
            .HasForeignKey(e => e.StudentId);
    }
}