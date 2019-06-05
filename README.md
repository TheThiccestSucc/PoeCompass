PoeCompass
[![Github All Releases](https://img.shields.io/github/downloads/flacostec/PoeCompass/total.svg)](https://github.com/flacostec/PoeCompass/releases)
===

An overlay aiming to improve the quality of life when running leveling in [Path of Exile](https://www.pathofexile.com/).
If for some reason, you have to resume your leveling process and you are in the part 2, just double click the layout, it will swap to P2.

Features
---

1. **Compass**: The overlay will display a list of the possible zone layout you are in and short information about the layout.
2. **Zone laytout updating**: The compass will automatically update the displayed layout once entering a new zone. 
3. **Timer**: Will be added soon.



Downloads
---

See the [release](https://github.com/flacostec/PoeCompass/releases) section.

Screenshots
---
![Zone 2](https://github.com/flacostec/PoeCompass/blob/master/images/screenshot2.PNG)

FAQ
---

**Q: How does it work ?**

A: Every time you enter a new area, the client logs "You have entered XXXXXX." to Client.txt, which is used by PoeCompass to determine your current location.

**Q: The application can't find the client.log, where can I specify it ?**

A: The file *configuration.json* contains a path that you can adjust to your need. Do not forget to write double backslash.

**Q: How can I edit the notes ?**

A: The file *configuration.json* contains all the information. You can freely edit it.
