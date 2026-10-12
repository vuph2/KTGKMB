FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN ln -s ~/.dotnet/tools/libman /usr/local/bin/libman

WORKDIR /src
COPY ["src/KTGKMD.Web.Mvc/KTGKMD.Web.Mvc.csproj", "src/KTGKMD.Web.Mvc/"]
COPY ["src/KTGKMD.Web.Core/KTGKMD.Web.Core.csproj", "src/KTGKMD.Web.Core/"]
COPY ["src/KTGKMD.Application/KTGKMD.Application.csproj", "src/KTGKMD.Application/"]
COPY ["src/KTGKMD.Core/KTGKMD.Core.csproj", "src/KTGKMD.Core/"]
COPY ["src/KTGKMD.EntityFrameworkCore/KTGKMD.EntityFrameworkCore.csproj", "src/KTGKMD.EntityFrameworkCore/"]
WORKDIR "/src/src/KTGKMD.Web.Mvc"
RUN dotnet restore 

WORKDIR /src
COPY ["src/KTGKMD.Web.Mvc", "src/KTGKMD.Web.Mvc"]
COPY ["src/KTGKMD.Web.Core", "src/KTGKMD.Web.Core"]
COPY ["src/KTGKMD.Application", "src/KTGKMD.Application"]
COPY ["src/KTGKMD.Core", "src/KTGKMD.Core"]
COPY ["src/KTGKMD.EntityFrameworkCore", "src/KTGKMD.EntityFrameworkCore"]
WORKDIR "/src/src/KTGKMD.Web.Mvc"
RUN libman restore
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "KTGKMD.Web.Mvc.dll"]
