using bateka.games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bateka.games.Data.TypeConfigurations;

public sealed class GameResultConfiguration : IEntityTypeConfiguration<GameResult>
{
    public void Configure(EntityTypeBuilder<GameResult> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.UserId).HasMaxLength(ColumnLengths.UserId).IsRequired();

        b.Property(x => x.Game).HasConversion<string>().HasMaxLength(ColumnLengths.EnumName);
        b.Property(x => x.Outcome).HasConversion<string>().HasMaxLength(ColumnLengths.EnumName);
        b.Property(x => x.Difficulty).HasConversion<string>().HasMaxLength(ColumnLengths.EnumName);

        // my stats: all my results
        b.HasIndex(x => new { x.UserId, x.Game });

        // the leaderboards (competition increment): one game, one size / difficulty, everyone
        b.HasIndex(x => new { x.Game, x.Difficulty });
        b.HasIndex(x => new { x.Game, x.Pairs });

        // Healthchecks' "games in the last 24 h"
        b.HasIndex(x => x.PlayedAt);
    }
}
