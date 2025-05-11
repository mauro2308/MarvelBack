using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Usuario",DataConstants.SCHEMA);

        builder.HasKey(row => row.UserId)
            .HasName("PK_Usuario");

        builder.Property(row => row.UserId)
            .IsRequired()
            .HasColumnName("ID_Usuario")
            .HasColumnType("int")
            .ValueGeneratedOnAdd();

        builder.Property(row => row.Name)
            .IsRequired()
            .HasColumnName("Nombre")
            .HasColumnType("varchar(250)");

        builder.Property(row => row.Identification)
            .IsRequired()
            .HasColumnName("Identificacion")
            .HasColumnType("varchar(250)");

        builder.Property(row => row.Password)
            .IsRequired()
            .HasColumnName("Contrasena")
            .HasColumnType("varchar(250)");

        builder.Property(row => row.Email)
            .IsRequired()
            .HasColumnName("Correo")
            .HasColumnType("varchar(250)");
    }
}