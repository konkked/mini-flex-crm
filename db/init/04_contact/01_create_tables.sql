CREATE TABLE IF NOT EXISTS "contact" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    title VARCHAR(255),
    email VARCHAR(255),
    email_verified BOOLEAN,
    phone VARCHAR(255),
    phone_verified BOOLEAN,
    can_text BOOLEAN,
    can_call BOOLEAN,
    can_email BOOLEAN,
    attributes JSON
);

CREATE TABLE IF NOT EXISTS "entity_contact"(
    id SERIAL PRIMARY KEY,
    significance_ordinal INT NOT NULL,
    entity_name VARCHAR(255) NOT NULL,
    entity_id INT NOT NULL,
    contact_id INT NOT NULL REFERENCES contact(id),
    CONSTRAINT unique_significance_ordinal_per_entity UNIQUE (entity_name, entity_id, significance_ordinal)
);