namespace EDD;

public class SettingsValues
{
    //---------------Emulation Paths---------------//
    public string? Dolphin { get; set; }
    public string? PCSX2 { get; set; }
    public string? Xemu { get; set; }
    public string? Cemu { get; set; }
    public string? RPCS3 { get; set; }
    public string? Xenia { get; set; }
    
    //-------------General Button Settings---------//
    public bool WillCache { get; set; }
    public bool WillAutoStart { get; set; }
    
    //-------------MPF---------//
    public string? MPF { get; set; }
}