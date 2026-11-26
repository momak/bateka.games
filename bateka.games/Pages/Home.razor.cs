using bateka.games.Services;
using bateka.games.Shared.Contracts.Notes;
using Bateka.UI.Components.Common;
using Microsoft.AspNetCore.Components;

namespace bateka.games.Pages;

/// <summary>The games, and who passed you on the boards since you last looked.</summary>
public partial class Home : BusyComponentBase
{
    [Inject] private NotesApiClient Notes { get; set; } = default!;

    /// <summary>Null while loading.</summary>
    private List<NoteDto>? _notes;

    protected override async Task OnInitializedAsync() =>
        _notes = await LoadAsync(() => Notes.ListAsync()) ?? [];

    /// <summary>"Got it": marks everything read, so the card goes until someone passes you again.</summary>
    private async Task DismissNotesAsync()
    {
        if (await RunAsync(() => Notes.MarkSeenAsync()))
        {
            _notes = [];
        }
    }
}
