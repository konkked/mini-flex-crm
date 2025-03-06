#!/bin/bash
# Exit on error
set -e

# Variables (customize as needed)
CLUSTER_NAME="miniflexcrm-eks-cluster" # Replace with your EKS cluster name
REGION="us-west-1"                      
ACCOUNT_ID=$(aws sts get-caller-identity --query Account --output text)
DOMAIN="miniflexcrm.com"
HOSTED_ZONE_ID="YOUR_HOSTED_ZONE_ID"    # Replace with your Route 53 Hosted Zone ID

echo "Setting up EKS prerequisites for $CLUSTER_NAME in $REGION..."

# 1. Install AWS Load Balancer Controller
echo "Installing AWS Load Balancer Controller..."

# Add Helm repo if not already added
helm repo add eks https://aws.github.io/eks-charts
helm repo update

# Create IAM policy for ALB controller if not exists
POLICY_ARN="arn:aws:iam::$ACCOUNT_ID:policy/AWSLoadBalancerControllerIAMPolicy"
if ! aws iam get-policy --policy-arn "$POLICY_ARN" > /dev/null 2>&1; then
  curl -o alb-policy.json https://raw.githubusercontent.com/kubernetes-sigs/aws-load-balancer-controller/main/docs/install/iam_policy.json
  aws iam create-policy --policy-name AWSLoadBalancerControllerIAMPolicy --policy-document file://alb-policy.json
  rm alb-policy.json
fi

# Create service account and attach IAM role
eksctl create iamserviceaccount \
  --cluster="$CLUSTER_NAME" \
  --namespace=kube-system \
  --name=aws-load-balancer-controller \
  --attach-policy-arn="$POLICY_ARN" \
  --override-existing-serviceaccounts \
  --region="$REGION" \
  --approve

# Install the controller
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName="$CLUSTER_NAME" \
  --set serviceAccount.create=false \
  --set serviceAccount.name=aws-load-balancer-controller

echo "AWS Load Balancer Controller installed."

# 2. Create ACM Certificate
echo "Creating ACM certificate for *.$DOMAIN..."
CERT_ARN=$(aws acm request-certificate \
  --domain-name "*.$DOMAIN" \
  --validation-method DNS \
  --region "$REGION" \
  --query CertificateArn \
  --output text)

echo "Certificate ARN: $CERT_ARN"
echo "Waiting for certificate validation (may require manual DNS validation)..."
aws acm wait certificate-validated --certificate-arn "$CERT_ARN" --region "$REGION" || {
  echo "Certificate validation pending. Please add the following DNS records to $DOMAIN in Route 53:"
  aws acm describe-certificate --certificate-arn "$CERT_ARN" --region "$REGION" --query 'Certificate.DomainValidationOptions[0].ResourceRecord'
  echo "Run this script again after validation."
  exit 1
}

# Update values/aws-values.yaml with the ARN
sed -i "s|alb.ingress.kubernetes.io/certificate-arn: .*|alb.ingress.kubernetes.io/certificate-arn: $CERT_ARN|" ./deploy/values/aws-values.yaml
echo "Updated values/aws-values.yaml with certificate ARN."

# 3. Configure Route 53 DNS (after deployment, we'll update with ALB DNS)
echo "Deploy the Helm chart first to get the ALB DNS name. Run this script again with '--update-dns' after deployment."
if [[ "$1" == "--update-dns" ]]; then
  ALB_DNS=$(kubectl get ingress -n miniflexcrm miniflex-crm-ingress -o jsonpath='{.status.loadBalancer.ingress[0].hostname}')
  if [ -z "$ALB_DNS" ]; then
    echo "Error: ALB DNS not found. Ensure the Ingress is deployed."
    exit 1
  fi

  echo "Configuring Route 53 for www.$DOMAIN and api.$DOMAIN to point to $ALB_DNS..."
  cat <<EOF > route53-changes.json
{
  "Changes": [
    {
      "Action": "UPSERT",
      "ResourceRecordSet": {
        "Name": "www.$DOMAIN",
        "Type": "CNAME",
        "TTL": 300,
        "ResourceRecords": [{"Value": "$ALB_DNS"}]
      }
    },
    {
      "Action": "UPSERT",
      "ResourceRecordSet": {
        "Name": "api.$DOMAIN",
        "Type": "CNAME",
        "TTL": 300,
        "ResourceRecords": [{"Value": "$ALB_DNS"}]
      }
    }
  ]
}
EOF
  aws route53 change-resource-record-sets \
    --hosted-zone-id "$HOSTED_ZONE_ID" \
    --change-batch file://route53-changes.json
  rm route53-changes.json
  echo "Route 53 updated."
else
  echo "Skipping DNS update. Run './setup_eks_prereqs.sh --update-dns' after deployment."
fi

echo "EKS prerequisites setup complete!"