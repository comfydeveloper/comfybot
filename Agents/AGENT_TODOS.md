After each Todo, make sure the AGENT_PROJECT.md file is up to date. Also remove the todo you completed from the list.

[] Move from an SQLite database to a postgres database connection for the Data project (.Bot and .Application use this). For that, a postgres docker container is already available, so the application needs a configuration for the connection string. 

[] The goal is as follows: the Gateway and the Application projects should be runnable using a shared docker-compose file. The file will connect both services. The redis connection will be provided through a connection string. The postgres connection as well.
The docker-compose file should contain all necessary application settings (e.g. connection strings, auth keys, etc.) as environment variables that can be overridden at runtime. No need for default values, only the setting names should be included as part of the structure of the file.
At the end, the user should be able to run a single command to start both the Gateway and the Application after providing the necessary environment variables.


[] Introduce a variables.css file which contains css variables for shared values, such as primary colors, accent colors, font colors, etc.
Ensure the variables are used in styles.