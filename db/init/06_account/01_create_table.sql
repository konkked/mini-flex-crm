CREATE TABLE IF NOT EXISTS "account" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(512) NOT NULL,
    account_manager_id INT NULL REFERENCES app_user(id),
    sales_rep_id INT NULL REFERENCES app_user(id),
    pre_sales_rep_id INT NULL REFERENCES app_user(id),
    tenant_id INT NOT NULL REFERENCES tenant(id),
    attributes JSON NULL,
    CONSTRAINT unique_account_name_per_tenant UNIQUE (tenant_id, name)
);

