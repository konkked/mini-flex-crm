# Build the frontend image
docker build -t konkked/frontend:latest -f /Users/charleskeyser/repos/MiniFlexCRM/frontend/Dockerfile .
docker push your-docker-repo/frontend:latest

# Build the backend image
docker build -t konkked/backend:latest -f /Users/charleskeyser/repos/MiniFlexCRM/backend/Dockerfile .
docker push konkked/backend:latest

# Build the database image (if needed)
docker build -t konkked/database:latest -f /Users/charleskeyser/repos/MiniFlexCRM/database/Dockerfile .
docker push konkked/database:latest