#!/bin/bash
# Exit on error
set -e

# Variables
NAMESPACE="miniflexcrm"
RELEASE_NAME="miniflexcrm"
VALUES_FILE="./deploy/values/local-values.yaml"
FRONTEND_DOMAIN="frontend.mfcrm-base.local"
BACKEND_DOMAIN="backend.mfcrm-base.local"
HOSTS_FILE="/etc/hosts"

echo "Deploying $RELEASE_NAME to Minikube in namespace $NAMESPACE..."
minikube status > /dev/null 2>&1 || minikube start
minikube addons enable ingress
kubectl get namespace "$NAMESPACE" > /dev/null 2>&1 || kubectl create namespace "$NAMESPACE"
helm lint .
helm upgrade --install "$RELEASE_NAME" . \
  -n "$NAMESPACE" \
  -f "$VALUES_FILE"

MINIKUBE_IP=$(minikube ip)
if [ -z "$MINIKUBE_IP" ]; then
  echo "Error: Could not retrieve Minikube IP. Exiting."
  exit 1
fi

echo "Minikube IP: $MINIKUBE_IP"
update_hosts() {
  local domain=$1
  local ip=$2

  # Check if the domain already exists in /etc/hosts
  if grep -q "$domain" "$HOSTS_FILE"; then
    # Update existing entry if IP differs
    if ! grep -q "$ip\s\+$domain" "$HOSTS_FILE"; then
      echo "Updating $domain in $HOSTS_FILE to point to $ip..."
      sudo sed -i "/$domain/d" "$HOSTS_FILE"  # Remove old entry
      echo "$ip $domain" | sudo tee -a "$HOSTS_FILE" > /dev/null
    else
      echo "$domain already points to $ip in $HOSTS_FILE. No update needed."
    fi
  else
    # Add new entry
    echo "Adding $domain to $HOSTS_FILE with IP $ip..."
    echo "$ip $domain" | sudo tee -a "$HOSTS_FILE" > /dev/null
  fi
}

# Update /etc/hosts with domain names
echo "Updating $HOSTS_FILE for local resolution..."
update_hosts "$FRONTEND_DOMAIN" "$MINIKUBE_IP"
update_hosts "$BACKEND_DOMAIN" "$MINIKUBE_IP"

echo "Deployment to Minikube completed!"
echo "Access the application at:"
echo "  - Frontend: http://$FRONTEND_DOMAIN"
echo "  - Backend: http://$BACKEND_DOMAIN"