#FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["PriceHunter.Web/PriceHunter.Web.csproj", "PriceHunter.Web/"]
#RUN dotnet restore "PriceHunter.Web/PriceHunter.Web.csproj"
#COPY ..
#WORKDIR "/src/PriceHunterWeb"
#RUN dotnet build "PriceHunter.Web.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "PriceHunter.Web.csproj" -c Release -o /app/publish
#
##FROM base AS final
##WORKDIR /app
##COPY --from=publish /app/publish .
##ENTRYPOINT ["dotnet", "PriceHunter.Web.dll"]
#
#FROM nginx:alpine AS final
#EXPOSE 80 
#COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html
##COPY /PriceHunterWeb/nginx.conf /etc/nginx/nginx.conf
#

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /PriceHunterWeb

COPY /web/PriceHunterWeb/ .
RUN dotnet restore "PriceHunter.Web.csproj"
RUN dotnet build "PriceHunter.Web.csproj" -c Release -o /build

FROM build-env AS publish
RUN dotnet publish "PriceHunter.Web.csproj" -c Release -o /publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

COPY --from=publish /publish/wwwroot /usr/local/webapp/nginx/html
COPY web/PriceHunterWeb/nginx.conf /etc/nginx/nginx.conf