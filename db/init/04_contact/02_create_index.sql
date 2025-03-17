CREATE INDEX IF NOT EXISTS idx__t_entityContact__contactId_entityId_entityName
ON entity_contact (contact_id, entity_id, entity_name);