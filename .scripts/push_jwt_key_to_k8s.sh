#!/bin/bash

# Generate a secure 256-bit (32-byte) Base64-encoded key
JWT_SECRET=$(openssl rand -base64 32)

# Create a Kubernetes secret with the generated JWT key
kubectl create secret generic miniflexcrm-jwt-secret --from-literal=jwt-key=$JWT_SECRET

# Verify the secret has been created
kubectl get secret miniflexcrm-jwt-secret