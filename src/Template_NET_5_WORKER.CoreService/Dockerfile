#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["src/Template_NET_5_WORKER.CoreService/Template_NET_5_WORKER.CoreService.csproj", "src/Template_NET_5_WORKER.CoreService/"]
RUN dotnet restore "src/Template_NET_5_WORKER.CoreService/Template_NET_5_WORKER.CoreService.csproj"
COPY . .
WORKDIR "/src/src/Template_NET_5_WORKER.CoreService"
RUN dotnet build "Template_NET_5_WORKER.CoreService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Template_NET_5_WORKER.CoreService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Template_NET_5_WORKER.CoreService.dll"]
