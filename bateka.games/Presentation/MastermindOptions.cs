using bateka.games.Engine.Mastermind;

namespace bateka.games.Presentation;

/// <summary>
/// What the options panel edits (bound to its sliders and switches, so mutable). The board part
/// becomes a <see cref="MastermindSetup"/>; <see cref="ShowNumbers"/> is only how pegs look.
/// </summary>
public sealed class MastermindOptions
{
    public bool AllowRepeats { get; set; } = MastermindRules.Classic.AllowRepeats;
    public int Colours { get; set; } = MastermindRules.Classic.Colours;
    public int Columns { get; set; } = MastermindRules.Classic.Columns;
    public int Guesses { get; set; } = MastermindRules.Classic.Guesses;
    public bool ShowNumbers { get; set; } = true;

    public MastermindSetup ToSetup() => new(Columns, Colours, AllowRepeats, Guesses);

    /// <summary>Without repeats a code can't have more columns than there are colours.</summary>
    public bool IsPlayable => MastermindRules.IsValidSetup(ToSetup());

    public MastermindOptions Copy() => (MastermindOptions)MemberwiseClone();
}
