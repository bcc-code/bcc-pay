ARG DOTNET_CONFIGURATION=Development
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
    libc6-dev \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
ENV PORT 8080
ENV HOST 0.0.0.0
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR "/src/"
RUN ls
COPY ["/BccPay.Core.Sample.Contracts/BccPay.Core.Sample.Contracts.csproj", "./"]
COPY ["/BccPay.Core.Cqrs/BccPay.Core.Cqrs.csproj", "./"]
COPY ["/BccPay.Core.DataAccess/BccPay.Core.DataAccess.csproj", "./"]
COPY ["/BccPay.Core.Domain/BccPay.Core.Domain.csproj", "./"]
COPY ["/BccPay.Core.Enums/BccPay.Core.Enums.csproj", "./"]
COPY ["/BccPay.Core.Infrastructure/BccPay.Core.Infrastructure.csproj", "./"]
COPY ["/BccPay.Core.Shared/BccPay.Core.Shared.csproj", "./"]
COPY ["/BccPay.Core.Sample.API/BccPay.Core.Sample.API.csproj", "./"]
COPY ["/BccPay.Core.Notifications/BccPay.Core.Notifications.csproj", "./"]

RUN dotnet restore "BccPay.Core.Sample.API.csproj"
COPY . .
WORKDIR "/src/BccPay.Core.Sample.API/"
ARG DOTNET_CONFIGURATION
RUN dotnet build "BccPay.Core.Sample.API.csproj" -c $DOTNET_CONFIGURATION -o /app/build

FROM build AS publish
ARG DOTNET_CONFIGURATION
RUN dotnet publish "BccPay.Core.Sample.API.csproj" -c $DOTNET_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ARG DOTNET_CONFIGURATION
FROM final AS branch-Development
RUN echo "Docker development config"
ENV ASPNETCORE_ENVIRONMENT=Development

ARG DOTNET_CONFIGURATION
FROM final AS branch-Release
RUN echo "Docker production config"
ENV ASPNETCORE_ENVIRONMENT=Production

ARG DOTNET_CONFIGURATION
FROM branch-${DOTNET_CONFIGURATION} AS end
ENV ASPNETCORE_URLS=http://*:${PORT}
ENTRYPOINT ["dotnet", "BccPay.Core.Sample.API.dll"]
