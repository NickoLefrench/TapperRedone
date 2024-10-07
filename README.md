# TapperRedone
 Redo of Tapper Project cleanly

## Code style enforcement
Go in Visual Studio: Tools -> Options -> Text Editor -> Code Cleanup. Set "Run Code Cleanup profile on Save".
Any file you save will automatically run a consistency formatting check.
The code style is defined in .editorconfig.
Due to gitignore preventing .csproj and .sln files into git, can't run Github Actions easily. Could potentially use game-ci/unity-builder if we want to eventually investigate that.
