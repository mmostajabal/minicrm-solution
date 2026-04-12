@echo off
REM miniCRM Solution – Start All Services
REM Run this script from the minicrm-solution root directory.
REM Requires .NET 8 SDK and Node.js 18+.

echo ====================================================
echo  miniCRM Solution – Starting all services
echo ====================================================
echo.

REM Load credentials from credentials.bat
if exist "%~dp0credentials.bat" (
    call "%~dp0credentials.bat"
    echo [OK] Credentials loaded from credentials.bat
) else (
    echo [WARNING] credentials.bat not found - using appsettings.json values
)
echo.

REM Mock miniCRM API (port 5090) – optional, for testing without real API key
echo [0/6] Starting Mock miniCRM API on port 5090...
start "MockApi" cmd /k "cd /d %~dp0minicrm-mock\MiniCRM.MockApi && dotnet run"

timeout /t 2 /nobreak >nul

REM ContactService (port 5081)
echo [1/6] Starting ContactService on port 5081...
start "ContactService" cmd /k "call "%~dp0credentials.bat" && cd /d %~dp0minicrm-services\MiniCRM.ContactService && dotnet run"

timeout /t 2 /nobreak >nul

REM ProjectService (port 5082)
echo [2/6] Starting ProjectService on port 5082...
start "ProjectService" cmd /k "call "%~dp0credentials.bat" && cd /d %~dp0minicrm-services\MiniCRM.ProjectService && dotnet run"

timeout /t 2 /nobreak >nul

REM TodoService (port 5083)
echo [3/6] Starting TodoService on port 5083...
start "TodoService" cmd /k "call "%~dp0credentials.bat" && cd /d %~dp0minicrm-services\MiniCRM.TodoService && dotnet run"

timeout /t 2 /nobreak >nul

REM InvoiceService (port 5084)
echo [4/6] Starting InvoiceService on port 5084...
start "InvoiceService" cmd /k "call "%~dp0credentials.bat" && cd /d %~dp0minicrm-services\MiniCRM.InvoiceService && dotnet run"

timeout /t 3 /nobreak >nul

REM API Gateway (port 5080)
echo [5/6] Starting API Gateway on port 5080...
start "API Gateway" cmd /k "call "%~dp0credentials.bat" && cd /d %~dp0minicrm-gateway && dotnet run"

echo.
echo ====================================================
echo  All services started!
echo  Gateway:        http://localhost:5080
echo  Swagger:        http://localhost:5080/swagger
echo  ContactService: http://localhost:5081
echo  ProjectService: http://localhost:5082
echo  TodoService:    http://localhost:5083
echo  InvoiceService: http://localhost:5084
echo  Mock API:       http://localhost:5090
echo ====================================================
echo.
echo  To use Mock API instead of real miniCRM:
echo  Set MINICRM__BaseUrl=http://localhost:5090 in credentials.bat
echo.
echo  Next: Start the MCP server with:
echo  cd minicrm-mcp ^&^& npm start
echo.
pause
