// Sharing an invite link: the phone's share sheet (Web Share API) or the clipboard.
// ES module, loaded through IJSObjectReference (Interop/ShareInterop.cs).

// True when the share sheet opened (and wasn't cancelled); false when the browser has none.
export async function share(title, text, url) {
    if (!navigator.share) return false;
    try {
        await navigator.share({ title, text, url });
        return true;
    } catch {
        // the user closed the sheet, or sharing isn't allowed here
        return false;
    }
}

export function canShare() {
    return !!navigator.share;
}

// True when the text is on the clipboard.
export async function copy(text) {
    try {
        await navigator.clipboard.writeText(text);
        return true;
    } catch {
        return false;
    }
}
