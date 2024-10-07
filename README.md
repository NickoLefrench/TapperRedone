# TapperRedone
 Redo of Tapper Project cleanly

## Code style enforcement
Run the following command for warnings, without overriding files
`dotnet format ./TapperRedone.sln --verify-no-changes --verbosity diagnostic --include ./Assets/Scripts/`
You can remove `--verify-no-changes` to allow it to make changes.
Also, Github Actions runs this command.
