FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Source/ComfyBot.Gateway/ComfyBot.Gateway.csproj", "Source/ComfyBot.Gateway/"]
COPY ["Source/ComfyBot.Gateway.Contracts/ComfyBot.Gateway.Contracts.csproj", "Source/ComfyBot.Gateway.Contracts/"]
COPY ["Source/ComfyBot.Common/ComfyBot.Common.csproj", "Source/ComfyBot.Common/"]
RUN dotnet restore "Source/ComfyBot.Gateway/ComfyBot.Gateway.csproj"

COPY . .
WORKDIR "/src/Source/ComfyBot.Gateway"
RUN dotnet build "ComfyBot.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ComfyBot.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5125
ENV ASPNETCORE_URLS=http://+:5010
ENTRYPOINT ["dotnet", "ComfyBot.Gateway.dll"]
