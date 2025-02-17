# Build the frontend image
docker build -t konkked/mfcrm-frontend:latest -f /Users/charleskeyser/repos/MiniFlexCRM/frontend/Dockerfile .
docker push konkked/mfcrm-frontend:latest

# Build the backend image
docker build -t konkked/mfcrm-backend:latest -f /Users/charleskeyser/repos/MiniFlexCRM/backend/Dockerfile .
docker push konkked/mfcrm-backend:latest

# Build the database image (if needed)
docker build -t konkked/mfcrm-database:latest -f /Users/charleskeyser/repos/MiniFlexCRM/database/Dockerfile .
docker push konkked/mfcrm-database:latest