CREATE INDEX IF NOT EXISTS idx_relationships_account_entity_id
ON relationships (account_id, entity, entity_id);