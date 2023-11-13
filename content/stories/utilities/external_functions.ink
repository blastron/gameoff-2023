VAR external_mode = -1
VAR external_speaker_name = ""

// Clears the screen of text. Has no effect on other things, such as backgrounds or talking characters.
EXTERNAL clear_screen()
=== function clear_screen() ===
>>The screen clears.<<
~ return

// Switches to the full-screen narration mode and clears the screen of text. If we're already in narration mode, does nothing.
EXTERNAL narration_mode()
=== function narration_mode() ===
{ external_mode != 0:
    <hr>
    >>Switched to narration mode.<<
    ~ external_mode = 0
}

~ return

// Switches to the half-screen dialogue mode, clears the screen of text, and hides any character sprites left over from the last time we entered dialogue mode. If we're already in dialogue mode, does nothing.
EXTERNAL dialogue_mode()
=== function dialogue_mode() ===
{ external_mode != 1:
    <hr>
    >>Switched to dialogue mode.<<
    >>Now speaking: [nobody]<<
    ~ external_mode = 1
    ~ external_speaker_name = ""
}

~ return

EXTERNAL set_speaker(name)
=== function set_speaker(name) ===
{ external_speaker_name != name:
    ~ external_speaker_name = name
    { name == "":
        <hr>
        >>Now speaking: [nobody]<<
    - else:
        <hr>
        >>Now speaking: {name}<<
    }
}

~ return