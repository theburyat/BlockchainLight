FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

EXPOSE 6001
EXPOSE 6002
EXPOSE 6003

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlockchainLight/BlockchainLight.csproj", "BlockchainLight/"]
RUN dotnet restore "BlockchainLight/BlockchainLight.csproj"
COPY . .
WORKDIR "/src/BlockchainLight"
RUN dotnet build "BlockchainLight.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlockchainLight.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlockchainLight.dll"]
