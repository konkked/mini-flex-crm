CREATE TABLE IF NOT EXISTS "attachment" (
    id SERIAL PRIMARY KEY,
    path VARCHAR(1024) NOT NULL, -- Stores the file path (e.g., "/uploads/image.jpg")
    file_content BYTEA NOT NULL, -- Stores the file as a blob
    notes TEXT,
    tenant_id INT NOT NULL REFERENCES tenant(id)
)