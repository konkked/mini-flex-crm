CREATE TABLE IF NOT EXISTS "payment" (
    id SERIAL PRIMARY KEY,
    type payment_type NOT NULL DEFAULT 'Cash',
    title VARCHAR(255) NOT NULL,
    value DECIMAL(10, 2) NOT NULL,
    sale_id INT NOT NULL REFERENCES sale(id),
    attributes JSON
)