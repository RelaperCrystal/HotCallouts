# Hot Call-outs
This is a plug-in for *LCPD First Response*, adds several call-outs and world events to *LCPD First Response* mod!

Download the latest: `https://github.com/RelaperCrystal/HotCallouts/releases/latest`

Please note the author only has two accounts: [@rcdraft](https://github.com/rcdraft) and [@RelaperCrystal](https://github.com/RelaperCrystal). Author will **never** give someone else push permissions. If you want to contribute to this project, please use **pull request**.

## Contents

Currently have *3 call-outs* and *1 world events*.

Call-outs:

- Firearm Attack On An Officer
- Dangerous Driving
- Killing Spree In Progress

World Events:

- Mugging

## Installation

1. Make sure LCPD First Response mod is installed and working properly. Then, download latest version of this plug-in at releases.

2. Put the `HotCallout.dll` and `HotCallout.pdb` into `Grand Theft Auto IV\LCPDFR\plugins\`, and the `Grand Theft Auto IV` represents where you installed Grand Theft Auto IV, or where the `GTAIV.exe` or `EFLC.exe` at. The `pdb` file is not required, but you better put it in the `plugins` folder.

3. Launch the game, make sure when you on duty *(after going out the station first time after launched the LCPDFR, or using `ForceDuty` command)*, having a text on text wall like this:

   `[SYSTEM] HotCallout, (C) 2019 RelaperCrystal`

4. If step 3 produces `[SYSTEM] HotCallout, (C) 2019 RelaperCrystal` text on text wall, the plug-in are **installed and loaded**. Good job, you can now enjoy the plug-in.

## Warnings

This plug-in probably **wont** load itself. If it does happens, please check the plug-in if even not installed, or in the  `Grand Theft Auto IV\LCPDFR\plugins\` folder. If it does, re-launch the game, check the text wall when you go on-duty and exits police station, or going on duty using `ForceDuty` command. You'll see these following text showing on the text wall if the plug-in is loaded correctly, and call-outs and world events should work.

`[SYSTEM] HotCallout, (C) 2019 RelaperCrystal`

If you still not seeing this text, then send a issue **ASAP.**

You probably receive LCPDFR Crash Message while stopping a suspect when `hMugging` world event happens. This is caused by `AimingManager` cannot takeover the suspect from the world event. I'm still working on this bug.

## Crashes

This plug-in **may crash one of the LCPDFR, this plug-in, or the game.** If it happens. please consider send an **issue** to make me know. Most of times, for now, all bug fixes was found by me, by playing the game, so I has the log file. But, your crash reporting to me requires either *clearly described steps to reproduce* or *the full log file after the crash*. They named by `LCPDFR.log`. By that, I can found *where the error happens*, if it's an LCPDFR crash or plug-in crash. For game crash, please describe clearly, *how did you do before game crash, or how did you made the game crash*, to let me reproduce the crash.

Please note that *random crash* was **unable** to diagnostic, or fix. If the random crash happens after some special action, please consider describe how did you to to initialize random crashes. 

Without a diagnostic, there's no way I can fix it. If I can reproduce this issue, or log file has the reason how did it crash, this is usually fixed in next release, or in the source. 