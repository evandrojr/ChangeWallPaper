using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
 
/**
 * A command line tool to set the desktop background wallpaper.
 * Takes a single argument that is the filename to the wallpaper to set.
 * Author: doug@neverfear.org
 * Date: 2010-05-29
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
 
        private static readonly int MAX_PATH = 260;
        private static readonly int SPI_GETDESKWALLPAPER = 0x73;
        private static readonly int SPI_SETDESKWALLPAPER = 0x14;
        private static readonly int SPIF_UPDATEINIFILE = 0x01;
        private static readonly int SPIF_SENDWININICHANGE = 0x02;
 
        static string GetDesktopWallpaper()
        {
            string wallpaper = new string('\0', MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, (int)wallpaper.Length, wallpaper, 0);
            return wallpaper.Substring(0, wallpaper.IndexOf('\0'));
        }
 
        static void SetDesktopWallpaper(string filename)
        {
            for (int i = 0; i < 3; ++i) {
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filename, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
                Thread.Sleep(3 * 60 * 1000);
                //    Thread.Sleep(10000);
            }
        }
 
        static void Main(string[] args)
        {
            System.Console.WriteLine("Current desktop wallpaper is at path: " + GetDesktopWallpaper());
            try
            {
                if (args.Length > 0)
                    SetDesktopWallpaper(args[0]);
                else {
                    string[] images = Directory.GetFiles(Environment.CurrentDirectory, "*.jpg");
                            SetDesktopWallpaper(images[0]);
                }
                
            }
            catch (IndexOutOfRangeException ex)
            {
                System.Console.WriteLine("Error." + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}