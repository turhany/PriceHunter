#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PriceHunter.Consumer.Parser/PriceHunter.Consumer.Parser.csproj", "PriceHunter.Consumer.Parser/"]
COPY ["PriceHunter.Common/PriceHunter.Common.csproj", "PriceHunter.Common/"]
COPY ["PriceHunter.Resources/PriceHunter.Resources.csproj", "PriceHunter.Resources/"]
COPY ["PriceHunter.Contract/PriceHunter.Contract.csproj", "PriceHunter.Contract/"]
COPY ["PriceHunter.Model/PriceHunter.Model.csproj", "PriceHunter.Model/"]
COPY ["PriceHunter.Data.MongoDB/PriceHunter.Data.MongoDB.csproj", "PriceHunter.Data.MongoDB/"]
COPY ["PriceHunter.Container/PriceHunter.Container.csproj", "PriceHunter.Container/"]
COPY ["PriceHunter.Data/PriceHunter.Data.csproj", "PriceHunter.Data/"]
COPY ["PriceHunter.Business/PriceHunter.Business.csproj", "PriceHunter.Business/"]
RUN dotnet restore "PriceHunter.Consumer.Parser/PriceHunter.Consumer.Parser.csproj"
COPY . .
WORKDIR "/src/PriceHunter.Consumer.Parser"
RUN dotnet build "PriceHunter.Consumer.Parser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PriceHunter.Consumer.Parser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PriceHunter.Consumer.Parser.dll"]