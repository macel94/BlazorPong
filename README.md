# BlazorPong
BlazorPong Project for one of my University Exams + Thesis

## No javascript allowed(except for the JSInterop to listen to an event not supported by blazor or to log in browser console).
Trying Blazor for the first time, please be patient.
Devops integrated, deployed as self-contained, 32 bit because i chose the free tier plan.
Using SignalR Core

**Server-Side Blazor**

Build status: [![Build Status](https://francesco-belacca.visualstudio.com/BlazorPong/_apis/build/status/BlazorPongServer.CI?branchName=master)](https://francesco-belacca.visualstudio.com/BlazorPong/_build/latest?definitionId=10&branchName=master)

Deploy status: ![Deployment Status](https://francesco-belacca.vsrm.visualstudio.com/_apis/public/Release/badge/ce5f42c0-8688-4de0-b486-36c5cebb3c0b/1/1)

**Client-Side(Wasm) .NET Core Hosted Blazor**

Build status: [![Build Status](https://francesco-belacca.visualstudio.com/BlazorPong/_apis/build/status/BlazorPongWasm.CI?branchName=master)](https://francesco-belacca.visualstudio.com/BlazorPong/_build/latest?definitionId=9&branchName=master)

Deploy status: ![Deployment Status](https://francesco-belacca.vsrm.visualstudio.com/_apis/public/Release/badge/ce5f42c0-8688-4de0-b486-36c5cebb3c0b/2/2)

**Docker Images BuildnPush**
Deploy status: ![Deployment Status](https://vsrm.dev.azure.com/francesco-belacca/_apis/public/Release/badge/ce5f42c0-8688-4de0-b486-36c5cebb3c0b/3/3)

# Websites
Server --> https://blazorpong-dev-as.azurewebsites.net/
Wasm --> https://blazorpongwasm.azurewebsites.net/
Wasm with Docker and NGINX --> https://www.dockerblazorpongwasm.cloud/

# Credits
Inspired by https://github.com/coffeeboyds/Demos/tree/master/PongSignalR
https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-3.1
https://codeburst.io/load-balancing-an-asp-net-core-web-app-using-nginx-and-docker-66753eb08204
https://nginx.org/en/docs
https://www.c-sharpcorner.com/article/fun-with-docker-compose-using-net-core-and-nginx/
https://github.com/jongio/BlazorDocker
https://medium.com/@dbillinghamuk/certbot-certificate-verification-through-nginx-container-710c299ec549

# DockerHub
https://hub.docker.com/u/macel94

# Instructions
psw example: asdqwertyu1234QQ123eqw12

--CAREFUL! USE POWERSHELL
--CLEAN EVERYTHING
dotnet dev-certs https --clean

--GENERATE CERT
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\aspnetapp.pfx -p asdqwertyu1234QQ123eqw12
dotnet dev-certs https --trust

--LAUNCH http only
docker run -it --rm -p 80:80 --name aspnetcore_sample mcr.microsoft.com/dotnet/core/samples:aspnetapp
docker run -it --rm -p 8000:80 --name blazorpongwasmserver macel94/blazorpongwasmserver


--Local dev LAUNCH https
docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="asdqwertyu1234QQ123eqw12" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v $env:USERPROFILE\.aspnet\https:/https/ mcr.microsoft.com/dotnet/core/samples:aspnetapp

--Local dev LAUNCH https blazorpongwasmserver
docker run --rm -it -p 8000:80 -p 8001:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="asdqwertyu1234QQ123eqw12" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v $env:USERPROFILE\.aspnet\https:/https/ macel94/blazorpongwasmserver
docker run --rm -it -p 80:80 -p 443:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=443 -e ASPNETCORE_Kestrel__Certificates__Default__Password="" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/var/www/dockerblazorpongwasm.cloud/ -v %USERPROFILE%\.aspnet\https:/https/ macel94/blazorpongwasmserver
docker run --rm -it -p 80:80 -p 443:443 -e ASPNETCORE_URLS="https://+;http://+" -e ASPNETCORE_HTTPS_PORT=443 -e ASPNETCORE_Kestrel__Certificates__Default__Password="" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/mycertificatename.pfx -v %USERPROFILE%\.aspnet\https:/https/ aspnetcore-react:latest


--ssh
https://stackoverflow.com/questions/42863913/key-load-public-invalid-format

--certbot
sudo snap install core; sudo snap refresh core
sudo snap install --classic certbot
sudo certbot certonly --webroot --agree-tos --email email@domain.it address -d dockerblazorpongwasm.cloud -w /var/www/dockerblazorpongwasm.cloud/

**DockerCompose**
```powershell

cd BlazorPong

docker-compose -f "Docker-Compose.yml" up -d --build

docker-compose -f "Docker-Compose.yml" down --remove-orphans

```

**PROD DEPLOY**
docker context create remote --docker "host=ssh://root@HOST"
CD source\repos\MACEL94\BlazorPong\BlazorPongWasm
docker-compose -c remote -f Docker-Compose.PROD.yml up

If you get paramiko.ssh_exception.PasswordRequiredException: private key file is encrypted
and you can't use ssh-agent on windows:
ssh-agent -s --> unable to start ssh-agent service, error :1058

Check for OpenSSH Authentication Agent service, mine was disabled.

It also needs to be updated so you can ssh-add correctly: https://github.com/PowerShell/Win32-OpenSSH/issues/1263#issuecomment-499542944

...still no luck. i ended up using scp
scp .\Docker-Compose.PROD.yml root@host:app 

Then ssh on machine as root
cd app
docker-compose -f "Docker-Compose.PROD.yml" down --remove-orphans
docker-compose -f "Docker-Compose.PROD.yml" up -d --build

sudo certbot certonly --webroot -w /root/certs-data/ -d dockerblazorpongwasm.cloud -d www.dockerblazorpongwasm.cloud

Save certificates to localhost
scp -r user@your.server.example.com:/path/to/foo /home/user/Desktop/

To generate localhost SSL
Use this Dockerfile

```Docker
# we use the tiny alpine linux as base
FROM alpine

# install openssl
RUN apk update && \
  apk add --no-cache openssl && \
  rm -rf "/var/cache/apk/*"

# create and set mount volume
WORKDIR /openssl-certs
VOLUME  /openssl-certs

ENTRYPOINT ["openssl"]
```

to use it 
```bash
docker build -t my-openssl:latest .

docker run -it --rm -v "C:/some/path:/openssl-certs" my-openssl
req -newkey rsa:2048 -keyout privkey.pem -x509 -days 365 -out fullchain.pem

req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout privkey.pem -out fullchain.pem
x509 -outform pem -in fullchain.pem -out RootCA.crt
```
