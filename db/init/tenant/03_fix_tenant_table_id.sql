
-- Set the sequence to start from 1 if the current value is less than 1
SELECT setval(pg_get_serial_sequence('tenant', 'id'), GREATEST(1, (SELECT MAX(id) FROM tenant) + 1));