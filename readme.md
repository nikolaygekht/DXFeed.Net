# Intro

The library is a prototype to test and validate approach and architecture for of
direct access to dxfeed's CometD interface over websocket.

The reason behind of that is that the target platform is not supported
by any of provided dxFeed Client APIs. C# is used just because I consider it
the easiest way to prototype :-).

To learn about the structure of the API read [documentation](Doc/readme.md).

The solution consists of three main projects:

* API
* Unit and integration test for API 
* A demo example that shows typical use of the API

## Warranties

This is an experimental prototype project. 

The code is created solely to validate the hypothesis that you need nothing beyond an ordinary websocket transport to work with a CometD feed. 

The author provides no warranties of any kind. Again: I do not promise commercial usefulness, freedom from errors or whatever you may wish to expect from this code :-). Use this code at your own risk. 

The project is not paid or otherwise endorsed by dxFeed or any other company. 

## What do you need to play with that

1) .NET 6.0 SDK
2) You need an URL to dxFeed access point
3) You need to have access to dxFeed access token

If you are the client of dxFeed you know how to get 2 and 3. If you don't, please become their client first :-). 

