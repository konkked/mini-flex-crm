CREATE TABLE IF NOT EXISTS "address" (
    id SERIAL PRIMARY KEY,
    content VARCHAR(255) NOT NULL,
    lat DOUBLE PRECISION NULL,
    lng DOUBLE PRECISION NULL,
    attributes JSON NULL
);

CREATE TABLE IF NOT EXISTS "entity_address"(
    id SERIAL PRIMARY KEY,
    significance_ordinal INT NOT NULL,
    entity_name VARCHAR(255) NOT NULL,
    entity_id INT NOT NULL,
    address_id INT NOT NULL REFERENCES address(id),
    CONSTRAINT unique_significance_ordinal_per_entity UNIQUE (entity_name, entity_id, significance_ordinal)
);
