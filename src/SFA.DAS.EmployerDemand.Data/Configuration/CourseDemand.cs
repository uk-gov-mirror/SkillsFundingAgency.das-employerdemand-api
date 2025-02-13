using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SFA.DAS.EmployerDemand.Data.Configuration
{
    public class CourseDemand : IEntityTypeConfiguration<Domain.Entities.CourseDemand>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.CourseDemand> builder)
        {
            builder.ToTable("CourseDemand");
            builder.HasKey(x=> x.Id);

            builder.Property(x => x.Id).HasColumnName("Id").HasColumnType("uniqueidentifier").IsRequired();
            builder.Property(x => x.ContactEmailAddress).HasColumnName("ContactEmailAddress").HasColumnType("varchar").HasMaxLength(255).IsRequired();
            builder.Property(x => x.OrganisationName).HasColumnName("OrganisationName").HasColumnType("varchar").HasMaxLength(1000).IsRequired();
            builder.Property(x => x.NumberOfApprentices).HasColumnName("NumberOfApprentices").HasColumnType("int").IsRequired();
            builder.Property(x => x.CourseId).HasColumnName("CourseId").HasColumnType("int").IsRequired();
            builder.Property(x => x.CourseTitle).HasColumnName("CourseTitle").HasColumnType("varchar").HasMaxLength(1000).IsRequired();
            builder.Property(x => x.CourseLevel).HasColumnName("CourseLevel").HasColumnType("int").IsRequired();
            builder.Property(x => x.LocationName).HasColumnName("LocationName").HasColumnType("varchar").HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Lat).HasColumnName("Lat").HasColumnType("float").IsRequired();
            builder.Property(x => x.Long).HasColumnName("Long").HasColumnType("float").IsRequired();
            builder.Property(x => x.EmailVerified).HasColumnName("EmailVerified").HasColumnType("bit").IsRequired();
            builder.Property(x => x.DateCreated).HasColumnName("DateCreated").HasColumnType("datetime").IsRequired().ValueGeneratedOnAdd();
            
            builder.HasIndex(x => x.Id).IsUnique();
        }
    }
}