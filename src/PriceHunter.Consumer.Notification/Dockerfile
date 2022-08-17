#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PriceHunter.Consumer.Notification/PriceHunter.Consumer.Notification.csproj", "PriceHunter.Consumer.Notification/"]
COPY ["PriceHunter.Common/PriceHunter.Common.csproj", "PriceHunter.Common/"]
COPY ["PriceHunter.Resources/PriceHunter.Resources.csproj", "PriceHunter.Resources/"]
COPY ["PriceHunter.Container/PriceHunter.Container.csproj", "PriceHunter.Container/"]
COPY ["PriceHunter.Business/PriceHunter.Business.csproj", "PriceHunter.Business/"]
COPY ["PriceHunter.Contract/PriceHunter.Contract.csproj", "PriceHunter.Contract/"]
COPY ["PriceHunter.Model/PriceHunter.Model.csproj", "PriceHunter.Model/"]
COPY ["PriceHunter.Data.MongoDB/PriceHunter.Data.MongoDB.csproj", "PriceHunter.Data.MongoDB/"]
COPY ["PriceHunter.Data/PriceHunter.Data.csproj", "PriceHunter.Data/"]
COPY ["PriceHunter.Notification.Email/PriceHunter.Notification.Email.csproj", "PriceHunter.Notification.Email/"]
COPY ["PriceHunter.Notification/PriceHunter.Notification.csproj", "PriceHunter.Notification/"]
COPY ["PriceHunter.Notification.MobilePush/PriceHunter.Notification.MobilePush.csproj", "PriceHunter.Notification.MobilePush/"]
COPY ["PriceHunter.Notification.Sms/PriceHunter.Notification.Sms.csproj", "PriceHunter.Notification.Sms/"]
COPY ["PriceHunter.Parser.Alibaba/PriceHunter.Parser.Alibaba.csproj", "PriceHunter.Parser.Alibaba/"]
COPY ["PriceHunter.Parser/PriceHunter.Parser.csproj", "PriceHunter.Parser/"]
COPY ["PriceHunter.Parser.AliExpress/PriceHunter.Parser.AliExpress.csproj", "PriceHunter.Parser.AliExpress/"]
COPY ["PriceHunter.Parser.Amazon/PriceHunter.Parser.Amazon.csproj", "PriceHunter.Parser.Amazon/"]
RUN dotnet restore "PriceHunter.Consumer.Notification/PriceHunter.Consumer.Notification.csproj"
COPY . .
WORKDIR "/src/PriceHunter.Consumer.Notification"
RUN dotnet build "PriceHunter.Consumer.Notification.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PriceHunter.Consumer.Notification.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PriceHunter.Consumer.Notification.dll"]