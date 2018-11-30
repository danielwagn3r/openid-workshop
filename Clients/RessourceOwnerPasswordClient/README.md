# .Net Core Ressource Owner Password Client Calling an API

This sample demonstrates how to make secure calls to an API after authenticating a user with Auth0. The calls to the API are made with the user's `access_token`.

## Getting Started

If you haven't already done so, [sign up](https://auth0.com) for your free Auth0 account and create a new client in the [dashboard](https://manage.auth0.com). Find the **domain** and **client ID** from the settings area and add the URL for your application to the **Allowed Callback URLs** box. If you are serving the application with the provided `serve` library, that URL is `http://localhost:3000`.

If you haven't already done so, create a new API in the [APIs section](https://manage.auth0.com/#/apis) and provide an identifier for it.

Clone the repo or download it from the JavaScript quickstart page in Auth0's documentation.

## Set the Client ID, Domain, User Credentials and API URL

After you clone the repo directly from Github, edit the `appsettings.json` file and provide the **client ID**, **client secret**, **domain**, **username** and **password** there.

Also provide the identifier for the `CalculatorApi` you created in the Auth0 dashboard as your `Address`.

## Run the Application

To build and run your the application start the api and execute the following command:

```bash
dotnet run
```

The application will authorize at your Auth0 tenant, get an access token, call your CalculatorApi and perform a token refresh.

