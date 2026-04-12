FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY minicrm-shared/MiniCRM.Shared/MiniCRM.Shared.csproj minicrm-shared/MiniCRM.Shared/
COPY minicrm-services/MiniCRM.InvoiceService/MiniCRM.InvoiceService.csproj minicrm-services/MiniCRM.InvoiceService/
RUN dotnet restore minicrm-services/MiniCRM.InvoiceService/MiniCRM.InvoiceService.csproj

COPY minicrm-shared/MiniCRM.Shared/ minicrm-shared/MiniCRM.Shared/
COPY minicrm-services/MiniCRM.InvoiceService/ minicrm-services/MiniCRM.InvoiceService/
RUN dotnet publish minicrm-services/MiniCRM.InvoiceService/MiniCRM.InvoiceService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5084
ENV ASPNETCORE_URLS=http://+:5084
ENTRYPOINT ["dotnet", "MiniCRM.InvoiceService.dll"]
