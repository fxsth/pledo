FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /app

# Install Node.js
RUN apt-get update \
    && apt-get install -y ca-certificates curl gnupg \
    && mkdir -p /etc/apt/keyrings \
    && curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --dearmor -o /etc/apt/keyrings/nodesource.gpg
RUN echo "deb [signed-by=/etc/apt/keyrings/nodesource.gpg] https://deb.nodesource.com/node_20.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list
RUN apt-get update \
    && apt-get install nodejs -y


WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
RUN dotnet restore "Web/Web.csproj" -a $TARGETARCH
WORKDIR "/src/Web"
COPY ./Web .

RUN dotnet publish "Web.csproj" -a $TARGETARCH --self-contained false -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Needed when csproj is .NET 7
ENV DOTNET_ROLL_FORWARD=Major

WORKDIR /app
COPY --from=build /app/publish .

VOLUME /config
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "pledo.dll"]
