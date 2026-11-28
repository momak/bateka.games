using bateka.games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bateka.games.Data.TypeConfigurations;

public sealed class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.Game).HasConversion<string>().HasMaxLength(ColumnLengths.EnumName);
        b.Property(x => x.Status).HasConversion<string>().HasMaxLength(ColumnLengths.EnumName);
        b.Property(x => x.EndReason).HasConversion<string>().HasMaxLength(ColumnLengths.EnumName);

        b.Property(x => x.CreatorId).HasMaxLength(ColumnLengths.UserId).IsRequired();
        b.Property(x => x.OpponentId).HasMaxLength(ColumnLengths.UserId);
        b.Property(x => x.InvitedId).HasMaxLength(ColumnLengths.UserId);
        b.Property(x => x.FirstPlayerId).HasMaxLength(ColumnLengths.UserId);
        b.Property(x => x.WinnerId).HasMaxLength(ColumnLengths.UserId);

        // integer[] in Postgres
        b.Property(x => x.Moves);

        // xmin as concurrency token (Npgsql maps a uint row version to the system column)
        b.Property(x => x.Version).IsRowVersion();

        // "my matches", from either seat
        b.HasIndex(x => new { x.CreatorId, x.Status });
        b.HasIndex(x => new { x.OpponentId, x.Status });
        b.HasIndex(x => new { x.InvitedId, x.Status });

        // the clock: playing matches past their deadline, waiting invites past their lifetime
        b.HasIndex(x => new { x.Status, x.MoveDeadline });
        b.HasIndex(x => new { x.Status, x.CreatedAt });
    }
}
