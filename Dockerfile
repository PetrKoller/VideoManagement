FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/PowerTrainer.VideoManagement/PowerTrainer.VideoManagement.csproj .
COPY Directory.Build.props .
RUN dotnet restore
COPY src/PowerTrainer.VideoManagement .
RUN dotnet publish "PowerTrainer.VideoManagement.csproj" -c Release -o /publish /p:UseAppHost=false
RUN dotnet dev-certs https

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /publish .
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/
EXPOSE 80 443
ENTRYPOINT ["/bin/sh", "-c", "echo 'Waiting for other services to start up' && sleep 15 && dotnet PowerTrainer.VideoManagement.dll"]
