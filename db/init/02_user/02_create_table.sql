
CREATE TABLE IF NOT EXISTS "app_user" (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(512) UNIQUE NOT NULL,
    attributes JSON NULL,
    password_hash TEXT NOT NULL,
    salt TEXT NOT NULL,
    role app_user_role NOT NULL DEFAULT 'standard',
    enabled boolean default false,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);