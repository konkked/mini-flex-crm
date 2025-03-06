#!/bin/bash

# Exit on error
set -e

# Variables
NAMESPACE="miniflexcrm"
RELEASE_NAME="miniflexcrm"
VALUES_FILE="./values/aws-values.yaml"

echo "Deploying $RELEASE_NAME to EKS in namespace $NAMESPACE..."
kubectl get namespace "$NAMESPACE" > /dev/null 2>&1 || kubectl create namespace "$NAMESPACE"
helm lint .
helm upgrade --install "$RELEASE_NAME" . \
  -n "$NAMESPACE" \
  -f "$VALUES_FILE"

echo "Deployment to EKS completed!"
echo "Check the ALB DNS with: kubectl get ingress -n $NAMESPACE"
echo "Then run './setup_eks_prereqs.sh --update-dns' to update Route 53."