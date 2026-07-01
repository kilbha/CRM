using CRM.Domain.Common.Constants;
using CRM.Domain.Entities;
using CRM.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRM.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable(TableNames.Customers);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerCode)
            .IsRequired()
            .HasMaxLength(FieldLengths.CustomerCode);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(FieldLengths.Name);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(FieldLengths.Email);

        builder.Property(x => x.Phone)
            .HasMaxLength(FieldLengths.Phone);

        builder.Property(x => x.Company)
            .HasMaxLength(FieldLengths.Company);

        builder.Property(x => x.Website)
            .HasMaxLength(FieldLengths.Website);

        builder.Property(x => x.Status)
            .HasConversion<string>();

        builder.Property(x => x.Source)
            .HasConversion<string>();

        // 👇 Address configuration goes here
        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(FieldLengths.Street);

            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(FieldLengths.City);

            address.Property(a => a.State)
                .HasColumnName("State")
                .HasMaxLength(FieldLengths.State);

            address.Property(a => a.Country)
                .HasColumnName("Country")
                .HasMaxLength(FieldLengths.Country);

            address.Property(a => a.PostalCode)
                .HasColumnName("PostalCode")
                .HasMaxLength(FieldLengths.PostalCode);
        });

        builder.HasIndex(x => x.Email)
            .IsUnique();

        builder.HasIndex(x => x.CustomerCode)
            .IsUnique();
    }
}