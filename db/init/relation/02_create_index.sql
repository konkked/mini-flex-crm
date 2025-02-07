CREATE INDEX IF NOT EXISTS idx_relation_customer_entity_id
ON relation (customer_id, entity, entity_id);