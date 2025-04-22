INSERT INTO "account" (name, attributes, tenant_id) VALUES
('Account One', '{}', 1),
('Account Two', '{}', 2),
('Account Three', '{}', 3)
ON CONFLICT DO NOTHING;