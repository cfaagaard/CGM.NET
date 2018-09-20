#FROM microsoft/dotnet:sdk AS build-env
#WORKDIR /app
#
## Copy csproj and restore as distinct layers
#COPY *.csproj ./
#RUN dotnet restore
#
## Copy everything else and build
#COPY . ./
#RUN dotnet publish -c Release -o out
#
## Build runtime image
#FROM microsoft/dotnet:aspnetcore-runtime
#WORKDIR /app
#COPY --from=build-env /app/out .
#ENTRYPOINT ["dotnet", "CGM.Web.dll"]

#dotnet publish -c Release -o publish
#docker build -t cgmweb .
#docker run -d -p 8080:80 --name mycgm cgmweb
#docker stop mycgm
#docker rm mycgm
#docker inspect --format='{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' cgmweb
#docker ps -a
#docker inspect mycgm

#  dotnet publish -r win8-arm
# https://dzone.com/articles/running-aspnet-core-2-apps-on-windows-10-iot-core

#netsh advfirewall firewall add rule name=”ASP.NET Core Web Server port”  dir=in action=allow protocol=TCP localport=5000 
FROM microsoft/dotnet:sdk AS build-env
FROM microsoft/dotnet:aspnetcore-runtime

COPY ./publish /publish
WORKDIR /publish
ENTRYPOINT ["dotnet", "CGM.Web.dll"]