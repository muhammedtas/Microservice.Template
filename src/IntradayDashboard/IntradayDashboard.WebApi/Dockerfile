FROM microsoft/dotnet:2.2-sdk
WORKDIR /app

# COPY ../../IntradayDashboard/*.sln .
# COPY ./IntradayDashboard.Infrastructure/IntradayDashboard.Infrastructure.csproj /app/IntradayDashboard/infra/
# COPY ./IntradayDashboard.Core/IntradayDashboard.Core.csproj /app/IntradayDashboard/core/
# COPY ./IntradayDashboard.WebApi/IntradayDashboard.WebApi.csproj /app/IntradayDashboard/web/

# COPY ["IntradayDashboard/IntradayDashboard.Infrastructure/IntradayDashboard.Infrastructure.csproj", "app/IntradayDashboard/infra/"]
# COPY ["IntradayDashboard/IntradayDashboard.Core/IntradayDashboard.Core.csproj", "app/IntradayDashboard/core/"]
# COPY ["IntradayDashboard/IntradayDashboard.WebApi/IntradayDashboard.WebApi.csproj", "app/IntradayDashboard/web/"]

ENV ASPNETCORE_URLS http://+:80
EXPOSE 80
COPY . .
RUN dotnet restore

# FROM microsoft/dotnet:2.2-aspnetcore-runtime
# WORKDIR /app
# COPY --from=build-env /app/out .

# Optional: Set this here if not setting it from docker-compose.yml
ENV ASPNETCORE_ENVIRONMENT Development
ENTRYPOINT ["dotnet", "IntradayDashboard/IntradayDashboard.WebApi.dll"]