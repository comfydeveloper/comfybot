FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Source/ComfyBot.Application/ComfyBot.Application.csproj", "Source/ComfyBot.Application/"]
COPY ["Source/ComfyBot.Bot/ComfyBot.Bot.csproj", "Source/ComfyBot.Bot/"]
COPY ["Source/ComfyBot.Data/ComfyBot.Data.csproj", "Source/ComfyBot.Data/"]
COPY ["Source/ComfyBot.Common/ComfyBot.Common.csproj", "Source/ComfyBot.Common/"]
RUN dotnet restore "Source/ComfyBot.Application/ComfyBot.Application.csproj"

COPY . .
WORKDIR "/src/Source/ComfyBot.Application"
RUN dotnet build "ComfyBot.Application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ComfyBot.Application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "ComfyBot.Application.dll"]
