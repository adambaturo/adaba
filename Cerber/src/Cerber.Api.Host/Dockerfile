FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Cerber/src/Cerber.Api.Host/Cerber.Api.Host.csproj", "Cerber/src/Cerber.Api.Host/"]
RUN dotnet restore "Cerber/src/Cerber.Api.Host/Cerber.Api.Host.csproj"
COPY . .
WORKDIR "/src/Cerber/src/Cerber.Api.Host"
RUN dotnet build "Cerber.Api.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cerber.Api.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cerber.Api.Host.dll"]
