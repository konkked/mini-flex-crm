CREATE TABLE IF NOT EXISTS "address" (
    id SERIAL PRIMARY KEY,
    content VARCHAR(255) NOT NULL,
    lat DOUBLE PRECISION NULL,
    lng DOUBLE PRECISION NULL,
    attributes JSON NULL,
    company_id INT NOT NULL REFERENCES company(id)
);
