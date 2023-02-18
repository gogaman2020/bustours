FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

COPY otc-infrastructure-core ./app/otc-infrastructure-core
COPY src ./app/src

WORKDIR /app/src/BusTour.WebApi

#RUN dotnet restore 

RUN echo "$PWD"

RUN dotnet publish ./BusTour.WebApi.csproj -c Stable -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime

WORKDIR /app

COPY --from=build-env /app/src/BusTour.WebApi/out ./

ENV ASPNETCORE_ENVIRONMENT=Stable

ENTRYPOINT ["dotnet", "BusTour.WebApi.dll"]
