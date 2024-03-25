# How to use Client.Payment.Authorization

## Pre-Requirements
### _Install Visual Studio Community 2022_
If you don't have it installed any version of Visual Studio 2022, download & install [Visual Studio Community 2022](https://c2rsetup.officeapps.live.com/c2r/downloadVS.aspx?sku=community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030:3e3b259ce50a4c5d928825e3b52b891b).
### _Install Docker_
If you don't have it installed yet, download and install [Docker](https://www.docker.com/get-started/) on your operating system.
### Install .NET 7.0 SKD
If you don't have it installed yet, download and install [.NET 7.0 SDK](https://dotnet.microsoft.com/es-es/download/dotnet/7.0)

## RabbitMQ
To use this solution, is needed to install an docker image of RabbitMQ manager.
For windows OS: Open a Command Prompt in Administrator Mode. Execute the follow command to install & run an image of RabbitMQ Manager: `docker run -d --hostname my-rabbit --name some-rabbit -p 15672:15672 -p 5672:5672 rabbitmq:3-management`.
When container named 'some-rabbit' are running with green icon in docker desktop, verify in any browser the url: `http://localhost:15672/#/` shows a RabbitMQ Managment Login page, use username: _guest_ & password: _guest_ to log in manager application.

## Docker
To Build an image of _Client.Payment.Authorization_, run the follow command in Package Manager Console in Visual Studio:
`docker build -t client.payment.authorizations .` (don't forget the space + final dot in this command)

To run the container run the follow command in in Package Manager Console in Visual Studio:
`docker run -p 5173:5173 client.payment.authorizations`

## Endpoint
- url: [GET] `/healtly`
	- Response: `Health`

- url: [POST] `/create-authorization`
	- Body:
	```
	{
		"amonut": 2500.00,
		"cliendId": "c4c24254-b36d-490d-84d7-32d26556dc64",
		"authorizationType": 1
	}
	```
	- Response:
	```
	{
		"id": "aba0a04b-b620-4175-9d82-7e542d376f3b",
		"amount": 2500.00,
		"cliendId": "c4c24254-b36d-490d-84d7-32d26556dc64",
		"createdDate": "2024-03-10T21:23:42.4834935-03:00",
		"authorizationType": "PAYMENT",
		"approved": true
	}
	```

- url: [POST] `/confirm-authorization`
	- Body:
	```
	{
		"AuthorizationId": "aba0a04b-b620-4175-9d82-7e542d376f3b",
		"Confirm": true
	}
	```
	- Response:
	```
	{
		"authorizationId": "bc72cfc4-5f0d-461c-a6f2-36fa564a0c0e",
		"confirmed": true,
		"resultCode": 0,
		"errorMsg": null
	}
	```