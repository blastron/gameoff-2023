INCLUDE ../utilities/external_functions.ink


~ narration_mode()

[b]Lorem ipsum[/b] dolor sit amet, consectetur adipiscing elit.

Etiam molestie eget turpis ut imperdiet. Mauris semper orci ex, non porttitor magna vulputate non. Sed sed orci orci. Duis eu [color=red]dignissim orci[/color], sed iaculis elit.

~ clear_screen()

[b]Nulla facilisi.[/b]

~ dialogue_mode()

~ speaker_name("")
~ left_character("tellus", "idle")

Cras sit amet arcu metus. Donec non ipsum ac quam pretium rutrum.

~ speaker_name("Tellus")
~ left_character("tellus", "talking")
"Mauris justo est? Pulvinar eget sagittis fringilla, rhoncus at nisi. Maecenas finibus sapien et orci mollis, vel auctor libero aliquet."

~ speaker_name("Aliquam")
~ left_character("tellus", "idle")
~ right_character("aliquam", "talking")

"Nam volutpat tincidunt, Tellus, sit amet auctor libero elementum eu."

"Etiam nec malesuada quam."

~ speaker_name("")
~ right_character("aliquam", "idle")

Donec [color=red]scelerisque[/color] purus arcu, vitae eleifend justo sollicitudin eu.

*   [Pellentesque et lorem at libero tincidunt pretium.]
    ~ speaker_name("Tellus")
    ~ left_character("tellus", "talking")
    "Curabitur mi nisl, dapibus eget risus eget, aliquet tempor eros. Praesent mi leo, imperdiet in eros vel, commodo eleifend ante."
    ~ left_character("tellus", "idle")
*   [Proin viverra eu risus vitae consectetur.]
    Morbi rutrum nibh at laoreet malesuada. Suspendisse lacinia vestibulum lectus, nec aliquam ligula eleifend nec.
            
-   ~ speaker_name("")
Vestibulum eu magna id magna lacinia lacinia.