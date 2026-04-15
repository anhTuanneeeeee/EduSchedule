FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Schedule/Schedule.csproj", "Schedule/"]
COPY ["Schedule_Repository/Schedule_Repository.csproj", "Schedule_Repository/"]
COPY ["Schedule_Service/Schedule_Service.csproj", "Schedule_Service/"]

RUN dotnet restore "Schedule/Schedule.csproj"

COPY . .
WORKDIR "/src/Schedule"
RUN dotnet build "Schedule.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Schedule.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Schedule.dll"]
