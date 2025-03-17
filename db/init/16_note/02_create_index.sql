CREATE INDEX IF NOT EXISTS idx_personalAppNote_tenantId_userId_route
ON personal_app_note (tenant_id, user_id, route);

CREATE INDEX IF NOT EXISTS idx_entityNote_tenantId_userId_route
ON entity_note (entity_name, entity_id);