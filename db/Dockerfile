FROM postgres:latest

# don't ever do this in real life, still working on this will add.
ENV POSTGRES_USER=admin
ENV POSTGRES_PASSWORD=admin
ENV POSTGRES_DB=miniflexcrm

# Copy init scripts to auto-run at container startup
COPY ./init /docker-entrypoint-initdb.d/
COPY ./init-db.sh /docker-entrypoint-initdb.d/init-db.sh

# Make the entrypoint script executable
RUN chmod +x /docker-entrypoint-initdb.d/init-db.sh

# Expose PostgreSQL port
EXPOSE 5432

# Set the custom entrypoint script
ENTRYPOINT ["/docker-entrypoint-initdb.d/init-db.sh"]
# Run the default PostgreSQL entrypoint script
# ENTRYPOINT ["docker-entrypoint.sh"]

# # Start PostgreSQL
# CMD ["postgres"]