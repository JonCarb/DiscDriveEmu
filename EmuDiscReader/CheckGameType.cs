using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscUtils;
using DiscUtils.Iso9660;
using DiscUtils.Udf;

namespace EmuDiscReader
{
    internal class CheckGameType
    {
        public bool ReadGCWII(string gamePath)
        {
            string bytes = "NULL";
            using (FileStream fs = new FileStream(gamePath, FileMode.Open, FileAccess.Read))
            {
                var buf = new byte[1];
                fs.Read(buf, 0, 1);
                bytes = System.Text.Encoding.ASCII.GetString(buf);
            }

            //Gamecube ISO is always G, Wii is either R, S, or D
            if (bytes[0] == 'G' || bytes[0] == 'R' || bytes[0] == 'S' || bytes[0] == 'D')
            {
                return true;
            }
            return false;
        }
        public bool ReadXBOX(string gamePath)
        {
            string bytes = "NULL";
            int offset = 65536;
            int bufLength = 20;
            try
            {
                using (FileStream fs = new FileStream(gamePath, FileMode.Open, FileAccess.Read))
                {
                    fs.Seek(offset, SeekOrigin.Begin);
                    var buf = new byte[bufLength];
                    fs.Read(buf, 0, bufLength);
                    bytes = System.Text.Encoding.ASCII.GetString(buf);
                    return (bytes == "MICROSOFT*XBOX*MEDIA");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
        public bool ReadPS3(string gamePath)
        {
            string bytes = "NULL";
            int offset = 0x800;
            int bufLength = 12;
            try
            {
                using (FileStream fs = new FileStream(gamePath, FileMode.Open, FileAccess.Read))
                {
                    fs.Seek(offset, SeekOrigin.Begin);
                    var buf = new byte[bufLength];
                    fs.Read(buf, 0, bufLength);
                    bytes = System.Text.Encoding.ASCII.GetString(buf);
                    return (bytes == "PlayStation3");
                }
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
                if (checkThroughISO996(fs, "SYSTEM.CNF")) { return true; }
                fs.Position = 0;
                if (checkThroughUDF(fs, "SYSTEM.CNF")) { return true; }
            }
            return false;
        }

        private bool checkThroughISO996(FileStream fs, string specificFile)
        {
            try
            {
                CDReader cd = new CDReader(fs, true);
                return cd.Exists(specificFile);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }
        private bool checkThroughUDF(FileStream fs, string specificFile)
        {
            //Open UDF and check for specific ps2 file or ps3 in the future
            try
            {
                UdfReader udf = new UdfReader(fs);
                return udf.Exists(specificFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }

        public bool ReadGCWIINonIso(string fileExtension)
        {
            if (fileExtension == ".RVZ" ||
                fileExtension == ".WBFS" ||
                fileExtension == ".WIA" ||
                fileExtension == ".GCZ" ||
                fileExtension == ".CISO" ||
                fileExtension == ".WAD")
            {
                return true;
            }
            return false;
        }
        public bool ReadWiiU(string fileExtension)
        {
            if (fileExtension == ".WUX" ||
                fileExtension == ".WUA" ||
                fileExtension == ".WUD" ||
                fileExtension == ".RPX")
            {
                return true;
            }
            return false;
        }
    }
}
