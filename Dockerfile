ARG REPO=microsoft/dotnet
FROM $REPO:2.2-runtime-deps-alpine3.8 as builder

# Disable the invariant mode (set in base image)
RUN apk add --no-cache \
    icu-libs \
    nodejs=8.14.0-r0 \
    nodejs-npm

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

# Install .NET Core SDK
ENV DOTNET_SDK_VERSION 2.2.100

RUN wget -O dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Sdk/$DOTNET_SDK_VERSION/dotnet-sdk-$DOTNET_SDK_VERSION-linux-musl-x64.tar.gz \
    && dotnet_sha512='668dbbfdee1b898de57ee7320b3f7f77c3bae896cb1480d64869512f26d6998edccc1163c9ca6ed62b326e2d745a81e82a35a24cf4f7e11319fec0c6904e566e' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -C /usr/share/dotnet -xzf dotnet.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
    && rm dotnet.tar.gz

# Enable correct mode for dotnet watch (only mode supported in a container)
ENV DOTNET_USE_POLLING_FILE_WATCHER=true \ 
    # Skip extraction of XML docs - generally not useful within an image/container - helps performance
    NUGET_XMLDOC_MODE=skip

# Trigger first run experience by running arbitrary cmd to populate local package cache
RUN dotnet help

WORKDIR /app

COPY . ./
RUN dotnet restore

# Copy everything else and build
RUN dotnet publish -c Release -o /app/out /p:PASSCORE_PROVIDER=LDAP

FROM $REPO:2.2-runtime-deps-alpine3.8

# Install ASP.NET Core
ENV ASPNETCORE_VERSION 2.2.0

RUN wget -O aspnetcore.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/aspnetcore/Runtime/$ASPNETCORE_VERSION/aspnetcore-runtime-$ASPNETCORE_VERSION-linux-musl-x64.tar.gz \
    && aspnetcore_sha512='c297f7196b72e02ec41a5a0c027dcec1648ad552bf44036fa491d67d9b4f09e3ade84fd51ebffd68e8fa8077f2497ad851e13c83dac6aba89dd03f6df0adca6f' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf aspnetcore.tar.gz -C /usr/share/dotnet \
    && rm aspnetcore.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

WORKDIR /app

COPY --from=builder /app/out .

EXPOSE 80
CMD ["dotnet", "/app/Unosquare.PassCore.Web.dll"]
