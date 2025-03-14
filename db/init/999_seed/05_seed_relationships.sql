INSERT INTO "relationships" (customer_id, entity, entity_id) VALUES
(1, 'company', 1),
(2, 'company', 2),
(3, 'company', 3)
ON CONFLICT DO NOTHING;