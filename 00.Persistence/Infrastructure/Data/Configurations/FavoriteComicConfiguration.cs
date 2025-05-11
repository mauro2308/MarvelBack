using Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

internal class FavoriteComicConfiguration : IEntityTypeConfiguration<FavoriteComic>
{
    public void Configure(EntityTypeBuilder<FavoriteComic> builder)
    {
        builder.ToTable("Comic_Favorito",DataConstants.SCHEMA);

        builder.HasKey(row => row.Id)
            .HasName("PK_Id");

        builder.Property(row => row.Id)
            .IsRequired()
            .HasColumnName("Id")
            .HasColumnType("int")
            .ValueGeneratedOnAdd();

        builder.Property(row => row.UserId)
            .IsRequired()
            .HasColumnName("ID_Usuario")
            .HasColumnType("int");

        builder.HasOne(cn => cn.NaviUser)
            .WithMany(cn => cn.FavoriteComics)
            .HasForeignKey(cn => cn.UserId)
            .HasConstraintName("FK_Comic_Favorito_Usuarios")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(row => row.ComicId)
            .IsRequired()
            .HasColumnName("ID_Comic")
            .HasColumnType("int");

        builder.Property(row => row.ComicTitle)
            .IsRequired()
            .HasColumnName("Titulo_Comic")
            .HasColumnType("varchar(250)");

        builder.Property(row => row.ComicDescription)
            .IsRequired(false)
            .HasColumnName("Descripcion_Comic")
            .HasColumnType("varchar(250)");

        builder.Property(row => row.ComicImageUrl)
            .IsRequired()
            .HasColumnName("Url_Comic_Imagen")
            .HasColumnType("varchar(250)");
    }
}