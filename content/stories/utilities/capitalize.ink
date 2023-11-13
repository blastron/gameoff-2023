// Ink capitalisation code created by IFcoltransG
// Released into public domain
// May be used under the MIT No Attribution License

CONST START = "^^"

LIST letters = (a), (b), (c), (d), (e), (f), (g), (h), (i), (j), (k), (l), (m), (n), (o), (p), (q), (r), (s), (t), (u), (v), (w), (x), (y), (z), /*
    */ A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z

Here is an example of the word "hello" capitalised: {capitalise_start("hello")}
Here is an example of the word "yay" shouted: {capitalise_whole("yay")}


/*
    Capitalises the first letter of a word made of ASCII English letters.
    Will cut off any word at the first punctuation.
    e.g. capitalise_start("hello") == "Hello"
    e.g. capitalise_start("HELLO") == "HELLO"
*/
=== function capitalise_start(word) ===
    {word == "":
        ~ return ""
    }
    ~ temp start = first_letter_after(word, "", LIST_ALL(letters))
    ~ return "{capitalise_letter(start)}" + rest_of_letters(word, "{start}")

/*
    Capitalises a word made of ASCII English letters.
    Will cut off any word at the first punctuation.
    e.g. capitalise_whole("hello") == "HELLO"
    e.g. capitalise_whole("HELLO") == "HELLO"
*/
=== function capitalise_whole(word) ===
    {word == "":
        ~ return ""
    }
    ~ temp start = first_letter_after(word, "", LIST_ALL(letters))
    ~ temp rest = rest_of_letters(word, "{start}")
    ~ return "{capitalise_letter(start)}" + capitalise_whole(rest)


// OTHER FUNCTIONS BELOW

/*
    Gets the rest of the ASCII English letters after a certain start.
    e.g. rest_of_letters("hello", "he") == "llo"
*/
=== function rest_of_letters(word, start) ===
    {word == start:
        ~ return ""
    }
    ~ temp next_letter = "{first_letter_after(word, start, LIST_ALL(letters))}"
    {next_letter == "":
            ~ return ""
        - else:
            ~ return next_letter + rest_of_letters(word, start + next_letter)
    }

/*
    Takes a list element from `letters` and capitalises it.
    e.g. capitalise_letter(a) == A
    e.g. capitalise_letter(A) == A
*/
=== function capitalise_letter(letter) ===
    {letters ? letter:
            ~ return letter + 26
        - else:
            ~ return letter
    }

/*
    Gets the first ASCII English letter after a certain start.
    `options` parameter is an internal detail and should be passed `LIST_ALL(letters)`
    e.g. first_letter_after("hello", "he", LIST_ALL(letters)) == "l"
*/
=== function first_letter_after(word, start, options) ===
    {options:
            ~ temp test_letter = pop(options)
            {starts_with(word, "{start}{test_letter}"):
                ~ return test_letter
            }
            ~ return first_letter_after(word, start, options)
        - else:
            ~ return ()
    }

/*
    Checks if a certain string starts with another string.
    The `START` constant should be set to a value that does not appear within strings.
*/
=== function starts_with(word, start) ===
    ~ return START + word ? START + start

/*
	Takes the bottom element from a list, and returns it, modifying the list.

	Returns the empty list () if the source list is empty.

	Usage: 

	LIST fruitBowl = (apple), (banana), (melon)

	I eat the {pop(fruitBowl)}. Now the bowl contains {fruitBowl}.

*/
=== function pop(ref _list) 
    ~ temp el = LIST_MIN(_list) 
    ~ _list -= el
    ~ return el 
