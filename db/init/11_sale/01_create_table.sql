CREATE TABLE IF NOT EXISTS "sale" (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT NULL,
    value DECIMAL(10, 2) NOT NULL,
    term_months INT NULL,
    attributes JSON,
    sales_opportunity_id INT NULL REFERENCES sales_opportunity(id)
);

DO $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM pg_type WHERE typname = 'payment_type') THEN
        CREATE TYPE lead_status_type AS ENUM ('Card','Cash','Charge-Off');
    END IF;
END $$;