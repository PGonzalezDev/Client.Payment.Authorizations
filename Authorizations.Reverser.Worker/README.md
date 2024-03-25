# How to use Authorizations.Reverser.Worker

## Pre-Requirements
### _Install Visual Studio Community 2022_
If you don't have it installed any version of Visual Studio 2022, download & install [Visual Studio Community 2022](https://c2rsetup.officeapps.live.com/c2r/downloadVS.aspx?sku=community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030:3e3b259ce50a4c5d928825e3b52b891b).
### _Install Docker_
If you don't have it installed yet, download and install [Docker](https://www.docker.com/get-started/) on your operating system.
### Install .NET 7.0 SKD
If you don't have it installed yet, download and install [.NET 7.0 SDK](https://dotnet.microsoft.com/es-es/download/dotnet/7.0)

## Docker
To Build an image of _Authorizations.Reverser.Worker_, run the follow command in Package Manager Console in Visual Studio:
`docker build -t authorizations.reverser.worker .` (don't forget the space + final dot in this command)

To run the container run the follow command in in Package Manager Console in Visual Studio:
`docker run authorizations.reverser.worker`