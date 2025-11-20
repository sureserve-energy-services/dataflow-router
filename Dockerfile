FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

ARG GH_PAT

# download and install latest credential provider. Not required after https://github.com/dotnet/dotnet-docker/issues/878
RUN wget -qO- https://raw.githubusercontent.com/Microsoft/artifacts-credprovider/master/helpers/installcredprovider.sh | bash

# Optional: Sometimes the http client hangs because of a .NEt issue. Setting this in dockerfile helps 
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0

# Environment variable to enable seesion token cache. More on this here: https://github.com/Microsoft/artifacts-credprovider#help
ENV NUGET_CREDENTIALPROVIDER_SESSIONTOKENCACHE_ENABLED true

# Copy csproj and restore as distinct layers
COPY . ./
RUN dotnet nuget add source --username lee.dale@proton.me --password $GH_PAT --store-password-in-clear-text --name github "https://nuget.pkg.github.com/sureserve-energy-services/index.json"
RUN dotnet restore Sureserve.Dataflows.Router/Sureserve.Dataflows.Router.csproj

# Copy everything else and build
RUN dotnet publish Sureserve.Dataflows.Router/Sureserve.Dataflows.Router.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

COPY --from=build-env /app/out .

# Electric Production
ARG ENV0_ENVIRONMENTTYPE
ARG ENV0_INPUTPATH
ARG ENV0_FLAG
ARG ENV0_OUTPUTTYPE
ARG ENV0_OUTPUTCOMMAND
ARG ENV0_OUTPUTPATH
ARG ENV0_DESTINATIONUSERNAME
ARG ENV0_DESTINATIONPASSWORD
ARG ENV0_FILEEXT0

# Electric QA
ARG ENV1_ENVIRONMENTTYPE
ARG ENV1_INPUTPATH
ARG ENV1_FLAG
ARG ENV1_OUTPUTTYPE
ARG ENV1_OUTPUTCOMMAND
ARG ENV1_OUTPUTPATH
ARG ENV1_DESTINATIONUSERNAME
ARG ENV1_DESTINATIONPASSWORD
ARG ENV1_FILEEXT0

# Gas Production
ARG ENV2_ENVIRONMENTTYPE
ARG ENV2_INPUTPATH
ARG ENV2_FLAG
ARG ENV2_OUTPUTTYPE
ARG ENV2_OUTPUTCOMMAND
ARG ENV2_OUTPUTPATH
ARG ENV2_DESTINATIONUSERNAME
ARG ENV2_DESTINATIONPASSWORD
ARG ENV2_FILEEXT0

ARG LOGGING_INGEST_URL
ARG LOGGING_API_KEY

ENV EnvironmentConfigs__Environments__0__EnvironmentType=$ENV0_ENVIRONMENTTYPE \
    EnvironmentConfigs__Environments__0__InputPath=$ENV0_INPUTPATH \
    EnvironmentConfigs__Environments__0__Flag=$ENV0_FLAG \
    EnvironmentConfigs__Environments__0__OutputType=$ENV0_OUTPUTTYPE \
    EnvironmentConfigs__Environments__0__OutputCommand=$ENV0_OUTPUTCOMMAND \
    EnvironmentConfigs__Environments__0__OutputPath=$ENV0_OUTPUTPATH \
    EnvironmentConfigs__Environments__0__DestinationUsername=$ENV0_DESTINATIONUSERNAME \
    EnvironmentConfigs__Environments__0__DestinationPassword=$ENV0_DESTINATIONPASSWORD \
    EnvironmentConfigs__Environments__0__FileExtensions__0=$ENV0_FILEEXT0 \
    EnvironmentConfigs__Environments__1__EnvironmentType=$ENV1_ENVIRONMENTTYPE \
    EnvironmentConfigs__Environments__1__InputPath=$ENV1_INPUTPATH \
    EnvironmentConfigs__Environments__1__Flag=$ENV1_FLAG \
    EnvironmentConfigs__Environments__1__OutputType=$ENV1_OUTPUTTYPE \
    EnvironmentConfigs__Environments__1__OutputCommand=$ENV1_OUTPUTCOMMAND \
    EnvironmentConfigs__Environments__1__OutputPath=$ENV1_OUTPUTPATH \
    EnvironmentConfigs__Environments__1__DestinationUsername=$ENV1_DESTINATIONUSERNAME \
    EnvironmentConfigs__Environments__1__DestinationPassword=$ENV1_DESTINATIONPASSWORD \
    EnvironmentConfigs__Environments__1__FileExtensions__0=$ENV1_FILEEXT0 \
    EnvironmentConfigs__Environments__2__InputPath=$ENV2_INPUTPATH \
    EnvironmentConfigs__Environments__2__Flag=$ENV2_FLAG \
    EnvironmentConfigs__Environments__2__OutputType=$ENV2_OUTPUTTYPE \
    EnvironmentConfigs__Environments__2__OutputCommand=$ENV2_OUTPUTCOMMAND \
    EnvironmentConfigs__Environments__2__OutputPath=$ENV2_OUTPUTPATH \
    EnvironmentConfigs__Environments__2__DestinationUsername=$ENV2_DESTINATIONUSERNAME \
    EnvironmentConfigs__Environments__2__DestinationPassword=$ENV2_DESTINATIONPASSWORD \
    EnvironmentConfigs__Environments__2__FileExtensions__0=$ENV2_FILEEXT0 \
	LoggingConfig__IngestUrl=$LOGGING_INGEST_URL \
	LoggingConfig__ApiKey=$LOGGING_API_KEY
	
RUN apt update && apt install -y openssh-client && apt install -y sshpass && rm -rf /var/lib/apt/lists/*

ENTRYPOINT ["dotnet", "Sureserve.Dataflows.Router.dll"]