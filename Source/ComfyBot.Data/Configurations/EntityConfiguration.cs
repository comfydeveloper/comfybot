using ComfyBot.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ComfyBot.Data.Configurations;

[ExcludeFromCodeCoverage]
public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    private readonly string tableName;

    protected EntityConfiguration(string tableName)
    {
        this.tableName = tableName;
    }

    public void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.ToTable(this.tableName);

        builder.HasKey(x => x.Id)
            .HasAnnotation(nameof(DatabaseGeneratedOption), DatabaseGeneratedOption.None);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.DateOfCreation)
            .IsRequired();

        this.ConfigureInternal(builder);
    }

    protected abstract void ConfigureInternal(EntityTypeBuilder<TEntity> builder);
}