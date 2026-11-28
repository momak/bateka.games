using bateka.games.Shared.Contracts.Matches;
using FluentValidation;

namespace bateka.games.Shared.Validation;

/// <summary>Only the shape: whether the square or column exists depends on the game (MatchRules, in the service).</summary>
public sealed class MakeMoveRequestValidator : AbstractValidator<MakeMoveRequest>
{
    public MakeMoveRequestValidator()
    {
        RuleFor(x => x.Move).GreaterThanOrEqualTo(0);
    }
}
