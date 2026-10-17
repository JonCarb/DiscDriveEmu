using System;
using System.Diagnostics;
using System.IO;
using DiscUtils.Iso9660;
using DiscUtils.Udf;

namespace EDD;

public class CheckForGameType
{
    public bool ReadGCWII(string gamePath)
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
    public bool ReadXBOX(string gamePath)
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
            Debug.WriteLine(ex.Message);
            return false;
        }
    }
    public bool ReadXBOX360(string gamePath)
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
            Debug.WriteLine(ex.Message);
            return false;
        }
    }
    public bool ReadPS3(string gamePath)
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
            Debug.WriteLine(ex.Message);
            return false;
        }
    }

    public bool ReadPS2(string gamePath)
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
            Debug.WriteLine(ex.Message);
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
            //Console.WriteLine(ex.Message);
        }
        return false;
    }

    public bool ReadGCWIINonIso(string fileExtension)
    {
        return fileExtension is ".RVZ" or ".WBFS" or ".WIA" or ".GCZ" or ".CISO" or ".WAD";
    }
    public bool ReadWiiU(string fileExtension)
    {
        return fileExtension is ".WUX" or ".WUA" or ".WUD" or ".RPX";
    }
    public bool ReadXbox360NonIso(string fileExtension)
    {
        return (fileExtension == ".XEX");
    }
}