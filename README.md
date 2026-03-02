# KickstartPalPatcher
A tool to patch Amiga Kickstart Roms so that they always start in PAL mode, even if an NTSC Agnus is installed 

Unfortunately, the 2MB capable PAL Agnus chips are hard to come by, but the cheaper NTSC variants are available.
However, since these chips can be switched from NTSC to PAL, it just needs something to do the switching right at system startup.

Therefore, some users of the A1K forum have asked for a way to change the Amiga's kickstart so that it immediately activates PAL mode (see https://www.a1k.org/forum/index.php?threads/78218/ for the discussion).

Thanks to the patches from A10001986 and DingensCGN I was able to create such a tool.

Starting with Version 2, this Tool has a GUI now.

## All Newer 3.x Roms are (should be) supported by the Autopatcher.<br/>
I testet Version 3.2.3 and Some Beta Versions of 3.3. They all worked.

If there are any special ROMs that the autopatcher does not recognize, the patches.json file now included in the project can be customized to enable individual patches.

## The Patcher still supports some older Kickstarts with fixed Versions and Patches. I cold remove all 3.x versions here, but I keep tkem in the patches.json as example for individual patches
That are: <br/>
Amiga 500/1000/2000:
Kickstart 1.2 Rev 33.192 original checksum: $56F2E2A6<br/>
Kickstart 1.3 Rev 34.5 original checksum: $15267DB3<br/>
Kickstart 2.04 Rev. 37.175 original checksum: $000B927C<br/>
Kickstart 2.05 Rev. 37.350 original checksum: $BA5D5FA4<br/>

Amiga 1200:
Kickstart 3.1 Rev 40.68 original checksum: $87BA7A3E<br/>
Kickstart 3.2 Rev 47.69 original checksum: $E3B7D1D5<br/>
Kickstart 3.2.1 Rev 47.102 original checksum: $7A47FC4D<br/>
Kickstart 3.2.2 Rev 47.111 original checksum: $DB198F9E<br/>

Amiga 3000:
Kickstart 3.2.2 Rev 47.111 original checksum: $70A81C49<br/>




