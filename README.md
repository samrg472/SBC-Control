# SBC Control

SBC Control improves functionality for Creative Sound Blaster Command.

### Features

- Advanced hotkey functionality using AutoHotkey
- Save and restore volumes for headphones and speakers

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
- Edit the public constructor
    - Open the IL instruction editor and insert the 2 instructions before the
    function return.
    ```
        ldarg.0
        call void ...SBC_Control.Api::Init(object)
    ```
- Save the module

## Installing SBC Control

- Close SBC if it is open
- Overwrite the original dll with our patched version
- Build the SBC Control source code
- Copy the `SBC_Control.dll` to the same location as the patched DLL
- Open Sound Blaster Command

Take a look at the AutoHotkey scripts for usage of SBC Control.

# Credits

- [G33kDude's Socket library](https://github.com/G33kDude/Socket.ahk) for AutoHotkey
