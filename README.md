

INTRODUCTION
------------

The CEW is a computer-controlled etymological dictionary which calculates reflexes from etymons. This project contains CEW-fr that implements the CEW for the French language.

In this GitHub project there are two zip files which include executable files with two different versions of the CEW-fr. The first one contains larger Latin and French lexicons (folder "CEW") whereas the second one provides only smaller lexicons which reduce the load time (folder "CEW mit kleinen Lexika").

 * For a full description of the program, see the accompanying doctoral thesis:
   Weber, Sylvia (2018): Konzeption und Umsetzung eines computergenerierten etymologischen Wörterbuchs am Beispiel des Französischen, Diss., Friedrich-Alexander-Universität Erlangen-Nürnberg.

   
REQUIREMENTS
------------

This program requires the following:

A) Use with Linux:

 * mono-runtime
 * gtk-sharp
 
 (The program has been tested with mono-runtime 4.6.2.7 and 5.12.0.226; gtk-sharp 2.12.40; Ubuntu 14.04 and 18.04.)


B) Use with Windows:

 * .Net Framework
 * Gtk# for .Net
 
 (The program has been tested with Gtk# for .Net 2.12.45.)
 
 or
 
 * Mono for Windows

 (The program has been tested with Mono for Windows 5.14.0.)

 
PROGRAM START
-------------

A) Use with Linux:
 
 * Start CEW.exe with Mono Runtime (see "Open With" in context menu).
 
 or
 
 * via Bash: mono CEW.exe
 
B) Use with Windows

 * Double-click the CEW.exe file.
 
NOTES
-----

If the International Phonetic Alphabet is not shown properly, please install "Deja Vu Sans" font.

 
AUTHOR
------

Sylvia Weber
