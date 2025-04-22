CREATE TABLE IF NOT EXISTS "product" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    suggested_price DECIMAL(10, 2) NOT NULL,
    attributes JSON,
    tenant_id INT NOT NULL REFERENCES tenant(id)
)