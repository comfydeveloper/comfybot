# Debugging Message Flow from Gateway to Bot

## Problem
Messages are being received by the Gateway from Twitch, but they're not being processed by the Bot application.

## Changes Made

### 1. Added Enhanced Logging
I've added detailed logging at each step of the message flow to help identify where messages might be getting lost:

#### Gateway (Program.cs)
- Now logs when messages are received from Twitch
- Logs when messages are forwarded to SignalR clients

#### GatewayClient
- Now logs when messages are received from SignalR (changed from Debug to Information level)

#### GatewayEventBridge
- Logs when handlers are registered
- Logs when message/command events are received
- Logs the count of handlers found
- Logs each handler invocation

### 2. Fixed Logging Level
Changed `Source\ComfyBot.Application\appsettings.json`:
- **Before**: `"Default": "Error"`
- **After**: `"Default": "Information"`

This was preventing Information-level logs from appearing in the Bot application.

## Message Flow

The expected message flow is:

1. **Twitch → TwitchService** (Gateway)
   - TwitchLib receives message
   - `TwitchService.HandleMessageReceived` or `HandleCommandReceived` creates event

2. **TwitchService → Program.cs** (Gateway)
   - Event handler in `Program.cs` receives the event
   - Should log: `"Gateway: Received message from Twitch, forwarding to SignalR clients"`

3. **Program.cs → SignalR Hub** (Gateway)
   - `hubContext.Clients.All.ReceiveMessage(messageEvent)` broadcasts to all connected clients
   - Should log: `"Gateway: Message forwarded to SignalR clients"`

4. **SignalR Hub → GatewayClient** (Bot)
   - SignalR client receives via `hubConnection.On<MessageReceivedEvent>("ReceiveMessage", ...)`
   - Should log: `"GatewayClient: Received message from SignalR: {UserName}: {Text}"`
   - Raises `OnMessageReceived` event

5. **GatewayClient → GatewayEventBridge** (Bot)
   - Event bridge subscribed to `gatewayClient.OnMessageReceived`
   - Should log: `"GatewayEventBridge: Received message event from Gateway"`
   - Should log: `"GatewayEventBridge: Found {Count} message handlers"`

6. **GatewayEventBridge → Handlers** (Bot)
   - Each registered handler is invoked
   - Should log: `"GatewayEventBridge: Invoking handler {HandlerType}"`

## Debugging Steps

### Step 1: Restart Both Applications
1. Stop both Gateway and Bot applications
2. Start the Gateway first
3. Wait for Gateway to connect to Twitch and join the channel
4. Start the Bot application
5. Wait for Bot to connect to Gateway

### Step 2: Send a Test Message
Send a message in the Twitch chat (not starting with `!` so it's a regular message, not a command).

### Step 3: Check the Logs

Look for these log messages in order:

#### Gateway Console/Logs:
```
Gateway: Received message from Twitch, forwarding to SignalR clients. User: {username}, Text: {message}
Gateway: Message forwarded to SignalR clients
```

#### Bot Console/Logs:
```
GatewayClient: Received message from SignalR: {username}: {message}
GatewayEventBridge: Received message event from Gateway. User: {username}, Text: {message}
GatewayEventBridge: Found {N} message handlers
GatewayEventBridge: Invoking handler {HandlerName}
```

### Step 4: Identify Where the Flow Breaks

If you see:
- ✅ Gateway logs but ❌ no Bot logs → SignalR connection issue
- ✅ GatewayClient logs but ❌ no GatewayEventBridge logs → Event subscription issue
- ✅ GatewayEventBridge "Received message" but ❌ "Found 0 handlers" → Handler registration issue
- ✅ "Found N handlers" but ❌ no "Invoking handler" logs → Handler iteration issue

## Common Issues

### Issue 1: SignalR Connection Not Established
**Symptom**: Gateway logs show messages but Bot shows no SignalR activity.

**Check**:
- Is the Gateway running and accessible at `http://localhost:5125`?
- Did the Bot log `"Connected to Gateway at http://localhost:5125"`?
- Check the Bot's `appsettings.json` for correct `GatewayUrl`

### Issue 2: Event Handlers Not Registered
**Symptom**: GatewayClient receives messages but GatewayEventBridge doesn't.

**Check**:
- Was `eventBridge.RegisterHandlers()` called in `ChatBot.Run()`?
- Look for log: `"GatewayEventBridge: Event handlers registered"`

### Issue 3: No Handlers Found
**Symptom**: GatewayEventBridge receives events but finds 0 handlers.

**Check**:
- Are handlers registered in `BotModule.RegisterServices()`?
- Are the handler classes implementing `IChatMessageHandler` or `ICommandHandler`?
- Check dependency injection setup

## Next Steps

After restarting both applications with the new logging in place, share the console output from both applications when you send a test message. This will help pinpoint exactly where the message flow is breaking.
