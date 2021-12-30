# What3Words (unofficial FiveM script)
What3Words is a UK-based resource for FiveM by Albo1125 that provides 3-word location functionality based on the What3Words app (this is not in any way affiliated with the official What3Words app).

## Installation & Usage
1. Download the latest release.
2. Unzip the What3Words folder into your resources folder on your FiveM server.
3. Add the following to your server.cfg file:
```text
ensure What3Words
```
4. Optionally, customise the words in `words.txt`.
5. Optionally, customise the command in `sv_What3Words.lua`.

## Commands & Controls
* /w3w W1.W2.W3 - Sets a waypoint to the location of W1.W2.W3. If unspecified, shows current W3W location and the W3W of your waypoint if set.


## Improvements & Licencing
Please view the license. Improvements and new feature additions are very welcome, please feel free to create a pull request. As a guideline, please do not release separate versions with minor modifications, but contribute to this repository directly. However, if you really do wish to release modified versions of my work, proper credit is always required and you should always link back to this original source and respect the licence.

## Libraries used (many thanks to their authors)
* [CitizenFX.Core.Client](https://www.nuget.org/packages/CitizenFX.Core.Client)
Thank you to the creators of the official What3Words.
