FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

ARG FEED_URL
ARG SHARED_FEED_URL
ARG USERNAME
ARG PAT

ENV Package=1

# download and install latest credential provider. Not required after https://github.com/dotnet/dotnet-docker/issues/878
RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash

# Optional: Sometimes the http client hangs because of a .NEt issue. Setting this in dockerfile helps 
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0

# Environment variable to enable seesion token cache. More on this here: https://github.com/Microsoft/artifacts-credprovider#help
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true

# Environment variable for adding endpoint credentials. More on this here: https://github.com/Microsoft/artifacts-credprovider#help
# Add "FEED_URL" AND "PAT" using --build-arg in docker build step. "endpointCredentials" field is an array, you can add multiple endpoint configurations.
# Make sure that you *do not* hard code the "PAT" here. That is a sensitive information and must not be checked in.
ENV VSS_NUGET_EXTERNAL_FEED_ENDPOINTS {\"endpointCredentials\": [{\"endpoint\":\"${FEED_URL}\", \"username\":\"${USERNAME}\", \"password\":\"${PAT}\"}]}

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet restore Sureserve.Dataflows.Router/Sureserve.Dataflows.Router.csproj --source $FEED_URL --source $SHARED_FEED_URL --source "https://api.nuget.org/v3/index.json"

# Copy everything else and build
RUN dotnet publish Sureserve.Dataflows.Router/Sureserve.Dataflows.Router.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

COPY --from=build-env /app/out .

ARG ELECTRIC_INCOMING_PATH
ARG GAS_INCOMING_PATH
ARG ELECTRIC_OUTGOING_PATH
ARG GAS_OUTGOING_PATH
ARG LOGGING_INGEST_URL
ARG LOGGING_API_KEY

ENV PathConfig__ElectricDataFlowsIncomingFolderPath=$ELECTRIC_INCOMING_PATH \
    PathConfig__GasDataFlowsIncomingFolderPath=$GAS_INCOMING_PATH \
    PathConfig__ElectricDataFlowsOutgoingFolderPath=$ELECTRIC_OUTGOING_PATH \
    PathConfig__GasDataFlowsIncomingFolderPath=$GAS_OUTGOING_PATH \
	LoggingConfig__IngestUrl=$LOGGING_INGEST_URL \
	LoggingConfig__ApiKey=$LOGGING_API_KEY

ENTRYPOINT ["dotnet", "Sureserve.Dataflows.Router.dll"]