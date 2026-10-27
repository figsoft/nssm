@echo off
setlocal EnableExtensions

set "VCVARS=C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VC\Auxiliary\Build\vcvarsall.bat"
set "SRC=%~dp0..\src"
set "OUT=%~dp0out"

call "%VCVARS%" x64
if errorlevel 1 ( echo [FAIL] vcvarsall & exit /b 1 )

if not exist "%OUT%" mkdir "%OUT%"

pushd "%SRC%"
echo CWD=%CD%

echo === 1/4 Generating version.h ===
call "%SRC%\version.cmd"
if errorlevel 1 ( echo [FAIL] version.cmd & exit /b 1 )

echo === 2/4 Message compiler (messages.h / messages.rc) ===
mc -u -U messages.mc -r . -h .
if errorlevel 1 ( echo [FAIL] mc & exit /b 1 )

set "CLFLAGS=/nologo /c /O2 /Ob1 /MT /W3 /utf-8 /D WIN32 /D NDEBUG /D _CONSOLE /D UNICODE /D _UNICODE /Fo%OUT%\"

echo === 3/4 Compiling sources ===
for %%f in (account console env event gui hook imports io nssm process registry service settings utf8) do (
  cl %CLFLAGS% "%%f.cpp"
  if errorlevel 1 ( echo [FAIL] compile %%f.cpp & exit /b 1 )
)

echo === 4/4 Compiling resources ===
rc /nologo /d NDEBUG /d _WIN64 /d UNICODE /d _UNICODE /fo "%OUT%\nssm.res" nssm.rc
if errorlevel 1 ( echo [FAIL] rc & exit /b 1 )

echo === Linking ===
link /nologo /OUT:"%OUT%\nssm.exe" /SUBSYSTEM:CONSOLE /MACHINE:X64 ^
  kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib ^
  shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib ^
  psapi.lib shlwapi.lib ^
  "%OUT%\account.obj" "%OUT%\console.obj" "%OUT%\env.obj" "%OUT%\event.obj" ^
  "%OUT%\gui.obj" "%OUT%\hook.obj" "%OUT%\imports.obj" "%OUT%\io.obj" ^
  "%OUT%\nssm.obj" "%OUT%\process.obj" "%OUT%\registry.obj" "%OUT%\service.obj" ^
  "%OUT%\settings.obj" "%OUT%\utf8.obj" "%OUT%\nssm.res"
if errorlevel 1 ( echo [FAIL] link & exit /b 1 )

popd
echo.
echo BUILD OK: %OUT%\nssm.exe
