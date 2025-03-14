# !/bin/bash
pushd db

docker stop miniflexcrm-db || true
docker rm miniflexcrm-db || true

docker build -t miniflexcrm-db:latest .

docker run -d --name miniflexcrm-db -p 5432:5432 miniflexcrm-db:latest

popd