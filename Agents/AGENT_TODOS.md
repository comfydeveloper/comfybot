After each Todo, make sure the AGENT_PROJECT.md file is up to date.

[] Upgrade the Gateway's TwitchLib dependency to the latest version and refactor the Twitch connection/message handling logic to match the new TwitchLib API. Expect breaking changes.

[] Refactor the .Application project's UI from a WPF implementation to a Blazor application. The /Features/ folder should remain untouched, but the UI should be built with Blazor instead of WPF. The application should then be runnable in a docker container. 
The whole UI right now resides in the MainWindow.xaml file. Split this into separate pages (TextCommands, MessageResponses, Variables) and use a shared layout for the common UI elements (e.g. navigation, header, etc.). The Blazor application should be structured in a way that allows for easy addition of new pages/features in the future.

[] Make the Gateway runnable as a docker container.

[] The goal is as follows: the Gateway and the Application projects should be runnable using a shared docker-compose file. The file will connect both services. The .Application project will continue to use an SQLite database, so there needs to be a volume for it that points at a physical path on the host machine where the sqlite file is. The redis connection will be provided through a connection string.
The docker-compose file should contain all necessary application settings (e.g. connection strings, auth keys, etc.) as environment variables that can be overridden at runtime. No need for default values, only the setting names should be included as part of the structure of the file.
At the end, the user should be able to run a single command to start both the Gateway and the Application after providing the necessary environment variables and volume mappings (redis will already be available).