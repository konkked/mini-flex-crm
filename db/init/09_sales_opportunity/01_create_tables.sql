CREATE TABLE IF NOT EXISTS "sales_opportunity" (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    status VARCHAR(50) NOT NULL, -- e.g., 'Open', 'Won', 'Lost'
    value DECIMAL(10, 2) NOT NULL,
    attributes JSON,
    customer_id INT REFERENCES customer(id),
    lead_id INT REFERENCES lead(id),
    tenant_id INT NOT NULL REFERENCES tenant(id),
    CONSTRAINT check_single_entity CHECK (
        (customer_id IS NOT NULL AND lead_id IS NULL) OR
        (customer_id IS NULL AND lead_id IS NOT NULL)
    )
);

CREATE TABLE IF NOT EXISTS "sales_opportunity_users"(
    id SERIAL PRIMARY KEY,
    sales_opportunity_id INT REFERENCES sales_opportunity(id),
    user_id INT REFERENCES user(id)
);

CREATE TABLE IF NOT EXISTS "sales_opportunity_products"(
    id SERIAL PRIMARY KEY,
    sales_opportunity_id INT REFERENCES sales_opportunity(id),
    product_id INT REFERENCES product(id)
);