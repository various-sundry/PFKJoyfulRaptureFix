#!/bin/bash
set -euxo pipefail

dotnet publish -c Release -o bin/JoyfulRaptureFix

VERSION=$(git tag --points-at HEAD | grep '^v[0-9]')
ZIPFILE="JoyfulRaptureFix-$VERSION.zip"

cd bin
rm -f "$ZIPFILE"
zip -r "$ZIPFILE" JoyfulRaptureFix
