# Use official Redis image as base
FROM redis:7.0-alpine

# Expose Redis port (default 6379)
EXPOSE 6379

# Define runtime environment variable
ENV REDIS_PASSWORD=""

# Command to run Redis, conditionally applying password
CMD ["sh", "-c", "if [ -z \"$REDIS_PASSWORD\" ]; then redis-server; else redis-server --requirepass \"$REDIS_PASSWORD\"; fi"]