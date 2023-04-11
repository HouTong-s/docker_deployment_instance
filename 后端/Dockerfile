#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CoreAPIs/CoreAPIs.csproj", "CoreAPIs/"]
RUN dotnet restore "CoreAPIs/CoreAPIs.csproj"
COPY . .
WORKDIR "/src/CoreAPIs"
RUN dotnet build "CoreAPIs.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CoreAPIs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir imgs
RUN mkdir export
COPY --from=publish /app/publish .
RUN sed -i "s@http://deb.debian.org@http://mirrors.aliyun.com@g" /etc/apt/sources.list
RUN cat /etc/apt/sources.list
RUN rm -Rf /var/lib/apt/lists/*
#要安装图形库libgdiplus，不然会出错
RUN apt-get update && apt-get install -y redis-server && apt-get install -y libgdiplus && apt-get install -y nginx
RUN sed -i 's|root /var/www/html;|root /app;|g' /etc/nginx/sites-available/default
RUN sed -i 's/^# requirepass foobared/requirepass 123456/' /etc/redis/redis.conf
EXPOSE 80 6379
#ENTRYPOINT ["dotnet" ,"CoreAPIs.dll"]
#nginx作为验证码图片服务器，redis作为缓存服务器
ENTRYPOINT dotnet CoreAPIs.dll & redis-server & nginx -g "daemon off;"