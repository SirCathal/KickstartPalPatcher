# KickstartPalPatcher
A tool to patch Amiga Kickstart Roms so that they always start in PAL mode, even if an NTSC Agnus is installed 

Unfortunately, the 2MB capable PAL Agnus chips are hard to come by, but the cheaper NTSC variants are available.
However, since these chips can be switched from NTSC to PAL, it just needs something to do the switching right at system startup.

Therefore, some users of the A1K forum have asked for a way to change the Amiga's kickstart so that it immediately activates PAL mode (see https://www.a1k.org/forum/index.php?threads/78218/ for the discussion).

Thanks to the patches from A10001986 and DingensCGN I was able to create such a tool.

It is a command line tool for Windows built with the .Net Framework

Usage:
```
PalPatcher [Kick1.rom] [Kick2.rom] ...
```
This will patch the given files only

```
PalPatcher [KickRomFolder]
```
This will patch all supported Kickstart files in the given folder

Important: No original file will be changed!
Any supported Kickstart will be copied and a '.Pal.Rom' suffix will be added.

Please note that the patched Kickstarts will only work with a 8372A and 8375 Agnus Chips
which are switchable from NTSC to PAL!<br/>

Attention! The program needs the .ROM executable files. 
The .bin "ByteSwap" versions for eprom burning are not recognized!
If the new ROM file created by the program is to be burned to an Eprom, a ByteSwap must be made before burning it.

New in V1.0.0.8.
Support for Kickstart 3.2.2 (A500, A600, A1000, A2000, CDTV and A1200)

Supported in this version:<br/>
Amiga 500/1000/2000:<br/>
Kickstart 1.2 Rev 33.192 original checksum: $56F2E2A6<br/>
Kickstart 1.3 Rev 34.5 original checksum: $15267DB3<br/>
Kickstart 2.04 Rev. 37.175 original checksum: $000B927C<br/>
Kickstart 2.05 Rev. 37.350 original checksum: $BA5D5FA4<br/>
Kickstart 3.1 Rev 40.63 original checksum: $9FDEEEF6<br/>
Kickstart 3.1.4 Rev 46.143 original checksum: $BE662BCF<br/>
Kickstart 3.2.0 Rev 47.69 original checksum: $35D98F3<br/>
Kickstart 3.2.1 Rev 47.102 original checksum: $4CB8FDD9<br/>
Kickstart 3.2.2 Rev 47.111 original checksum: $B1728E0A<br/>

Amiga 1200:<br/>
Kickstart 3.1 Rev 40.68 original checksum: $87BA7A3E<br/>
Kickstart 3.2 Rev 47.69 original checksum: $E3B7D1D5<br/>
Kickstart 3.2.1 Rev 47.102 original checksum: $7A47FC4D<br/>
Kickstart 3.2.2 Rev 47.111 original checksum: $DB198F9E<br/>

Special thanks to the A1K.org users A10001986 and DingensCGN for the patch data<br/>
