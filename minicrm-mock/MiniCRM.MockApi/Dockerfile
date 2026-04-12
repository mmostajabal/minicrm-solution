FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY minicrm-mock/MiniCRM.MockApi/MiniCRM.MockApi.csproj minicrm-mock/MiniCRM.MockApi/
RUN dotnet restore minicrm-mock/MiniCRM.MockApi/MiniCRM.MockApi.csproj

COPY minicrm-mock/MiniCRM.MockApi/ minicrm-mock/MiniCRM.MockApi/
RUN dotnet publish minicrm-mock/MiniCRM.MockApi/MiniCRM.MockApi.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5090
ENV ASPNETCORE_URLS=http://+:5090
ENTRYPOINT ["dotnet", "MiniCRM.MockApi.dll"]
