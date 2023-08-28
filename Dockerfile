FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build

WORKDIR /app

RUN apk add --update npm

WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
RUN dotnet restore "Web/Web.csproj" --use-current-runtime
WORKDIR "/src/Web"
COPY ./Web .

RUN dotnet publish "Web.csproj" --use-current-runtime --self-contained false -c Release -o /app/publish 

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=build /app/publish .

VOLUME /config
EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "pledo.dll"]
