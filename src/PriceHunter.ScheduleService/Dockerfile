#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PriceHunter.ScheduleService/PriceHunter.ScheduleService.csproj", "PriceHunter.ScheduleService/"]
COPY ["PriceHunter.Common/PriceHunter.Common.csproj", "PriceHunter.Common/"]
COPY ["PriceHunter.Resources/PriceHunter.Resources.csproj", "PriceHunter.Resources/"]
COPY ["PriceHunter.Container/PriceHunter.Container.csproj", "PriceHunter.Container/"]
COPY ["PriceHunter.Business/PriceHunter.Business.csproj", "PriceHunter.Business/"]
COPY ["PriceHunter.Contract/PriceHunter.Contract.csproj", "PriceHunter.Contract/"]
COPY ["PriceHunter.Model/PriceHunter.Model.csproj", "PriceHunter.Model/"]
COPY ["PriceHunter.Data.MongoDB/PriceHunter.Data.MongoDB.csproj", "PriceHunter.Data.MongoDB/"]
COPY ["PriceHunter.Data/PriceHunter.Data.csproj", "PriceHunter.Data/"]
RUN dotnet restore "PriceHunter.ScheduleService/PriceHunter.ScheduleService.csproj"
COPY . .
WORKDIR "/src/PriceHunter.ScheduleService"
RUN dotnet build "PriceHunter.ScheduleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PriceHunter.ScheduleService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PriceHunter.ScheduleService.dll"]