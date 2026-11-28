using bateka.games.Shared.Contracts.Matches;
using bateka.games.Shared.Matches;
using FluentValidation;

namespace bateka.games.Shared.Validation;

public sealed class CreateMatchRequestValidator : AbstractValidator<CreateMatchRequest>
{
    private const string NotOnline = "This game can't be played online.";
    private const string NotAMoveTime = "Pick one of the offered times per move.";

    public CreateMatchRequestValidator()
    {
        RuleFor(x => x.Game).IsInEnum().Must(MatchRules.Supports).WithMessage(NotOnline);
        RuleFor(x => x.MoveSeconds).Must(seconds => MatchLimits.MoveSecondsOptions.Contains(seconds)).WithMessage(NotAMoveTime);
        RuleFor(x => x.Opponent).NotEmpty().MaximumLength(MatchLimits.MaxUsernameLength).When(x => x.Opponent is not null);
    }
}
