CREATE TABLE IF NOT EXISTS "customer" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(512) NOT NULL,
    attributes JSON NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id),
    CONSTRAINT unique_company_name_per_tenant UNIQUE (tenant_id, name)
);

