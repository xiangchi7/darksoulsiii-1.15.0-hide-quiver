@echo off
setlocal enabledelayedexpansion
rem HideQuiver installer - backs up the originals as *.pre-hidequiver, then drops the patched models in.
set "SRC=%~dp0files"
if not exist "%SRC%\" (
  echo [x] files\ folder not found - this looks like the git repository, not the release package.
  echo     Download HideQuiver-1.0.zip from the Releases page, extract it, and run install.cmd from there.
  exit /b 1
)
set "GAME=%~1"
if "%GAME%"=="" set "GAME=D:\steam\steamapps\common\DARK SOULS III\Game"
if not exist "%GAME%\parts" (
  echo [x] parts folder not found: "%GAME%"
  echo     usage: install.cmd "D:\path\to\DARK SOULS III\Game"
  exit /b 1
)
set /a n=0
for %%F in ("%SRC%\*.partsbnd.dcx") do (
  set "NAME=%%~nxF"
  if not exist "%GAME%\parts\!NAME!.pre-hidequiver" copy /Y "%GAME%\parts\!NAME!" "%GAME%\parts\!NAME!.pre-hidequiver" >nul
  copy /Y "%%F" "%GAME%\parts\!NAME!" >nul
  echo [ok] !NAME!
  set /a n+=1
)
echo.
echo installed !n! file^(s^) into "%GAME%\parts"
echo restart Dark Souls III for the change to take effect.
echo undo with: restore.cmd
endlocal
