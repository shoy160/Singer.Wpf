"NuGet.exe" "pack" "..\src\Singer.Core\Singer.Core.csproj" -Properties Configuration=Release -IncludeReferencedProject
"NuGet.exe" "pack" "..\src\Singer.Client\Singer.Client.csproj" -Properties Configuration=Release -IncludeReferencedProjects
"NuGet.exe" "pack" "..\src\Singer.Update\Singer.Update.csproj" -Properties Configuration=Release -IncludeReferencedProjects