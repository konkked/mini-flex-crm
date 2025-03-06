# Build the frontend image
cd frontend
docker build -t konkked/mfcrm-frontend:latest -f Dockerfile .
docker push konkked/mfcrm-frontend:latest
cd ..

# Build the backend image
cd backend
docker build -t konkked/mfcrm-backend:latest -f Dockerfile .
docker push konkked/mfcrm-backend:latest
cd ..

# Build the database image (if needed)
cd db
docker build -t konkked/mfcrm-database:latest -f Dockerfile .
docker push konkked/mfcrm-database:latest
cd ..

# Build the database image (if needed)
cd cache
docker build -t konkked/mfcrm-cache:latest -f Dockerfile .
docker push konkked/mfcrm-cache:latest
cd ..
