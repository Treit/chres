
//////////////////////////////////////////////////////////////
// Title:       chres.exe
// Purpose:     Utility to change display modes
// Author:      mtreit@microsoft.com
// Created:     05/17/2000
// Updated:     03/03/2005 - add Scale option per feedback
//              from Scott Urfer (a-surfer@microsoft.com)
//////////////////////////////////////////////////////////////

#include "chres.h"
#define WAITPERIOD 3000

int main(int argc, char **argv)
{
    UINT iWidth = 0;
    UINT iHeight = 0;
    UINT iColors = 16;
    UINT iFrequency = 60;
    UINT iScale = 0;
    UINT iModeIndex = 0;
    
    INT iNumArgs, i;

    BOOL bPrintModes = FALSE;
    BOOL bUseModeIndex = FALSE;
    BOOL bTestMode = FALSE;
    BOOL bGetCurrMode = FALSE;

    LPTSTR *pszArgv;
    LPTSTR lpMode = NULL;

    
    //Get command line arguments
    pszArgv = argv;
    iNumArgs = argc;

    if (iNumArgs < 2)
    {
        PrintHelp();
        return(FALSE);	
    }

    for(i=1;i<=iNumArgs-1;i++)
    {
        if(pszArgv[i][0] != '/')
        {
            PrintHelp();
            return(FALSE);
        }

            
        switch(pszArgv[i][1])
        {

            case '?':
            {
                PrintHelp();
                return(FALSE);
                break;
            }


            case 'I':
            case 'i':
            {
                LPTSTR szIndex = &(pszArgv[i][2]);
                while(*szIndex != ':')
                {
                    if(*szIndex == '\0')
                    {
                        PrintHelp();
                        return(FALSE);
                    }
                    szIndex++;
                }
                szIndex++;
                iModeIndex = atoi(szIndex);
                bUseModeIndex = TRUE;
                break;
            }
            
            
            case 'W':
            case 'w':
            {
                LPTSTR szWidth = &(pszArgv[i][2]);
                while(*szWidth != ':')
                {
                    if(*szWidth == '\0')
                    {
                        PrintHelp();
                        return(FALSE);
                    }
                    szWidth++;
                }
                szWidth++;
                iWidth = atoi(szWidth);
                break;
            }

            case 'H':
            case 'h':
            {
                LPTSTR szHeight = &(pszArgv[i][2]);
                while(*szHeight != ':')
                {
                    if(*szHeight == '\0')
                    {
                        PrintHelp();
                        return(FALSE);
                    }
                    szHeight++;
                }
                szHeight++;
                iHeight = atoi(szHeight);
                break;
            }

            case 'C':
            case 'c':
            {
                LPTSTR szColor = &(pszArgv[i][2]);
                while(*szColor != ':')
                {
                    if(*szColor == '\0')
                    {
                        PrintHelp();
                        return(FALSE);
                    }
                    szColor++;
                }
                szColor++;
                iColors = atoi(szColor);
                break;
            }

            case 'F':
            case 'f':
            {
                LPTSTR szFreq = &(pszArgv[i][2]);
                while(*szFreq != ':')
                {
                    if(*szFreq == '\0')
                    {
                        PrintHelp();
                        return(FALSE);
                    }
                    szFreq++;
                }
                szFreq++;
                iFrequency = atoi(szFreq);
                break;
            }

            case 'S':
            case 's':
            {
                LPTSTR szScale = &(pszArgv[i][2]);
                while(*szScale != ':')
                {
                    if(*szScale == '\0')
                    {
                        PrintHelp();
                        return(FALSE);
                    }
                    szScale++;
                }
                szScale++;
                iScale = atoi(szScale);
                break;
            }

            case 'P':
            case 'p':
            {
                bPrintModes = TRUE;
                break;
            }

            
            case 'G':
            case 'g':
            {
                bGetCurrMode = TRUE;
                break;
            }

            case 'T':
            case 't':
            {
                bTestMode = TRUE;
                break;
            }

        }


    }

    
    //Perform the action specified
    if (bPrintModes == FALSE)
    {
        if (bGetCurrMode == TRUE)
        {
            printCurrMode();
        }
        else
        {
            if (bUseModeIndex == FALSE)
            {
                changeMode(iWidth, iHeight, iColors, iFrequency, iScale, bTestMode);
            }
            else
            {
                changeModeByIndex(iModeIndex, bTestMode);
            }
        }
    }
    else
    {
        printDevModes();
    }

    
    return 0;
}

VOID changeMode(UINT w, UINT h, UINT c, UINT f, UINT s, BOOL bTestMode)
{

    BOOL bModeFound = FALSE;
    UINT i = 0;
    LONG lresult;

    //Allocate a DEVMODE structure
    DEVMODE *lpDevMode = malloc(sizeof(DEVMODE) + 1024);
    lpDevMode->dmSize = sizeof(DEVMODE);
    
    //Add room for extra driver data.
    lpDevMode->dmDriverExtra = 1024;

    //Enumerate all supported modes for the specified device	
    EnumDisplaySettings(NULL,i,lpDevMode);
    do
    {
        i++;

        //Test for a mode matching the specified parameters
        if (lpDevMode->dmPelsWidth ==  w &&
            lpDevMode->dmPelsHeight == h &&
            lpDevMode->dmBitsPerPel == c &&
            lpDevMode->dmDisplayFrequency == f &&
            lpDevMode->dmScale == s )
        {
            bModeFound = TRUE;
            
            printf("\nChanging to the following mode:\n\n");
            printf("W:%i\n",lpDevMode->dmPelsWidth);
            printf("H:%i\n",lpDevMode->dmPelsHeight);
            printf("C:%i\n",lpDevMode->dmBitsPerPel);
            printf("F:%i\n",lpDevMode->dmDisplayFrequency);
            printf("S:%i\n",lpDevMode->dmScale);
    
            if (bTestMode == FALSE)
            {
                lresult = ChangeDisplaySettingsEx(NULL, lpDevMode, NULL, CDS_UPDATEREGISTRY, NULL);
            }
            else
            {
                lresult = ChangeDisplaySettingsEx(NULL, lpDevMode, NULL, 0, NULL);
            }
                            
            switch(lresult)
            {
                case DISP_CHANGE_SUCCESSFUL:
                printf("\nDISP_CHANGE_SUCCESSFUL\n");
                break;
            
                case DISP_CHANGE_BADFLAGS:
                printf("\nDISP_CHANGE_BADFLAGS\n");
                break;

                case DISP_CHANGE_BADMODE:
                printf("\nDISP_CHANGE_BADMODE\n");
                break;

                case DISP_CHANGE_BADPARAM:
                printf("\nDISP_CHANGE_BADPARAM\n");
                break;

                case DISP_CHANGE_NOTUPDATED:
                printf("\nDISP_CHANGE_NOTUPDATED\n");
                break;

                case DISP_CHANGE_RESTART:
                printf("\nDISP_CHANGE_RESTART\n");
                break;
            }	
        }		
    }
    while (EnumDisplaySettings(NULL,i,lpDevMode));

    if (bModeFound == FALSE)
    {
        printf("\nNo mode found matching the requested settings.\n");
    }

    //If testmode is true, sleep for a few seconds and then return to the original mode
    if (bTestMode == TRUE && bModeFound == TRUE)
    {
        Sleep(WAITPERIOD);
        ChangeDisplaySettingsEx(NULL,NULL,NULL,0,NULL);
    }

    //Cleanup
    free(lpDevMode);

    return;
}

VOID changeModeByIndex(UINT iIndex, BOOL bTestMode)
{

    BOOL bModeFound = FALSE;
    UINT i = 0;
    LONG lresult;

    //Allocate a DEVMODE structure
    DEVMODE *lpDevMode = malloc(sizeof(DEVMODE) + 1024);
    lpDevMode->dmSize = sizeof(DEVMODE);
    
    //Add room for extra driver data.
    lpDevMode->dmDriverExtra = 1024;

    //Enumerate all supported modes for the specified device	
    EnumDisplaySettings(NULL,i,lpDevMode);
    do
    {

        //Test for a mode matching the specified parameters
        if (i == iIndex)
        {
            bModeFound = TRUE;
            
            printf("\nChanging to the following mode:\n\n");
            printf("W:%i\n",lpDevMode->dmPelsWidth);
            printf("H:%i\n",lpDevMode->dmPelsHeight);
            printf("C:%i\n",lpDevMode->dmBitsPerPel);
            printf("F:%i\n",lpDevMode->dmDisplayFrequency);
            printf("S:%i\n",lpDevMode->dmScale);
    
    
            if (bTestMode == FALSE)
            {
                lresult = ChangeDisplaySettingsEx(NULL,lpDevMode,NULL,CDS_UPDATEREGISTRY,NULL);
            }
            else
            {
                lresult = ChangeDisplaySettingsEx(NULL,lpDevMode,NULL,0,NULL);
            }

                            
            switch(lresult)
            {
                case DISP_CHANGE_SUCCESSFUL:
                printf("\nDISP_CHANGE_SUCCESSFUL\n");
                break;
            
                case DISP_CHANGE_BADFLAGS:
                printf("\nDISP_CHANGE_BADFLAGS\n");
                break;

                case DISP_CHANGE_BADMODE:
                printf("\nDISP_CHANGE_BADMODE\n");
                break;

                case DISP_CHANGE_BADPARAM:
                printf("\nDISP_CHANGE_BADPARAM\n");
                break;

                case DISP_CHANGE_NOTUPDATED:
                printf("\nDISP_CHANGE_NOTUPDATED\n");
                break;

                case DISP_CHANGE_RESTART:
                printf("\nDISP_CHANGE_RESTART\n");
                break;
            }	
        }

        i++;
    }
    while (EnumDisplaySettings(NULL,i,lpDevMode));

    if (bModeFound == FALSE)
    {
        printf("\nNo mode found matching the requested settings.\n");
    }

    //If testmode is true, sleep for a few seconds and then return to the original mode
    if (bTestMode == TRUE && bModeFound == TRUE)
    {
        Sleep(WAITPERIOD);
        ChangeDisplaySettingsEx(NULL,NULL,NULL,0,NULL);
    }

    //Cleanup
    free(lpDevMode);

    return;
}


VOID printCurrMode()
{
    DEVMODE *lpDevMode = malloc(sizeof(DEVMODE));

    EnumDisplaySettings(NULL,ENUM_CURRENT_SETTINGS,lpDevMode);
    
    printf("\nCurrent mode:\n\n");
    
    printf("Width:         %i\n",lpDevMode->dmPelsWidth);
    printf("Height:        %i\n",lpDevMode->dmPelsHeight);
    printf("Color Depth:   %i\n",lpDevMode->dmBitsPerPel);
    printf("Frequency:     %i\n",lpDevMode->dmDisplayFrequency);
    printf("Scale:         %i\n",lpDevMode->dmScale);
        
    free(lpDevMode);
    return;
}

VOID printDevModes()
{

    DEVMODE *lpDevMode = malloc(sizeof(DEVMODE));
    int i = 0;
    
    printf("===================================================================\n");
    printf("Displaying all possible modes for the current device.\n");
    printf("===================================================================\n");

    EnumDisplaySettings(NULL,i,lpDevMode);
    do
    {
        printf("\nMode Index:   %i\n",i);
        printf("Width:        %i\n",lpDevMode->dmPelsWidth);
        printf("Height:       %i\n",lpDevMode->dmPelsHeight);
        printf("Color Depth:  %i\n",lpDevMode->dmBitsPerPel);
        printf("Frequency:    %i\n",lpDevMode->dmDisplayFrequency);
        printf("Scale:        %i\n",lpDevMode->dmScale);
        i++;
    }
    while (EnumDisplaySettings(NULL,i,lpDevMode));

    //Cleanup
    free(lpDevMode);

    return;
}

VOID PrintHelp()
{
    printf("\nChanges the current display settings.\n");
    printf("\nUsage:\n");
    printf("chres.exe [/W:<Width> /H:<Height>] [/C:<ColorDepth>] [/F:<Frequency>] [/S:<Scale>]\n");
    printf("          [/I[ndex]:<ModeIndex>]\n");
    printf("          [/P[rintModes]]\n");
    printf("          [/G[etCurrentMode]]\n");
    printf("          [/T[est]]\n\n");
    printf("          /W:<Width>\t\tSets the screen width\n");
    printf("          /H:<Height>\t\tSets the screen height\n");
    printf("          /C:<ColorDepth>\tSets the color depth (in bits)\n");
    printf("          /F:<Frequency>\tSets the refresh frequency\n\n");
    printf("          /S:<Scale>\tSets the scale (use to ensure proper orientation.)\n\n");
    printf("          /I[ndex]:<ModeIndex>\tSets the mode using a specific index\n\n");
    printf("          /P[rintModes]\t\tPrints all available display modes\n\n");
    printf("          /G[etCurrentMode]\tPrints the current display mode\n\n");
    printf("          /T[est]\t\tReturns to the previous mode after\n");
    printf("          \t\t\ta few seconds\n\n");
    printf("          Examples:\n");
    printf("          chres.exe /PrintModes\n");
    printf("          chres.exe /Index:54\n");
    printf("          chres.exe /W:1024 /H:768 /Test\n");
    printf("          chres.exe /W:1152 /H:864 /F:75 /C:16 /S:0\n\n");
    printf("          If not specified, frequency defaults to 60Hz,\n");
    printf("          color depth to 16 bit and scale to 0.\n");

    return;
}
