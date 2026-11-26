# Build from the solution root:
#   docker build -f bateka.games.Api/Dockerfile -t games-api .

# Stage 1: base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:11.0 AS base
WORKDIR /app
EXPOSE 8080
USER $APP_UID

# Stage 2: restore (separate layer = cached unless a .csproj changes)
FROM mcr.microsoft.com/dotnet/sdk:11.0 AS restore
WORKDIR /src
COPY ["bateka.games.Engine/bateka.games.Engine.csproj", "bateka.games.Engine/"]
COPY ["bateka.games.Shared/bateka.games.Shared.csproj", "bateka.games.Shared/"]
COPY ["bateka.games.Domain/bateka.games.Domain.csproj", "bateka.games.Domain/"]
COPY ["bateka.games.Data/bateka.games.Data.csproj", "bateka.games.Data/"]
COPY ["bateka.games.Services/bateka.games.Services.csproj", "bateka.games.Services/"]
COPY ["bateka.games.Repositories/bateka.games.Repositories.csproj", "bateka.games.Repositories/"]
COPY ["bateka.games.Api/bateka.games.Api.csproj", "bateka.games.Api/"]
RUN dotnet restore "./bateka.games.Api/bateka.games.Api.csproj"

# Stage 3: build
FROM restore AS build
ARG BUILD_CONFIGURATION=Release
COPY bateka.games.Engine/ bateka.games.Engine/
COPY bateka.games.Shared/ bateka.games.Shared/
COPY bateka.games.Domain/ bateka.games.Domain/
COPY bateka.games.Data/ bateka.games.Data/
COPY bateka.games.Services/ bateka.games.Services/
COPY bateka.games.Repositories/ bateka.games.Repositories/
COPY bateka.games.Api/ bateka.games.Api/
WORKDIR "/src/bateka.games.Api"
RUN dotnet build "./bateka.games.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# Stage 4: publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./bateka.games.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

# Stage 5: final runtime image
FROM base AS final
WORKDIR /app
# --chmod: so the non-root app user can always read its own files (same fix as fit and Bateka.AI).
COPY --from=publish --chmod=755 /app/publish .
ENTRYPOINT ["dotnet", "bateka.games.Api.dll"]
