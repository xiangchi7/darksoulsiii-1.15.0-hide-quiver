@echo off
setlocal enabledelayedexpansion
set "SRC=%~dp0files"
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
