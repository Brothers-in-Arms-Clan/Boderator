#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["ArmaForces.Boderator.WebService/ArmaForces.Boderator.WebService.csproj", "ArmaForces.Boderator.WebService/"]
RUN dotnet restore "ArmaForces.Boderator.WebService/ArmaForces.Boderator.WebService.csproj"
COPY . .
WORKDIR "/src/ArmaForces.Boderator.WebService"
RUN dotnet build "ArmaForces.Boderator.WebService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArmaForces.Boderator.WebService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArmaForces.Boderator.WebService.dll"]