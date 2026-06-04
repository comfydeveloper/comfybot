# Comfybot

A simple self-hosted twitch chat bot.
This is a small learning side-project. If you look for a place to learn or to find ideas for your own first bot, feel free to check out my code!

## Features

The bot currently has two main features.

### Text Commands

The text command feature lets you add simple commands that only reply with one or more random text messages.

#### Parameters

When a user types a command like `!test`, they can provide parameters after the command, e.g. `!test Hello World`. These parameters can be accessed from inside a command response.

Given the `!test Hello World` command:

| Pattern        | Sample Output |                                                     |
| -------------- | ------------- | --------------------------------------------------- |
| {{parameters}  | Hello World   | Returns all parameters just as the user wrote them. |
| {{parameter1}} | Hello         | Returns the first parameter.                        |
| {{parameter2}} | World         | Returns the second parameter.                       |

If a command does not have enough matching parameters defined for a response (e.g. the user provides 5, but the command only handles 1 or 3), the bot will try to return the one matching most individual parameters first. 

### Message Responses

To make the chat more lively and give your bot some more charm, you can let it reply to certain people or messages or keywords with one or more random messages.

### Variables

Variables are named values that can be put into any text. Updating one will replace it with the new value in all places where it is referenced.

They can be referenced using `[v:{variableName}]`. Writing `v++` also increased the value by 1 whenever it is used.

### Wildcards

Wildcards can be used in any text command and message response.

| Pattern    | Sample                        | Output             | Description                                         |
| ---------- | ----------------------------- | ------------------ | --------------------------------------------------- |
| [n:x-y]    | The chance is [n:0-100]%      | The chance is 69%. | Returns a **random** number between x and y.        |
| [w:{list}] | You feel [w:good,neutral,bad] | You feel neutral.  | Returns a **random** string from the {list}.        |
| {{user}}   | Hello {{user}}!               | Hello Username!    | Returns the name of the user who wrote the message. |




## Technologies

- NET 8.0
- WPF
- TwitchLib
- EF Core with SQLite
- NUnit
- NSubstitute
- FluentAssertions
