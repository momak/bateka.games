# syntax=docker/dockerfile:1
# Build from the solution root:
#   docker build -f bateka.games/Dockerfile --secret id=github_token,env=GITHUB_TOKEN -t games-client .
# GITHUB_TOKEN: a token that can read GitHub Packages (Bateka.UI). In Actions it's the built-in one.

# --- Build ---
FROM mcr.microsoft.com/dotnet/sdk:11.0 AS build
WORKDIR /src

COPY nuget.config ./
COPY bateka.games.Engine/bateka.games.Engine.csproj bateka.games.Engine/
COPY bateka.games.Shared/bateka.games.Shared.csproj bateka.games.Shared/
COPY bateka.games/bateka.games.csproj bateka.games/

# The token is mounted for this one step only: it never ends up in a layer, the image or its history.
RUN --mount=type=secret,id=github_token,required=true \
    NuGetPackageSourceCredentials_github="Username=docker;Password=$(cat /run/secrets/github_token)" \
    dotnet restore bateka.games/bateka.games.csproj

COPY bateka.games.Engine/ bateka.games.Engine/
COPY bateka.games.Shared/ bateka.games.Shared/
COPY bateka.games/ bateka.games/
RUN dotnet publish bateka.games/bateka.games.csproj -c Release -o /publish --no-restore

# --- Runtime: plain nginx, no .NET runtime needed for static files ---
FROM nginx:1.29-alpine AS runtime
# --chmod: so nginx's worker can always read the files (same fix as fit, BillTracker, Auth, AI).
COPY --from=build --chmod=755 /publish/wwwroot /usr/share/nginx/html
COPY bateka.games/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
