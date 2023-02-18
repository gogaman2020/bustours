# docker build -t bustour_scheduler -f src/BusTour.Scheduler/Dockerfile .
# docker run -d --name bustour_scheduler bustour_scheduler

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

COPY otc-infrastructure-core ./app/otc-infrastructure-core
COPY src ./app/src

WORKDIR /app/src/BusTour.Scheduler

RUN dotnet publish ./BusTour.Scheduler.csproj -c Debug -o out

# FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime

WORKDIR /app

COPY  --from=build-env /app/src/BusTour.Scheduler/out ./

ENV ASPNETCORE_ENVIRONMENT=DevelopmentDocker

ENTRYPOINT ["dotnet", "BusTour.Scheduler.dll"]
