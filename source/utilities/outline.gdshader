shader_type canvas_item;

uniform bool enabled = false;
uniform vec4 enabled_color : source_color = vec4(1.0);
uniform vec4 disabled_color : source_color = vec4(0.0);
uniform int line_thickness : hint_range(0, 10) = 1;

const vec2 OFFSETS[8] = {
	vec2(-1, -1), vec2(-1, 0), vec2(-1, 1), vec2(0, -1), vec2(0, 1), 
	vec2(1, -1), vec2(1, 0), vec2(1, 1)
};

void fragment() {
	vec2 size = TEXTURE_PIXEL_SIZE * float(line_thickness);
	float outline = 0.0;
	
	for (int i = 0; i < OFFSETS.length(); i++) {
		outline += texture(TEXTURE, UV + size * OFFSETS[i]).a;
	}
	outline = min(outline, 1.0);
	
	vec4 color = texture(TEXTURE, UV);
	COLOR = mix(color, enabled ? enabled_color : disabled_color, outline - color.a);
}