ARG DOTNET_VERSION=3.1
FROM mcr.microsoft.com/dotnet/core/sdk:$DOTNET_VERSION-alpine3.9 AS build

# Disable the invariant mode (set in base image)
RUN apk add --no-cache \
    icu-libs \
    nodejs=10.19.0-r0 \
    nodejs-npm

WORKDIR /build

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /publish /p:PASSCORE_PROVIDER=LDAP

FROM mcr.microsoft.com/dotnet/core/aspnet:$DOTNET_VERSION-alpine3.9 AS release

WORKDIR /app

COPY --from=build /publish .

EXPOSE 80
CMD ["dotnet", "/app/Unosquare.PassCore.Web.dll"]
