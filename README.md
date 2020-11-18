# LogicTask
Task 3: Logic

Create a script (preferred a scriptable object, to also work in editor mode) which allows to parse/find out, if a sentence you put into an input field (e.g. “I like bananas, but I prefer strawberries!”) is part of pre-defined parsing-codes. Such a parsing code could be e.g.: “&like &banana[s]”, where & means that this word needs to be within the sentence and [s] means that this “s” is optional and both versions would work, banana as well as bananas. This is only an example; of course your parsing should also work with other parsing codes and other sentences.
(Optional, medium): Additionally increase your solution so that for each of many parsing codes you can set a priority and in the end, the parsing code which was true and had the highest priority, will be the result of the check. E.g. “&prefer &strawberr[y/ies]” has a higher priority than “&like &banana[s]” and so, even if both parsing codes are true, the prefer-strawberries-parsing will be the result.
(Optional, hard / more effort): Additionally add an “|” as “or” (alternative to “and” with “&”), brackets “()” to summarize conditions and negotiations with “!”, which means that this word or bracket-content must NOT be true, or the parsing will fail. Example: “(&prefer &strawberr[y/ies]) | !(&like & banana[s])”


READMY
Parsing rules syntaxis:
1) Use "&" like a and. This base iteration.
Example:
&work &ladder
Text must contain "work" and "ladder".
2) Use "[/]" for optional word endings.
Example:
&burn[ed/s] & cod[es/ing]
Text must contain (burned or burns) and (codes or coding).
3) Case sensitive. &fire can't find "Fire" in text, only "fire".
4) Use order for parser priority. Ordere must be >= 0.
5) Use "|" as or. Example
&fire | &water
Text must contain fire OR water
6) Use "()" for summarize conditions.
Example
(&fire | &water) !&earth
Text contain fire or water and not contain earth
