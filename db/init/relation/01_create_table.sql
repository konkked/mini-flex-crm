CREATE TABLE IF NOT EXISTS "relation" (
    id SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customer(id),
    entity VARCHAR(255) NOT NULL,
    entity_id INT NOT NULL
);
