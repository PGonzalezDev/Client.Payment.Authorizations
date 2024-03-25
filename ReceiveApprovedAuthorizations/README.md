# How to use Receiver.Approved.Auth.Console

## Pre-Requirements
### _Install Visual Studio Community 2022_
If you don't have it installed any version of Visual Studio 2022, download & install [Visual Studio Community 2022](https://c2rsetup.officeapps.live.com/c2r/downloadVS.aspx?sku=community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030:3e3b259ce50a4c5d928825e3b52b891b).
### _Install Docker_
If you don't have it installed yet, download and install [Docker](https://www.docker.com/get-started/) on your operating system.
### Install .NET 7.0 SKD
If you don't have it installed yet, download and install [.NET 7.0 SDK](https://dotnet.microsoft.com/es-es/download/dotnet/7.0)

## RabbitMQ
To use this console, is needed to install an docker image of RabbitMQ manager.
For windows OS: Open a Command Prompt in Administrator Mode. Execute the follow command to install & run an image of RabbitMQ Manager: `docker run -d --hostname my-rabbit --name some-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management`.
When container named 'some-rabbit' are running with green icon in docker desktop, verify in any browser the url: `http://localhost:15672/#/` shows a RabbitMQ Managment Login page, use username: _guest_ & password: _guest_ to log in manager application.

## Run Console
To run this console, open Visual Studio in your own window, right click in 'Receiver.Approved.Auth.Console' and click on 'Set starup project'.
After that, click in green 'play' button in the top of Visual Studio IDE and get the console run in background.