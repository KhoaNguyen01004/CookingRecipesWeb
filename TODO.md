# TODO: Fix Supabase Namespace Issue

## Steps to Complete

- [x] Update CookingRecipesWeb.csproj to add the 'Supabase' package reference
- [x] Update Program.cs to register the main Supabase.Client as a singleton using IConfiguration
- [x] Update Services/UserService.cs:
  - [x] Change using statement to 'using Supabase;'
  - [x] Declare 'private readonly Client _client;'
  - [x] Update constructor to inject Client
  - [x] Replace '_postgrestClient' with '_client.Postgrest' in all methods
- [x] Run 'dotnet restore' to restore packages
- [x] Run 'dotnet build' to verify the changes
