FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY minicrm-shared/MiniCRM.Shared/MiniCRM.Shared.csproj minicrm-shared/MiniCRM.Shared/
COPY minicrm-services/MiniCRM.ProjectService/MiniCRM.ProjectService.csproj minicrm-services/MiniCRM.ProjectService/
RUN dotnet restore minicrm-services/MiniCRM.ProjectService/MiniCRM.ProjectService.csproj

COPY minicrm-shared/MiniCRM.Shared/ minicrm-shared/MiniCRM.Shared/
COPY minicrm-services/MiniCRM.ProjectService/ minicrm-services/MiniCRM.ProjectService/
RUN dotnet publish minicrm-services/MiniCRM.ProjectService/MiniCRM.ProjectService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5082
ENV ASPNETCORE_URLS=http://+:5082
ENTRYPOINT ["dotnet", "MiniCRM.ProjectService.dll"]
