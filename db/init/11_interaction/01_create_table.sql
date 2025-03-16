CREATE TABLE IF NOT EXISTS "interaction" (
    interaction_id SERIAL PRIMARY KEY,
    type VARCHAR(50) NOT NULL, -- e.g., 'email', 'call', 'meeting'
    interaction_date DATE NOT NULL,
    notes TEXT,
    attributes JSON,
    customer_id INT REFERENCES customer(id),
    contact_id INT REFERENCES contact(contact_id),
    lead_id INT REFERENCES lead(lead_id),
    tenant_id INT NOT NULL REFERENCES tenant(id),
    CONSTRAINT check_single_entity CHECK (
        (customer_id IS NOT NULL AND contact_id IS NULL AND lead_id IS NULL) OR
        (customer_id IS NULL AND contact_id IS NOT NULL AND lead_id IS NULL) OR
        (customer_id IS NULL AND contact_id IS NULL AND lead_id IS NOT NULL) OR
        (customer_id IS NULL AND contact_id IS NULL AND lead_id IS NULL)
    )
);