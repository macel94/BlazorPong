#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-focal AS base
WORKDIR /app
EXPOSE 80
#La espongo solo per local development, non verrà mappata nel docker compose e non verrà usata da nginx, andrò di SSL offload
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-focal AS build
WORKDIR /src
COPY ["BlazorPongWasm/Server/BlazorPongWasm.Server.csproj", "BlazorPongWasm/Server/"]
COPY ["BlazorPong.BL/BlazorPong.BL.csproj", "BlazorPong.BL/"]
COPY ["BlazorPongWasm/Client/BlazorPongWasm.Client.csproj", "BlazorPongWasm/Client/"]
COPY ["BlazorPongComponentsLibrary/BlazorPong.Components.csproj", "BlazorPongComponentsLibrary/"]
RUN dotnet restore "BlazorPongWasm/Server/BlazorPongWasm.Server.csproj"
COPY . .
WORKDIR "/src/BlazorPongWasm/Server"
RUN dotnet build "BlazorPongWasm.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlazorPongWasm.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlazorPongWasm.Server.dll"]