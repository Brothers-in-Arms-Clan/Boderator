#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ArmaForces.Boderator/ArmaForces.Boderator.csproj", "ArmaForces.Boderator/"]
RUN dotnet restore "ArmaForces.Boderator/ArmaForces.Boderator.csproj"
COPY . .
WORKDIR "/src/ArmaForces.Boderator"
RUN dotnet build "ArmaForces.Boderator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArmaForces.Boderator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArmaForces.Boderator.dll"]