# KickstartPalPatcher
A tool to patch Amiga Kickstart Roms so that they always start in Pal mode, even if an NTSC Agnus is installed 

Unfortunately, the 2MB capable PAL Agnus chips are hard to come by, but the cheaper NTSC variants are.
However, since these chips can be switched from NTSC to PAL, it just needs something to do the switching right at system startup.

Therefore, some users of the A1K forum have asked for a way to change the Amiga's kickstart so that it immediately activates Pal mode.

Thanks to the patches from A10001986 and DingensCGN I was able to create such a tool.

It is a command line tool for Windows built with the .Net Framework

Usage: PalPatcher [Kick1.rom] [Kick2.rom] ...
That will patch the given Files only

Usage  PalPatcher [KickRomFolder]
That will patch all supportet Kickstart Files in the given folder

Important: No original File will be changed!
Any supportet Kickstart will be copied and a '.Pal.Rom' suffix will be added

Please keep in Mind that the patchet Kickstarts will only work with a 8372A and 8375 Agnus Chips
which are switchable from NTSC to PAL!

Supported in this Version:
Kickstart 1.2 Rev 33.192 original Checksum: $56F2E2A6
Kickstart 1.3 Rev 34.5 original Checksum: $15267DB3
Kickstart 3.1 Rev 40.63 original Checksum: $9FDEEEF6
Kickstart 3.1.4 Rev 46.143 original Checksum: $BE662BCF
Kickstart 3.2.0 Rev 47.69 original Checksum: $35D98F3
Kickstart 3.2.1 Rev 47.102 original Checksum: $4CB8FDD9

Spechial Thanks to the A1K.org Users A10001986 and DingensCGN for the Patch Data