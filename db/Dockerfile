FROM postgres:latest
ENV POSTGRES_USER=admin
ENV POSTGRES_PASSWORD=admin
ENV POSTGRES_DB=miniflexcrm

# Copy init scripts to auto-run at container startup
COPY ./init /docker-entrypoint-initdb.d/
