# Build Stage
FROM node:20-alpine AS build

# Set working directory
WORKDIR /app

# Copy package.json and install dependencies
COPY frontend/package.json frontend/package-lock.json ./
RUN npm install

# Copy the project files
COPY frontend/. ./

# Build the React application
RUN npm run build

# Serve the application using Nginx
FROM nginx:alpine AS production

# Copy built files to Nginx directory
COPY --from=build /app/build /usr/share/nginx/html

# Expose port 80 for Nginx
EXPOSE 3000

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]
