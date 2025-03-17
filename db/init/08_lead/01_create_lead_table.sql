CREATE TABLE IF NOT EXISTS "lead" (
    id SERIAL PRIMARY KEY,
    status lead_status_type NOT NULL DEFAULT 'Raw',
    lead_data_origin lead_data_origin_type NOT NULL DEFAULT 'Manual',
    name VARCHAR(255) NOT NULL,
    company_name VARCHAR(255),
    industry VARCHAR(255),
    approximate_comapny_size INT,
    approximate_revenue INT,
    email VARCHAR(255),
    phone VARCHAR(255),
    attributes JSON,
    tenant_id INT NOT NULL REFERENCES tenant(id)
);