
-- Set the sequence to start from 1 if the current value is less than 1
SELECT setval(pg_get_serial_sequence('user', 'id'), GREATEST(1, (SELECT MAX(id) FROM user) + 1));