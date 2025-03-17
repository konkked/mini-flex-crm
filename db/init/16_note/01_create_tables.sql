CREATE TABLE IF NOT EXISTS "personal_app_note" (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES app_user(id),
    tenant_id INT NOT NULL REFERENCES tenant(id),
    route VARCHAR(255) NOT NULL,
    title VARCHAR(255) NOT NULL,
    content TEXT NOT NULL,
    pinned BOOLEAN DEFAULT FALSE
);


CREATE TABLE IF NOT EXISTS "entity_note"(
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    content TEXT NOT NULL,
    entity_name VARCHAR(255) NOT NULL,
    entity_id INT NOT NULL,
    ordinal INT NOT NULL,
    CONSTRAINT unique_ordinal_per_entity UNIQUE (entity_name, entity_id, ordinal)
);