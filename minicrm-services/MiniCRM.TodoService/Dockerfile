FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY minicrm-shared/MiniCRM.Shared/MiniCRM.Shared.csproj minicrm-shared/MiniCRM.Shared/
COPY minicrm-services/MiniCRM.TodoService/MiniCRM.TodoService.csproj minicrm-services/MiniCRM.TodoService/
RUN dotnet restore minicrm-services/MiniCRM.TodoService/MiniCRM.TodoService.csproj

COPY minicrm-shared/MiniCRM.Shared/ minicrm-shared/MiniCRM.Shared/
COPY minicrm-services/MiniCRM.TodoService/ minicrm-services/MiniCRM.TodoService/
RUN dotnet publish minicrm-services/MiniCRM.TodoService/MiniCRM.TodoService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5083
ENV ASPNETCORE_URLS=http://+:5083
ENTRYPOINT ["dotnet", "MiniCRM.TodoService.dll"]
