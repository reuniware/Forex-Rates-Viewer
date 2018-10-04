# Forex-Rates-Viewer
Send rates from Metatrader 5 to Dot Net C# Application !

Compile the .MQ5 file in Metatrader and attach it to a financial instrument (eg. EUR/USD).

In your Metatrader 5 platform, add "http://127.0.0.1" in the list of URL that are authorized for Webrequests (in Options/Expert Consultants).

Unzip the .Zip file and open the project in Visual Studio 2017, then run it with administrator privileges (because it is a HTTP server that needs to open the 9090 port for receiving data from Metatrader 5 through HTTP POST).

The Metatrader 5 platform will then call the URL "http://127.0.0.1:9090/send_data/" on each tick, and will send all the forex rates to the C# HTTP Server.

In C#, you can then code any algorithm for real-time analysis of forex rates (and all other financial instruments).

If you need help of need me for a project, feel free to contact me at investdatasystems@yahoo.com.

