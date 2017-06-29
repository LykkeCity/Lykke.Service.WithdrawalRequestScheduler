FROM microsoft/dotnet:1.1.2-runtime
ENTRYPOINT ["dotnet", "WithdrawalRequestScheduler.Job.dll"]
ARG source=.
WORKDIR /app
COPY $source .
