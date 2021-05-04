# SBC Control

SBC Control will provide hotkey functionality for Sound Blaster Command.

## Patching Sound Blaster Command for SBC Control compatibility

### Tools needed
- [dnSpy](https://github.com/dnSpy/dnSpy)
- [Reflexil](https://github.com/sailro/Reflexil)

Target patch location:
- `C:\Program Files (x86)\Creative\Sound Blaster Command\Platform\Creative.SBConnect.UI.Framework.dll`

### Patching

- Using reflexil, inject an assembly reference to `SBC_Control`
    - Save the new assembly and open it in `dnSpy`
- Navigate to `Creative.SBConnect.UI.Framework.CMDViewModels.BottomBar.BaseBottomBarViewModel`
- Insert a new `private` method named `InitSbcControl`
- Edit the method and copy pasta the code below, make sure everything compiles
- Edit the public constructor
    - Open the IL instruction editor and insert the 2 instructions before the
    function return.
    ```
        ldarg.0
        call instance void ...InitSbcControl
    ```
- Save the module

```c#
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using Creative.Platform.Devices.Features.SpeakerConfigs;
using Creative.Platform.Devices.Models;
using Creative.Platform.Devices.Selections;
using Creative.Platform.Mixer;
using Creative.Platform.Profiles.Audio;
using Creative.Platform.Profiles.Eq;
using Creative.SBConnect.UI.Framework.CMDViewModels.Cinematic;
using Creative.SBConnect.UI.Framework.Utils;
using Creative.SBConnect.UI.Framework.ViewModels;
using log4net;
using SBC_Control;

namespace Creative.SBConnect.UI.Framework.CMDViewModels.BottomBar
{

    public partial class BaseBottomBarViewModel : BottomBarViewModel
    {
        public void InitSbcControl()
        {
            Api.Init(this.mainThreadDispatcher, this);
        }
    }
}
```

## Installing SBC Control

- Close SBC if it is open
- Overwrite the original dll with our patched version
- Build the SBC Control source code
- Copy the `SBC_Control.dll` to the same location as the patched DLL
- Open Sound Blaster Command

Take a look at the AutoHotkey scripts for usage of SBC Control.

# Credits

- [G33kDude's Socket library](`https://github.com/G33kDude/Socket.ahk`) for AutoHotkey
