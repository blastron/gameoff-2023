VAR external_mode = -1

// Clears the screen of text. Has no effect on other things, such as backgrounds or talking characters.
EXTERNAL clear_screen()
=== function clear_screen() ===
>>The screen clears.<<
~ return

// Switches to the full-screen narration mode and clears the screen of text. If we're already in narration mode, does nothing.
EXTERNAL narration_mode()
=== function narration_mode() ===
{ external_mode == 0:
    >>Already in narration mode.<<
- else:
    >>Switched to narration mode.<<
    ~ external_mode = 0
}

~ return

// Switches to the half-screen dialogue mode, clears the screen of text, and hides any character sprites left over from the last time we entered dialogue mode. If we're already in dialogue mode, does nothing.
EXTERNAL dialogue_mode()
=== function dialogue_mode() ===
{ external_mode == 1:
    >>Already in dialogue mode.<<
- else:
    >>Switched to narration mode.<<
    ~ external_mode = 1
}

~ return