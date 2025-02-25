FROM postgres:latest

# dont ever do this irl, still working on this will add.
ENV POSTGRES_USER=admin
ENV POSTGRES_PASSWORD=admin
ENV POSTGRES_DB=miniflexcrm

#concat all sql files in the init folder into one file
RUN ./../../scripts/concat-sql.sh init init.sql

# Copy init scripts to auto-run at container startup
COPY ./init.sql /docker-entrypoint-initdb.d/init.sql

# Expose PostgreSQL port
EXPOSE 5432

