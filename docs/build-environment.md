# Build Environment

- Use C:\sts2\dotnet\dotnet.exe when a later task explicitly requires a build.
- NuGet.Config and .local-nuget are local ignored files.
- Do not use system dotnet.
- Do not alter package feeds or global NuGet configuration casually.
- Build commands are intentionally deferred until the empty-Mod bootstrap task.
