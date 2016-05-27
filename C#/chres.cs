////////////////////////////////////////////////////////////////////////
// Title:        chres.exe - .net version
// Purpose:      Utility to change display modes
//               Rewritten in C# for kicks.
// Author:       mtreit
// Created:      11/22/2000
// Updated:      03/03/2005 by a-surfer: added option to 
//               specify scale due to a specific video card 
//               using this member to specify different 
//               "orientation" for the same width/height/color/freq.
////////////////////////////////////////////////////////////////////////
using System;
using System.Runtime.InteropServices;


public class chres
{

    private const int WAITPERIOD = 3000;

    public static void Main(string[] args)
    {
        uint iHeight = 0;
        uint iWidth = 0;
        uint iColorDepth = 16;
        uint iFrequency = 60;
        uint iScale = 0;
        int  iModeIndex = 0;
        bool bPrintModes = false;
        bool bGetCurrentMode = false;
        bool bTestMode = false;
        bool bUseModeIndex = false;

        if (args.Length == 0)
        {
            PrintHelp();
            return;	
        }

        //Parse command line arguments
        foreach (string arg in args)
        {
            if (arg == "/?" || arg == "?")
            {
                PrintHelp();
                return;
            }

            // other than a "?", all arguments should have more than one character
            // If you try to do Substring(0,2) on an arg with only one char,
            // like "/" it crashes.
            if ( arg.Length == 1 )
            {
                Console.WriteLine("\nInvalid argument ({0}).\n", arg);            
                PrintHelp();
                return;	
            }

            switch (arg.Substring(0,2))
            {
                case "/H":
                case "/h":
                {
                    if (arg.Substring(2,1) != ":") {break;}
                    iHeight = Convert.ToUInt32(arg.Substring(3));
                    break;
                }

                case "/W":
                case "/w":
                {
                    if (arg.Substring(2,1) != ":") {break;}
                    iWidth = Convert.ToUInt32(arg.Substring(3));
                    break;
                }

                case "/C":
                case "/c":
                {
                    if (arg.Substring(2,1) != ":") {break;}
                    iColorDepth = Convert.ToUInt32(arg.Substring(3));
                    break;
                }

                case "/F":
                case "/f":
                {
                    if (arg.Substring(2,1) != ":") {break;}
                    iFrequency = Convert.ToUInt32(arg.Substring(3));
                    break;
                }

                case "/S":
                case "/s":
                {
                    if (arg.Substring(2,1) != ":") {break;}
                    iScale = Convert.ToUInt32(arg.Substring(3));
                    break;
                }

                case "/I":
                case "/i":
                {
                    if (arg.Substring(2,1) != ":") {break;}

                    iModeIndex = Convert.ToInt32(arg.Substring(3));
                    bUseModeIndex = true;
                    break;
                }
             
                case "/G":
                case "/g":
                {
                    bGetCurrentMode = true;
                    break;
                }

                case "/P":
                case "/p":
                {
                    bPrintModes = true;
                    break;
                }

                case "/T":
                case "/t":
                {
                    bTestMode = true;
                    break;
                }

                default:  
                {
                    Console.WriteLine("\nInvalid argument ({0}).\n", arg);            
                    PrintHelp();
                    return;	
                    break;
                }
            }
         
        }

        //Perform the action specified
        if (bPrintModes == false)
        {
            if (bGetCurrentMode == true)
            {
                PrintCurrentMode();
            }
            else
            {
                if (bUseModeIndex == false)
                {
                    ChangeMode(iWidth, iHeight, iColorDepth, iFrequency, iScale, bTestMode);
                }
                else
                {
                    ChangeModeByIndex(iModeIndex, bTestMode);
                }
            }
        }
        else
        {
            PrintModes();
        }


    }

    private static void PrintCurrentMode()
    {
        //Allocate a DEVMODE structure
        DEVMODE dm = new DEVMODE();

        //Print out the current mode
        EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, dm);
        Console.WriteLine("\nCurrent Mode:\n");
        Console.WriteLine("Width:       {0}",dm.dmPelsWidth);
        Console.WriteLine("Height:      {0}",dm.dmPelsHeight);
        Console.WriteLine("Color Depth: {0}",dm.dmBitsPerPel);
        Console.WriteLine("Frequency:   {0}",dm.dmDisplayFrequency);
        Console.WriteLine("Scale:       {0}",dm.dmScale);
    }

    private static void PrintModes()
    {

        //Allocate a DEVMODE structure
        DEVMODE dm = new DEVMODE();
        int i = 0;

        Console.WriteLine("===================================================================\n");
        Console.WriteLine("Displaying all possible modes for the current device.\n");
        Console.WriteLine("===================================================================\n");

        //Print out all possible modes for the current device
        EnumDisplaySettings(null,i,dm);
        do
        {
            Console.WriteLine("\nMode Index:  {0}",i);
            Console.WriteLine("Width:       {0}",dm.dmPelsWidth);
            Console.WriteLine("Height:      {0}",dm.dmPelsHeight);
            Console.WriteLine("Color Depth: {0}",dm.dmBitsPerPel);
            Console.WriteLine("Frequency:   {0}",dm.dmDisplayFrequency);
            Console.WriteLine("Scale:       {0}",dm.dmScale);
            i++;
        }
        while (EnumDisplaySettings(null,i,dm));

    }

    private static void ChangeMode(uint w, uint h, uint c, uint f, uint s, bool bTestMode)
    {
        bool bModeFound = false;
        int i = 0;
        int iResult;
    
        //Allocate a DEVMODE structure
        DEVMODE dm = new DEVMODE();
        dm.dmSize = (ushort)Marshal.SizeOf(dm);
        dm.dmDriverExtra = 1024;

        //Enumerate all supported modes for the specified device    
        EnumDisplaySettings(null,i,dm);

        do
        {
            i++;
            //Test for a mode matching the specified parameters
            if (dm.dmPelsWidth == w &&
                dm.dmPelsHeight == h &&
                dm.dmBitsPerPel == c &&
                dm.dmDisplayFrequency == f &&
                dm.dmScale == s )
            {
                bModeFound = true;
                Console.WriteLine("Changing to the following mode:\n");
                Console.WriteLine("W: {0}", dm.dmPelsWidth);
                Console.WriteLine("H: {0}", dm.dmPelsHeight);
                Console.WriteLine("C: {0}", dm.dmBitsPerPel);
                Console.WriteLine("F: {0}", dm.dmDisplayFrequency);
                Console.WriteLine("S: {0}", dm.dmScale);

                if (bTestMode == false)
                {
                    iResult = ChangeDisplaySettingsEx(null,dm,0,CDS_UPDATEREGISTRY,0);
                }
                else
                {
                    iResult = ChangeDisplaySettingsEx(null,dm,0,0,0);
                }

                switch(iResult)
                {
                    case DISP_CHANGE_SUCCESSFUL:
                        Console.WriteLine("\nDISP_CHANGE_SUCCESSFUL\n");
                        break;
            
                    case DISP_CHANGE_BADFLAGS:
                        Console.WriteLine("\nDISP_CHANGE_BADFLAGS\n");
                        break;

                    case DISP_CHANGE_BADMODE:
                        Console.WriteLine("\nDISP_CHANGE_BADMODE\n");
                        break;

                    case DISP_CHANGE_BADPARAM:
                        Console.WriteLine("\nDISP_CHANGE_BADPARAM\n");
                        break;

                    case DISP_CHANGE_NOTUPDATED:
                        Console.WriteLine("\nDISP_CHANGE_NOTUPDATED\n");
                        break;

                    case DISP_CHANGE_RESTART:
                        Console.WriteLine("\nDISP_CHANGE_RESTART\n");
                        break;
                }    
            }
        }
        while (EnumDisplaySettings(null,i,dm));

        if (bModeFound == false)
        {
            Console.WriteLine("\nNo mode found matching the requested setting.\n");
        }    

        //If testmode is true, sleep for a few seconds and then return to the original mode
        if (bTestMode == true && bModeFound == true)
        {
            Sleep(WAITPERIOD);
            ChangeDisplaySettingsEx(null,null,0,0,0);
        }

    }

    private static void ChangeModeByIndex(int iIndex, bool bTestMode)
    {

        bool bModeFound = false;
        int i = 0;
        int iResult;
    
        //Allocate a DEVMODE structure
        DEVMODE dm = new DEVMODE();
        dm.dmSize = (ushort)Marshal.SizeOf(dm);
        dm.dmDriverExtra = 1024;

        //Enumerate all supported modes for the specified device    
        EnumDisplaySettings(null,i,dm);

        do
        {

            //Test for a mode matching the specified parameters
            if (i == iIndex )
            {
                bModeFound = true;
                Console.WriteLine("Changing to the following mode:\n");
                Console.WriteLine("W: {0}", dm.dmPelsWidth);
                Console.WriteLine("H: {0}", dm.dmPelsHeight);
                Console.WriteLine("C: {0}", dm.dmBitsPerPel);
                Console.WriteLine("F: {0}", dm.dmDisplayFrequency);
                Console.WriteLine("S: {0}", dm.dmScale);

                if (bTestMode == false)
                {
                    iResult = ChangeDisplaySettingsEx(null,dm,0,CDS_UPDATEREGISTRY,0);
                }
                else
                {
                    iResult = ChangeDisplaySettingsEx(null,dm,0,0,0);
                }

                switch(iResult)
                {
                    case DISP_CHANGE_SUCCESSFUL:
                        Console.WriteLine("\nDISP_CHANGE_SUCCESSFUL\n");
                        break;
            
                    case DISP_CHANGE_BADFLAGS:
                        Console.WriteLine("\nDISP_CHANGE_BADFLAGS\n");
                        break;

                    case DISP_CHANGE_BADMODE:
                        Console.WriteLine("\nDISP_CHANGE_BADMODE\n");
                        break;

                    case DISP_CHANGE_BADPARAM:
                        Console.WriteLine("\nDISP_CHANGE_BADPARAM\n");
                        break;

                    case DISP_CHANGE_NOTUPDATED:
                        Console.WriteLine("\nDISP_CHANGE_NOTUPDATED\n");
                        break;

                    case DISP_CHANGE_RESTART:
                        Console.WriteLine("\nDISP_CHANGE_RESTART\n");
                        break;
                }    
            }

            i++;
        }
        while (EnumDisplaySettings(null,i,dm));

        if (bModeFound == false)
        {
            Console.WriteLine("\nNo mode found matching the requested setting.\n");
        }    

        //If testmode is true, sleep for a few seconds and then return to the original mode
        if (bTestMode == true && bModeFound == true)
        {
            Sleep(WAITPERIOD);
            ChangeDisplaySettingsEx(null,null,0,0,0);
        }

    }

    private static void Sleep(int SleepTime)
    {
        System.Threading.Thread.Sleep(SleepTime);
    }

    private static void PrintHelp()
    {
        Console.WriteLine("\n===== chres .NET edition =====");
        Console.WriteLine("Changes the current display settings.");
        Console.WriteLine("\nUsage:");
        Console.WriteLine("chres.exe    [/W:<Width> /H:<Height>] [/C:<ColorDepth>] [/F:<Frequency>]");
        Console.WriteLine("        [/I[ndex]:<ModeIndex>]");
        Console.WriteLine("        [/P[rintModes]]");
        Console.WriteLine("        [/G[etCurrentMode]]");
        Console.WriteLine("        [/T[est]]\n");
        Console.WriteLine("        /W:<Width>            Sets the screen width");
        Console.WriteLine("        /H:<Height>           Sets the screen height");
        Console.WriteLine("        /C:<ColorDepth>       Sets the color depth (in bits)");
        Console.WriteLine("        /F:<Frequency>        Sets the refresh frequency");
        Console.WriteLine("        /S:<Scale>            Sets the \"scale\" (defaults to 0)\n");
        Console.WriteLine("        /I[ndex]:<ModeIndex>  Sets the mode using a specific index\n");
        Console.WriteLine("        /P[rintModes]         Prints all available display modes\n");
        Console.WriteLine("        /G[etCurrentMode]     Prints the current display mode\n");
        Console.WriteLine("        /T[est]               Returns to the previous mode after");
        Console.WriteLine("                              a few seconds\n");
        Console.WriteLine("        Examples:");
        Console.WriteLine("        chres.exe /PrintModes");
        Console.WriteLine("        chres.exe /Index:54");
        Console.WriteLine("        chres.exe /W:1024 /H:768 /Test");
        Console.WriteLine("        chres.exe /W:1152 /H:864 /F:75 /C:16\n");
        Console.WriteLine("        If not specified, frequency defaults to 60Hz and");
        Console.WriteLine("        color depth to 16 bit.");
    }


    //
    // API related declarations
    //

    [DllImport("user32.dll")]
    private static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, [Out] DEVMODE lpDevMode);

    [DllImport("user32.dll")]
    private static extern int ChangeDisplaySettingsEx(string lpszDeviceName, DEVMODE lpDevMode, long hwnd, uint dwFlags, long lParam);

    [StructLayout(LayoutKind.Sequential)]
    private class DEVMODE
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)] 
        public string   dmDeviceName;
        public ushort   dmSpecVersion;
        public ushort   dmDriverVersion;
        public ushort   dmSize;
        public ushort   dmDriverExtra;
        public uint     dmFields;
        public short    dmOrientation;
        public short    dmPaperSize;
        public short    dmPaperLength;
        public short    dmPaperWidth;
        public short    dmScale;
        public short    dmCopies;
        public short    dmDefaultSource;
        public short    dmPrintQuality;
        public short    dmColor;
        public short    dmDuplex;
        public short    dmYResolution;
        public short    dmTTOption;
        public short    dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)] 
        public string   dmFormName;
        public ushort   dmUnusedPadding;
        public ushort   dmBitsPerPel;
        public uint     dmPelsWidth;
        public uint     dmPelsHeight;
        public uint     dmDisplayFlags;
        public uint     dmDisplayFrequency;
    }     

    private const int ENUM_CURRENT_SETTINGS     = -1;
    private const int ENUM_REGISTRY_SETTINGS    = -2;
    private const int CDS_UPDATEREGISTRY        = 0x00000001;
    private const int CDS_TEST                  = 0x00000002;
    private const int CDS_FULLSCREEN            = 0x00000004;
    private const int CDS_GLOBAL                = 0x00000008;
    private const int CDS_SET_PRIMARY           = 0x00000010;
    private const int CDS_VIDEOPARAMETERS       = 0x00000020;
    private const int CDS_RESET                 = 0x40000000;
    private const int CDS_NORESET               = 0x10000000;
    private const int DISP_CHANGE_SUCCESSFUL    = 0;
    private const int DISP_CHANGE_RESTART       = 1;
    private const int DISP_CHANGE_FAILED        = -1;
    private const int DISP_CHANGE_BADMODE       = -2;
    private const int DISP_CHANGE_NOTUPDATED    = -3;
    private const int DISP_CHANGE_BADFLAGS      = -4;
    private const int DISP_CHANGE_BADPARAM      = -5;

}