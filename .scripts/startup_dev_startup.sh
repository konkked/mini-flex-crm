#!/bin/bash

# Set environment variables for the backend
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=miniflexcrm;Username=admin;Password=admin"

# Start the PostgreSQL database
echo "Starting PostgreSQL database..."
pg_ctl -D /usr/local/var/postgres start

# Start the backend API
echo "Starting backend API..."
cd /Users/charleskeyser/repos/MiniFlexCRM/backend/api/MiniFlexCrmApi
dotnet run &

# Start the frontend application
echo "Starting frontend application..."
cd /Users/charleskeyser/repos/MiniFlexCRM/frontend
yarn start &

echo "All services started successfully."