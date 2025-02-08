#!/bin/bash

# Generate a secure 256-bit (32-byte) Base64-encoded key
JWT_SECRET=$(openssl rand -base64 32)

# Export it as an environment variable (only for the current session)
export MINIFLEXCRMAPI_JWT_KEY="$JWT_SECRET"

# Append to shell profile for persistence (Mac/Linux)
if [[ "$SHELL" == *"zsh"* ]]; then
    echo "export MINIFLEXCRMAPI_JWT_KEY=\"$JWT_SECRET\"" >> ~/.zshrc
elif [[ "$SHELL" == *"bash"* ]]; then
    echo "export MINIFLEXCRMAPI_JWT_KEY=\"$JWT_SECRET\"" >> ~/.bashrc
fi

# Windows PowerShell users (Persistent)
if [[ "$OS" == "Windows_NT" ]]; then
    powershell -Command "[System.Environment]::SetEnvironmentVariable('MINIFLEXCRMAPI_JWT_KEY', '$JWT_SECRET', 'User')"
fi

echo "JWT Secret Key Generated & Stored in Environment Variable"
echo "Key: $JWT_SECRET"
