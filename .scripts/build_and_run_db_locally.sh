# !/bin/bash
pushd db

docker stop mfcrm-database || true
docker rm mfcrm-database || true

docker build -t konkked/mfcrm-database:latest .

docker run -d --name mfcrm-database -p 5432:5432 konkked/mfcrm-database:latest

popd