CREATE TABLE IF NOT EXISTS "deal" (
    id SERIAL PRIMARY KEY,
    status deal_status_type NOT NULL DEFAULT 'Qualified',
    lead_id INT NOT NULL REFERENCES lead(id),
    name VARCHAR(255) NOT NULL,
    company_name VARCHAR(255),
    industry VARCHAR(255),
    approximate_comapny_size INT,
    approximate_revenue INT,
    email VARCHAR(255),
    phone VARCHAR(255),
    attributes JSON,
    tenant_id INT NOT NULL REFERENCES tenant(id),
    owner_id INT NOT NULL REFERENCES app_user(id)
);