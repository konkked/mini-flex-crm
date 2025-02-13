CREATE TABLE IF NOT EXISTS "tenant" (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL
);

-- Insert tenant record with id 0 if it does not exist
INSERT INTO "tenant" (id, name)
VALUES (0, 'root')
ON CONFLICT (id) DO NOTHING;

-- Set the sequence to start from 1 if the current value is less than 1
DO $$
BEGIN
    IF (SELECT setval(pg_get_serial_sequence('tenant', 'id'), 1, false)) < 1 THEN
        PERFORM setval(pg_get_serial_sequence('tenant', 'id'), 1, false);
    END IF;
END $$;
