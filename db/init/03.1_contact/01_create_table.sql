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
    attributes JSON,
    company_id INT NOT NULL REFERENCES company(id)
);