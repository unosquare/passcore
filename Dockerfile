FROM node:latest AS node_base

RUN echo "NODE Version:" && node --version
RUN echo "NPM Version:" && npm --version

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

COPY --from=node_base . .

WORKDIR /src
COPY ./ ./

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app /p:PASSCORE_PROVIDER=LDAP --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Unosquare.PassCore.Web.dll"]
