#!/bin/bash
if [ "$EUID" -ne 0 ]
  then echo "Please run as root"
else
 sudo dotnet run
fi
