After each Todo, make sure the AGENT_PROJECT.md file is up to date.

[] On the three main pages, there are methods that handle callbacks, such as HandleVariableChanged() on the Variables page. These methods are currently synchronous. Make them async and ensure the async handlers within them are awaited.

[] Analyze and fix these nuget warnings in the .Application 
PackageReference Microsoft.Extensions.Hosting will not be pruned. Consider removing this package from your dependencies, as it is likely unnecessary.
PackageReference Microsoft.Extensions.DependencyInjection will not be pruned. Consider removing this package from your dependencies, as it is likely unnecessary.
PackageReference Microsoft.AspNetCore.Components.Web will not be pruned. Consider removing this package from your dependencies, as it is likely unnecessary.

[] For all buttons in the application, there should be a shared button component for shared styling and behavior. Buttons should be functionally dumb and only render child content as a RenderFragment, provide an OnClick event callback, a priority parameter (enum with values Primary, Secondary, Tertiary) and a parameter to disable it.
Add this in the folder ComfyBot.Application/Components/Atoms.

[] Move from an SQLite database to a postgres database connection for the Data project (.Bot and .Application use this). For that, a postgres docker container is already available, so the application needs a configuration for the connection string. 

[] The goal is as follows: the Gateway and the Application projects should be runnable using a shared docker-compose file. The file will connect both services. The redis connection will be provided through a connection string. The postgres connection as well.
The docker-compose file should contain all necessary application settings (e.g. connection strings, auth keys, etc.) as environment variables that can be overridden at runtime. No need for default values, only the setting names should be included as part of the structure of the file.
At the end, the user should be able to run a single command to start both the Gateway and the Application after providing the necessary environment variables.


[] Introduce a variables.css file which contains css variables for shared values, such as primary colors, accent colors, font colors, etc.
Ensure the variables are used in styles.