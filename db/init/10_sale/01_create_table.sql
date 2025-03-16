CREATE TABLE IF NOT EXISTS "sale" (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    value DECIMAL(10, 2) NOT NULL,
    attributes JSON,
    sales_opportunity_id INT NOT NULL REFERENCES sales_opportunity(id)
);