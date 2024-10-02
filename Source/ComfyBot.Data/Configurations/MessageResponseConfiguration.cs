using ComfyBot.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace ComfyBot.Data.Configurations;

[ExcludeFromCodeCoverage]
public class MessageResponseConfiguration : EntityConfiguration<MessageResponse>
{
    public MessageResponseConfiguration() : base("MessageResponse")
    {
    }

    protected override void ConfigureInternal(EntityTypeBuilder<MessageResponse> builder)
    {
        builder.Property(x => x.Users);
        builder.Property(x => x.LooseKeywords);
        builder.Property(x => x.AllKeywords);
        builder.Property(x => x.ExactKeywords);
        builder.Property(x => x.Replies);
        builder.Property(x => x.LastUsedAt).IsRequired(false);
        builder.Property(x => x.TimeoutInSeconds).IsRequired().HasDefaultValue(30);
        builder.Property(x => x.UseCount).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.Priority).IsRequired().HasDefaultValue(0);
        builder.Property(x => x.AlwaysReply).IsRequired().HasDefaultValue(false);
    }
}