CREATE INDEX IF NOT EXISTS idx__t_entityAddress__contactId_entityId_entityName
ON entity_address (contact_id, entity_id, entity_name);