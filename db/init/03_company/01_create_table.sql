CREATE TABLE IF NOT EXISTS "company" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description VARCHAR(255) NULL,
    industry VARCHAR(255) NULL,
    website VARCHAR(255) NULL,
    annual_revenue NUMERIC(15, 2) NULL,
    number_of_employees INT NULL,
    founded_date DATE NULL,
    status VARCHAR(50) NULL,
    logo BYTEA NULL,
    tenant_id INT NOT NULL REFERENCES tenant(id),
    attributes JSON NULL
);
