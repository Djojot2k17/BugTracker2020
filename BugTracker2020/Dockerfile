#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["BugTracker2020/BugTracker2020.csproj", "BugTracker2020/"]
RUN dotnet restore "BugTracker2020/BugTracker2020.csproj"
COPY . .
WORKDIR "/src/BugTracker2020"
RUN dotnet build "BugTracker2020.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BugTracker2020.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BugTracker2020.dll"]