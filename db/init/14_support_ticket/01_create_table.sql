CREATE TABLE IF NOT EXISTS "support_ticket" (
    id SERIAL PRIMARY KEY,
    issue TEXT NOT NULL,
    status support_ticket_status_type NOT NULL DEFAULT 'open', -- e.g., 'Open', 'Resolved'
    attributes JSON,
    user_id INT NOT NULL REFERENCES user(id),
    tenant_id INT NOT NULL REFERENCES tenant(id)
);