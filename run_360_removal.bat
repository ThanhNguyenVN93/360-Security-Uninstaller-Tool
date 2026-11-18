@echo off
chcp 65001 >nul
title 360 Total Security Removal Tool

echo.
echo  ============================================
echo   360 Total Security - Removal Tool
echo  ============================================
echo.
echo  [1] Tieng Viet
echo  [2] English
echo.
set /p CHOICE= Select language / Chon ngon ngu (1/2):

if "%CHOICE%"=="1" (
    set LANG=VI
    echo.
    echo  Da chon: Tieng Viet
) else (
    set LANG=EN
    echo.
    echo  Selected: English
)

echo.
powershell -ExecutionPolicy Bypass -File "%~dp01_boot_safemode.ps1" -Lang %LANG%
pause
