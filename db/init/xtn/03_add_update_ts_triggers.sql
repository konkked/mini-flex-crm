DO $$
DECLARE
    table_name TEXT;
    trigger_name TEXT;
BEGIN
    FOR table_name IN
        SELECT tablename
        FROM pg_tables
        WHERE schemaname = 'public'  -- Adjust schema if necessary
    LOOP
        -- Generate a unique trigger name for the table
        trigger_name := 'trg_update_updated_ts_' || table_name;

        -- Check if the trigger already exists
        IF NOT EXISTS (
            SELECT 1
            FROM pg_trigger
            WHERE tgname = trigger_name
              AND tgrelid = (SELECT oid FROM pg_class WHERE relname = table_name AND relnamespace = 'public'::regnamespace)
        ) THEN
            -- Create the trigger if it doesn't exist
            EXECUTE format('
                CREATE TRIGGER %I
                BEFORE UPDATE ON %I
                FOR EACH ROW
                EXECUTE FUNCTION update_updated_ts();
            ', trigger_name, table_name);
        END IF;
    END LOOP;
END $$;