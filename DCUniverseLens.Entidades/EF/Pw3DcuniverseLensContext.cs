using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DCUniverseLens.Entidades.EF;

public partial class Pw3DcuniverseLensContext : DbContext
{
    public Pw3DcuniverseLensContext()
    {
    }

    public Pw3DcuniverseLensContext(DbContextOptions<Pw3DcuniverseLensContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<Pelicula> Peliculas { get; set; }

    public virtual DbSet<Personaje> Personajes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=PC-GAMER;Database=PW3-DCUniverseLens;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Actor__3214EC07190E73DF");

            entity.ToTable("Actor");

            entity.Property(e => e.FechaNacimiento)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Nacimiento");
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.PaisOrigen).HasMaxLength(50);
        });

        modelBuilder.Entity<Pelicula>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pelicula__3214EC078FB7334B");

            entity.ToTable("Pelicula");

            entity.Property(e => e.Ano).HasMaxLength(4);
            entity.Property(e => e.Descripcion).HasColumnType("text");
            entity.Property(e => e.Nombre).HasMaxLength(50);
        });

        modelBuilder.Entity<Personaje>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personaj__3214EC07F3979459");

            entity.ToTable("Personaje");

            entity.Property(e => e.Apodo).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Poderes).HasMaxLength(500);

            entity.HasOne(d => d.IdActorNavigation).WithMany(p => p.Personajes)
                .HasForeignKey(d => d.IdActor)
                .HasConstraintName("FK_Personaje_Actor");

            entity.HasMany(d => d.IdPeliculas).WithMany(p => p.IdPersonajes)
                .UsingEntity<Dictionary<string, object>>(
                    "PersonajePelicula",
                    r => r.HasOne<Pelicula>().WithMany()
                        .HasForeignKey("IdPelicula")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Personaje_Pelicula_IdPelicula"),
                    l => l.HasOne<Personaje>().WithMany()
                        .HasForeignKey("IdPersonaje")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Personaje_Pelicula_IdPersonaje"),
                    j =>
                    {
                        j.HasKey("IdPersonaje", "IdPelicula");
                        j.ToTable("Personaje_Pelicula");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
