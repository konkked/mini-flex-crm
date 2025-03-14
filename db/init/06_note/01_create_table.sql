CREATE TABLE IF NOT EXISTS "note" (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES app_user(id),
    tenant_id INT NOT NULL REFERENCES tenant(id),
    route VARCHAR(255) NOT NULL,
    title VARCHAR(255) NOT NULL,
    content VARCHAR(512) NOT NULL,
    pinned BOOLEAN DEFAULT FALSE
);