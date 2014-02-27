Remoting
========

Client requests data from the server whilst sending her credentials along with the request. Server authenticates the
request and responds by sending data from a database file . Simple and easy. I have created a custom sink that intercepts
the communication between server and client to add a little encryption and authentication of my own.
