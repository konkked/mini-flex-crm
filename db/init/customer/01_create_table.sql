CREATE TABLE IF NOT EXISTS "customer" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    attributes JSON NOT NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);
