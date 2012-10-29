using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Drawing;
using Microsoft.Win32;

/**
 * 
 * Parts found at: http://stackoverflow.com
 * 
 *  
 * Adapted by evandrojr@gmail 
 * Date: 2012-10-18
 */
namespace ChangeWallPaper
{
    static class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction,
            int uParam, string lpvParam, int fuWinIni);


        public enum Style : int
        {
            Tiled,
            Centered,
            Stretched
        }
 
        //private static readonly int MAX_PATH = 260;
        //private static readonly int SPI_GETDESKWALLPAPER = 0x73;
        private static readonly int SPI_SETDESKWALLPAPER = 0x14;
        private static readonly int SPIF_UPDATEINIFILE = 0x01;
        private static readonly int SPIF_SENDWININICHANGE = 0x02;
 
        //static string GetDesktopWallpaper()
        //{
        //    string wallpaper = new string('\0', MAX_PATH);
        //    SystemParametersInfo(SPI_GETDESKWALLPAPER, (int)wallpaper.Length, wallpaper, 0);
        //    return wallpaper.Substring(0, wallpaper.IndexOf('\0'));
        //}
 
        static void SetDesktopWallpaperWindows7(string filename)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filename, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }


        static string getOSInfo() {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows) {
                //This is a pre-NT version of Windows
                switch (vs.Minor) {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            } else if (os.Platform == PlatformID.Win32NT) {
                switch (vs.Major) {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else
                            operatingSystem = "7";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "") {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "") {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                //operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }

        static void Main(string[] args)
        {
            string os = getOSInfo();
            System.Console.WriteLine("####################################################################");
            System.Console.WriteLine();
            System.Console.WriteLine(" Trocador de papel de parede, viva a liberdade de expressão visual!");
            System.Console.WriteLine();
            System.Console.WriteLine("####################################################################");
            System.Console.WriteLine();
            System.Console.WriteLine("Sistema operacional detectado: " + os);
            System.Console.WriteLine();
            System.Console.WriteLine("Se o script de logon da rede já tiver finalizado pode fechar essa");
            System.Console.WriteLine("janela, caso contrário ela se fechará automaticamente em 6 minutos.");
            System.Console.WriteLine();
            System.Console.WriteLine("Repositório: https://github.com/evandrojr/ChangeWallPaper");

            for (int i = 0; i < 3; ++i) {
                try {
                    if (args.Length > 0)
                        SetDesktopWindows(args[0], Style.Centered);
                    else {
                        string[] images = Directory.GetFiles(Environment.CurrentDirectory, "*.jpg");
                        if (images.Length == 0) {
                            System.Console.WriteLine();
                            System.Console.WriteLine("É necessário passar uma imagem via linha de comando ou colocá-la no mesmo diretório do executável: " + Environment.CurrentDirectory);
                            Thread.Sleep(2 * 60 * 1000);
                            Environment.Exit(1);
                        }
                        SetDesktopWindows(images[0], Style.Centered);
                    }

                } catch (IndexOutOfRangeException ex) {
                    System.Console.WriteLine("Error." + ex.Message + Environment.NewLine + ex.StackTrace);
                }
                Thread.Sleep(2 * 60 * 1000);
            }
        }


        public static void SetDesktopWindows(string wpaper, Style style) {
            System.Drawing.Image img = System.Drawing.Image.FromFile(Path.GetFullPath(wpaper));
            string tempPath = Path.Combine(Path.GetTempPath(), "wallpaper.bmp");
            img.Save(tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
            if (style == Style.Stretched) {
                key.SetValue(@"WallpaperStyle", 2.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Centered) {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 0.ToString());
            }
            if (style == Style.Tiled) {
                key.SetValue(@"WallpaperStyle", 1.ToString());
                key.SetValue(@"TileWallpaper", 1.ToString());
            }

            SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                tempPath,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }

    }
}