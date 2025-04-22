CREATE TABLE IF NOT EXISTS "relationships" (
    id SERIAL PRIMARY KEY,
    account_id INT NOT NULL REFERENCES account(id),
    entity VARCHAR(255) NOT NULL,
    entity_id INT NOT NULL
);
