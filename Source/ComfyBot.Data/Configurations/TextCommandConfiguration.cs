using ComfyBot.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace ComfyBot.Data.Configurations;

[ExcludeFromCodeCoverage]
public class TextCommandConfiguration : EntityConfiguration<TextCommand>
{
    public TextCommandConfiguration() : base("TextCommandOld")
    {
    }

    protected override void ConfigureInternal(EntityTypeBuilder<TextCommand> builder)
    {
        builder.Property(x => x.Replies);
        builder.Property(x => x.Commands);
        builder.Property(x => x.LastUsedAt).IsRequired(false);
        builder.Property(x => x.UseCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.TimeoutInSeconds).IsRequired().HasDefaultValue(0);
    }
}