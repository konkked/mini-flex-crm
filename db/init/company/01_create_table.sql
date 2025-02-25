CREATE TABLE IF NOT EXISTS "company" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    attributes JSON NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);
