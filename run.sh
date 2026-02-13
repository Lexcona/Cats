#!/bin/sh

clear
set -e

cd SimpleImageWidgit

dotnet build --no-incremental

cd bin/Debug/net9.0

clear
./SimpleImageWidgit

