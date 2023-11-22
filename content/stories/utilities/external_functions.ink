INCLUDE ../utilities/capitalize.ink

VAR external_mode = -1
VAR external_speaker_name = ""
VAR external_character_left_name = ""
VAR external_character_left_expression = ""
VAR external_character_right_name = ""
VAR external_character_right_expression = ""
VAR external_walkaround_location = ""

// Clears the screen of text. Has no effect on other things, such as backgrounds or talking characters.
EXTERNAL clear_screen()
=== function clear_screen() ===
<hr>
~ return

// Switches to the full-screen narration mode and clears the screen of text. If we're already in narration mode, does nothing.
EXTERNAL narration_mode()
=== function narration_mode() ===
{ external_mode != 0:
    ~ external_mode = 0
    <hr>
    <b><tt>Switched to narration mode.</tt></b>
}

~ return

// Switches to the half-screen dialogue mode, clears the screen of text, and hides any character sprites left over from the last time we entered dialogue mode. If we're already in dialogue mode, does nothing.
EXTERNAL dialogue_mode()
=== function dialogue_mode() ===
{ external_mode != 1:
    ~ external_mode = 1
    ~ external_speaker_name = ""
    <hr>
    <b><tt>Switched to dialogue mode. Nobody is speaking.</tt></b>
}

~ return

// If the player was not in walkaround mode, clears and hides all text, then switches to walkaround mode. If the given location is not the location the player was previously at, moves the player to that location and places them at the spawn point; otherwise, the player will be left as-is.
// If the player was already in walkaround mode, but was not at the given location, moves the player to that location at the given spawn point.
EXTERNAL walkaround_mode(location, spawn)
=== function walkaround_mode(location, spawn) ===
{
    - external_mode != 2:
        ~ external_mode = 2
        <hr>
        { external_walkaround_location != location:
            ~ external_walkaround_location = location
            <b><tt>Switching to walkaround mode, moving to <u>{location}</u> and spawning at <u>{spawn}</u>.</tt></b>
        - else:
            <b><tt>Switched back to walkaround mode. Currently at <u>{location}</u>.</tt></b>
        }
    - external_walkaround_location != location:
        ~ external_walkaround_location = location
        <b><tt>Moving to <u>{location}</u>.</tt></b>
}

~ return

EXTERNAL speaker_name(name)
=== function speaker_name(name) ===
{ external_speaker_name != name:
    ~ external_speaker_name = name
    { name == "":
        <hr>
        <b><tt>Now speaking: [nobody]</tt></b>
    - else:
        <hr>
        <b><tt>Now speaking: <u>{capitalise_start(name)}</u></tt></b>
    }
}

~ return

EXTERNAL left_character(name, expression)
=== function left_character(name, expression) ===
~ temp changed = false
{ external_character_left_name != name:
    ~ changed = true
    ~ external_character_left_name = name
}

{ external_character_left_expression != expression:
    ~ changed = true
    ~ external_character_left_expression = expression
}

{ changed:
    { external_character_left_name == "":
        <b><tt>Cleared the left sprite.</tt></b>
    - else:
        <b><tt><u>{capitalise_start(external_character_left_name)}</u>, on the left, is <u>{external_character_left_expression}</u>.</tt></b>
    }
}

~ return

EXTERNAL right_character(name, expression)
=== function right_character(name, expression) ===
~ temp changed = false
{ external_character_right_name != name:
    ~ changed = true
    ~ external_character_right_name = name
}

{ external_character_right_expression != expression:
    ~ changed = true
    ~ external_character_right_expression = expression
}

{ changed:
    { external_character_right_name == "":
        <b></tt>Cleared the right sprite.</tt></b>
    - else:
        <b><tt><u>{capitalise_start(external_character_right_name)}</u>, on the right, is <u>{external_character_right_expression}</u>.</tt></b>
    }
}

~ return