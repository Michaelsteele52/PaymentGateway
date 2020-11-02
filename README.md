# PaymentGateway

Technology Used:
- .Net Core 3.1
- Mock Bank -> Wiremock
- Docker

Add your dev certificate to the right directory using:
dotnet dev-certs https -ep c:\\temp\PaymentGateway.pfx -p password

Can be run using:
 docker-compose up -d --build

Can navigate to the swagger UI via:
https://localhost:4431/Index.html

This allows the API to be tested.

To Do:
- Build pipeline
- Docker-compose
- Wiremock response files

