using ComfyBot.Data.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ComfyBot.Data.Configurations;

public class VariableConfiguration : EntityConfiguration<Variable>
{
    public VariableConfiguration() : base("Variable")
    {
    }

    protected override void ConfigureInternal(EntityTypeBuilder<Variable> builder)
    {
        builder.Property(x => x.Name).IsRequired(false);
        builder.Property(x => x.Value).IsRequired(false);
    }
}