#NoEnv
#NoTrayIcon
#SingleInstance
#Include .\socket.ahk

; Configuration

global dialog_x_pos := 50
global dialog_y_pos := 201

; Script

global gui_notif_line1 := ""
global gui_notif_line2 := ""

BuildNotificationPopup()

Shift & F1::
    VarSetCapacity(buf, 1, 0x01)
    client := ConnectToSBC()
    client.Send(&buf, 1)
    client.Recv(buf, 5)
    client.Disconnect()

    volume := Round(NumGet(&buf, 0, "Float"))
    is_muted := NumGet(&buf, 4, "UChar")

    ShowDialog("Switched to headphones", volume, is_muted)
return

Shift & F2::
    VarSetCapacity(buf, 1, 0x02)
    client := ConnectToSBC()
    client.Send(&buf, 1)
    client.Recv(buf, 5)
    client.Disconnect()

    volume := Round(NumGet(&buf, 0, "Float"))
    is_muted := NumGet(&buf, 4, "UChar")

    ShowDialog("Switched to surround sound", volume, is_muted)
return

Shift & F3::
    VarSetCapacity(buf, 1, 0x03)
    client := ConnectToSBC()
    client.Send(&buf, 1)
    client.Recv(buf, 1)
    client.Disconnect()

    device := NumGet(&buf, 0, "UChar")
    SoundGet, volume
    volume := Round(volume)

    SoundGet, is_muted,, MUTE
    if (is_muted == "On")
        is_muted := True
    else
        is_muted := False

    if (device == 0x01) {
        ShowDialog("Headphones active", volume, is_muted)
    } else if (device == 0x02) {
        ShowDialog("Surround sound active", volume, is_muted)
    }
return

ConnectToSBC() {
    client := new SocketTCP()
    client.Connect(["127.0.0.1", 9051])
    return client
}

ShowDialog(msg, volume, is_muted) {
    GuiControl, Text, gui_notif_line1, %msg%

    if (is_muted) {
        GuiControl, Text, gui_notif_line2, % "Volume: muted (" . volume . ")"
    } else {
        GuiControl, Text, gui_notif_line2, % "Volume: " . volume
    }

    Gui, Show, NA x%dialog_x_pos% y%dialog_y_pos%
    SetTimer, HideNotificationPopup, -2300
}

HideNotificationPopup:
    Gui, Hide
return

BuildNotificationPopup() {
    Gui, +AlwaysOnTop +ToolWindow -Caption -Border
    Gui, Margin, 10, 10
    Gui, Color, 121212

    Gui, Font, s12
    Gui, Add, Text, vgui_notif_line1 cf7f7f7 w200,
    Gui, Add, Text, vgui_notif_line2 ce3e3e3 w150,

    Gui +LastFound
    WinSet, Transparent, 245

    Gui, Hide
}
