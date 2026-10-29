@echo off
setlocal enabledelayedexpansion
set "SRC=%~dp0files"
if not exist "%SRC%\" (
  echo [x] files\ folder not found - this looks like the git repository, not the release package.
  echo     Download HideQuiver-1.0.zip from the Releases page and run restore.cmd from the extracted folder.
  exit /b 1
)
set "GAME=%~1"
if "%GAME%"=="" set "GAME=D:\steam\steamapps\common\DARK SOULS III\Game"
set /a n=0
for %%F in ("%SRC%\*.partsbnd.dcx") do (
  set "NAME=%%~nxF"
  if exist "%GAME%\parts\!NAME!.pre-hidequiver" (
    copy /Y "%GAME%\parts\!NAME!.pre-hidequiver" "%GAME%\parts\!NAME!" >nul
    del "%GAME%\parts\!NAME!.pre-hidequiver"
    echo [restored] !NAME!
  ) else (
    if exist "%GAME%\parts\!NAME!" (
      del "%GAME%\parts\!NAME!"
      echo [removed add-on] !NAME!
    )
  )
  set /a n+=1
)
echo.
echo processed !n! file^(s^) - vanilla files are back, add-on files removed
endlocal
