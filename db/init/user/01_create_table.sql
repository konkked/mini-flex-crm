CREATE TABLE IF NOT EXISTS "user" (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    salt TEXT NOT NULL,
    role VARCHAR(50) NOT NULL,
    enabled boolean default false,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);
