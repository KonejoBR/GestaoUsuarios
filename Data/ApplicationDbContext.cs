using GestaoUsuarios.Models.Usuarios;
using Microsoft.EntityFrameworkCore;

namespace GestaoUsuarios.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("TbUsuario");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nome).HasMaxLength(150).IsRequired();
            entity.Property(u => u.ValorHora).HasColumnType("decimal(18,2)");
            entity.Property(u => u.DataCadastro).HasColumnType("datetime");
            entity.Property(u => u.Ativo).IsRequired();
        });
    }
}
