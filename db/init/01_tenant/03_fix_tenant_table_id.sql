-- Set the sequence to start from the maximum id value in the tenant table plus one
DO $$ 
DECLARE
    max_id INTEGER;
BEGIN
    SELECT MAX(id) INTO max_id FROM tenant;
    IF max_id IS NULL THEN
        max_id := 1;
    ELSE
        max_id := max_id + 1;
    END IF;
    PERFORM setval(pg_get_serial_sequence('tenant', 'id'), max_id, false);
END $$;