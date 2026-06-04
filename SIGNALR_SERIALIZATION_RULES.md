# SignalR Serialization Rules for Gateway Contracts

## The Problem
System.Text.Json (used by SignalR) **cannot serialize or deserialize interface types** because it doesn't know which concrete implementation to use.

## Rule: Always Use Concrete Types for Serialization

### ❌ BAD - Interface Properties
```csharp
public class MessageReceivedEvent
{
	public IChatMessage Message { get; set; }  // ❌ Will fail to deserialize
}

public class ChatCommand
{
	public IChatMessage ChatMessage { get; set; }  // ❌ Will throw NotSupportedException
}
```

### ✅ GOOD - Concrete Properties
```csharp
public class MessageReceivedEvent
{
	public ChatMessage Message { get; set; }  // ✅ Serializes/deserializes correctly
}

public class ChatCommand
{
	public ChatMessage ChatMessage { get; set; }  // ✅ Serializes/deserializes correctly
}
```

## Symptoms of Interface Serialization Issues

### Silent Failure (Deserialization)
- Gateway logs show messages being sent
- Bot never receives them (event handler never fires)
- No error in logs (SignalR fails silently)
- Breakpoints in `hubConnection.On<T>(...)` never hit

### NotSupportedException (Serialization)
- `Exception thrown: 'System.NotSupportedException' in System.Text.Json.dll`
- Appears in Debug Output window
- May not appear in application logs
- Occurs when trying to serialize nested interface properties

## Where to Use Concrete vs Interface Types

### Use Concrete Types ✅
- DTOs / Events / Requests / Responses (anything crossing the wire)
- Any property that will be serialized by SignalR/JSON
- Collection items that will be serialized

### Use Interface Types ✅
- Handler method parameters
- Service dependencies (DI)
- Local variables in business logic
- Return types of factory methods

## Current Gateway Contracts (Fixed)

All serializable types now use concrete types:

```csharp
// Events
public class MessageReceivedEvent
{
	public ChatMessage Message { get; set; }  // Concrete
}

public class CommandReceivedEvent
{
	public ChatCommand Command { get; set; }  // Concrete
}

// Models
public class ChatCommand : IChatCommand
{
	public ChatMessage ChatMessage { get; set; }  // Concrete (was IChatMessage)
}

public class ChatMessage : IChatMessage
{
	// All value types - no serialization issues
}
```

## Interfaces Still Exist For Polymorphism

The interfaces (`IChatMessage`, `IChatCommand`) still exist and are used by:
- Handler method signatures
- Dependency injection abstractions
- Local code that doesn't cross serialization boundaries

```csharp
// This still works because ChatCommand implements IChatCommand
// and ChatCommand.ChatMessage returns a concrete ChatMessage (which implements IChatMessage)
public class TextCommandHandler : ICommandHandler
{
	public void Handle(IChatCommand command)  // Interface parameter ✅
	{
		string userName = command.ChatMessage.UserName;  // Works fine
	}
}
```

## Checklist for New Contracts

When creating new Gateway contracts:

- [ ] Event classes use concrete types for all properties
- [ ] Model classes use concrete types for nested objects
- [ ] Collections use concrete types: `List<ChatMessage>` not `List<IChatMessage>`
- [ ] Build succeeds
- [ ] Messages flow from Gateway to Bot
- [ ] No `NotSupportedException` in debug output
- [ ] Handlers receive and process data correctly

## Reference
- System.Text.Json polymorphic serialization requires explicit configuration
- SignalR uses System.Text.Json by default
- Interface deserialization would require custom JsonConverter implementations
- Using concrete types is simpler and more maintainable
