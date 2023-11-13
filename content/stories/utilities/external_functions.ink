INCLUDE ../utilities/capitalize.ink

VAR external_mode = -1
VAR external_speaker_name = ""
VAR external_character_left_name = ""
VAR external_character_left_expression = ""
VAR external_character_right_name = ""
VAR external_character_right_expression = ""

// Clears the screen of text. Has no effect on other things, such as backgrounds or talking characters.
EXTERNAL clear_screen()
=== function clear_screen() ===
<hr>
~ return

// Switches to the full-screen narration mode and clears the screen of text. If we're already in narration mode, does nothing.
EXTERNAL narration_mode()
=== function narration_mode() ===
{ external_mode != 0:
    <hr>
    >> Switched to narration mode. <<
    ~ external_mode = 0
}

~ return

// Switches to the half-screen dialogue mode, clears the screen of text, and hides any character sprites left over from the last time we entered dialogue mode. If we're already in dialogue mode, does nothing.
EXTERNAL dialogue_mode()
=== function dialogue_mode() ===
{ external_mode != 1:
    <hr>
    >> Switched to dialogue mode. <<
    >> Now speaking: [nobody] <<
    ~ external_mode = 1
    ~ external_speaker_name = ""
}

~ return

EXTERNAL speaker_name(name)
=== function speaker_name(name) ===
{ external_speaker_name != name:
    ~ external_speaker_name = name
    { name == "":
        <hr>
        >> Now speaking: [nobody] <<
    - else:
        <hr>
        >> Now speaking: {name} <<
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
        >> Nobody is on the left. <<
    - else:
        >> {capitalise_start(external_character_left_name)}, on the left, is {external_character_left_expression}. <<
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
        >> Nobody is on the right. <<
    - else:
        >> {capitalise_start(external_character_right_name)}, on the right, is {external_character_right_expression}. <<
    }
}

~ return