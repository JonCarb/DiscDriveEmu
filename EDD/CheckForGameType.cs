using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using DiscUtils.Iso9660;
using DiscUtils.Udf;

namespace EDD;

public class CheckForGameType
{
    public async Task<App.Emulators> ChooseEmulator(string? gamePath, App.DiscType disc)
    {
        if (gamePath == null) return App.Emulators.None;

        switch (disc)
        {
            //Checks meant for REAL discs
            case App.DiscType.Xbox360:
                return App.Emulators.Xenia;
            case App.DiscType.PS3:
                return App.Emulators.RPCS3;
            case App.DiscType.PS2:
                return App.Emulators.PCSX2;
        }

        //These are for discs with game files burned into them
        App.Emulators emuName =  App.Emulators.None;
        await Task.Run(() =>
        {
            string fileEx = Path.GetExtension(gamePath).ToUpper();
            if (fileEx == ".ISO")
            {
                if (ReadGCWII(gamePath)) { emuName = App.Emulators.Dolphin; }       //reads 1 byte
                else if (ReadPS3(gamePath)) { emuName = App.Emulators.RPCS3; }      //seeks and reads 12 bytes
                else if (ReadXBOX(gamePath)) { emuName = App.Emulators.Xemu; }      //seeks and reads 20 bytes
                else if (ReadXBOX360(gamePath)) { emuName = App.Emulators.Xenia; }  //seeks and reads 20 bytes
                else if (ReadPS2(gamePath)) { emuName = App.Emulators.PCSX2; }      //converts whole iso (twice maybe) and checks (expensive so last case)
                //Add other emulators
            }
            else
            {
                //Add method for Non-iso files 
                if (ReadWiiU(fileEx)) { emuName = App.Emulators.Cemu; }                 //Check for Cemu supported extensions
                else if (ReadGCWIINonIso(fileEx)) { emuName = App.Emulators.Dolphin; }  //Checks for dolphin supported extensions that's not ISO
                else if(ReadXbox360NonIso(fileEx)) { emuName = App.Emulators.Xenia; }   //Checks if file is xex for Xenia
                else if (ReadPS1(fileEx)) { emuName = App.Emulators.Duckstation; }      //Checks for cue or chd
            }
        });
        
        return emuName;
    }

    private bool ReadGCWII(string gamePath)
    {
        string bytes = "NULL";
        using (FileStream fs = new(gamePath, FileMode.Open, FileAccess.Read))
        {
            var buf = new byte[1];
            fs.ReadExactly(buf, 0, 1);
            bytes = System.Text.Encoding.ASCII.GetString(buf);
        }

        //Gamecube ISO is always G, Wii is either R, S, or D
        return bytes[0] == 'G' || bytes[0] == 'R' || bytes[0] == 'S' || bytes[0] == 'D';
    }

    private bool ReadXBOX(string gamePath)
    {
        const int offset = 65536;
        const int bufLength = 20;
        try
        {
            using FileStream fs = new(gamePath, FileMode.Open, FileAccess.Read);
            fs.Seek(offset, SeekOrigin.Begin);
            var buf = new byte[bufLength];
            fs.ReadExactly(buf, 0, bufLength);
            return (System.Text.Encoding.ASCII.GetString(buf) == "MICROSOFT*XBOX*MEDIA");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    private bool ReadXBOX360(string gamePath)
    {
        const int offset = 0x0FDA0000;
        const int bufLength = 20;
        try
        {
            using FileStream fs = new(gamePath, FileMode.Open, FileAccess.Read);
            fs.Seek(offset, SeekOrigin.Begin);
            var buf = new byte[bufLength];
            fs.ReadExactly(buf, 0, bufLength);
            return (System.Text.Encoding.ASCII.GetString(buf) == "MICROSOFT*XBOX*MEDIA");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    private bool ReadPS3(string gamePath)
    {
        const int offset = 0x800;
        const int bufLength = 12;
        try
        {
            using FileStream fs = new(gamePath, FileMode.Open, FileAccess.Read);
            fs.Seek(offset, SeekOrigin.Begin);
            var buf = new byte[bufLength];
            fs.ReadExactly(buf, 0, bufLength);
            return (System.Text.Encoding.ASCII.GetString(buf) == "PlayStation3");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    private bool ReadPS2(string gamePath)
    {
        using (FileStream fs = new FileStream(gamePath, FileMode.Open, FileAccess.Read))
        {
            if (CheckThroughISO996(fs, "SYSTEM.CNF")) { return true; }
            fs.Position = 0;
            if (CheckThroughUDF(fs, "SYSTEM.CNF")) { return true; }
        }
        return false;
    }
    private bool CheckThroughISO996(FileStream fs, string specificFile)
    {
        try
        {
            CDReader cd = new(fs, true);
            return cd.Exists(specificFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }
    private bool CheckThroughUDF(FileStream fs, string specificFile)
    {
        //Open UDF and check for specific ps2 file or ps3 in the future
        try
        {
            UdfReader udf = new(fs);
            return udf.Exists(specificFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    private bool ReadPS1(string fileExtension)
    {
        return (fileExtension is ".CUE" or ".CHD");
    }

    private bool ReadGCWIINonIso(string fileExtension)
    {
        return fileExtension is ".RVZ" or ".WBFS" or ".WIA" or ".GCZ" or ".CISO" or ".WAD";
    }

    private bool ReadWiiU(string fileExtension)
    {
        return fileExtension is ".WUX" or ".WUA" or ".WUD" or ".RPX";
    }

    private bool ReadXbox360NonIso(string fileExtension)
    {
        return (fileExtension == ".XEX");
    }
}