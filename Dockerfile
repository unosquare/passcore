FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

# Disable the invariant mode (set in base image)
RUN apk add --no-cache \
    icu-libs \
    nodejs=10.19.0-r0 \
    nodejs-npm

WORKDIR /src
COPY ./ ./

COPY . .

RUN dotnet restore
RUN dotnet publish -c Release -o /app /p:PASSCORE_PROVIDER=LDAP --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app ./
EXPOSE 80
ENTRYPOINT ["dotnet", "Unosquare.PassCore.Web.dll"]
