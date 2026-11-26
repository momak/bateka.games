using bateka.games.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace bateka.games.Data.TypeConfigurations;

public sealed class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> b)
    {
        b.HasKey(x => x.UserId);
        b.Property(x => x.UserId).HasMaxLength(ColumnLengths.UserId);
    }
}
