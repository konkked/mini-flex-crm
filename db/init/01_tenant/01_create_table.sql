CREATE TABLE IF NOT EXISTS "tenant" (
    id SERIAL PRIMARY KEY,
    short_id VARCHAR(255) UNIQUE NOT NULL,
    name VARCHAR(255) UNIQUE NOT NULL,
    theme VARCHAR(255) NOT NULL,
    attributes JSON NULL
);