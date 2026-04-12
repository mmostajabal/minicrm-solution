FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY minicrm-shared/MiniCRM.Shared/MiniCRM.Shared.csproj minicrm-shared/MiniCRM.Shared/
COPY minicrm-services/MiniCRM.ContactService/MiniCRM.ContactService.csproj minicrm-services/MiniCRM.ContactService/
RUN dotnet restore minicrm-services/MiniCRM.ContactService/MiniCRM.ContactService.csproj

COPY minicrm-shared/MiniCRM.Shared/ minicrm-shared/MiniCRM.Shared/
COPY minicrm-services/MiniCRM.ContactService/ minicrm-services/MiniCRM.ContactService/
RUN dotnet publish minicrm-services/MiniCRM.ContactService/MiniCRM.ContactService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5081
ENV ASPNETCORE_URLS=http://+:5081
ENTRYPOINT ["dotnet", "MiniCRM.ContactService.dll"]
