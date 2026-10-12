FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/KTGKMD.Web.Host/KTGKMD.Web.Host.csproj", "src/KTGKMD.Web.Host/"]
COPY ["src/KTGKMD.Web.Core/KTGKMD.Web.Core.csproj", "src/KTGKMD.Web.Core/"]
COPY ["src/KTGKMD.Application/KTGKMD.Application.csproj", "src/KTGKMD.Application/"]
COPY ["src/KTGKMD.Core/KTGKMD.Core.csproj", "src/KTGKMD.Core/"]
COPY ["src/KTGKMD.EntityFrameworkCore/KTGKMD.EntityFrameworkCore.csproj", "src/KTGKMD.EntityFrameworkCore/"]
WORKDIR "/src/src/KTGKMD.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/KTGKMD.Web.Host", "src/KTGKMD.Web.Host"]
COPY ["src/KTGKMD.Web.Core", "src/KTGKMD.Web.Core"]
COPY ["src/KTGKMD.Application", "src/KTGKMD.Application"]
COPY ["src/KTGKMD.Core", "src/KTGKMD.Core"]
COPY ["src/KTGKMD.EntityFrameworkCore", "src/KTGKMD.EntityFrameworkCore"]
WORKDIR "/src/src/KTGKMD.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "KTGKMD.Web.Host.dll"]
