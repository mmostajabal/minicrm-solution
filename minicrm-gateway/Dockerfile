FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY minicrm-shared/MiniCRM.Shared/MiniCRM.Shared.csproj minicrm-shared/MiniCRM.Shared/
COPY minicrm-gateway/MiniCRM.Gateway.csproj minicrm-gateway/
RUN dotnet restore minicrm-gateway/MiniCRM.Gateway.csproj

COPY minicrm-shared/MiniCRM.Shared/ minicrm-shared/MiniCRM.Shared/
COPY minicrm-gateway/ minicrm-gateway/
RUN dotnet publish minicrm-gateway/MiniCRM.Gateway.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5080
ENV ASPNETCORE_URLS=http://+:5080
ENTRYPOINT ["dotnet", "MiniCRM.Gateway.dll"]
