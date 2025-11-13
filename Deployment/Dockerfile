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
RUN dotnet restore Providor.MHHS.Electric.RequestService/Providor.MHHS.Electric.RequestService.csproj --source $FEED_URL --source $SHARED_FEED_URL --source "https://api.nuget.org/v3/index.json"

# Copy everything else and build
RUN dotnet publish Providor.MHHS.Electric.RequestService/Providor.MHHS.Electric.RequestService.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

COPY --from=build-env /app/out .

ARG MHHS_MESSAGING_CONNECTIONSTRING
ARG ELECTRIC_MESSAGING_CONNECTIONSTRING
ARG ASSETAPPOINTMENT_QUEUENAME
ARG ASSETAPPOINTMENTCONFIRMATION_QUEUENAME
ARG ASSETTERMINATION_QUEUENAME
ARG MESSAGING_REQUESTQUEUENAME
ARG MESSAGING_RESPONSEQUEUENAME
ARG MESSAGING_RESPONSEFROMASSETREGISTERQUEUENAME
ARG MESSAGING_ARJOBSTATUSCHANGEQUEUENAME
ARG ELECTRIC_DATAFLOWS_REQUESTQUEUENAME
ARG MHHS_EXTERNAL_DATABASE_CONNECTIONSTRING
ARG MHHS_INTERNAL_DATABASE_CONNECTIONSTRING
ARG LOGGING_INGEST_URL
ARG LOGGING_API_KEY

ENV MessagingConfig__MhhsServiceBusConnectionString=$MHHS_MESSAGING_CONNECTIONSTRING \
    MessagingConfig__ElectricServiceBusConnectionString=$ELECTRIC_MESSAGING_CONNECTIONSTRING \
    MessagingConfig__MhhsRequestQueueName=$MESSAGING_REQUESTQUEUENAME \
    MessagingConfig__MhhsResponseQueueName=$MESSAGING_RESPONSEQUEUENAME \
    MessagingConfig__AssetAppointmentQueueName=$ASSETAPPOINTMENT_QUEUENAME \
    MessagingConfig__AssetAppointmentConfirmationQueueName=$ASSETAPPOINTMENTCONFIRMATION_QUEUENAME \
    MessagingConfig__AssetTerminationQueueName=$ASSETTERMINATION_QUEUENAME \
    MessagingConfig__MhhsResponseFromAssetRegisterQueueName=$MESSAGING_RESPONSEFROMASSETREGISTERQUEUENAME \
    MessagingConfig__AssetRegisterJobStatusChangeQueueName=$MESSAGING_ARJOBSTATUSCHANGEQUEUENAME \
    MessagingConfig__ElectricDataFlowRequestQueueName=$ELECTRIC_DATAFLOWS_REQUESTQUEUENAME \
	ConnectionStrings__MHHSInternal=$MHHS_INTERNAL_DATABASE_CONNECTIONSTRING \
	ConnectionStrings__MHHSExternal=$MHHS_EXTERNAL_DATABASE_CONNECTIONSTRING \
	LoggingConfig__IngestUrl=$LOGGING_INGEST_URL \
	LoggingConfig__ApiKey=$LOGGING_API_KEY

ENTRYPOINT ["dotnet", "Providor.MHHS.Electric.RequestService.dll"]