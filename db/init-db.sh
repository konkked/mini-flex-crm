#!/bin/bash
set -e

# Start the PostgreSQL server in the background
docker-entrypoint.sh postgres &

# Wait for the PostgreSQL server to start
until pg_isready --username "$POSTGRES_USER" --dbname "$POSTGRES_DB"; do
    echo "Waiting for PostgreSQL to start..."
    sleep 2
done

# Find all .sql files and execute them
for sql_file in $(find /docker-entrypoint-initdb.d -type f -name "*.sql" | sort); do
    echo "Executing $sql_file..."
    psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" -f "$sql_file"
done

# Keep the PostgreSQL server running in the foreground
wait