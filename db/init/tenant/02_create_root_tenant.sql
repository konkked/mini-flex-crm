
-- Insert tenant record with id 0 if it does not exist
INSERT INTO "tenant" (id, name) VALUES (0, 'root') ON CONFLICT (id) DO NOTHING;
