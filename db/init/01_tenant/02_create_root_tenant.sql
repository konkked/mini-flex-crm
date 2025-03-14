
-- Insert tenant record with id 0 if it does not exist
INSERT INTO "tenant" (id, name, short_id, theme) VALUES (0, 'root', 'root', 'professional') ON CONFLICT (id) DO NOTHING;
