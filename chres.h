#include <windows.h>
#include <stdio.h>
#include <tchar.h>
#include <stdlib.h>

VOID printDevModes();
VOID printCurrMode();
VOID changeMode(UINT w, UINT h, UINT c, UINT f, UINT s, BOOL bTestMode);
VOID changeModeByIndex(UINT iIndex, BOOL bTestMode);
VOID PrintHelp();