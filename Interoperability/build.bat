REM --> @echo off

REM --> This basically copies the (newly?)-built
REM --> interoperability.dll into the plugins folder
REM --> of MissionPlanner, and then runs the program.

copy "C:\Users\adrom\Documents\GitHub\MissionPlanner\Interoperability\bin\x86\Debug\interoperability.dll" "C:\Program Files (x86)\Mission Planner\plugins" /y
start "C:\Program Files (x86)\Mission Planner\MissionPlanner.exe"