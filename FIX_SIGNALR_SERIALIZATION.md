# Fix: SignalR Interface Serialization Issue

## Problem Identified
Messages were being sent from the Gateway to SignalR but never received by the Bot client. The Bot's `GatewayClient.On<>("ReceiveMessage", ...)` handler was never being invoked.

## Root Cause
The event contracts used **interface types** for their properties:

```csharp
// BEFORE (broken)
public class MessageReceivedEvent
{
	public IChatMessage Message { get; set; }  // ❌ Interface type
}

public class CommandReceivedEvent
{
	public IChatCommand Command { get; set; }  // ❌ Interface type
}
```

**SignalR uses System.Text.Json for serialization**, which cannot deserialize into interface types because it doesn't know which concrete implementation to instantiate.

The flow was:
1. Gateway creates `ChatMessage` instance (concrete type) ✅
2. Gateway serializes it as JSON ✅
3. SignalR sends JSON to Bot ✅
4. Bot's SignalR client receives JSON ✅
5. SignalR tries to deserialize JSON into `IChatMessage` ❌ **FAILS SILENTLY**
6. Event handler never invoked because deserialization failed

## Solution
Changed the event properties to use **concrete types** instead of interfaces:

```csharp
// AFTER (working)
public class MessageReceivedEvent
{
	public ChatMessage Message { get; set; }  // ✅ Concrete type
}

public class CommandReceivedEvent
{
	public ChatCommand Command { get; set; }  // ✅ Concrete type
}
```

## Why This Works
- `ChatMessage` implements `IChatMessage`
- `ChatCommand` implements `IChatCommand`
- Event properties now use concrete types (serializable)
- Handlers still receive interface types (polymorphic)
- No breaking changes to handler code

## Files Changed
1. `Source\ComfyBot.Gateway.Contracts\Events\MessageReceivedEvent.cs`
   - Changed `IChatMessage Message` → `ChatMessage Message`

2. `Source\ComfyBot.Gateway.Contracts\Events\CommandReceivedEvent.cs`
   - Changed `IChatCommand Command` → `ChatCommand Command`

## Testing
After rebuilding both Gateway and Bot:
1. Restart the Gateway
2. Restart the Bot
3. Send a test message in chat
4. You should now see logs in the Bot showing:
   - `"GatewayClient: Received message from SignalR"`
   - `"GatewayEventBridge: Received message event from Gateway"`
   - `"GatewayEventBridge: Found N message handlers"`
   - Handler execution logs
