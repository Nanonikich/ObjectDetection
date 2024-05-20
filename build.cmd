@set DOTNET_CLI_UI_LANGUAGE=en
@echo Building ObjectDetection...
cd src
@dotnet build --configuration Release ObjectDetection.sln
@pause