#!/bin/sh

dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true -p:SelfContained=true
dotnet publish -c Release -r win-x64 -p:PublishSingleFile=true -p:SelfContained=true
