# KickstartPalPatcher
A tool to patch Amiga Kickstart Roms so that they always start in Pal mode, even if an NTSC Agnus is installed 

Unfortunately, the 2MB capable PAL Agnus chips are hard to come by, but the cheaper NTSC variants are.
However, since these chips can be switched from NTSC to PAL, it just needs something to do the switching right at system startup.

Therefore, some users of the A1K forum have asked for a way to change the Amiga's kickstart so that it immediately activates Pal mode.

Thanks to the patches from A10001986 and DingesCGN I was able to create such a tool.

It is a command line tool for Windows built with the .Net Framework
