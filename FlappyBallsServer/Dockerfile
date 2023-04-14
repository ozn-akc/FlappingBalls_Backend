FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

WORKDIR /flappy

EXPOSE 5000/tcp

COPY FlappyBallsServer.sln .
COPY DataMapper/DataMapper.csproj DataMapper/
COPY FlappyBallsServer/FlappyBallsServer.csproj FlappyBallsServer/
COPY SocketLibrary/SocketLibrary.csproj SocketLibrary/

RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS final-env

WORKDIR /flappy

COPY --from=build-env /flappy/out .
ENTRYPOINT ["dotnet", "FlappyBallsServer.dll"]