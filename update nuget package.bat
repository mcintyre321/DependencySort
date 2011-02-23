msbuild /property:Configuration=Release
robocopy .\DependencySort\bin\Release\ .\Nuget\lib\ /MIR
cd nuget 
nuget pack
cd ..
pause