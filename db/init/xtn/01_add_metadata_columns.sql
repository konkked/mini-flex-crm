DO $$
DECLARE
    table_name TEXT;
BEGIN
    FOR table_name IN
        SELECT tablename
        FROM pg_tables
        WHERE schemaname = 'public'  -- Adjust schema if necessary
    LOOP
        EXECUTE format('
            ALTER TABLE %I
            ADD COLUMN IF NOT EXISTS created_ts BIGINT NOT NULL DEFAULT EXTRACT(EPOCH FROM NOW()),
            ADD COLUMN IF NOT EXISTS updated_ts BIGINT NOT NULL DEFAULT EXTRACT(EPOCH FROM NOW()),
            ADD COLUMN IF NOT EXISTS updated_by_id INT NOT NULL DEFAULT 0;
        ', table_name);
    END LOOP;
END $$;