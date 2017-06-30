FROM microsoft/dotnet:1.1.2-runtime
ENTRYPOINT ["dotnet", "Lykke.Service.WithdrawalRequestScheduler.dll"]
ARG source=.
WORKDIR /app
COPY $source .
