CREATE INDEX IF NOT EXISTS idx_relationships_customer_entity_id
ON relationships (customer_id, entity, entity_id);