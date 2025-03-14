INSERT INTO "customer" (name, attributes, tenant_id) VALUES
('Customer One', '{}', 1),
('Customer Two', '{}', 2),
('Customer Three', '{}', 3)
ON CONFLICT DO NOTHING;