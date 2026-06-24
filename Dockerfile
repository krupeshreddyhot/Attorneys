# Build from repository root (folder containing Attorneys.API/).
# Example: docker build -t attorneys-api .

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Attorneys.Domain/Attorneys.Domain.csproj Attorneys.Domain/
COPY Attorneys.Application/Attorneys.Application.csproj Attorneys.Application/
COPY Attorneys.Infrastructure/Attorneys.Infrastructure.csproj Attorneys.Infrastructure/
COPY Attorneys.API/Attorneys.API.csproj Attorneys.API/

RUN dotnet restore Attorneys.API/Attorneys.API.csproj

COPY Attorneys.Domain/ Attorneys.Domain/
COPY Attorneys.Application/ Attorneys.Application/
COPY Attorneys.Infrastructure/ Attorneys.Infrastructure/
COPY Attorneys.API/ Attorneys.API/

WORKDIR /src/Attorneys.API
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Attorneys.API.dll"]
