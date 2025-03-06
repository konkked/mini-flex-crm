#!/bin/bash

# Generate a secure 256-bit (32-byte) Base64-encoded key
JWT_SECRET=$(openssl rand -base64 32)

# Create a Kubernetes secret with the generated JWT key
kubectl create secret generic jwt-secret --from-literal=jwt-key=$JWT_SECRET

# Verify the secret has been created
kubectl get secret jwt-secret

REDIS_PASSWORD=$(openssl rand -base64 32)

# Create a Kubernetes secret with the generated Redis password
kubectl create secret generic redis-secret --from-literal=redis-password=$REDIS_PASSWORD

# Verify the secret has been created
kubectl get secret redis-secret