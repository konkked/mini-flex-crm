DO $$
DECLARE
    new_id integer = 0;
BEGIN
    SELECT MAX(id) + 1 INTO new_id FROM "app_user";
    PERFORM setval(pg_get_serial_sequence('app_user', 'id'), GREATEST(1, new_id));
END $$;