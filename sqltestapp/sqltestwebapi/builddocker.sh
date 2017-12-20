#!/bin/bash
dotnet build && dotnet publish -c Releae -o ./obj/Docker/publish

docker rmi myimage -f
docker build -t myimage .
docker run -p 22354:80 myimage
